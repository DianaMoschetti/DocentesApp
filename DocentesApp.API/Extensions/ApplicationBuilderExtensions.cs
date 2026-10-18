using DocentesApp.Data.Context;
using DocentesApp.Data.Identity;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task SeedIdentityDataAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var environment = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();

            // 1) Roles. Los roles viejos se renombran (no se migran usuarios): AspNetUserRoles apunta al Id del rol,
            //    así que los usuarios que tenían Admin/User quedan con Administrador/Consultante. Es idempotente.
            await RenombrarRolAsync(roleManager, "Admin", Roles.Administrador);
            await RenombrarRolAsync(roleManager, "User", Roles.Consultante);

            foreach (var role in Roles.Todos)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Development y Testing: usuarios de prueba y contraseña del admin por defecto (ver README)
            var esEntornoDePrueba = environment.IsDevelopment() || environment.IsEnvironment("Testing");

            // 2) Administrador inicial, en todos los entornos. La contraseña solo se usa si el usuario todavía no existe.
            const string adminEmail = "admin@docentesapp.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminPassword = configuration["Seed:AdminPassword"];
                if (string.IsNullOrWhiteSpace(adminPassword))
                {
                    if (!esEntornoDePrueba)
                        throw new InvalidOperationException(
                            "Falta la contraseña del administrador inicial (Seed:AdminPassword); no se guarda en appsettings.json. " +
                            "Configurala con la variable de entorno Seed__AdminPassword (ver README). " +
                            "Solo se usa la primera vez, cuando el usuario admin todavía no existe.");

                    adminPassword = "Admin123!"; // solo Development/Testing
                }

                await CrearUsuarioAsync(userManager, adminEmail, "admin", "Administrador", "Sistema", adminPassword, Roles.Administrador);
            }

            // 3) Usuarios de prueba: solo en Development y Testing
            if (esEntornoDePrueba)
            {
                if (await userManager.FindByEmailAsync("editor@docentesapp.com") == null)
                    await CrearUsuarioAsync(userManager, "editor@docentesapp.com", "editor", "Usuario", "Editor", "Editor123!", Roles.Editor);

                if (await userManager.FindByEmailAsync("user@docentesapp.com") == null)
                    await CrearUsuarioAsync(userManager, "user@docentesapp.com", "user", "Usuario", "Consulta", "User123!", Roles.Consultante);
            }
        }

        // Renombra un rol conservando su Id (y por lo tanto sus usuarios). No hace nada si el viejo no existe o el nuevo ya existe.
        private static async Task RenombrarRolAsync(RoleManager<IdentityRole> roleManager, string nombreViejo, string nombreNuevo)
        {
            var rolViejo = await roleManager.FindByNameAsync(nombreViejo);
            if (rolViejo == null || await roleManager.RoleExistsAsync(nombreNuevo))
                return;

            await roleManager.SetRoleNameAsync(rolViejo, nombreNuevo);
            var result = await roleManager.UpdateAsync(rolViejo); // UpdateAsync también actualiza NormalizedName
            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"No se pudo renombrar el rol '{nombreViejo}' a '{nombreNuevo}': {string.Join(" | ", result.Errors.Select(e => e.Description))}");
        }

        // Antes un error al crear el usuario se ignoraba en silencio (y quedaba sin admin); ahora corta el arranque con el motivo
        private static async Task CrearUsuarioAsync(UserManager<ApplicationUser> userManager,
            string email, string userName, string nombre, string apellido, string password, string rol)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                Nombre = nombre,
                Apellido = apellido,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
                result = await userManager.AddToRoleAsync(user, rol);

            if (!result.Succeeded)
                throw new InvalidOperationException(
                    $"No se pudo crear el usuario inicial '{email}': {string.Join(" | ", result.Errors.Select(e => e.Description))}");
        }

        public static async Task SeedDocentesAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<DocentesDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ApplicationBuilderExtensions));

            try
            {
                if (await context.Docentes.AnyAsync())
                    return;

                // Legajo, Apellido, Nombre, Dni
                var docentesData = new List<(int Legajo, string Apellido, string Nombre, string? Dni)>
                {
                    (93509, "ALLEMAND", "FLORENCIA NOELIA", "35249926"),
                    (89207, "AMAYA", "CARINA ALEJANDRA", "22068886"),
                    (46080, "AMAYA", "VANINA ALEJANDRA", "24282204"),
                    (72396, "ANGIORAMA", "MARINA CELESTE", "30025904"),
                    (96671, "AZURMENDI", "MARCELO", "43802695"),
                    (27824, "BAETTI", "JORGE RAUL", "14143596"),
                    (91679, "BAGILET", "MARIA GUILLERMINA", "34905787"),
                    (80345, "BARREA", "LEONARDO DAMIAN", "28101697"),
                    (91930, "BERGER", "LAURA ALEJANDRA", "28530039"),
                    (89817, "BETTUCCI", "PATRICIA NORA", "25178574"),
                    (76326, "BOLOGNA", "MARIA NOEL", "25328559"),
                    (58471, "BORRELL", "JUAN JOSE", "25900154"),
                    (91931, "BRAVO", "BARBARA", "30069748"),
                    (82408, "BRSTILO", "CAREN LORELEY", "32015909"),
                    (87520, "CABRAL", "JULIA", "32587503"),
                    (95310, "CABRERA", "MIGUEL", "30741748"),
                    (18982, "CABRERA", "RITA ALCIRA", "13580796"),
                    (88249, "CALUARI", "DAVID LISANDRO", "26358716"),
                    (89685, "CAMANI", "ALCIRA SUSANA", "23645418"),
                    (93331, "CANTABILE", "MARIA LAURA", "25715909"),
                    (92338, "CAPPONI", "FLORENCIA", "32779852"),
                    (50784, "CARRACEDO", "ALBERTO", "21415266"),
                    (36007, "CARRANZA", "CARLOS ALBERTO", "18436812"),
                    (56746, "CELIS", "MARIA BELEN", "27033374"),
                    (87475, "CIANCIARDO", "CINTIA GEORGINA", "25007251"),
                    (95131, "CORETTI", "MAURO", "26883084"),
                    (22863, "CRER", "DANIEL ELIAS", "18490919"),
                    (75257, "D'ALESSANDRO", "LUCAS IVAN", "32957939"),
                    (38137, "DE FEDERICO", "SARA ESTER", "16778156"),
                    (89816, "DE SANCTIS", "MARIANA", "12788391"),
                    (92472, "DE VITO", "MARCOS LUCIANO", "32078064"),
                    (58470, "DEL GRECO", "DANIEL EDGARDO", "13169996"),
                    (47016, "DIANDA", "PATRICIA MONICA", "18094360"),
                    (19576, "DOBBOLETTA", "ELSA BEATRIZ", "14453126"),
                    (90541, "DONZELLI", "VALERIA CARLA", "25505380"),
                    (80347, "FACCIANO", "MARIA LUCRECIA", "32166204"),
                    (89219, "FAVIERE", "GABRIELA SOLEDAD", "36632123"),
                    (88248, "FREIRE", "MARTIN MIGUEL", "32189935"),
                    (26775, "GAGO", "EDUARDO ALBERTO", "16852380"),
                    (74495, "GALETTI", "VALERIA MARIA", "34171791"),
                    (87091, "GALLO", "ALEJANDRO NICOLAS", "30313582"),
                    (96063, "GARCIA", "FRANCO", "42124462"),
                    (94893, "GONZALEZ", "IRENE", "33562282"),
                    (58475, "GRANERO FERRER", "MARIA BELEN", "27620755"),
                    (32289, "GUTIERREZ", "GABRIELA", "21689815"),
                    (29812, "HEIT", "FERNANDO ARIEL", "22681592"),
                    (55179, "HERNANDEZ", "SILVANA LAURA", "29200982"),
                    (91241, "HORTAL", "MARIA LARA", "36571217"),
                    (91678, "IBARS", "EZEQUIEL GERVASIO", "23185240"),
                    (96855, "LAMBRETCH", "JUAN", "41604211"),
                    (94873, "LANDALUCE", "NATALIA", "32217127"),
                    (93468, "LENARDUZZI", "NATALIA AGUSTINA", "41634894"),
                    (59540, "LOPEZ", "VANINA MARIA", "30256069"),
                    (19899, "LUCERO", "HECTOR DANIEL", "13326794"),
                    (88210, "MACAT", "PAULA BELEN", "32648004"),
                    (87786, "MANSILLA", "ANDREA ELISABETH", "30089890"),
                    (33975, "MANSILLA", "SANDRA MARIA", "20173596"),
                    (72397, "MARINI", "ANALIA LAURA", "24386284"),
                    (41197, "MARTINEZ", "DIANA ELINA", "24675959"),
                    (83153, "MARVULLI", "HECTOR NAHUEL", "34168715"),
                    (80881, "MASETRO", "ADRIAN MAURICIO", "26835632"),
                    (80493, "MASSACCESI", "GUSTAVO", "28398924"),
                    (86247, "MASSACHIODI", "VERONICA ANDREA", "24586347"),
                    (88818, "MASSON", "ROMAN ANTONIO", "37831960"),
                    (87930, "MAULION", "EVANGELINA", "32287545"),
                    (90183, "MAURICI AQUILANO", "BRENDA ARIANA", "31340212"),
                    (45873, "MEOLI", "JORGELINA JULIA", "18242587"),
                    (73637, "MERLO", "ROSANA LUJAN", "24868109"),
                    (60159, "MORZAN", "MARINA", "23370710"),
                    (52643, "MOSCHETTI", "DIANA INES", "30170088"),
                    (55495, "MUNOZ", "LORENA RAQUEL", "24282908"),
                    (89579, "NUNEZ", "JULIETA RAYEN", "36009041"),
                    (45682, "OLIVA", "ALICIA DELIA", "12520043"),
                    (90540, "ORONA", "NAIR ALE", "37450017"),
                    (91632, "PAGLIARO", "CARLA CLARA", "36657896"),
                    (92205, "PALLOTTI", "MARIELA INES", "22337676"),
                    (18922, "PANERO", "ARIEL RAUL", "13528506"),
                    (19980, "PELLEGRINI", "JULIA INES", "12527762"),
                    (96743, "PERALTA", "LETICIA", "26739666"),
                    (26756, "PEREZ SOTTILE", "RICARDO", "16985592"),
                    (66213, "PEREZ", "MARIANA DEL VALLE", "22828780"),
                    (95028, "PIETROBON", "ORNELLA", "42609063"),
                    (33128, "POMATA", "DANIELA NOEMI", "18113040"),
                    (86615, "PROPERZI", "MARIA FLORENCIA", "34168691"),
                    (87092, "RAMINI", "GIULIANA", "35641998"),
                    (63411, "RIVAS", "PABLO ANDRES", "27291129"),
                    (90847, "RODRIGUEZ", "JUAN MANUEL", "28536160"),
                    (63916, "RODRIGUEZ", "MARCELO CARLOS", "16674373"),
                    (94540, "ROMERO", "MARTIN", "22777895"),
                    (77047, "ROMERO", "MATIAS FRANCISCO", "30256072"),
                    (91240, "ROMERO", "SILVANA GABRIELA", "33961611"),
                    (91330, "RUGGIERO", "FRANCO LORENZO", "33318232"),
                    (59229, "SABATINELLI", "PABLO AGUSTIN", "26066065"),
                    (77312, "SANTA CRUZ", "BERENICE", "33733323"),
                    (75314, "SANTA CRUZ", "JUDITH AILEN", "31990490"),
                    (58891, "SARGES GUERRA", "ACACIO", "17025047"),
                    (87476, "SEVERINO", "MARCELO", "18406819"),
                    (77331, "SFULCINI", "FABRICIO CARLOS", "30256414"),
                    (41826, "SILVESTER", "SANDRA HAYDEE", "17413385"),
                    (29760, "SILVESTER", "SILVIA ALEJANDRA", "16464406"),
                    (80228, "SORIA", "JORGELINA", "27971026"),
                    (51613, "SORRENTI", "JUAN PABLO", "22592055"),
                    (91929, "SOTO", "MARTIN EDUARDO", "23032093"),
                    (46077, "STOPPANI", "FERNANDO SANTIAGO", "25073117"),
                    (44951, "SZEKIETA", "PAOLA ANDREA", "25453485"),
                    (87923, "SZELIGA", "MARIA INES", "26642019"),
                    (92030, "TALARN", "LUCIANA MARIA", "28697213"),
                    (93498, "TASADA", "FLORENCIA", "29311466"),
                    (87656, "TASSONE", "NATALIA MARINA", "34496636"),
                    (26796, "TORRES", "DIEGO FERMIN", "17229254"),
                    (76965, "TRESALLI", "CRISTIAN ARIEL", "34933790"),
                    (77332, "TULLIANI", "MARIO LUIS", "28575959"),
                    (91935, "TZIRIMIS", "NOELIA FERNANDA", "31695860"),
                    (88243, "VACCARO", "GUILLERMO JOSE MARIA", "21889201"),
                    (19013, "VALERO", "MARCELO NORBERTO", "14758285"),
                    (59235, "VIGNADUZZO", "BIBIANA SUSANA", "12725754"),
                    (92990, "VITALE", "TATIANA", "34477008"),
                    (23718, "VOZZI", "ANA MARIA", "12720841"),
                    (58449, "ZANCHETTA", "MARIA ALEJANDRA", "22068933"),
                    (89594, "ZURBRIGGEN", "MARCELO MATIAS", "35584579"),
                    // Sin DNI - solo en plantas
                    (93130, "BEHR", "BERNARDO", null),
                    (95782, "CHEIJ", "ROSANA", null),
                    (96143, "LONDERO", "CAROLINA", null),
                    (96142, "TOWNSEND", "DAMARIS", null),
                    (95536, "VACCHINO", "LUCAS", null),
                };

                var docentes = docentesData.Select(d => new Docente
                {
                    Legajo = d.Legajo,
                    Apellido = d.Apellido,
                    Nombre = d.Nombre,
                    Dni = d.Dni
                }).ToList();

                await context.Docentes.AddRangeAsync(docentes);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al seedear Docentes");
            }
        }

        public static async Task SeedUdbsAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<DocentesDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ApplicationBuilderExtensions));

            try
            {
                if (await context.Udbs.AnyAsync())
                    return;

                // Nombre, Legajo Director, Legajo Secretario
                var udbsData = new List<(string Nombre, int? DirectorLegajo, int? SecretarioLegajo)>
                {
                    ("Dirección de Básicas", 41826, 46077),
                    ("UDB Matemática", 26775, null),
                    ("UDB Física", 45682, 18922),
                    ("UDB Química", 73637, null),
                    ("UDB Sistemas de Representación", 26796, null),
                    ("UDB Legislación y Economía", 27824, null),
                    ("UDB Cultura e Idiomas", 19576, 63411),
                    ("Laboratorio de Informática", 44951, null),
                };

                var udbs = udbsData.Select(u => new Udb { Nombre = u.Nombre }).ToList();

                await context.Udbs.AddRangeAsync(udbs);
                await context.SaveChangesAsync();

                // Setear Director y Secretario buscando el Id del docente por su legajo
                var legajos = udbsData
                    .SelectMany(u => new[] { u.DirectorLegajo, u.SecretarioLegajo })
                    .Where(l => l.HasValue)
                    .Select(l => l!.Value)
                    .Distinct();

                var docentesPorLegajo = await context.Docentes
                    .AsNoTracking()
                    .Where(d => d.Legajo.HasValue && legajos.Contains(d.Legajo.Value))
                    .ToDictionaryAsync(d => d.Legajo!.Value, d => d.Id);

                for (var i = 0; i < udbsData.Count; i++)
                {
                    var data = udbsData[i];
                    var udb = udbs[i];

                    if (data.DirectorLegajo.HasValue && docentesPorLegajo.TryGetValue(data.DirectorLegajo.Value, out var directorId))
                        udb.DirectorDocenteId = directorId;

                    if (data.SecretarioLegajo.HasValue && docentesPorLegajo.TryGetValue(data.SecretarioLegajo.Value, out var secretarioId))
                        udb.SecretarioDocenteId = secretarioId;
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al seedear Udbs");
            }
        }

        public static async Task SeedDocenteUdbAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<DocentesDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ApplicationBuilderExtensions));

            try
            {
                if (await context.DocenteUdbs.AnyAsync())
                    return;

                // Nombre de la Udb -> legajos de los docentes que pertenecen a ella
                var membresias = new Dictionary<string, int[]>
                {
                    ["Dirección de Básicas"] = new[] { 41826, 46077, 19013, 51613, 52643 },
                    ["UDB Matemática"] = new[]
                    {
                        46080, 72396, 96671, 80345, 76326, 87520, 18982, 88249, 89685, 93331, 92338,
                        56746, 87475, 92472, 38137, 32289, 55179, 91241, 91678, 94873, 93468, 59540, 88210,
                        87786, 33975, 72397, 83153, 80881, 80493, 87930, 90183, 60159, 55495, 89579, 91632,
                        92205, 19899, 87092, 90847, 91240, 91330, 75257, 94893, 95310, 93498, 87656, 76965,
                        88243, 80228, 91929, 95782, 96063, 96855, 94540, 77047, 77312, 75314, 86615, 92030,
                        96143, 96142, 95536, 93130, 26775, 66213
                    },
                    ["UDB Física"] = new[]
                    {
                        45682, 18922, 26756, 88249, 41197, 45873, 80493, 58470, 58891, 29760, 75314,
                        95131, 59235, 93331, 74495, 87092, 83153, 77331, 91241, 87476, 88818, 96743, 80347,
                        89594, 91930, 91935, 87091
                    },
                    ["UDB Química"] = new[] { 73637 },
                    ["UDB Sistemas de Representación"] = new[] { 26796, 36007, 47016 },
                    ["UDB Legislación y Economía"] = new[] { 27824, 19980, 89685, 86247, 80228, 93509, 50784, 58475, 92990 },
                    ["UDB Cultura e Idiomas"] = new[] { 19576, 63411, 95028, 90541, 89816, 90540, 91330, 91679, 89817 },
                    ["Laboratorio de Informática"] = new[]
                    {
                        44951, 26775, 82408, 92338, 75257, 89219, 96063, 55179, 38137, 89579,
                        86615, 90847, 77047, 91240, 89594
                    },
                };

                var docentesPorLegajo = await context.Docentes
                    .AsNoTracking()
                    .Where(d => d.Legajo.HasValue)
                    .ToDictionaryAsync(d => d.Legajo!.Value, d => d.Id);

                var udbsPorNombre = await context.Udbs
                    .AsNoTracking()
                    .ToDictionaryAsync(u => u.Nombre, u => u.Id);

                var docenteUdbs = new List<DocenteUdb>();

                foreach (var (nombreUdb, legajos) in membresias)
                {
                    if (!udbsPorNombre.TryGetValue(nombreUdb, out var udbId))
                        continue;

                    foreach (var legajo in legajos)
                    {
                        if (!docentesPorLegajo.TryGetValue(legajo, out var docenteId))
                            continue;

                        docenteUdbs.Add(new DocenteUdb
                        {
                            DocenteId = docenteId,
                            UdbId = udbId,
                            Rol = RolUdb.Docente
                        });
                    }
                }

                await context.DocenteUdbs.AddRangeAsync(docenteUdbs);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al seedear DocenteUdb");
            }
        }

        // Inserta solo las asignaturas que faltan (por UDB + Nombre, sin distinguir mayúsculas; el Nivel no se compara).
        // Nunca actualiza ni borra las existentes: las cargadas a mano no se duplican.
        // Nivel = el nivel más habitual; algunas asignaturas se dictan en distintos años según la carrera
        // (el año real lo da el Curso).
        public static async Task SeedAsignaturasAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<DocentesDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(ApplicationBuilderExtensions));

            try
            {
                // Nombre de la Udb (igual que en SeedUdbsAsync), Nombre, Nivel, Frecuencia
                var asignaturasData = new List<(string Udb, string Nombre, Nivel Nivel, Frecuencia Frecuencia)>
                {
                    ("UDB Matemática", "Análisis Matemático I", Nivel.Primero, Frecuencia.Anual),
                    ("UDB Matemática", "Álgebra y Geometría Analítica", Nivel.Primero, Frecuencia.Anual),
                    ("UDB Matemática", "Análisis Matemático II", Nivel.Segundo, Frecuencia.Anual),
                    ("UDB Matemática", "Probabilidad y Estadística", Nivel.Segundo, Frecuencia.Anual),

                    ("UDB Física", "Física I", Nivel.Primero, Frecuencia.Anual),
                    ("UDB Física", "Física II", Nivel.Segundo, Frecuencia.Anual),
                    ("UDB Física", "Taller de Óptica Geométrica", Nivel.Primero, Frecuencia.Taller),

                    ("UDB Química", "Química General", Nivel.Primero, Frecuencia.Anual),
                    ("UDB Química", "Química Aplicada a la Informática", Nivel.Segundo, Frecuencia.Cuatrimestral),

                    ("UDB Cultura e Idiomas", "Inglés I", Nivel.Primero, Frecuencia.Anual),
                    ("UDB Cultura e Idiomas", "Inglés II", Nivel.Segundo, Frecuencia.Anual),
                    ("UDB Cultura e Idiomas", "Ingeniería y Sociedad", Nivel.Primero, Frecuencia.Anual),

                    ("UDB Legislación y Economía", "Economía", Nivel.Tercero, Frecuencia.Anual),
                    ("UDB Legislación y Economía", "Legislación", Nivel.Cuarto, Frecuencia.Anual),
                    ("UDB Legislación y Economía", "Organización y Administración de Empresas", Nivel.Quinto, Frecuencia.Anual),

                    ("UDB Sistemas de Representación", "Sistemas de Representación", Nivel.Primero, Frecuencia.Anual),
                };

                var udbsPorNombre = await context.Udbs
                    .AsNoTracking()
                    .ToDictionaryAsync(u => u.Nombre, u => u.Id);

                // Asignaturas existentes como (UdbId, Nombre) para no duplicarlas
                var existentes = (await context.Asignaturas
                        .AsNoTracking()
                        .Select(a => new { a.UdbId, a.Nombre })
                        .ToListAsync())
                    .Select(a => (a.UdbId, Nombre: a.Nombre.Trim().ToUpperInvariant()))
                    .ToHashSet();

                var nuevas = new List<Asignatura>();

                foreach (var data in asignaturasData)
                {
                    if (!udbsPorNombre.TryGetValue(data.Udb, out var udbId))
                    {
                        logger.LogWarning("Seed de asignaturas: no se encontró la UDB '{Udb}'; se omite '{Asignatura}'.", data.Udb, data.Nombre);
                        continue;
                    }

                    if (!existentes.Add((udbId, data.Nombre.Trim().ToUpperInvariant())))
                        continue;

                    nuevas.Add(new Asignatura
                    {
                        Nombre = data.Nombre,
                        Nivel = data.Nivel,
                        Frecuencia = data.Frecuencia,
                        EsVigente = true,
                        UdbId = udbId
                    });
                }

                if (nuevas.Count == 0)
                    return;

                await context.Asignaturas.AddRangeAsync(nuevas);
                await context.SaveChangesAsync();

                logger.LogInformation("Seed de asignaturas: se insertaron {Cantidad} asignaturas.", nuevas.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al seedear Asignaturas");
            }
        }
    }
}
