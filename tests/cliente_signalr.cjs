const readline = require('node:readline');
const {createRequire} = require('node:module');
const requireProject = createRequire(require('node:path').resolve(__dirname, '../frontend/package.json'));
const signalR = requireProject('@microsoft/signalr');
const lines = readline.createInterface({input: process.stdin});
let connection;
const timeout = setTimeout(() => process.exit(2), 180000);
function output(value) { process.stdout.write(JSON.stringify(value) + '\n'); }
lines.on('line', async line => {
    const command = JSON.parse(line);
    try {
        if (command.resubscribe) { await connection.invoke('SuscribirseStock'); output({tipo:'suscrito'}); return; }
        if (command.stop) {
            await connection?.stop();
            clearTimeout(timeout);
            process.exit(0);
        }
        connection = new signalR.HubConnectionBuilder()
            .withUrl(command.url, {accessTokenFactory: () => command.token})
            .configureLogging(signalR.LogLevel.None)
            .build();
        connection.on('StockSesionRevocada', () => output({tipo:'revocada'}));
        connection.on('StockBajoDetectado', alerta => output({tipo:'alerta', alerta}));
        await connection.start();
        await connection.invoke('SuscribirseStock');
        output({tipo:'conectado'});
    } catch (error) {
        output({tipo:'error', nombre:error.name});
        clearTimeout(timeout);
        process.exit(1);
    }
});
