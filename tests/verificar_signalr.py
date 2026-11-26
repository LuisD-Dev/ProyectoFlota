"""SignalR real + HTTP + rollback sobre una base NUEVA y exclusiva QA.
Requiere backend y auxiliar SqlQa compilados, npm install y Node en PATH.
Usa user-secrets sin mostrar credenciales; conserva la base para revisión.
FLOTA_QA_CONFIGURATION permite cambiar Release sin detener la API habitual.
"""
from pathlib import Path
from urllib.request import Request, urlopen
from urllib.error import HTTPError, URLError
from datetime import datetime, timezone, timedelta
import importlib.util, json, os, re, secrets, socket, subprocess, threading, queue, time, xml.etree.ElementTree as ET

ROOT=Path(__file__).resolve().parents[1]
WORK=ROOT/'tests/resultados-signalr'
WORK.mkdir(exist_ok=True)
CONFIGURATION=os.environ.get('FLOTA_QA_CONFIGURATION','Release')
NODE=os.environ.get('FLOTA_QA_NODE','node')
API=ROOT/'src/MantenimientoFlota.Api'
BASE='http://127.0.0.1:5401'
DB='MantenimientoFlotaDb_QA_Belen_SignalR_'+datetime.now().strftime('%Y%m%d_%H%M%S')
with socket.socket() as check:
    check.bind(('127.0.0.1',5401))
project=ET.parse(API/'MantenimientoFlota.Api.csproj')
secret_id=project.find('.//UserSecretsId').text
settings=json.loads((Path(os.environ['APPDATA'])/'Microsoft/UserSecrets'/secret_id/'secrets.json').read_text(encoding='utf-8-sig'))
connection=settings.get('ConnectionStrings:DefaultConnection') or settings.get('ConnectionStrings',{}).get('DefaultConnection')
assert connection and re.search(r'(?i)(database|initial catalog)\s*=',connection)
connection=re.sub(r'(?i)(database|initial catalog)\s*=\s*[^;]*','Database='+DB,connection)
password='SignalR.'+secrets.token_hex(10)
env={**os.environ,'ASPNETCORE_ENVIRONMENT':'Development','DOTNET_ENVIRONMENT':'Development',
    'ConnectionStrings__DefaultConnection':connection,'Seed__Admin__Password':password,
    'Jwt__Key':secrets.token_urlsafe(48),'Jwt__Issuer':'SignalR-QA','Jwt__Audience':'SignalR-QA',
    'Jwt__ExpirationMinutes':'60','ApiLogs__Directorio':str(WORK/'logs'),
    'ActivoImagenes__Directorio':str(WORK/'imagenes')}
log=(WORK/'arranque-qa.log').open('w',encoding='utf-8')
server=subprocess.Popen(['dotnet',str(API/f'bin/{CONFIGURATION}/net10.0/MantenimientoFlota.Api.dll'),'--urls',BASE],cwd=API,env=env,stdout=log,stderr=subprocess.STDOUT)
client=None
results=[]
def request(method,path,body=None,token=None,expected=200,headers=None):
    h={'Content-Type':'application/json',**(headers or {})}
    if token: h['Authorization']='Bearer '+token
    try:
        with urlopen(Request(BASE+path,data=json.dumps(body).encode() if body is not None else None,headers=h,method=method),timeout=20) as r:
            code, raw, response_headers=r.status,r.read(),r.headers
    except HTTPError as e:
        code,raw,response_headers=e.code,e.read(),e.headers
    assert code==expected, f'{method} {path.split("?")[0]}: esperado {expected}; recibido {code}'
    results.append({'metodo':method,'ruta':path.split('?')[0],'estado':code})
    return (json.loads(raw) if raw else None),response_headers
def data(method,path,body=None,token=None,expected=200):
    return request(method,'/api/'+path,body,token,expected)[0]['data']
events=queue.Queue()
def empty():
    try: msg=events.get(timeout=0.8)
    except queue.Empty: return
    raise AssertionError('Se recibió un evento inesperado: '+msg['tipo'])
def alert(rid,previous,current):
    msg=events.get(timeout=8)
    assert msg['tipo']=='alerta',msg['tipo']
    value=msg['alerta']
    assert (value['repuestoId'],value['stockAnterior'],value['stockActual'],value['stockMinimo'])==(rid,previous,current,5)
    assert value['eventoId'] and value['fechaUtc']
    results.append({'caso':'Evento SignalR recibido','anterior':previous,'actual':current,'minimo':5})
