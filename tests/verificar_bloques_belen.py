"""Pruebas HTTP de los bloques 5-8 en una base NUEVA, nunca en la principal.
Python 3, solo biblioteca estándar. Compilar Release antes de ejecutar.
Lee la conexión local desde user-secrets sin mostrarla y cambia SOLO el catálogo.
No borra bases; conserva la de pruebas para revisión. No guarda tokens ni claves.
"""
from pathlib import Path
from datetime import datetime, timezone, timedelta
from urllib.request import Request, urlopen
from urllib.error import HTTPError, URLError
from concurrent.futures import ThreadPoolExecutor
import xml.etree.ElementTree as ET
import os, re, json, subprocess, time, secrets, socket

ROOT = Path(__file__).resolve().parents[1]
API = ROOT / 'src/MantenimientoFlota.Api'
CONFIGURATION = os.environ.get('FLOTA_QA_CONFIGURATION', 'Release')
BASE = 'http://127.0.0.1:5399'
DB = 'MantenimientoFlotaDb_QA_Belen_' + datetime.now().strftime('%Y%m%d_%H%M%S')
RESULTADOS = []
PASSWORD = 'Pruebas.' + secrets.token_hex(12)

def req(method, path, body=None, token=None, expected=200):
    headers = {'Content-Type': 'application/json'}
    if token:
        headers['Authorization'] = 'Bearer ' + token
    request = Request(BASE + path, data=json.dumps(body).encode() if body is not None else None,
                      headers=headers, method=method)
    try:
        with urlopen(request, timeout=30) as response:
            status, raw, response_headers = response.status, response.read(), response.headers
    except HTTPError as error:
        status, raw, response_headers = error.code, error.read(), error.headers
    try:
        payload = json.loads(raw) if raw else None
    except ValueError:
        payload = None
    if expected is not None:
        assert status == expected, f'{method} {path}: esperado {expected}, recibido {status}; {payload}'
        RESULTADOS.append({'metodo': method, 'ruta': path, 'estado': status})
    return payload, status, response_headers

def data(method, path, body=None, token=None, expected=200):
    return req(method, path, body, token, expected)[0]['data']

def login(login):
    return data('POST', '/api/Auth/login', {'login': login, 'password': PASSWORD})['token']

def sql_test(mode):
    # La verificación se repite dentro del auxiliar .NET, antes de abrir SQL.
    assert DB.startswith('MantenimientoFlotaDb_QA_Belen_')
    env = {**os.environ, 'FLOTA_QA_CONNECTION': QA_CONNECTION}
    helper = ROOT/f'tests/SqlQa/bin/{CONFIGURATION}/net10.0/SqlQa.dll'
    result = subprocess.run(['dotnet', str(helper), mode], env=env, capture_output=True)
    if result.returncode:
        raise RuntimeError('Falló el auxiliar .NET de la base aislada de pruebas.')

