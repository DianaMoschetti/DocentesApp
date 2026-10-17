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

## Roles y usuarios iniciales

### Roles

Cada usuario tiene **un solo rol**:

| Rol | Puede |
|---|---|
| **Administrador** | Todo, incluida la gestión de usuarios. |
| **Editor** | Todo menos la gestión de usuarios: altas, modificaciones y bajas de datos y de catálogos (Puntos por cargo, UDB, Asignaturas, Cursos). |
| **Consultante** | Solo consultar: listados, detalles y reportes. |

Los controllers y las páginas no usan los nombres de los roles sino **políticas** (`DocentesApp.Shared/Auth/Politicas.cs`): `PuedeConsultar`, `PuedeEditar` y `EsAdministrador`.

**Bases existentes:** los roles viejos se renombran solos al arrancar la API (`Admin` → `Administrador`, `User` → `Consultante`). Los usuarios conservan su rol; no hace falta resembrar la base. Los usuarios logueados antes del cambio tienen que volver a iniciar sesión, porque su token tiene el nombre viejo del rol.

### Usuarios que se crean al arrancar

| Usuario | Rol | Contraseña | Entornos |
|---|---|---|---|
| `admin@docentesapp.com` (`admin`) | Administrador | `Seed:AdminPassword` (ver abajo) | Todos |
| `editor@docentesapp.com` (`editor`) | Editor | `Editor123!` | Solo Development y Testing |
| `user@docentesapp.com` (`user`) | Consultante | `User123!` | Solo Development y Testing |

Solo se crean si todavía no existen: si el usuario ya está en la base, no se toca (ni su contraseña).

### Contraseña del administrador inicial (`Seed:AdminPassword`)

- **En desarrollo** no hace falta configurar nada: si falta, se usa `Admin123!`. Si querés otra, guardala como user secret:

  ```powershell
  dotnet user-secrets set "Seed:AdminPassword" "<contraseña>" --project DocentesApp.API
  ```

- **Fuera de desarrollo** es obligatoria **la primera vez** (cuando el usuario `admin` todavía no existe). Se configura con la variable de entorno `Seed__AdminPassword` (con dos guiones bajos). Si falta, la API no arranca y muestra este error:

  ```
  Falta la contraseña del administrador inicial (Seed:AdminPassword); no se guarda en appsettings.json. Configurala con la variable de entorno Seed__AdminPassword ...
  ```

- Tiene que cumplir las reglas de contraseña de la API (al menos 6 caracteres, con un número y una minúscula). Si no las cumple, la API no arranca y dice por qué.
- Una vez creado el admin, la variable ya no se usa: cambiar la contraseña después se hace desde la aplicación.