ok=False
try:
    for _ in range(100):
        if server.poll() is not None: raise RuntimeError('La API no inició; revisar arranque-qa.log.')
        try:
            urlopen(BASE+'/openapi/v1.json',timeout=1).close();break
        except (URLError,TimeoutError):time.sleep(.3)
    else:raise RuntimeError('La API no respondió.')
    # La preflight debe usar la política y permitir las credenciales del cliente.
    _,cors=request('OPTIONS','/hubs/stock/negotiate?negotiateVersion=1',expected=204,headers={
        'Origin':'http://localhost:5173','Access-Control-Request-Method':'POST',
        'Access-Control-Request-Headers':'authorization,x-signalr-user-agent,x-requested-with'})
    assert cors.get('Access-Control-Allow-Origin')=='http://localhost:5173'
    assert cors.get('Access-Control-Allow-Credentials')=='true'
    request('POST','/hubs/stock/negotiate?negotiateVersion=1',expected=401)
    admin=data('POST','Auth/login',{'login':'admin','password':password})['token']
    roles={r['nombre']:r['id'] for r in data('GET','Usuario/roles',token=admin)}
    for login,role in [('coord-signal','Coordinador/Técnico'),('solic-signal','Solicitante/Consulta')]:
        data('POST','Usuario',{'login':login,'password':password,'correo':login+'@qa.example',
            'nombreCompleto':'Prueba SignalR','rolId':roles[role]},admin,201)
    coord=data('POST','Auth/login',{'login':'coord-signal','password':password})['token']
    solic=data('POST','Auth/login',{'login':'solic-signal','password':password})['token']
    request('POST','/hubs/stock/negotiate?negotiateVersion=1',token=solic,expected=403)
    request('POST','/hubs/stock/negotiate?negotiateVersion=1',token=coord)
    client=subprocess.Popen([NODE,str(ROOT/'tests/cliente_signalr.cjs')],stdin=subprocess.PIPE,stdout=subprocess.PIPE,stderr=subprocess.PIPE,text=True,encoding='utf-8')
    threading.Thread(target=lambda:[events.put(json.loads(line)) for line in client.stdout],daemon=True).start()
    client.stdin.write(json.dumps({'url':BASE+'/hubs/stock','token':admin})+'\n');client.stdin.flush()
    assert events.get(timeout=10)['tipo']=='conectado'
    results.append({'caso':'Cliente real SignalR conectado y suscrito'})
    rep=data('POST','Repuesto',{'codigo':'REP-SIGNALR-QA-001','nombre':'Filtro QA SignalR',
        'unidadMedida':'Unidad','stockActual':6,'stockMinimo':5},admin,201)
    rid=rep['id'];empty()
    def update(stock):
        data('PUT',f'Repuesto/{rid}',{'codigo':rep['codigo'],'nombre':rep['nombre'],
            'unidadMedida':'Unidad','stockActual':stock,'stockMinimo':5},admin)
    update(5);empty()
    update(4);alert(rid,5,4)
    update(3);empty()
    update(6);empty()
    # Consumo real mediante orden, en una sesión HTTP diferente del receptor.
    cat=data('GET','CategoriaActivo',token=admin)[0]['id']
    esp=data('GET','Especialidad',token=admin)[0]['id']
    proveedor=data('GET','Proveedor',token=admin)[0]['id']
    data('POST',f'Repuesto/{rid}/proveedores',{'proveedorId':proveedor,'precioOfrecido':10,'tiempoEntregaDias':1},admin,201)
    act=data('POST','Activo',{'codigo':'VEH-SIGNALR-QA-001','nombre':'Activo QA SignalR',
        'fechaAdquisicion':'2022-01-15','valorReferencia':1000,'estado':0,
        'kilometrajeActual':100,'categoriaActivoId':cat},admin,201)
    order=data('POST','OrdenMantenimiento',{'activoId':act['id'],'motivo':'Demo stock bajo',
        'tipoMantenimiento':'Preventivo','prioridad':'Alta'},admin,201)
    path=f"OrdenMantenimiento/{order['id']}"
    data('PATCH',path+'/estado',{'estado':1,'especialidadRequeridaId':esp},coord)
    data('PATCH',path+'/estado',{'estado':2,'fechaProgramada':(datetime.now(timezone.utc)+timedelta(days=1)).isoformat().replace('+00:00','Z')},coord)
    data('PATCH',path+'/estado',{'estado':4},coord)
    data('POST',path+'/repuestos',{'repuestos':[{'repuestoId':rid,'cantidad':2}]},coord)
    alert(rid,6,4)
    assert data('GET',f'Repuesto/{rid}',token=admin)['stockActual']==4
    empty()
    results.append({'caso':'Stock persistido y alerta recibida tras consumo de orden'})
    # Repetir la suscripción no debe duplicar eventos.
    client.stdin.write('{"resubscribe":true}\n');client.stdin.flush()
    assert events.get(timeout=5)['tipo']=='suscrito'
    update(6);empty()
    update(4);alert(rid,6,4);empty()
    results.append({'caso':'Suscripción repetida sin notificación duplicada'})
    # Fallo de persistencia: no se publica una alerta de un consumo revertido.
    update(6);empty()
    def sql(mode):
        out=subprocess.run(['dotnet',str(ROOT/f'tests/SqlQa/bin/{CONFIGURATION}/net10.0/SqlQa.dll'),mode],
            env={**os.environ,'FLOTA_QA_CONNECTION':connection},capture_output=True)
        assert out.returncode==0,'Falló el auxiliar SQL aislado'
    before=data('GET',path+'/repuestos',token=admin)
    sql('fallar-consumos')
    try:
        request('POST','/api/'+path+'/repuestos',{'repuestos':[{'repuestoId':rid,'cantidad':2}]},coord,500)
        empty()
        assert data('GET',f'Repuesto/{rid}',token=admin)['stockActual']==6
        assert data('GET',path+'/repuestos',token=admin)==before
        results.append({'caso':'Rollback: sin cambio de stock, detalle ni evento'})
    finally: sql('limpiar-fallo')
    # Sesión ya conectada: desactivar la cuenta debe impedir nuevos avisos.
    uid=data('POST','Usuario',{'login':'receptor-qa','password':password,'correo':'receptor@qa.example',
        'nombreCompleto':'Receptor temporal','rolId':roles['Coordinador/Técnico']},admin,201)['id']
    receiver=data('POST','Auth/login',{'login':'receptor-qa','password':password})['token']
    second=subprocess.Popen([NODE,str(ROOT/'tests/cliente_signalr.cjs')],
        stdin=subprocess.PIPE,stdout=subprocess.PIPE,stderr=subprocess.PIPE,text=True,encoding='utf-8')
    other=queue.Queue()
    threading.Thread(target=lambda:[other.put(json.loads(line)) for line in second.stdout],daemon=True).start()
    try:
        second.stdin.write(json.dumps({'url':BASE+'/hubs/stock','token':receiver})+'\n');second.stdin.flush()
        assert other.get(timeout=10)['tipo']=='conectado'
        update(4);alert(rid,6,4)
        assert other.get(timeout=5)['tipo']=='alerta'
        update(6);empty()
        request('DELETE',f'/api/Usuario/{uid}',token=admin,expected=200)
        update(4);alert(rid,6,4)
        assert other.get(timeout=5)['tipo']=='revocada'
        try: other.get(timeout=.8);raise AssertionError('Usuario desactivado recibió inventario')
        except queue.Empty: pass
        results.append({'caso':'Dos receptores autorizados; cuenta desactivada excluida en conexión abierta'})
    finally:
        second.stdin.write('{"stop":true}\n');second.stdin.flush();second.wait(timeout=5)
    ok=True
finally:
    if client:
        try:
            client.stdin.write('{"stop":true}\n');client.stdin.flush();client.wait(timeout=5)
        except (OSError,subprocess.TimeoutExpired):client.kill()
    server.terminate()
    try:server.wait(timeout=10)
    except subprocess.TimeoutExpired:server.kill()
    log.close()
    (WORK/'resumen.json').write_text(json.dumps({'base':DB,'correcto':ok,'verificaciones':len(results),'resultados':results},ensure_ascii=False,indent=2),encoding='utf-8')
    print(json.dumps({'base':DB,'correcto':ok,'verificaciones':len(results)},ensure_ascii=False))
