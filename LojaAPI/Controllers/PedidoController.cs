using LojaAPI.Models;
using LojaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;


namespace LojaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {
        private readonly PedidoRepository _pedidoRepository;

        public PedidoController(PedidoRepository pedidoRepository)
        {
            _pedidoRepository = pedidoRepository;
        }
        [HttpGet("listar-pedidos")]
        public async Task<IActionResult> ListarPedidos(int usuarioId)
        {
            var pedidos = await _pedidoRepository.ListarPedidoDB(usuarioId);
            return Ok(pedidos);
        }

        [HttpGet("consultar-status")]
        public async Task<IActionResult> ConsultarStatusPedidos(int pedidoId)
        {
            var status = await _pedidoRepository.ConsultarStatusPedidoDb(pedidoId);
            return Ok(status);
        }


        [HttpPost("registrar-pedido")]
        public async Task<IActionResult> RegistrarPedido(int usuarioId)
        {
            try
            {
                var pedidoId = await _pedidoRepository.RegistrarPedidoDB(usuarioId);
                return Ok(new { mensagem = "Pedido efetuado com sucesso", pedidoId });

            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new { mensagem= ex.Message });
            }

        }
    }
}
