using DomainBitacoraVehicular.Entities;
using InfraBitacoraVehicular.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace InfraBitacoraVehicular.Seeders
{
    public static class InitialDataSeeder
    {
        public static async Task SeedAsync(BitacoraVehicularDbContext context)
        {
            await context.Database.MigrateAsync();

            await SeedRolesAsync(context);
            await SeedContextosEstadoAsync(context);
            await SeedEstadosAsync(context);
            await SeedTiposCombustibleAsync(context);
            await SeedTiposMantencionAsync(context);
            await SeedSuperAdminAsync(context);
        }

        private static async Task SeedRolesAsync(BitacoraVehicularDbContext context)
        {
            var rolesRequeridos = new[]
            {
                new { Nombre = "Administrador", Descripcion = "Administrador del sistema" },
                new { Nombre = "Funcionario", Descripcion = "Usuario funcionario del sistema" }
            };

            var rolesExistentes = await context.Roles
                .Select(r => r.Nombre)
                .ToListAsync();

            var nuevosRoles = rolesRequeridos
                .Where(r => !rolesExistentes.Contains(r.Nombre))
                .Select(r => new Rol
                {
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                })
                .ToList();

            if (nuevosRoles.Count == 0)
                return;

            await context.Roles.AddRangeAsync(nuevosRoles);
            await context.SaveChangesAsync();
        }

        private static async Task SeedContextosEstadoAsync(BitacoraVehicularDbContext context)
        {
            var contextosRequeridos = new[]
            {
                new { Nombre = "USUARIO", Descripcion = "Estados para usuarios del sistema" },
                new { Nombre = "VEHICULO", Descripcion = "Estados para vehículos del sistema" },
                new { Nombre = "VIAJE", Descripcion = "Estados para viajes del sistema" },
                new { Nombre = "ASIGNACION", Descripcion = "Estados para asignaciones de vehículos" },
                new { Nombre = "MANTENCION", Descripcion = "Estados para mantenciones del sistema" }
            };

            var contextosExistentes = await context.ContextosEstado
                .Select(c => c.Nombre)
                .ToListAsync();

            var nuevosContextos = contextosRequeridos
                .Where(c => !contextosExistentes.Contains(c.Nombre))
                .Select(c => new ContextoEstado
                {
                    Nombre = c.Nombre,
                    Descripcion = c.Descripcion,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                })
                .ToList();

            if (nuevosContextos.Count == 0)
                return;

            await context.ContextosEstado.AddRangeAsync(nuevosContextos);
            await context.SaveChangesAsync();
        }

        private static async Task SeedEstadosAsync(BitacoraVehicularDbContext context)
        {
            var definiciones = new Dictionary<string, List<string>>
            {
                { "USUARIO", new List<string> { "Pendiente", "Activo", "Inactivo" } },
                { "VEHICULO", new List<string> { "Disponible", "EnUso", "Mantencion", "NoDisponible", "Deshabilitado" } },
                { "VIAJE", new List<string> { "Abierto", "Cerrado", "Cancelado" } },
                { "ASIGNACION", new List<string> { "Activa", "Finalizada" } },
                { "MANTENCION", new List<string> { "Pendiente", "EnProceso", "Finalizada" } }
            };

            var contextos = await context.ContextosEstado.ToListAsync();

            foreach (var definicion in definiciones)
            {
                var contexto = contextos.FirstOrDefault(c => c.Nombre == definicion.Key);
                if (contexto is null)
                    continue;

                var estadosExistentes = await context.Estados
                    .Where(e => e.ContextoEstadoId == contexto.ContextoEstadoId)
                    .Select(e => e.Nombre)
                    .ToListAsync();

                var nuevosEstados = definicion.Value
                    .Where(nombreEstado => !estadosExistentes.Contains(nombreEstado))
                    .Select(nombreEstado => new Estado
                    {
                        ContextoEstadoId = contexto.ContextoEstadoId,
                        Nombre = nombreEstado,
                        Descripcion = $"Estado {nombreEstado} para contexto {contexto.Nombre}",
                        Activo = true,
                        FechaCreacion = DateTime.UtcNow
                    })
                    .ToList();

                if (nuevosEstados.Count == 0)
                    continue;

                await context.Estados.AddRangeAsync(nuevosEstados);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedTiposCombustibleAsync(BitacoraVehicularDbContext context)
        {
            var tiposRequeridos = new[]
            {
                "Gasolina 93",
                "Gasolina 95",
                "Gasolina 97",
                "Diesel"
            };

            var existentes = await context.TiposCombustible
                .Select(t => t.Nombre)
                .ToListAsync();

            var nuevos = tiposRequeridos
                .Where(nombre => !existentes.Contains(nombre))
                .Select(nombre => new TipoCombustible
                {
                    Nombre = nombre,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                })
                .ToList();

            if (nuevos.Count == 0)
                return;

            await context.TiposCombustible.AddRangeAsync(nuevos);
            await context.SaveChangesAsync();
        }

        private static async Task SeedTiposMantencionAsync(BitacoraVehicularDbContext context)
        {
            var tiposRequeridos = new[]
            {
                new { Nombre = "Cambio de aceite", Descripcion = "Mantención preventiva de aceite y filtros" },
                new { Nombre = "Cambio de frenos", Descripcion = "Revisión o cambio de sistema de frenos" },
                new { Nombre = "Revisión general", Descripcion = "Chequeo general del vehículo" },
                new { Nombre = "Alineación y balanceo", Descripcion = "Ajuste de dirección y ruedas" },
                new { Nombre = "Mantención correctiva", Descripcion = "Reparación por falla detectada" }
            };

            var existentes = await context.TiposMantencion
                .Select(t => t.Nombre)
                .ToListAsync();

            var nuevos = tiposRequeridos
                .Where(t => !existentes.Contains(t.Nombre))
                .Select(t => new TipoMantencion
                {
                    Nombre = t.Nombre,
                    Descripcion = t.Descripcion,
                    Activo = true,
                    FechaCreacion = DateTime.UtcNow
                })
                .ToList();

            if (nuevos.Count == 0)
                return;

            await context.TiposMantencion.AddRangeAsync(nuevos);
            await context.SaveChangesAsync();
        }

        private static async Task SeedSuperAdminAsync(BitacoraVehicularDbContext context)
        {
            var existeAdmin = await context.Usuarios
            .Include(u => u.Rol)
            .AnyAsync(u => u.Rol != null && u.Rol.Nombre == "Administrador");

            if (existeAdmin)
                return;

            var rolAdmin = await context.Roles
                .FirstOrDefaultAsync(r => r.Nombre == "Administrador");

            if (rolAdmin is null)
                return;

            var contextoUsuario = await context.ContextosEstado
                .FirstOrDefaultAsync(c => c.Nombre == "USUARIO");

            if (contextoUsuario is null)
                return;

            var estadoActivo = await context.Estados
                .FirstOrDefaultAsync(e =>
                    e.ContextoEstadoId == contextoUsuario.ContextoEstadoId &&
                    e.Nombre == "Activo");

            if (estadoActivo is null)
                return;

            var usuarioAdmin = new Usuario
            {
                NombreCompleto = "Administrador General",
                Correo = "admin@bitacora.local",
                NombreUsuario = "admin",
                RolId = rolAdmin.RolId,
                EstadoId = estadoActivo.EstadoId,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            var passwordHasher = new PasswordHasher<Usuario>();
            usuarioAdmin.PasswordHash = passwordHasher.HashPassword(usuarioAdmin, "Admin1234*");

            await context.Usuarios.AddAsync(usuarioAdmin);
            await context.SaveChangesAsync();
        }
    }
}