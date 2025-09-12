using System.Data;
using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Repositories;
using cdi.core;
using System.Data.Common;

namespace cdi.rejufid.infrastructure.Repositories
{
    public class MateriaRepository : IMateriaRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public MateriaRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<MateriaEntity>> GetAllAsync()
        {
            var lista = new List<MateriaEntity>();
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "SELECT * FROM Materias";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new MateriaEntity
                {
                    Id_materia = reader.GetInt32(0),
                    Materias = reader.GetString(1)
                });
            }

            return lista;
        }
    }
}
