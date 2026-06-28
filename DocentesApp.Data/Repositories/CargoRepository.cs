using DocentesApp.Data.Context;
using DocentesApp.Application.Interfaces.Repositories;
using DocentesApp.Domain.Entities;
using DocentesApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DocentesApp.Data.Repositories
{
    public class CargoRepository : ICargoRepository
    {
        private readonly DocentesDbContext _context;

        public CargoRepository(DocentesDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cargo>> GetAllAsync()
        {
            return await _context.Cargos
                .AsNoTracking() // para traer los datos sin seguimiento de cambios, lo que mejora el rendimiento
                .ToListAsync();
        }

        public async Task<Cargo?> GetByIdAsync(int id)
        {
            return await _context.Cargos
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<bool> ExistsAsync(DenominacionCargo denominacion, TipoCargo tipoCargo, Condicion condicion)
        {
            return await _context.Cargos
                .AnyAsync(c => c.Denominacion == denominacion
                    && c.TipoCargo == tipoCargo
                    && c.Condicion == condicion);
        }

        public async Task AddAsync(Cargo cargo)
        {
            await _context.Cargos.AddAsync(cargo);
        }

        public void Delete(Cargo cargo)
        {
            _context.Cargos.Remove(cargo);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasDesignacionesAsync(int cargoId)
        {
            return await _context.Designaciones
                .AnyAsync(d => d.CargoId == cargoId);
        }
    }
}