using Dapper;
using LojaAPI.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace LojaAPI.Repositories
{
    public class ProdutoRepository
    {
        private readonly string _connectionString;

        public ProdutoRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        private readonly CarrinhoRepository _carrinhoRepository;

        private IDbConnection Connection => new MySqlConnection(_connectionString);

        public async Task<IEnumerable<Produto>> ListarProdutosDB()
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Produtos";
                return await conn.QueryAsync<Produto>(sql);
            }
        }
        public async Task<int> RegistrarProdutoDB(Produto produto)
        {
            using (var conn = Connection)
            {
                var sql = "INSERT INTO Produtos (Nome, Descricao, Preco, QuantidadeEstoque) VALUES (@Nome, @Descricao, @Preco, @QuantidadeEstoque);" +
                          "SELECT LAST_INSERT_ID();";
                return await conn.ExecuteScalarAsync<int>(sql, produto);
            }
        }
        public async Task<int> AtualizarProdutoDB(Produto produto)
        {
            using (var conn = Connection)
            {
                var sql = "UPDATE Produtos SET Nome = @Nome, Descricao = @Descricao, Preco = @Preco, QuantidadeEstoque = @QuantidadeEstoque WHERE Id = @Id";
                return await conn.ExecuteAsync(sql, produto);
            }
        }

        public async Task<int> ExcluirProdutoDB(int id)
        {
            if (Connection == null)
            {
                throw new InvalidOperationException("A conexão com o banco de dados não foi inicializada.");
            }

            using (var conn = Connection)
            {
                Console.WriteLine("Conexão aberta com sucesso.");

                var produtoExistente = await conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM Produtos WHERE Id = @Id", new { Id = id });

                if (produtoExistente == 0)
                {
                    throw new KeyNotFoundException("Produto não encontrado.");
                }

                var produtoEmCarrinhoAtivo = await conn.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM Carrinho WHERE ProdutoId = @Id", new { Id = id });

                if (produtoEmCarrinhoAtivo > 0)
                {
                    throw new InvalidOperationException("O produto está em um carrinho ativo e não pode ser excluído.");
                }
                var sqlExcluirProduto = "DELETE FROM Produtos WHERE Id = @Id";
                var rowsAffected = await conn.ExecuteAsync(sqlExcluirProduto, new { Id = id });

                return rowsAffected;
            }
        }


        public async Task<IEnumerable<Produto>> ConsultarProduto(string? nome = null, string? descricao = null, int? preco = null)
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Produto WHERE 1=1";

                if (!string.IsNullOrEmpty(nome))
                {
                    sql += " AND Nome = @Nome";
                }
                if (!string.IsNullOrEmpty(descricao))
                {
                    sql += " AND Descricao = @Descricao";
                }
                if (preco.HasValue)
                {
                    sql += " AND Preco = @Preco";
                }

                return await conn.QueryAsync<Produto>(sql, new { Nome = nome, Descricao = descricao, Preco = preco });
            }
        }

        internal Task UpdateProdutoAsync(object produto)
        {
            throw new NotImplementedException();
        }

        internal Task GetProdutoByIdAsync(object produtoId)
        {
            throw new NotImplementedException();
        }
    }
}
