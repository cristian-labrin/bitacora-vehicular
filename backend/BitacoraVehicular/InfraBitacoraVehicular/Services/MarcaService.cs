using AppBitacoraVehicular.DTOs.Marca;
using AppBitacoraVehicular.Interfaces;
using DomainBitacoraVehicular.Entities;
using InfraBitacoraVehicular.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InfraBitacoraVehicular.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly BitacoraVehicularDbContext _context;

        public MarcaService(BitacoraVehicularDbContext context)
        {
            _context = context;
        }

        public async Task<List<MarcaDto>> GetAllAsync()
        {
            return await _context.Marcas
                .OrderBy(m => m.Nombre)
                .Select(m => new MarcaDto
                {
                    MarcaId = m.MarcaId,
                    Nombre = m.Nombre,
                    Activo = m.Activo
                })
                .ToListAsync();
        }

        public async Task<MarcaDto?> GetByIdAsync(int id)
        {
            return await _context.Marcas
                .Where(m => m.MarcaId == id)
                .Select(m => new MarcaDto
                {
                    MarcaId = m.MarcaId,
                    Nombre = m.Nombre,
                    Activo = m.Activo
                })
                .FirstOrDefaultAsync();
        }

        public async Task<MarcaDto> CreateAsync(CreateMarcaDto request)
        {
            var nombre = request.Nombre.Trim();

            var existe = await _context.Marcas.AnyAsync(m => m.Nombre == nombre);
            if (existe)
                throw new InvalidOperationException("La marca ya existe.");

            var marca = new Marca
            {
                Nombre = nombre,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();

            return new MarcaDto
            {
                MarcaId = marca.MarcaId,
                Nombre = marca.Nombre,
                Activo = marca.Activo
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateMarcaDto request)
        {
            var marca = await _context.Marcas.FirstOrDefaultAsync(m => m.MarcaId == id);
            if (marca is null)
                return false;

            var nombre = request.Nombre.Trim();

            var existe = await _context.Marcas
                .AnyAsync(m => m.MarcaId != id && m.Nombre == nombre);

            if (existe)
                throw new InvalidOperationException("Ya existe otra marca con ese nombre.");

            marca.Nombre = nombre;
            marca.Activo = request.Activo;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var marca = await _context.Marcas.FirstOrDefaultAsync(m => m.MarcaId == id);
            if (marca is null)
                return false;

            marca.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