def verify():
    req('GET', '/api/Activo', expected=401)
    req('GET', '/scalar', expected=200)
    req('GET', '/openapi/v1.json', expected=200)
    admin = login('admin')
    def call(method, path, body=None, expected=200, token=admin):
        return data(method, '/api/' + path, body, token, expected)
    roles = call('GET', 'Usuario/roles')
    roles = {r['nombre']: r['id'] for r in roles}
    def usuario(login_name, role):
        return call('POST', 'Usuario', {'login': login_name, 'password': PASSWORD,
            'nombreCompleto': 'Usuario de prueba', 'correo': login_name+'@qa.example',
            'rolId': roles[role]}, 201)
    coord_user = usuario('coordqa', 'Coordinador/Técnico')
    s1_user = usuario('solicqa1', 'Solicitante/Consulta')
    usuario('solicqa2', 'Solicitante/Consulta')
    coord, s1, s2 = login('coordqa'), login('solicqa1'), login('solicqa2')
    # Ninguna respuesta administrativa expone Password/PasswordHash.
    users = call('GET', 'Usuario')
    assert all(not any('password' in key.lower() for key in u) for u in users)
    call('GET', 'Usuario', token=s1, expected=403)
    call('GET', 'Tecnico', token=coord)
    call('GET', 'Tecnico', token=s1, expected=403)
    call('POST', 'CategoriaActivo', {'nombre':'Rechazada', 'intervaloMantenimientoKm':1}, token=coord, expected=403)
    usuario_body = {'login':'solicqa1','password':PASSWORD,'nombreCompleto':'Duplicado',
        'correo':'duplicado@qa.example','rolId':roles['Solicitante/Consulta']}
    call('POST', 'Usuario', usuario_body, expected=409)
    call('POST', 'Usuario', {**usuario_body, 'login':'invalido', 'password':'corta'}, expected=400)
    call('POST', 'Usuario', {**usuario_body, 'login':'sinrol','rolId':99999}, expected=404)
    admin_id = next(u['id'] for u in users if u['login']=='admin')
    call('DELETE', f'Usuario/{admin_id}', expected=409)
    call('PUT', f'Usuario/{admin_id}', {'login':'admin','nombreCompleto':'Admin',
        'correo':'admin@flota.example','rolId':roles['Solicitante/Consulta']}, expected=409)
    cat = call('POST', 'CategoriaActivo', {'nombre':'QA Proceso', 'intervaloMantenimientoKm':5000}, 201)
    esp = call('POST', 'Especialidad', {'nombre':'QA Mecánica'}, 201)
    def activo(code):
        return call('POST', 'Activo', {'codigo':code,'nombre':'Activo de prueba',
            'fechaAdquisicion':'2022-01-15','valorReferencia':1000,'estado':0,
            'kilometrajeActual':100,'categoriaActivoId':cat['id']}, 201)
    act = activo('VEH-QA-PROCESO-001')
    other = activo('VEH-QA-PROCESO-002')
    a = act['id']
    def orden(activo_id=a, token=s1):
        return call('POST', 'OrdenMantenimiento', {'activoId':activo_id,'motivo':'QA flujo',
            'tipoMantenimiento':'Preventivo','prioridad':'Alta'}, 201, token)
    call('POST', 'OrdenMantenimiento', {'activoId':a,'motivo':'QA','tipoMantenimiento':'P','prioridad':'Alta'}, token=coord, expected=403)
    om = orden()
    oid = om['id']
    ruta = f'OrdenMantenimiento/{oid}'
    assert call('GET', 'OrdenMantenimiento', token=s2)==[]
    assert len(call('GET', 'OrdenMantenimiento', token=s1))==1
    call('GET', ruta, token=s2, expected=403)
    call('GET', ruta+'/historial', token=s2, expected=403)
    call('GET', ruta+'/repuestos', token=s2, expected=403)
    call('GET', ruta+'/tecnicos', token=s2, expected=403)
    assert call('GET', f'Activo/{a}', token=s2)['ordenes']==[]
    hist = call('GET', ruta+'/historial')
    assert len(hist)==1 and hist[0]['usuarioId']==s1_user['id']
    def estado(n, extra=None, expected=200):
        return call('PATCH', ruta+'/estado', {'estado':n, **(extra or {})}, expected, coord)
    estado(6, expected=409)
    estado(1, expected=400)
    estado(1, {'especialidadRequeridaId':esp['id']})
    assert call('GET', ruta)['especialidadRequeridaId']==esp['id']
    estado(1, expected=409)
    estado(2, expected=400)
    estado(2, {'fechaProgramada':(datetime.now(timezone.utc)+timedelta(days=1)).isoformat().replace('+00:00','Z')})
    estado(4)
    assert call('GET', f'Activo/{a}')['estado']==1
    estado(6, expected=409)
    tecnico = call('POST', 'Tecnico', {'nombreCompleto':'Técnico QA','identificacion':'QA-TEC-001',
        'disponible':True,'tarifaPorHora':100}, 201)
    tid = tecnico['id']
    call('POST', ruta+'/tecnicos', {'tecnicoId':tid, 'horasTrabajadas':2}, expected=409, token=coord)
    call('POST', f'Tecnico/{tid}/especialidades', {'especialidadId':esp['id'],
        'nivelCertificacion':'Avanzado','fechaObtencion':'2020-01-15'}, 201)
    call('POST', ruta+'/tecnicos', {'tecnicoId':tid, 'horasTrabajadas':2.001}, expected=400, token=coord)
    call('POST', ruta+'/tecnicos', {'tecnicoId':tid, 'horasTrabajadas':2}, token=coord)
    call('POST', ruta+'/tecnicos', {'tecnicoId':tid, 'horasTrabajadas':2}, expected=409, token=coord)
    prov = call('POST', 'Proveedor', {'nombre':'Proveedor QA'}, 201)
    def repuesto(code, stock):
        r=call('POST', 'Repuesto', {'codigo':code,'nombre':code,'unidadMedida':'Unidad',
            'stockActual':stock,'stockMinimo':0},201)
        call('POST', f"Repuesto/{r['id']}/proveedores", {'proveedorId':prov['id'],
            'precioOfrecido':10,'tiempoEntregaDias':1},201)
        return r
    r1, r2 = repuesto('REP-QA-PROCESO-001',10), repuesto('REP-QA-PROCESO-002',0)
    rid1, rid2 = r1['id'], r2['id']
    def stock(rid): return call('GET', f'Repuesto/{rid}')['stockActual']
    call('POST', ruta+'/repuestos', {'repuestos':[{'repuestoId':rid1,'cantidad':1},
        {'repuestoId':2147483647,'cantidad':1}]}, expected=404, token=coord)
    assert stock(rid1)==10 and call('GET', ruta+'/repuestos')==[]
    call('POST', ruta+'/repuestos', {'repuestos':[{'repuestoId':rid1,'cantidad':2}]}, token=coord)
    assert stock(rid1)==8
    call('POST', ruta+'/repuestos', {'repuestos':[{'repuestoId':rid1,'cantidad':1},
        {'repuestoId':rid2,'cantidad':2}]},token=coord)
    assert stock(rid1)==8 and stock(rid2)==0
    assert call('GET', ruta)['estadoActual']==5
    estado(6, expected=409)
    estado(4, expected=409)
    call('PUT', f'Repuesto/{rid2}', {'codigo':r2['codigo'],'nombre':r2['nombre'],
        'unidadMedida':'Unidad','stockActual':5,'stockMinimo':0})
    estado(4)
    assert stock(rid1)==7 and stock(rid2)==3
    estado(6)
    final = call('GET', ruta)
    assert final['costoTotal']==250 and final['fechaCierre'] is not None
    assert call('GET', f'Activo/{a}')['estado']==0
    assert [x['estadoNuevo'] for x in call('GET', ruta+'/historial')]==[0,1,2,4,5,4,6]
    assert [x['estadoNuevo'] for x in call('GET', f'Activo/{a}/historial')]==[1,0]
    estado(4, expected=409)
    call('POST', ruta+'/repuestos', {'repuestos':[{'repuestoId':rid1,'cantidad':1}]},expected=409,token=coord)
    call('POST', ruta+'/tecnicos', {'tecnicoId':tid,'horasTrabajadas':1},expected=409,token=coord)
    # Dos consumos concurrentes: el stock nunca puede quedar negativo.
    def preparar_ejecucion(activo_id):
        o=orden(activo_id)
        path=f"OrdenMantenimiento/{o['id']}/estado"
        call('PATCH',path,{'estado':1,'especialidadRequeridaId':esp['id']},token=coord)
        call('PATCH',path,{'estado':2,'fechaProgramada':(datetime.now(timezone.utc)+timedelta(days=1)).isoformat().replace('+00:00','Z')},token=coord)
        call('PATCH',path,{'estado':4},token=coord)
        return o['id']
    o1, o2 = preparar_ejecucion(a), preparar_ejecucion(other['id'])
    before=stock(rid1)
    history_before=call('GET',f'OrdenMantenimiento/{o1}/historial')
    # Falla SQL DESPUÉS de intentar insertar el detalle; la transacción debe
    # revertir también cualquier UPDATE de stock ejecutado antes del INSERT.
    sql_test("fallar-consumos")
    try:
        req('POST',f'/api/OrdenMantenimiento/{o1}/repuestos',
            {'repuestos':[{'repuestoId':rid1,'cantidad':1}]},coord,expected=500)
        assert stock(rid1)==before
        assert call('GET',f'OrdenMantenimiento/{o1}/repuestos')==[]
        assert call('GET',f'OrdenMantenimiento/{o1}/historial')==history_before
        RESULTADOS.append({'caso':'rollback ante fallo SQL: stock, detalles e historial intactos'})
    finally:
        sql_test('limpiar-fallo')
    def consumir(orden_id):
        return req('POST',f'/api/OrdenMantenimiento/{orden_id}/repuestos',
            {'repuestos':[{'repuestoId':rid1,'cantidad':5}]},coord,expected=None)[1]
    with ThreadPoolExecutor(max_workers=2) as pool:
        statuses=list(pool.map(consumir,[o1,o2]))
    assert all(s in (200,409) for s in statuses), statuses
    after=stock(rid1)
    assert after==before-5 and after>=0
    RESULTADOS.append({'caso':'consumos concurrentes: sin stock negativo','estados':statuses})
    # Auditoría de cambios administrativos; reenvío del mismo estado no duplica.
    free=activo('VEH-QA-PROCESO-003')['id']
    call('PATCH',f'Activo/{free}/estado',{'estado':2})
    call('PATCH',f'Activo/{free}/estado',{'estado':2})
    assert len(call('GET',f'Activo/{free}/historial'))==1
    # Los tokens previos dejan de funcionar al cambiar rol o desactivar usuario.
    call('PUT',f"Usuario/{coord_user['id']}",{'login':'coordqa','nombreCompleto':'Actualizado',
        'correo':'coordqa@qa.example','rolId':roles['Solicitante/Consulta']})
    req('GET','/api/Activo',token=coord,expected=401)
    call('DELETE',f"Usuario/{s1_user['id']}")
    req('GET','/api/Activo',token=s1,expected=401)
    req('POST','/api/Auth/login',{'login':'solicqa1','password':PASSWORD},expected=401)
    rejected=req('POST','/api/Auth/login',{'login':'admin','password':'incorrecta'},expected=429)
    assert rejected[0]['errorCode']=='RATE_LIMITED'
    assert rejected[2].get('Retry-After') is not None
    req('GET','/api/Activo',token=admin) # el límite solo afecta login

