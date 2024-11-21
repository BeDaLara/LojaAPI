using LojaAPI.Models;
using LojaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LojaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrinhoController : ControllerBase
    {
        private readonly CarrinhoRepository _carrinhoRepository;

        public CarrinhoController(CarrinhoRepository carrinhoRepository)
        {
            _carrinhoRepository = carrinhoRepository;
        }
        [HttpPost("registrar-carrinho")]
        public async Task<IActionResult> RegistrarCarrinho(int usuarioId, int produtoId, int quantidade)
        {
            await _carrinhoRepository.RegistrarCarrinhoDB(usuarioId, produtoId, quantidade);

            return Ok(new { mensagem = "Adicionado ao Carrinho com sucesso"});
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirProdutoCarrinho(int usuarioId, int produtoId)
        {
            await _carrinhoRepository.ExcluirCarrinhoDB(usuarioId, produtoId);

            return Ok(new { mensagem = "Produto excluído com sucesso" });
        }

        [HttpGet("consultar-carrinho")]
        public async Task<IActionResult> ListarCarrinho(int usuarioId)
        {
            var itens = await _carrinhoRepository.ConsultarCarrinho(usuarioId);

            var valorTotal = await _carrinhoRepository.ValorTotalCarrinho(usuarioId);

            return Ok(new {itens, valorTotal});
        }
    }
}
