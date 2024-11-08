using LojaAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using System.Data;

namespace LojaAPI.Repositories
{
    public class UsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection Connection => new MySqlConnection(_connectionString);

        public async Task<IEnumerable<Usuario>> ListarUsuariosDB()
        {
            using (var conn = Connection)
            {
                var sql = "SELECT * FROM Usuarios";
                return await conn.QueryAsync<Usuario>(sql);
            }
        }
        public async Task<int> RegistrarUsuarioDB(Usuario usuario)
        {
            using (var conn = Connection)
            {
                var sql = "INSERT INTO Usuarios (Nome, Email, Endereco) VALUES (@Nome, @Email, @Endereco);" +
                          "SELECT LAST_INSERT_ID();";
                return await conn.ExecuteScalarAsync<int>(sql, usuario);
            }
        }
    }
}