def main():
    with socket.socket() as port_check:
        try:
            port_check.bind(('127.0.0.1', 5399))
        except OSError:
            raise RuntimeError('El puerto 5399 está ocupado; no se ejecutarán pruebas sobre otro servidor.')
    global QA_CONNECTION
    project=ET.parse(API/'MantenimientoFlota.Api.csproj')
    secret_id=project.find('.//UserSecretsId').text
    secret_path=Path(os.environ['APPDATA'])/'Microsoft/UserSecrets'/secret_id/'secrets.json'
    settings=json.loads(secret_path.read_text(encoding='utf-8-sig'))
    connection=settings.get('ConnectionStrings:DefaultConnection') or settings.get('ConnectionStrings',{}).get('DefaultConnection')
    if not connection:
        raise RuntimeError('Falta la conexión local en user-secrets.')
    if not re.search(r'(?i)(database|initial catalog)\s*=',connection):
        raise RuntimeError('No se encontró el catálogo; no se iniciará la prueba.')
    connection=re.sub(r'(?i)(database|initial catalog)\s*=\s*[^;]*', 'Database='+DB,connection)
    QA_CONNECTION=connection
    output=ROOT/'tests/resultados-belen'
    output.mkdir(exist_ok=True)
    env={**os.environ,'ASPNETCORE_ENVIRONMENT':'Development','DOTNET_ENVIRONMENT':'Development',
         'ConnectionStrings__DefaultConnection':connection,'Seed__Admin__Password':PASSWORD,
         'Jwt__Key':secrets.token_urlsafe(48),'Jwt__Issuer':'Flota-QA','Jwt__Audience':'Flota-QA',
         'Jwt__ExpirationMinutes':'60','ApiLogs__Directorio':str(output/'logs'),
         'ActivoImagenes__Directorio':str(output/'imagenes')}
    # No heredar una variable temporal con el nombre incorrecto usado en sesiones anteriores.
    env.pop('ConnectionStringsDefaultConnection',None)
    log=(output/'servidor.log').open('w',encoding='utf-8')
    proc=subprocess.Popen(['dotnet',str(API/f'bin/{CONFIGURATION}/net10.0/MantenimientoFlota.Api.dll'),
        '--urls',BASE],cwd=API,env=env,stdout=log,stderr=subprocess.STDOUT)
    ok=False
    try:
        for _ in range(120):
            if proc.poll() is not None:
                raise RuntimeError('La API de pruebas no inició; revisar tests/resultados-belen/servidor.log.')
            try:
                urlopen(BASE+'/openapi/v1.json',timeout=1).close()
                break
            except (URLError,TimeoutError): time.sleep(.5)
        else: raise RuntimeError('La API no respondió a tiempo.')
        verify()
        ok=True
    finally:
        proc.terminate()
        try: proc.wait(timeout=10)
        except subprocess.TimeoutExpired: proc.kill()
        log.close()
        (output/'resumen.json').write_text(json.dumps({'base':DB,'correcto':ok,
            'verificaciones':len(RESULTADOS),'resultados':RESULTADOS},ensure_ascii=False,indent=2),encoding='utf-8')
        print(f'Base de pruebas: {DB}')
        print(f'Verificaciones HTTP y de concurrencia: {len(RESULTADOS)}; correcto: {ok}')

if __name__=='__main__': main()
