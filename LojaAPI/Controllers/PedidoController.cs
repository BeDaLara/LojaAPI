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


        [HttpPost("registrar-pedido")]
        public async Task<IActionResult> RegistrarProduto([FromBody] Pedido pedido)
        {
            var pedidoId = await _pedidoRepository.RegistrarPedidoDB(pedido);

            return Ok(new { mensagem = "Pedido efetuado com sucesso", pedidoId });
        }
    }
}
