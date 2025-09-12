using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Repositories;
using cdi.core;
using System.Data.Common;

namespace cdi.rejufid.infrastructure.Repositories
{
    public class TipoOrganoRepository : ITipoOrganoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TipoOrganoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<TipoOrganoEntity>> GetAllAsync()
        {
            var lista = new List<TipoOrganoEntity>();
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "SELECT * FROM Tipo_organo";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new TipoOrganoEntity
                {
                    Id_tipo_organo = reader.GetInt32(0),
                    Tipo_organo = reader.GetString(1)
                });
            }

            return lista;
        }
    }
}
