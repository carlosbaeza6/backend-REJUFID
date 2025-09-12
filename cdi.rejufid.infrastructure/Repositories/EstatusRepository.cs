using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Repositories;
using System.Data.Common;
using Dapper;
using cdi.core;

namespace cdi.rejufid.infrastructure.Repositories
{
    public class EstatusRepository : IEstatusRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public EstatusRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<EstatusEntity>> GetAllAsync()
        {
            var lista = new List<EstatusEntity>();
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "SELECT * FROM Estatus";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new EstatusEntity
                {
                    Id_estatus = reader.GetInt32(0),
                    Estatus = reader.GetString(1)
                });
            }

            return lista;
        }
    }
}
