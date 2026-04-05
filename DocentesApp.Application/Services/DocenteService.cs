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
            try
            {
                await _docenteRepository.SaveChangesAsync();
            }
            catch (Exception ex) when (TryMapUniqueConstraint(ex, out var mappedException))
            {
                throw mappedException;
            }

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
            try
            {
                await _docenteRepository.SaveChangesAsync();
            }
            catch (Exception ex) when (TryMapUniqueConstraint(ex, out var mappedException))
            {
                throw mappedException;
            }
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
            try
            {
                await _docenteRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ConflictException("No se puede eliminar el docente porque tiene designaciones asociadas.");
            }
        }

        private static bool TryMapUniqueConstraint(Exception ex, out AppException mappedException)
        {
            if (ex.GetType().Name != "DbUpdateException")
            {
                mappedException = null!;
                return false;
            }

            var message = ex.InnerException?.Message ?? ex.Message;
            var normalized = message.ToLowerInvariant();

            if ((normalized.Contains("ix_docentes_legajo") || normalized.Contains("legajo")) && normalized.Contains("unique"))
            {
                mappedException = new BadRequestException("Ya existe un docente con ese legajo.");
                return true;
            }

            if ((normalized.Contains("ix_docentes_dni") || normalized.Contains("dni")) && normalized.Contains("unique"))
            {
                mappedException = new BadRequestException("Ya existe un docente con ese DNI.");
                return true;
            }

            mappedException = null!;
            return false;
        }
    }
}
