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
        public async Task<IActionResult> RegistrarCarrinho([FromBody] Carrinho carrinho)
        {
            var carrinhoId = await _carrinhoRepository.RegistrarCarrinhoDB(carrinho);

            return Ok(new { mensagem = "Adicionado ao Carrinho com sucesso", carrinhoId });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirProdutoCarrinho(int id)
        {
            await _carrinhoRepository.ExcluirCarrinhoDB(id);

            return Ok(new { mensagem = "Produto excluído com sucesso" });
        }

        [HttpGet("consultar-carrinho")]
        public async Task<IActionResult> ListarCarrinho(int usuarioId)
        {
            var produtos = await _carrinhoRepository.BuscarCarrinho(usuarioId);
            return Ok(produtos);
        }
    }
}
