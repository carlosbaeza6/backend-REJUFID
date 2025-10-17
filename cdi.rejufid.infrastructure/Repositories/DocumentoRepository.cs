using cdi.core;
using cdi.rejufid.core.DTOs;
using cdi.rejufid.core.Entities;
using cdi.rejufid.core.Interfaces.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.IO;

namespace cdi.rejufid.infrastructure.Repositories
{
    public class DocumentoRepository : IDocumentoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public DocumentoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<string> SavePDFAsync(byte[] contenido, string nombreArchivo)
        {
            var year = DateTime.Now.Year.ToString();
            var month = DateTime.Now.Month.ToString("D2");

            var rutaBase = Path.Combine(Directory.GetCurrentDirectory(), "archivos", year, month);
            if (!Directory.Exists(rutaBase))
                Directory.CreateDirectory(rutaBase);

            var rutaCompleta = Path.Combine(rutaBase, nombreArchivo);
            await File.WriteAllBytesAsync(rutaCompleta, contenido);

            var rutaPublica = Path.Combine("/archivos", year, month, nombreArchivo).Replace("\\", "/");
            return rutaPublica;
        }

        public async Task<int> AddAsync(DocumentoEntity doc)
        {
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Documentos (
                    Id_expediente,
                    Id_tipo_archivo,
                    Nombre_archivo,
                    Ruta_archivo,
                    Esta_firmado,
                    Hash_documento,
                    Uuid_firma,
                    Firma_base64,
                    Usuario_carga,
                    Fecha_carga,
                    Usuario_firma,
                    Fecha_firma
                )
                VALUES (
                    @Id_expediente,
                    @Id_tipo_archivo,
                    @Nombre_archivo,
                    @Ruta_archivo,
                    @Esta_firmado,
                    @Hash_documento,
                    @Uuid_firma,
                    @Firma_base64,
                    @Usuario_carga,
                    GETDATE(),
                    @Usuario_firma,
                    @Fecha_firma
                );
                SELECT SCOPE_IDENTITY();";

            void Add(string name, object? value)
            {
                var p = command.CreateParameter();
                p.ParameterName = name;
                p.Value = value ?? DBNull.Value;
                command.Parameters.Add(p);
            }

            Add("@Id_expediente", doc.Id_expediente);
            Add("@Id_tipo_archivo", doc.Id_tipo_archivo);
            Add("@Nombre_archivo", doc.Nombre_archivo);
            Add("@Ruta_archivo", doc.Ruta_archivo);
            Add("@Usuario_carga", doc.Usuario_carga);

            Add("@Esta_firmado", doc.Esta_firmado ? 1 : 0);
            Add("@Hash_documento", string.IsNullOrWhiteSpace(doc.Hash_documento) ? DBNull.Value : doc.Hash_documento);
            Add("@Uuid_firma", string.IsNullOrWhiteSpace(doc.Uuid_firma) ? DBNull.Value : doc.Uuid_firma);
            Add("@Firma_base64", string.IsNullOrWhiteSpace(doc.Firma_base64) ? DBNull.Value : doc.Firma_base64);
            Add("@Usuario_firma", string.IsNullOrWhiteSpace(doc.Usuario_firma) ? DBNull.Value : doc.Usuario_firma);
            Add("@Fecha_firma", doc.Esta_firmado ? (object?)(doc.Fecha_firma ?? DateTime.Now) : DBNull.Value);

            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<IEnumerable<DocumentoEntity>> GetByExpedienteAsync(int idExpediente)
        {
            var documentos = new List<DocumentoEntity>();

            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Documentos WHERE Id_expediente = @IdExpediente";
            var param = command.CreateParameter();
            param.ParameterName = "@IdExpediente";
            param.Value = idExpediente;
            command.Parameters.Add(param);

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                documentos.Add(MapDocumento(reader));

            return documentos;
        }

        public async Task<DocumentoEntity?> GetByUuidAsync(string uuid)
        {
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"SELECT TOP 1 * 
                             FROM Documentos 
                             WHERE LOWER(Uuid_firma) = LOWER(@Uuid)";
            var p = command.CreateParameter();
            p.ParameterName = "@Uuid";
            p.Value = uuid;
            command.Parameters.Add(p);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapDocumento(reader);

            return null;
        }

        public async Task DeleteByExpedienteAsync(int idExpediente)
        {
            using var connection = (DbConnection)_connectionFactory.CreateDbConnection(DBConnectionsNames.REJUFIDDB);
            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Documentos WHERE Id_expediente = @IdExpediente";
            command.Parameters.Add(new SqlParameter("@IdExpediente", idExpediente));

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        private DocumentoEntity MapDocumento(IDataReader reader)
        {
            return new DocumentoEntity
            {
                Id_documento = reader.GetInt32(reader.GetOrdinal("Id_documento")),
                Id_expediente = reader.GetInt32(reader.GetOrdinal("Id_expediente")),
                Id_tipo_archivo = reader.GetInt32(reader.GetOrdinal("Id_tipo_archivo")),
                Nombre_archivo = reader.GetString(reader.GetOrdinal("Nombre_archivo")),
                Ruta_archivo = reader.GetString(reader.GetOrdinal("Ruta_archivo")),
                Esta_firmado = reader.GetBoolean(reader.GetOrdinal("Esta_firmado")),
                Fecha_carga = reader.GetDateTime(reader.GetOrdinal("Fecha_carga")),
                Usuario_carga = reader.GetString(reader.GetOrdinal("Usuario_carga")),
                Usuario_firma = reader.IsDBNull(reader.GetOrdinal("Usuario_firma")) ? null : reader.GetString(reader.GetOrdinal("Usuario_firma")),
                Fecha_firma = reader.IsDBNull(reader.GetOrdinal("Fecha_firma")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Fecha_firma")),
                Hash_documento = reader.IsDBNull(reader.GetOrdinal("Hash_documento")) ? null : reader.GetString(reader.GetOrdinal("Hash_documento")),
                Uuid_firma = reader.IsDBNull(reader.GetOrdinal("Uuid_firma")) ? null : reader.GetString(reader.GetOrdinal("Uuid_firma")),
                Firma_base64 = reader.IsDBNull(reader.GetOrdinal("Firma_base64")) ? null : reader.GetString(reader.GetOrdinal("Firma_base64"))
            };
        }
    }
}
