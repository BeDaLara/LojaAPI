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
        public async Task<int> RegistrarCarrinhoDB(Carrinho carrinho)
        {
            using (var conn = Connection)
            {
                var sql = "INSERT INTO Carrinho (UsuarioId, ProdutoId, Quantidade) VALUES (@UsuarioId, @ProdutoId, @Quantidade);" +
                          "SELECT LAST_INSERT_ID();";
                return await conn.ExecuteScalarAsync<int>(sql, carrinho);
            }
        }
        public async Task<int> ExcluirCarrinhoDB(int id)
        {
            using (var conn = Connection)
            {
                var sqlExcluirCarrinho = "DELETE FROM Carrinho WHERE Id = @Id";
                return await conn.ExecuteAsync(sqlExcluirCarrinho, new { Id = id });
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
