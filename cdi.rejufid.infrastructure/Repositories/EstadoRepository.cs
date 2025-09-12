using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using Dapper;
using cdi.core;
using System.Data.Common;

namespace cdi.rejufid.infrastructure.Repositories
{
    public class EstadoRepository : IEstadoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EstadoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<EstadoEntity>> GetAllAsync()
        {
            var lista = new List<EstadoEntity>();
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "SELECT * FROM Estados";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new EstadoEntity
                {
                    Id_estado = reader.GetInt32(0),
                    Estados = reader.GetString(1)
                });
            }

            return lista;
        }
    }
}
