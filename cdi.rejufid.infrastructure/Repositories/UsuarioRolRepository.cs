using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Repositories;
using System.Data.Common;
using System.Data;
using cdi.core;

namespace cdi.rejufid.infrastructure.Repositories
{
    public class UsuarioRolRepository : IUsuarioRolRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UsuarioRolRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<UsuarioRolEntity>> GetAllAsync()
        {
            var lista = new List<UsuarioRolEntity>();
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "SELECT * FROM Usuarios_roles";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(MapFromReader(reader));
            }

            return lista;
        }

        public async Task<UsuarioRolEntity?> GetByIdAsync(int id)
        {
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "SELECT * FROM Usuarios_roles WHERE Id_usuario_roles = @id";

            var param = command.CreateParameter();
            param.ParameterName = "@id";
            param.Value = id;
            command.Parameters.Add(param);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapFromReader(reader);
            }

            return null;
        }

        public async Task<UsuarioRolEntity?> GetByEmailAsync(string correo)
        {
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();

            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "SELECT * FROM Usuarios_roles WHERE Correo = @Correo";

            var parametro = command.CreateParameter();
            parametro.ParameterName = "@Correo";
            parametro.Value = correo;
            command.Parameters.Add(parametro);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapFromReader(reader);
            }

            return null;
        }

        public async Task<int> CreateAsync(UsuarioRolEntity entity)
        {
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();

            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Usuarios_roles (Id_usuario, Nombre_completo, Matricula, Correo, Id_rol, Fecha_asignacion, Contrasena)
                VALUES (@Id_usuario, @Nombre_completo, @Matricula, @Correo, @Id_rol, @Fecha_asignacion, @Contrasena);
                SELECT SCOPE_IDENTITY();
            ";

            AddParameters(command, entity);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> UpdateAsync(UsuarioRolEntity entity)
        {
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = @"
                UPDATE Usuarios_roles SET
                    Id_usuario = @Id_usuario,
                    Nombre_completo = @Nombre_completo,
                    Matricula = @Matricula,
                    Correo = @Correo,
                    Id_rol = @Id_rol,
                    Fecha_asignacion = @Fecha_asignacion,
                    Contrasena = @Contrasena
                WHERE Id_usuario_roles = @Id_usuario_roles;
            ";

            AddParameters(command, entity, includeId: true);
            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "DELETE FROM Usuarios_roles WHERE Id_usuario_roles = @id";

            var param = command.CreateParameter();
            param.ParameterName = "@id";
            param.Value = id;
            command.Parameters.Add(param);

            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        private UsuarioRolEntity MapFromReader(DbDataReader reader)
        {
            return new UsuarioRolEntity
            {
                Id_usuario_roles = reader.GetInt32(0),
                Id_usuario = reader.IsDBNull(1) ? null : reader.GetString(1),
                Nombre_completo = reader.IsDBNull(2) ? null : reader.GetString(2),
                Matricula = reader.IsDBNull(3) ? null : reader.GetString(3),
                Correo = reader.IsDBNull(4) ? null : reader.GetString(4),
                Id_rol = reader.GetInt32(5),
                Fecha_asignacion = reader.GetDateTime(6),
                Contrasena = reader.IsDBNull(7) ? null : reader.GetString(7)
            };
        }

        private void AddParameters(IDbCommand command, UsuarioRolEntity entity, bool includeId = false)
        {
            void Add(string name, object value)
            {
                var p = command.CreateParameter();
                p.ParameterName = name;
                p.Value = value ?? DBNull.Value;
                command.Parameters.Add(p);
            }

            if (includeId) Add("@Id_usuario_roles", entity.Id_usuario_roles);
            Add("@Id_usuario", entity.Id_usuario);
            Add("@Nombre_completo", entity.Nombre_completo);
            Add("@Matricula", entity.Matricula);
            Add("@Correo", entity.Correo);
            Add("@Id_rol", entity.Id_rol);
            Add("@Fecha_asignacion", entity.Fecha_asignacion);
            Add("@Contrasena", entity.Contrasena);
        }
    }
}

