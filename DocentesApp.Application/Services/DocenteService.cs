using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Domain.Entities;
using MapsterMapper;

namespace DocentesApp.Application.Services
{
    public class DocenteService : IDocenteService
    {
        private readonly IDocenteRepository _docenteRepository;
        private readonly IMapper _mapper;

        public DocenteService(IDocenteRepository docenteRepository, IMapper mapper)
        {
            _docenteRepository = docenteRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ListDocenteDto>> GetAllAsync()
        {
            var docentes = await _docenteRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ListDocenteDto>>(docentes);
        }

        public async Task<DocenteDto> GetByIdAsync(int id)
        {
            var docente = await _docenteRepository.GetByIdAsync(id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            return _mapper.Map<DocenteDto>(docente);
        }

        public async Task<DocenteDto> CreateAsync(CreateDocenteDto dto)
        {
            var yaExisteLegajo = await _docenteRepository.ExistsByLegajoAsync(dto.Legajo);

            if (yaExisteLegajo)
                throw new BadRequestException("Ya existe un docente con ese legajo.");

            if (!string.IsNullOrWhiteSpace(dto.Dni))
            {
                var yaExisteDni = await _docenteRepository.ExistsByDniAsync(dto.Dni);

                if (yaExisteDni)
                    throw new BadRequestException("Ya existe un docente con ese DNI.");
            }

            var docente = _mapper.Map<Docente>(dto);

            await _docenteRepository.AddAsync(docente);
            await _docenteRepository.SaveChangesAsync();

            return _mapper.Map<DocenteDto>(docente);
        }

        public async Task UpdateAsync(int id, UpdateDocenteDto dto)
        {
            var docente = await _docenteRepository.GetByIdAsync(id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            if (dto.Legajo != docente.Legajo)
            {
                var yaExisteLegajo = await _docenteRepository.ExistsByLegajoAsync(dto.Legajo);

                if (yaExisteLegajo)
                    throw new BadRequestException("Ya existe un docente con ese legajo.");
            }
            if (!string.IsNullOrWhiteSpace(dto.Dni) && dto.Dni != docente.Dni)
            {
                var yaExisteDni = await _docenteRepository.ExistsAnotherByDniAsync(dto.Dni, id);

                if (yaExisteDni)
                    throw new BadRequestException("Ya existe un docente con ese DNI.");
            }

            _mapper.Map(dto, docente);

            _docenteRepository.Update(docente);
            await _docenteRepository.SaveChangesAsync();
        }

        public async Task UpdateContactoAsync(int id, UpdateContactoDocenteDto dto)
        {
            var docente = await _docenteRepository.GetByIdAsync(id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            _mapper.Map(dto, docente);

            _docenteRepository.Update(docente);
            await _docenteRepository.SaveChangesAsync();
        }

        public async Task UpdateAcademicoAsync(int id, UpdateAcademicoDocenteDto dto)
        {
            var docente = await _docenteRepository.GetByIdAsync(id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            _mapper.Map(dto, docente);

            _docenteRepository.Update(docente);
            await _docenteRepository.SaveChangesAsync();
        }

        public async Task UpdateObservacionesAsync(int id, UpdateObservacionesDocenteDto dto)
        {
            var docente = await _docenteRepository.GetByIdAsync(id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            _mapper.Map(dto, docente);

            _docenteRepository.Update(docente);
            await _docenteRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var docente = await _docenteRepository.GetByIdAsync(id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            var tieneDesignaciones = await _docenteRepository.HasDesignacionesAsync(id);

            if (tieneDesignaciones)
                throw new ConflictException("No se puede eliminar el docente porque tiene designaciones asociadas.");

            _docenteRepository.Delete(docente);
            await _docenteRepository.SaveChangesAsync();
        }

       
    }
}
