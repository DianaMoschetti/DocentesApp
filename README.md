# DocentesApp

## Configuración de la clave JWT (obligatoria)

La API firma los tokens de login con una clave secreta (`Jwt:Key`). Esa clave **no está en el repositorio**: `appsettings.json` ya no la tiene. Cada persona configura la suya en su máquina con *user secrets* de .NET, que se guardan fuera del proyecto y no se suben a git.

Si falta la clave, la API no arranca y muestra este error:

```
Falta la clave JWT (Jwt:Key); no se guarda en appsettings.json. En desarrollo configurala con: dotnet user-secrets set "Jwt:Key" "<clave>" --project DocentesApp.API ...
```

### Pasos (una sola vez por máquina)

Desde la carpeta raíz del repositorio, en PowerShell:

1. Generá una clave aleatoria de 64 bytes:

   ```powershell
   $bytes = New-Object byte[] 64
   [System.Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($bytes)
   $clave = [Convert]::ToBase64String($bytes)
   ```

2. Guardala como user secret de la API:

   ```powershell
   dotnet user-secrets set "Jwt:Key" $clave --project DocentesApp.API
   ```

3. Verificá que quedó configurada (debe aparecer `Jwt:Key = ...`):

   ```powershell
   dotnet user-secrets list --project DocentesApp.API
   ```

Después de esto, la API arranca normalmente desde Visual Studio o con `dotnet run`.

### Para tener en cuenta

- **Cada máquina tiene su propia clave.** Un token generado en una máquina no sirve en otra, y está bien que sea así: no hace falta (ni conviene) compartir la clave por chat o mail.
- **Mínimo 32 bytes.** Si la clave es más corta, la API no arranca. La del paso 1 tiene 64 bytes.
- **Si cambiás la clave**, los usuarios logueados tienen que volver a iniciar sesión: sus tokens dejan de ser válidos.
- **La clave vieja ya no sirve.** Antes la clave estaba en `appsettings.json` y sigue visible en el historial de git, pero la API ya no la usa: cada máquina usa una clave nueva.
- **Fuera de desarrollo**, los user secrets no se usan. Ahí la clave se configura con la variable de entorno `Jwt__Key` (con dos guiones bajos).
- **Tests:** los tests de integración usan su propia clave de prueba (`CustomWebApplicationFactory.TestJwtKey`), así que `dotnet test` funciona sin configurar nada.
