using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Interfaces.Services;

namespace cdi.rejufid.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;

        public LoginController(ILoginService loginService)
        {
            _loginService = loginService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDTO request)
        {
            var token = await _loginService.AutenticarAsync(request);

            if (token == null)
                return Unauthorized("Credenciales inválidas");

            return Ok(new
            {
                token
            });
        }
    }
}
