-- SOLO PARA PRUEBAS LOCALES del bloque 5 (no ejecutar en un ambiente compartido).
-- Crea un Coordinador/Técnico y dos Solicitantes/Consulta con la contraseña: Prueba.Flota2026!
-- Cuando exista la administración de usuarios (bloque 6), bórrelos y créelos desde la API.
-- Ejecutar con la base MantenimientoFlotaDb seleccionada. Es seguro repetirlo.

DECLARE @hash NVARCHAR(512) = N'$2b$12$7bSn/vsOes9vC3HF7bDr3uxKcz1rTPjSx8n1vbKBFzXAoyqP74GVC';

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Login = N'coord1')
    INSERT INTO Usuarios (RolId, Login, PasswordHash, NombreCompleto, Correo, Activo, FechaCreacionUtc)
    VALUES (2, N'coord1', @hash, N'Coordinador de Prueba', N'coord1@flota.example', 1, SYSUTCDATETIME());

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Login = N'solic1')
    INSERT INTO Usuarios (RolId, Login, PasswordHash, NombreCompleto, Correo, Activo, FechaCreacionUtc)
    VALUES (3, N'solic1', @hash, N'Solicitante Uno', N'solic1@flota.example', 1, SYSUTCDATETIME());

IF NOT EXISTS (SELECT 1 FROM Usuarios WHERE Login = N'solic2')
    INSERT INTO Usuarios (RolId, Login, PasswordHash, NombreCompleto, Correo, Activo, FechaCreacionUtc)
    VALUES (3, N'solic2', @hash, N'Solicitante Dos', N'solic2@flota.example', 1, SYSUTCDATETIME());

SELECT u.Id, u.Login, r.Nombre AS Rol, u.Activo
FROM Usuarios u JOIN Roles r ON r.Id = u.RolId;
