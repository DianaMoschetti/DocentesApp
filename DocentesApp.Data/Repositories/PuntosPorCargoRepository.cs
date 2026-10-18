using DocentesApp.Data.Context;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;
namespace DocentesApp.Data.Repositories
{
    public class PuntosPorCargoRepository : IPuntosPorCargoRepository
    {
        private readonly DocentesDbContext _context;
        public PuntosPorCargoRepository(DocentesDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<PuntosPorCargo>> GetAllAsync()
        {
            return await _context.PuntosPorCargo
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<PuntosPorCargo?> GetByIdAsync(int id)
        {
            return await _context.PuntosPorCargo
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<PuntosPorCargo?> GetByCargoAsync(CargoDocente cargo)
        {
            return await _context.PuntosPorCargo
                .FirstOrDefaultAsync(p => p.Cargo == cargo);
        }
        public async Task<bool> EstaEnUsoAsync(CargoDocente cargo)
        {
            return await _context.CargosDesignacion
                .AnyAsync(c => c.Cargo == cargo);
        }
        public async Task AddAsync(PuntosPorCargo puntosPorCargo)
        {
            await _context.PuntosPorCargo.AddAsync(puntosPorCargo);
        }
        public void Delete(PuntosPorCargo puntosPorCargo)
        {
            _context.PuntosPorCargo.Remove(puntosPorCargo);
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
