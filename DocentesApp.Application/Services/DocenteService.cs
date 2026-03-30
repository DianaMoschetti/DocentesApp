using DocentesApp.Application.Common.Exceptions;
using DocentesApp.Application.DTOs.Docentes;
using DocentesApp.Application.Interfaces.Services;
using DocentesApp.Data.Context;
using DocentesApp.Model;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocentesApp.Application.Services
{
    public class DocenteService : IDocenteService
    {
        private readonly DocentesDbContext _context;
        private readonly IMapper _mapper;

        public DocenteService(DocentesDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ListDocenteDto>> GetAllAsync()
        {
            var docentes = await _context.Docentes.ToListAsync();
            return _mapper.Map<List<ListDocenteDto>>(docentes);
        }

        public async Task<DocenteDto> GetByIdAsync(int id)
        {
            var docente = await _context.Docentes.FirstOrDefaultAsync(d => d.Id == id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            return _mapper.Map<DocenteDto>(docente);
        }

        public async Task<DocenteDto> CreateAsync(CreateDocenteDto dto)
        {
            var yaExisteLegajo = await _context.Docentes.AnyAsync(d => d.Legajo == dto.Legajo);

            if (yaExisteLegajo)
                throw new BadRequestException("Ya existe un docente con ese legajo.");

            var docente = _mapper.Map<Docente>(dto);

            _context.Docentes.Add(docente);
            await _context.SaveChangesAsync();

            return _mapper.Map<DocenteDto>(docente);
        }

        public async Task UpdateAsync(int id, UpdateDocenteDto dto)
        {
            var docente = await _context.Docentes.FindAsync(id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            _mapper.Map(dto, docente);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var docente = await _context.Docentes.FindAsync(id);

            if (docente == null)
                throw new NotFoundException($"No se encontró el docente con ID {id}.");

            var tieneDesignaciones = await _context.Designaciones.AnyAsync(d => d.DocenteId == id);

            if (tieneDesignaciones)
                throw new ConflictException("No se puede eliminar el docente porque tiene designaciones asociadas.");

            _context.Docentes.Remove(docente);
            await _context.SaveChangesAsync();
        }
    }
}
