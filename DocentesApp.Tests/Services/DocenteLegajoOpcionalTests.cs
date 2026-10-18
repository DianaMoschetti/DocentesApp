using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Services;
using DocentesApp.Data.Repositories;
using DocentesApp.Domain.Enums;
using DocentesApp.Shared.DTOs.Docentes;
using DocentesApp.Tests.Helpers;
using AwesomeAssertions;

namespace DocentesApp.Tests.Services
{
    // v5: Legajo opcional. En SQLite se aplica el índice único filtrado ([Legajo] IS NOT NULL),
    // así que se prueba también que varios docentes sin legajo no chocan.
    public class DocenteLegajoOpcionalTests : IDisposable
    {
        private readonly SqliteTestDatabase _db = new();
        private readonly MapsterMapper.IMapper _mapper = MapperFactory.Create();

        public void Dispose() => _db.Dispose();

        [Fact]
        public async Task CreateAsync_DosDocentesSinLegajo_SePermite()
        {
            await CrearAsync(Dto("11.111.111", legajo: null));
            var act = () => CrearAsync(Dto("22.222.222", legajo: null));

            await act.Should().NotThrowAsync();

            using var context = _db.CrearContexto();
            context.Docentes.Count(d => d.Legajo == null).Should().Be(2);
        }

        [Fact]
        public async Task CreateAsync_LegajoRepetido_LanzaBadRequest()
        {
            await CrearAsync(Dto("11.111.111", legajo: 500));

            var act = () => CrearAsync(Dto("22.222.222", legajo: 500));

            await act.Should().ThrowAsync<BadRequestException>()
                .WithMessage("Ya existe un docente con ese legajo.");
        }

        private async Task<DocenteDto> CrearAsync(CreateDocenteDto dto)
        {
            using var context = _db.CrearContexto();
            var service = new DocenteService(new DocenteRepository(context), _mapper);
            return await service.CreateAsync(dto);
        }

        private static CreateDocenteDto Dto(string dni, int? legajo) => new()
        {
            Dni = dni,
            Legajo = legajo,
            Nombre = "Test",
            Apellido = dni,
            MaxNivelAcademico = Titulo.Universitario
        };
    }
}
