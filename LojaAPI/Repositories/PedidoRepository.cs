using LojaAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;

namespace LojaAPI.Repositories
{
    public class PedidoRepository
    {
        private readonly string _connectionString;
        private readonly CarrinhoRepository _carrinhoRepository;

   
        public PedidoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public PedidoRepository(string connectionString, CarrinhoRepository carrinhoRepository)
        {
            _connectionString = connectionString;
            _carrinhoRepository = carrinhoRepository;
        }

        private IDbConnection Connection => new MySqlConnection(_connectionString);

        public async Task<int> RegistrarPedidoDB(Pedido pedido)
        {
            using (var conn = Connection)
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {

                    var itensCarrinho = await _carrinhoRepository.ConsultarCarrinho(pedido.UsuarioId);

                    var valortotal = await _carrinhoRepository.ValorTotalCarrinho(pedido.UsuarioId);

                    if (itensCarrinho == null || !itensCarrinho.Any())
                        throw new
                            InvalidOperationException("O carrinho esta vazio");

                    var dadosPedidos = new Pedido
                    {
                        UsuarioId = pedido.UsuarioId
                    };


                    var sqlRegistrandoPedido = "INSERT INTO Pedidos (UsuarioId, DataPedido, StatusPedido, ValorTotal) VALUES (@UsuarioId, @DataPedido, @StatusPedido, @valortotal);" +
                          "SELECT LAST_INSERT_ID();";

                    var pedidoId = await conn.ExecuteScalarAsync<int>(sqlRegistrandoPedido, dadosPedidos, transaction);

                    foreach (var item in itensCarrinho)
                    {
                        var itemP = new PedidoProduto
                        {
                            PedidoId = pedidoId,
                            Preco = item.Preco,
                          ProdutoId= item.ProdutoId,
                          Quantidade= item.Quantidade,
                        };

                        var sqlPedido = "INSERT INTO PedidoProdutos (PedidoId, ProdutoId, Quantidade, Preco) VALUES (@pedidoId, @ProdutoId, @Quantidade, @Preco);" +
                        "SELECT LAST_INSERT_ID();";

                        await conn.ExecuteAsync(sqlPedido, itemP, transaction);

                        var sqlAtualizarCarrinho = "DELETE FROM Carrinho WHERE UsuarioId = @UsuarioId";
                        await conn.ExecuteAsync(sqlAtualizarCarrinho, new { UsuarioId = pedido.UsuarioId }, transaction);
                        

                        transaction.Commit();
                    }

                    return pedidoId;
                }
            }
        }
        public async Task<IEnumerable<Pedido>> ListarPedidoDB(int usuarioId)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Pedidos where UsuarioId = @UsuarioId";
                return await conn.QueryAsync<Pedido>(sql, new { UsuarioId = usuarioId });
            }
        }
    }
}
