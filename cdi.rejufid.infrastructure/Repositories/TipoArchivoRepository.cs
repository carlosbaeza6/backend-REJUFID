using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Repositories;
using cdi.core;
using System.Data.Common;

namespace cdi.rejufid.infrastructure.Repositories
{
    public class TipoArchivoRepository : ITipoArchivoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TipoArchivoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<TipoArchivoEntity>> GetAllAsync()
        {
            var lista = new List<TipoArchivoEntity>();
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "SELECT * FROM Tipos_archivo";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new TipoArchivoEntity
                {
                    Id_tipos_archivo = reader.GetInt32(0),
                    Tipos_archivo = reader.GetString(1)
                });
            }

            return lista;
        }
    }
}
