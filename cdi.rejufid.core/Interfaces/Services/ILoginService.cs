using cdi.rejufid.core.DTOs;

namespace cdi.rejufid.core.Interfaces.Services
{
    public interface ILoginService
    {
        Task<string?> AutenticarAsync(UsuarioLoginDTO loginDto);
    }
}