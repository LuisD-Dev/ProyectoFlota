# Revisión de SignalR e integración — 26/11/2026

Se verificó la implementación local, sin hacer commit ni push. Las operaciones de prueba se ejecutaron exclusivamente en bases nuevas con prefijo `MantenimientoFlotaDb_QA_`. No se cambiaron las conexiones, los secretos ni los registros de la base habitual. Los servidores temporales de pruebas se detuvieron al finalizar.

## Correcciones

- Una sola política CORS registrada antes de construir la aplicación; middleware CORS situado antes de autenticación y del Hub. Admite los encabezados usados por SignalR y credenciales únicamente para los orígenes configurados.
- El Hub solo admite administradores y coordinadores. El token por query se lee únicamente en la ruta del Hub.
- Se comprueba nuevamente el usuario al suscribirse y antes de publicar. Las conexiones de cuentas desactivadas o con login/rol diferente se retiran del grupo y reciben un aviso privado de sesión revocada; React borra el token y vuelve al login.
- Alertas al cruzar desde stock igual/superior al mínimo hacia stock inferior; no se duplica un aviso por seguir bajo mínimo. La suscripción repetida tampoco duplica entregas.
- El consumo de órdenes prepara las alertas dentro de la transacción y las publica después de confirmar. Un fallo SQL no genera avisos de consumos revertidos.
- Limpieza de conexiones y promesas del hook, incluyendo el montaje doble de React StrictMode en desarrollo.
- Fechas UTC de órdenes recuperadas desde SQL sin zona: se interpretan como UTC antes de mostrarlas, evitando seis horas de desfase en Costa Rica. Las fechas de adquisición, que no tienen hora, conservan su manejo anterior.
- Dependencias actualizadas dentro del rango de shadcn; auditoría npm completa con cero vulnerabilidades.

## Resultado de las comprobaciones

- Backend compilado: cero errores y cero advertencias del compilador. Se usó la configuración `SignalRQA` para evitar bloquear los archivos de la API que estaba abierta.
- Frontend: build correcto y lint correcto.
- `git diff --check`: correcto.
- Regresión de permisos, usuarios y proceso transaccional: **107 verificaciones**, correctas. Base: `MantenimientoFlotaDb_QA_Belen_20261126_172807`.
- Suite SignalR real, HTTP y rollback: **49 verificaciones registradas**, correctas. Base final: `MantenimientoFlotaDb_QA_Belen_SignalR_20261126_173639`.
- Navegador: login y validaciones obligatorias, listado/formulario de activos, categorías, listado/detalle de órdenes y consulta de stock. Se comprobó que un consumo real de una orden (6 → 4, mínimo 5) aparece sin recargar y actualiza la consulta del inventario.
- Navegador: al cambiar el rol de un coordinador conectado, el siguiente aviso provoca la revocación de su suscripción y el retorno automático al login. También se comprobó con clientes reales que una cuenta desactivada deja de recibir eventos de inventario.

La suite de SignalR cubre CORS, 401 sin autenticación, 403 para solicitantes, acceso de coordinador, eventos reales, igualdad con el mínimo, cruce de umbral, permanencia bajo mínimo, reposición, consumo transaccional, suscripción repetida, rollback y dos receptores autorizados.

## Repetir las pruebas

Desde la raíz del repositorio, con SQL local y user-secrets configurados, Node y Python disponibles:

```powershell
dotnet build src/MantenimientoFlota.Api/MantenimientoFlota.Api.csproj -c SignalRQA --no-restore
dotnet build tests/SqlQa/SqlQa.csproj -c SignalRQA --no-restore
$env:FLOTA_QA_CONFIGURATION = "SignalRQA"
python tests/verificar_bloques_belen.py
python tests/verificar_signalr.py
```

El frontend debe tener sus dependencias instaladas. Cada ejecución genera su propia base; no borra ninguna. Si 5399 o 5401 están ocupados, las pruebas se detienen antes de operar. Los resultados y logs se guardan en `tests/resultados-belen/` y `tests/resultados-signalr/`, ignorados por Git. No se guardan tokens ni contraseñas en los reportes.

## Límites conocidos

Vite mantiene un aviso de tamaño del archivo JavaScript (aproximadamente 756 kB sin comprimir). La compilación termina correctamente; queda como mejora de rendimiento dividir las páginas en cargas separadas.

Los eventos son avisos en vivo, sin historial persistente ni entrega garantizada. La consulta HTTP muestra el inventario actual al entrar, reconectar o actualizar. Esta revisión no simula una caída completa de la red ni un despliegue en producción y no equivale a garantizar ausencia absoluta de fallos.
