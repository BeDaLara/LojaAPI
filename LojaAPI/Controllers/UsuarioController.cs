using LojaAPI.Models;
using LojaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LojaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioController(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("listar-usuarios")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var usuarios = await _usuarioRepository.ListarUsuariosDB();
            return Ok(usuarios);
        }
        [HttpPost("registrar-usuario")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] Usuario usuario)
        {
            var usuarioId = await _usuarioRepository.RegistrarUsuarioDB(usuario);

            return Ok(new { mensagem = "Usuário cadastrado com sucesso", usuarioId });
        }
    }
}
