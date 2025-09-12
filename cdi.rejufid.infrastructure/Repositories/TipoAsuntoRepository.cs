using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Repositories;
using cdi.core;
using System.Data.Common;

namespace cdi.rejufid.infrastructure.Repositories
{
    public class TipoAsuntoRepository : ITipoAsuntoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TipoAsuntoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<TipoAsuntoEntity>> GetAllAsync()
        {
            var lista = new List<TipoAsuntoEntity>();
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "SELECT * FROM Tipo_asunto";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new TipoAsuntoEntity
                {
                    Id_tipo_asunto = reader.GetInt32(0),
                    Tipo_asunto = reader.GetString(1)
                });
            }

            return lista;
        }
    }
}
