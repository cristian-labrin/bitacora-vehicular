using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppBitacoraVehicular.Common.Settings;
using AppBitacoraVehicular.DTOs.Auth;
using AppBitacoraVehicular.Interfaces;
using DomainBitacoraVehicular.Entities;
using InfraBitacoraVehicular.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace InfraBitacoraVehicular.Services
{
    public class AuthService : IAuthService
    {
        private readonly BitacoraVehicularDbContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly PasswordHasher<Usuario> _passwordHasher;

        public AuthService(
            BitacoraVehicularDbContext context,
            IOptions<JwtSettings> jwtSettings)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
            _passwordHasher = new PasswordHasher<Usuario>();
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            if (request.Password != request.ConfirmPassword)
            {
                return new AuthResponseDto
                {
                    Exito = false,
                    Mensaje = "Las contraseñas no coinciden."
                };
            }

            var correoExiste = await _context.Usuarios.AnyAsync(u => u.Correo == request.Correo);
            if (correoExiste)
            {
                return new AuthResponseDto
                {
                    Exito = false,
                    Mensaje = "El correo ya está registrado."
                };
            }

            var nombreUsuarioExiste = await _context.Usuarios.AnyAsync(u => u.NombreUsuario == request.NombreUsuario);
            if (nombreUsuarioExiste)
            {
                return new AuthResponseDto
                {
                    Exito = false,
                    Mensaje = "El nombre de usuario ya está registrado."
                };
            }

            var rolFuncionario = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Funcionario");
            if (rolFuncionario is null)
            {
                return new AuthResponseDto
                {
                    Exito = false,
                    Mensaje = "No existe el rol 'Funcionario'."
                };
            }

            var contextoUsuario = await _context.ContextosEstado.FirstOrDefaultAsync(c => c.Nombre == "USUARIO");
            if (contextoUsuario is null)
            {
                return new AuthResponseDto
                {
                    Exito = false,
                    Mensaje = "No existe el contexto de estado 'USUARIO'."
                };
            }

            var estadoPendiente = await _context.Estados
                .FirstOrDefaultAsync(e => e.ContextoEstadoId == contextoUsuario.ContextoEstadoId && e.Nombre == "Pendiente");

            if (estadoPendiente is null)
            {
                return new AuthResponseDto
                {
                    Exito = false,
                    Mensaje = "No existe el estado 'Pendiente' para usuarios."
                };
            }

            var usuario = new Usuario
            {
                NombreCompleto = request.NombreCompleto.Trim(),
                Correo = request.Correo.Trim(),
                NombreUsuario = request.NombreUsuario.Trim(),
                RolId = rolFuncionario.RolId,
                EstadoId = estadoPendiente.EstadoId,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            usuario.PasswordHash = _passwordHasher.HashPassword(usuario, request.Password);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Exito = true,
                Mensaje = "Usuario registrado correctamente. Queda pendiente de habilitación."
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Estado)
                .FirstOrDefaultAsync(u => u.Correo == request.Correo);

            if (usuario is null)
            {
                return new AuthResponseDto
                {
                    Exito = false,
                    Mensaje = "Credenciales inválidas."
                };
            }

            var resultadoPassword = _passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, request.Password);
            if (resultadoPassword == PasswordVerificationResult.Failed)
            {
                return new AuthResponseDto
                {
                    Exito = false,
                    Mensaje = "Credenciales inválidas."
                };
            }

            if (!usuario.Activo)
            {
                return new AuthResponseDto
                {
                    Exito = false,
                    Mensaje = "La cuenta está inactiva."
                };
            }

            if (!string.Equals(usuario.Estado?.Nombre, "Activo", StringComparison.OrdinalIgnoreCase))
            {
                return new AuthResponseDto
                {
                    Exito = false,
                    Mensaje = $"La cuenta no está habilitada. Estado actual: {usuario.Estado?.Nombre ?? "Desconocido"}."
                };
            }

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, usuario.UsuarioId.ToString()),
                new(ClaimTypes.Email, usuario.Correo),
                new(ClaimTypes.NameIdentifier, usuario.UsuarioId.ToString()),
                new(ClaimTypes.Name, usuario.NombreUsuario),
                new(ClaimTypes.Role, usuario.Rol?.Nombre ?? string.Empty),
                new("nombreCompleto", usuario.NombreCompleto)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return new AuthResponseDto
            {
                Exito = true,
                Mensaje = "Inicio de sesión exitoso.",
                Token = token,
                Expiracion = expiration,
                UsuarioId = usuario.UsuarioId,
                NombreCompleto = usuario.NombreCompleto,
                Correo = usuario.Correo,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol?.Nombre,
                Estado = usuario.Estado?.Nombre
            };
        }
    }
}
