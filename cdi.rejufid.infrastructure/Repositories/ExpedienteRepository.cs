using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using cdi.core;
using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Repositories;

namespace cdi.rejufid.infrastructure.Repositories
{
    public class ExpedienteRepository : IExpedienteRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ExpedienteRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<ExpedienteEntity>> GetAllAsync()
        {
            var lista = new List<ExpedienteEntity>();
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "SELECT * FROM Expedientes";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new ExpedienteEntity
                {
                    Id_expediente = reader.GetInt32(0),
                    Id_estado = reader.GetInt32(1),
                    Id_tipo_organo = reader.GetInt32(2),
                    Id_materia = reader.GetInt32(3),
                    Id_organo = reader.GetInt32(4),
                    Id_tipo_asunto = reader.GetInt32(5),
                    Numero_expediente = reader.GetString(6),
                    Anio_expediente = reader.GetInt32(7),
                    Fecha_expediente = reader.GetDateTime(8),
                    Observacion = reader.IsDBNull(9) ? "" : reader.GetString(9),
                    Id_estatus = reader.GetInt32(10),
                    Usuario_registro = reader.GetString(11),
                    Fecha_registro = reader.GetDateTime(12)
                });
            }

            return lista;
        }

        public async Task<ExpedienteEntity?> GetByIdAsync(int id)
        {
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "SELECT * FROM Expedientes WHERE Id_expediente = @id";
            var param = command.CreateParameter();
            param.ParameterName = "@id";
            param.Value = id;
            command.Parameters.Add(param);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new ExpedienteEntity
                {
                    Id_expediente = reader.GetInt32(0),
                    Id_estado = reader.GetInt32(1),
                    Id_tipo_organo = reader.GetInt32(2),
                    Id_materia = reader.GetInt32(3),
                    Id_organo = reader.GetInt32(4),
                    Id_tipo_asunto = reader.GetInt32(5),
                    Numero_expediente = reader.GetString(6),
                    Anio_expediente = reader.GetInt32(7),
                    Fecha_expediente = reader.GetDateTime(8),
                    Observacion = reader.IsDBNull(9) ? "" : reader.GetString(9),
                    Id_estatus = reader.GetInt32(10),
                    Usuario_registro = reader.GetString(11),
                    Fecha_registro = reader.GetDateTime(12)
                };
            }

            return null;
        }

        public async Task<int> CreateAsync(ExpedienteEntity expediente)
        {
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();

            // Validar duplicado (Numero_expediente + Id_organo)
            using (var checkCommand = connection.CreateCommand())
            {
                checkCommand.CommandText = @"
                    SELECT COUNT(*) 
                    FROM Expedientes 
                    WHERE Numero_expediente = @Numero_expediente AND Id_organo = @Id_organo";

                var p1 = checkCommand.CreateParameter();
                p1.ParameterName = "@Numero_expediente";
                p1.Value = expediente.Numero_expediente;
                checkCommand.Parameters.Add(p1);

                var p2 = checkCommand.CreateParameter();
                p2.ParameterName = "@Id_organo";
                p2.Value = expediente.Id_organo;
                checkCommand.Parameters.Add(p2);

                var exists = Convert.ToInt32(await checkCommand.ExecuteScalarAsync());
                if (exists > 0)
                    throw new InvalidOperationException("Ya existe un expediente con ese número en el órgano seleccionado.");
            }

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Expedientes (
                    Id_estado, Id_tipo_organo, Id_materia, Id_organo, Id_tipo_asunto,
                    Numero_expediente, Anio_expediente, Fecha_expediente, Observacion,
                    Id_estatus, Usuario_registro, Fecha_registro
                )
                VALUES (
                    @Id_estado, @Id_tipo_organo, @Id_materia, @Id_organo, @Id_tipo_asunto,
                    @Numero_expediente, @Anio_expediente, @Fecha_expediente, @Observacion,
                    @Id_estatus, @Usuario_registro, @Fecha_registro
                );
                SELECT SCOPE_IDENTITY();";

            AddParameters(command, expediente);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = "DELETE FROM Expedientes WHERE Id_expediente = @id";
            var param = command.CreateParameter();
            param.ParameterName = "@id";
            param.Value = id;
            command.Parameters.Add(param);

            var rows = await command.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<(bool Existed, List<string> RutasRelativas)> DeleteDeepAsync(
            int idExpediente,
            CancellationToken ct = default)
        {
            var rutas = new List<string>();

            await using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync(ct);
            await using var tx = await connection.BeginTransactionAsync(ct);

            try
            {
                await using (var cmd = connection.CreateCommand())
                {
                    cmd.Transaction = (DbTransaction)tx;
                    cmd.CommandText = @"
                        SELECT Ruta_archivo
                        FROM Documentos
                        WHERE Id_expediente = @Id";
                    var p = cmd.CreateParameter(); p.ParameterName = "@Id"; p.Value = idExpediente;
                    cmd.Parameters.Add(p);

                    await using var reader = await cmd.ExecuteReaderAsync(ct);
                    while (await reader.ReadAsync(ct))
                    {
                        if (!reader.IsDBNull(0))
                            rutas.Add(reader.GetString(0));
                    }
                }

                await using (var cmdDelDocs = connection.CreateCommand())
                {
                    cmdDelDocs.Transaction = (DbTransaction)tx;
                    cmdDelDocs.CommandText = @"DELETE FROM Documentos WHERE Id_expediente = @Id;";
                    var p = cmdDelDocs.CreateParameter(); p.ParameterName = "@Id"; p.Value = idExpediente;
                    cmdDelDocs.Parameters.Add(p);
                    await cmdDelDocs.ExecuteNonQueryAsync(ct);
                }

                int rowsExp;
                await using (var cmdDelExp = connection.CreateCommand())
                {
                    cmdDelExp.Transaction = (DbTransaction)tx;
                    cmdDelExp.CommandText = @"DELETE FROM Expedientes WHERE Id_expediente = @Id;";
                    var p = cmdDelExp.CreateParameter(); p.ParameterName = "@Id"; p.Value = idExpediente;
                    cmdDelExp.Parameters.Add(p);
                    rowsExp = await cmdDelExp.ExecuteNonQueryAsync(ct);
                }

                if (rowsExp == 0)
                {
                    await tx.RollbackAsync(ct);
                    return (false, new List<string>());
                }

                await tx.CommitAsync(ct);
                return (true, rutas);
            }
            catch
            {
                try { await tx.RollbackAsync(ct); } catch { /* noop */ }
                throw;
            }
        }

        public async Task<IEnumerable<ExpedienteDetalleDTO>> GetLast100Async()
        {
            var lista = new List<ExpedienteDetalleDTO>();
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();
            command.CommandText = @"
                SELECT TOP 100 
                    e.Id_expediente, e.Id_estado, e.Id_tipo_organo, e.Id_materia, e.Id_organo, e.Id_tipo_asunto,
                    e.Numero_expediente, e.Anio_expediente, e.Fecha_expediente, e.Observacion, e.Id_estatus,
                    e.Usuario_registro, e.Fecha_registro,
                    est.Estados AS Estado, to2.Tipo_organo AS Tipo_organo, m.Materias AS Materia,
                    o.Organo AS Organo, ta.Tipo_asunto AS Tipo_asunto, es.Estatus AS Estatus
                FROM Expedientes e
                JOIN Estados est ON e.Id_estado = est.Id_estado
                JOIN Tipo_organo to2 ON e.Id_tipo_organo = to2.Id_tipo_organo
                JOIN Materias m ON e.Id_materia = m.Id_materia
                JOIN Organos o ON e.Id_organo = o.Id_organo
                JOIN Tipo_asunto ta ON e.Id_tipo_asunto = ta.Id_tipo_asunto
                JOIN Estatus es ON e.Id_estatus = es.Id_estatus
                ORDER BY e.Fecha_expediente DESC";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(MapToDetalleDTO(reader));
            }

            return lista;
        }

        public async Task<IEnumerable<ExpedienteDetalleDTO>> GetFilteredAsync(
            string? tipoOrgano,
            string? organo,
            string? materia,
            string? palabraClave)
        {
            var lista = new List<ExpedienteDetalleDTO>();
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();
            using var command = (DbCommand)connection.CreateCommand();

            var sql = @"
                SELECT 
                    e.Id_expediente, e.Id_estado, e.Id_tipo_organo, e.Id_materia, e.Id_organo, e.Id_tipo_asunto,
                    e.Numero_expediente, e.Anio_expediente, e.Fecha_expediente, e.Observacion, e.Id_estatus,
                    e.Usuario_registro, e.Fecha_registro,
                    est.Estados AS Estado, to2.Tipo_organo AS Tipo_organo, m.Materias AS Materia,
                    o.Organo AS Organo, ta.Tipo_asunto AS Tipo_asunto, es.Estatus AS Estatus
                FROM Expedientes e
                JOIN Estados est ON e.Id_estado = est.Id_estado
                JOIN Tipo_organo to2 ON e.Id_tipo_organo = to2.Id_tipo_organo
                JOIN Materias m ON e.Id_materia = m.Id_materia
                JOIN Organos o ON e.Id_organo = o.Id_organo
                JOIN Tipo_asunto ta ON e.Id_tipo_asunto = ta.Id_tipo_asunto
                JOIN Estatus es ON e.Id_estatus = es.Id_estatus
                WHERE 1=1";

            void AddParam(string name, object value)
            {
                var p = command.CreateParameter();
                p.ParameterName = name;
                p.Value = value ?? DBNull.Value;
                command.Parameters.Add(p);
            }

            if (!string.IsNullOrWhiteSpace(tipoOrgano))
            {
                sql += " AND to2.Tipo_organo = @tipoOrgano";
                AddParam("@tipoOrgano", tipoOrgano);
            }

            if (!string.IsNullOrWhiteSpace(organo))
            {
                sql += " AND o.Organo = @organo";
                AddParam("@organo", organo);
            }

            if (!string.IsNullOrWhiteSpace(materia))
            {
                sql += " AND m.Materias = @materia";
                AddParam("@materia", materia);
            }

            if (!string.IsNullOrWhiteSpace(palabraClave))
            {
                sql += " AND (e.Numero_expediente LIKE '%' + @palabraClave + '%' OR e.Observacion LIKE '%' + @palabraClave + '%')";
                AddParam("@palabraClave", palabraClave);
            }

            command.CommandText = sql;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(MapToDetalleDTO(reader));
            }

            return lista;
        }

        private ExpedienteDetalleDTO MapToDetalleDTO(DbDataReader reader)
        {
            return new ExpedienteDetalleDTO
            {
                Id_expediente = reader.GetInt32(0),
                Id_estado = reader.GetInt32(1),
                Id_tipo_organo = reader.GetInt32(2),
                Id_materia = reader.GetInt32(3),
                Id_organo = reader.GetInt32(4),
                Id_tipo_asunto = reader.GetInt32(5),
                Numero_expediente = reader.GetString(6),
                Anio_expediente = reader.GetInt32(7),
                Fecha_expediente = reader.GetDateTime(8),
                Observacion = reader.IsDBNull(9) ? "" : reader.GetString(9),
                Id_estatus = reader.GetInt32(10),
                Usuario_registro = reader.GetString(11),
                Fecha_registro = reader.GetDateTime(12),
                Estado = reader.GetString(13),
                Tipo_organo = reader.GetString(14),
                Materia = reader.GetString(15),
                Organo = reader.GetString(16),
                Tipo_asunto = reader.GetString(17),
                Estatus = reader.GetString(18)
            };
        }

        private void AddParameters(IDbCommand command, ExpedienteEntity expediente, bool includeId = false)
        {
            void Add(string name, object value)
            {
                var p = command.CreateParameter();
                p.ParameterName = name;
                p.Value = value ?? DBNull.Value;
                command.Parameters.Add(p);
            }

            if (includeId) Add("@Id_expediente", expediente.Id_expediente);
            Add("@Id_estado", expediente.Id_estado);
            Add("@Id_tipo_organo", expediente.Id_tipo_organo);
            Add("@Id_materia", expediente.Id_materia);
            Add("@Id_organo", expediente.Id_organo);
            Add("@Id_tipo_asunto", expediente.Id_tipo_asunto);
            Add("@Numero_expediente", expediente.Numero_expediente);
            Add("@Anio_expediente", expediente.Anio_expediente);
            Add("@Fecha_expediente", expediente.Fecha_expediente);
            Add("@Observacion", expediente.Observacion);
            Add("@Id_estatus", expediente.Id_estatus);
            Add("@Usuario_registro", expediente.Usuario_registro);
            Add("@Fecha_registro", expediente.Fecha_registro);
        }
    }
}
