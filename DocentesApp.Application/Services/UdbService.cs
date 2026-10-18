using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Domain.Entities;
using DocentesApp.Shared.DTOs.Udbs;
using MapsterMapper;

namespace DocentesApp.Application.Services
{
    public class UdbService : IUdbService
    {
        private readonly IUdbRepository _udbRepository;
        private readonly IMapper _mapper;

        public UdbService(IUdbRepository udbRepository, IMapper mapper)
        {
            _udbRepository = udbRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ListUdbDto>> GetAllAsync()
        {
            var udbs = await _udbRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ListUdbDto>>(udbs);
        }

        public async Task<UdbDto> GetByIdAsync(int id)
        {
            var udb = await _udbRepository.GetByIdAsync(id);

            if (udb == null)
                throw new NotFoundException($"No se encontró la UDB con ID {id}.");

            return _mapper.Map<UdbDto>(udb);
        }

        public async Task<UdbDto> CreateAsync(CreateUdbDto dto)
        {
            var existe = await _udbRepository.ExistsAsync(dto.Nombre);

            if (existe)
                throw new BadRequestException("Ya existe una UDB con ese nombre.");

            var udb = _mapper.Map<Udb>(dto);

            await _udbRepository.AddAsync(udb);
            await _udbRepository.SaveChangesAsync();

            var udbConNavegaciones = await _udbRepository.GetByIdAsync(udb.Id);
            return _mapper.Map<UdbDto>(udbConNavegaciones!);
        }

        public async Task UpdateAsync(int id, UpdateUdbDto dto)
        {
            // [Diana desde v4.0 OBSOLETO] cargaba Director y Secretario, que no hacen falta para editar
            //var udb = await _udbRepository.GetByIdAsync(id);
            var udb = await _udbRepository.GetByIdParaEditarAsync(id);

            if (udb == null)
                throw new NotFoundException($"No se encontró la UDB con ID {id}.");

            var existe = await _udbRepository.ExistsAsync(dto.Nombre, excludeId: id);

            if (existe)
                throw new BadRequestException("Ya existe una UDB con ese nombre.");

            // Un id de docente inexistente era una violación de FK en SaveChanges (500): ahora es un 400
            await ValidarDocenteAsync(dto.DirectorDocenteId, "director");
            await ValidarDocenteAsync(dto.SecretarioDocenteId, "secretario");

            _mapper.Map(dto, udb);

            // [Diana desde v4.0 OBSOLETO] la UDB ya está trackeada: Update() marcaba todas sus columnas como modificadas
            // (también CreatedDate/CreatedBy) y escribía la fila aunque no cambiara nada
            //_udbRepository.Update(udb);
            await _udbRepository.SaveChangesAsync();
        }

        private async Task ValidarDocenteAsync(int? docenteId, string rol)
        {
            if (docenteId.HasValue && !await _udbRepository.ExisteDocenteAsync(docenteId.Value))
                throw new BadRequestException($"No existe el docente con ID {docenteId} elegido como {rol}.");
        }

        public async Task DeleteAsync(int id)
        {
            // [Diana desde v4.0 OBSOLETO] sin las membresías, la cascada a DocentesUdb la hacía solo la base (sin historial)
            //var udb = await _udbRepository.GetByIdAsync(id);
            var udb = await _udbRepository.GetByIdConMembresiasAsync(id);

            if (udb == null)
                throw new NotFoundException($"No se encontró la UDB con ID {id}.");

            var tieneAsignaturas = await _udbRepository.HasAsignaturasAsync(id);

            if (tieneAsignaturas)
                throw new ConflictException("No se puede eliminar la UDB porque tiene asignaturas asociadas.");

            _udbRepository.Delete(udb);
            try
            {
                await _udbRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ConflictException("No se puede eliminar la UDB porque tiene asignaturas asociadas.");
            }
        }
    }
}
