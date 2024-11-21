using LojaAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;
using Dapper;

namespace LojaAPI.Repositories
{
    public class CarrinhoRepository
    {
        private readonly string _connectionString;

        public CarrinhoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        private IDbConnection Connection => new MySqlConnection(_connectionString);
        public async Task<int> RegistrarCarrinhoDB(int usuarioId, int produtoId, int quantidade)
        {
            using (var conn = Connection)
            {
                //Validação de estoque ao adicionar ao carrinho
                var sqlEstoque = "SELECT QuantidadeEstoque FROM Produtos WHERE Id = @ProdutoId";
                var quantidadeEstoque = await conn.ExecuteScalarAsync<int>(sqlEstoque, new { ProdutoId = produtoId });

                //Verificar se há quantidade suficiente em estoque
                if(quantidade > quantidadeEstoque)
                {
                    throw new InvalidOperationException("Produto sem estoque!!");
                }

                //Consulta se o produto ja tá no carrinho
                var sqlVerificarProdutoCarrinho = "SELECT COUNT(*) FROM Carrinho WHERE UsuarioId = @UsuarioId AND ProdutoId = @ProdutoId";
                var produtoExists = await conn.ExecuteScalarAsync<int>(sqlVerificarProdutoCarrinho, new { UsuarioId = usuarioId, ProdutoId = produtoId });

                if(produtoExists > 0)
                {
                    //Atualizar Quantidade se o produto ja esta no carrinho
                    var sqlQuantidade = "UPDATE Carrinho SET Quantidade = Quantidade + @Quantidade WHERE UsuarioId = @UsuarioId AND ProdutoId = @ProdutoId";
                    return await conn.ExecuteScalarAsync<int>(sqlQuantidade, new { UsuarioId = usuarioId, ProdutoId = produtoId, Quantidade = quantidade });
                }
                else
                {
                    var sqlAdicionar = "INSERT INTO Carrinho (UsuarioId, ProdutoId, Quantidade) VALUES (@UsuarioId, @ProdutoId, @Quantidade);" +
                         "SELECT LAST_INSERT_ID();";
                    return await conn.ExecuteScalarAsync<int>(sqlAdicionar, new { UsuarioId = usuarioId, ProdutoId = produtoId, Quantidade = quantidade });
                }
               
            }
        }
        public async Task<int> ExcluirCarrinhoDB(int usuarioId, int produtoId)
        {
            using (var conn = Connection)
            {
                var sqlExcluirCarrinho = "DELETE FROM Carrinho WHERE UsuarioID = @UsuarioId AND ProdutoId = @ProdutoId";
                return await conn.ExecuteAsync(sqlExcluirCarrinho, new { UsuarioId = usuarioId, ProdutoId = produtoId });
            }
        }
        public async Task<IEnumerable<Carrinho>> BuscarCarrinho(int usuarioId)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Carrinho WHERE UsuarioId = @UsuarioId";

                return await conn.QueryAsync<Carrinho>(sql, new { UsuarioId = usuarioId });
            }
        }
        public async Task<bool> ProdutoCarrinho(int produtoId)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT COUNT(*) FROM Carrinho WHERE ProdutoId = @ProdutoId";
                var count = await conn.ExecuteScalarAsync<int>(sql, new { ProdutoId = produtoId });

                return count > 0;
            }
        }
        public async Task<IEnumerable<dynamic>> ConsultarCarrinho(int usuarioId)
        {
            using (var conn = Connection)
            {
                var sql = @"
                        SELECT c.ProdutoId, p.Nome, c.Quantidade, p.Preco, 
                            (c.Quantidade * p.Preco) AS ValorTotal 
                        FROM Carrinho c
                        JOIN Produtos p ON c.ProdutoId = p.Id
                        WHERE c.UsuarioId = @UsuarioId";

               return await conn.QueryAsync<dynamic>(sql, new { UsuarioId = usuarioId });

            }
        }
        public async Task<decimal> ValorTotalCarrinho(int usuarioId)
        {
            using (var conn = Connection)
            {
                var sql = @"
                        SELECT SUM(c.Quantidade * p.Preco) 
                        FROM Carrinho c 
                        JOIN Produtos p ON c.ProdutoId = p.Id
                        WHERE c.UsuarioId = @UsuarioId";

                var valorTotal = await conn.ExecuteScalarAsync<decimal>(sql, new { UsuarioId = usuarioId });

                return valorTotal;
            }
        }

        internal Task ClearCarrinhoAsync(int usuarioId)
        {
            throw new NotImplementedException();
        }

        internal Task GetCarrinhoByUsuarioAsync(int usuarioId)
        {
            throw new NotImplementedException();
        }
    }
}
