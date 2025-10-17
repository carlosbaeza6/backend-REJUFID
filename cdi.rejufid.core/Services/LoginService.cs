using AutoMapper;
using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Interfaces.Repositories;
using cdi.rejufid.core.Interfaces.Services;
using BCrypt.Net;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cdi.rejufid.core.Helpers;

namespace cdi.rejufid.core.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUsuarioRolRepository usuarioRolRepository;
        private readonly IMapper mapper;
        private readonly JwtSettings jwtSettings;

        public LoginService(IUsuarioRolRepository usuarioRolRepository, IMapper mapper, IOptions<JwtSettings> jwtOptions)
        {
            this.usuarioRolRepository = usuarioRolRepository;
            this.mapper = mapper;
            this.jwtSettings = jwtOptions.Value;
        }

        public async Task<string?> AutenticarAsync(UsuarioLoginDTO loginDto)
        {
            var usuario = await usuarioRolRepository.GetByEmailAsync(loginDto.Correo);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginDto.Contrasena, usuario.Contrasena))
                return null;

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, usuario.Id_usuario_roles.ToString()),
        new Claim(ClaimTypes.Name, usuario.Correo),
        new Claim("nombre_completo", usuario.Nombre_completo ?? ""),
        new Claim("role", usuario.Id_rol == 1 ? "admin" : "basico")
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
