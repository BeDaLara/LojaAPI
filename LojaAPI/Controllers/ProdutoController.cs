using LojaAPI.Models;
using LojaAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LojaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoRepository _produtoRepository;

        public ProdutoController(ProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        [HttpGet("listar-produtos")]
        public async Task<IActionResult> ListarProdutos()
        {
            var produtos = await _produtoRepository.ListarProdutosDB();
            return Ok(produtos);
        }

        [HttpPost("registrar-produto")]
        public async Task<IActionResult> RegistrarProduto([FromBody] Produto produto)
        {
            var produtoId = await _produtoRepository.RegistrarProdutoDB(produto);

            return Ok(new { mensagem = "Produto cadastrado com sucesso", produtoId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarProduto(int id, [FromBody] Produto produto)
        {
            produto.Id = id;
            await _produtoRepository.AtualizarProdutoDB(produto);

            return Ok(new { mensagem = "Produto atualizado com sucesso" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirProduto(int id)
        {
            try
            {
                var resultado = await _produtoRepository.ExcluirProdutoDB(id);

                if (resultado == 0)
                {
                    return NotFound(new { mensagem = "Produto não encontrado." });
                }

                return Ok(new { mensagem = "Produto excluído com sucesso." });
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Erro InvalidOperationException: {ex.Message}");
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Erro KeyNotFoundException: {ex.Message}");
                return NotFound(new { mensagem = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro inesperado: {ex.Message}");
                return StatusCode(500, new { mensagem = "Erro interno no servidor", erro = ex.Message });
            }
        }





    }
}
