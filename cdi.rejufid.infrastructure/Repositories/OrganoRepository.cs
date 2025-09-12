using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Repositories;
using cdi.core;
using System.Data.Common;

namespace cdi.rejufid.infrastructure.Repositories
{
    public class OrganoRepository : IOrganoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OrganoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<OrganoEntity>> GetAllAsync()
        {
            var lista = new List<OrganoEntity>();
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "SELECT * FROM Organos";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new OrganoEntity
                {
                    Id_organo = reader.GetInt32(0),
                    Organos = reader.GetString(1)
                });
            }

            return lista;
        }
    }
}
