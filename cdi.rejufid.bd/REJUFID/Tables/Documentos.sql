CREATE TABLE Documentos (
    Id_documento INT IDENTITY(1,1) PRIMARY KEY,
    Id_expediente INT,
    Id_tipo_archivo INT,
    Nombre_archivo VARCHAR(100),
    Ruta_archivo VARCHAR(MAX),
    Esta_firmado BIT,
    Fecha_carga DATETIME DEFAULT GETDATE(),
    Usuario_carga VARCHAR(100),
    Usuario_firma VARCHAR(100),
    Fecha_firma DATETIME,
    Hash_documento VARCHAR(256),
    Uuid_firma NVARCHAR(36),           
    Firma_base64 TEXT,
    FOREIGN KEY (Id_expediente) REFERENCES Expedientes(Id_expediente),
    FOREIGN KEY (Id_tipo_archivo) REFERENCES Tipos_archivo(Id_tipo_archivo)
);

GO

CREATE INDEX IX_Documentos_HashUuid ON Documentos (Hash_documento, Uuid_firma);