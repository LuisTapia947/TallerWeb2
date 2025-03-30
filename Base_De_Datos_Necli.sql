USE necli;

DROP TABLE IF EXISTS Transacciones;
DROP TABLE IF EXISTS Cuentas;
DROP TABLE IF EXISTS Usuarios;

CREATE TABLE Usuarios (
    Id VARCHAR(20) PRIMARY KEY,  
    Contraseña VARCHAR(255), 
    NombreUsuario VARCHAR(50),
    ApellidoUsuario VARCHAR(50),
    Correo VARCHAR(100) UNIQUE
);

CREATE TABLE Cuentas (
    Numero VARCHAR(10) PRIMARY KEY, 
    UsuarioId VARCHAR(20),    
    Saldo DECIMAL(10,2) DEFAULT 0.00,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UsuarioId) REFERENCES Usuarios(Id)
);

CREATE TABLE Transacciones (
    NumeroTransaccion INT PRIMARY KEY IDENTITY(1,1), 
    FechaTransaccion DATETIME DEFAULT GETDATE(),
    NumeroCuentaOrigen VARCHAR(10), 
    NumeroCuentaDestino VARCHAR(10),
    Monto DECIMAL(10,2) CHECK (Monto BETWEEN 1000 AND 5000000),
    Tipo TINYINT CHECK (Tipo IN (1, 2)),
    FOREIGN KEY (NumeroCuentaOrigen) REFERENCES Cuentas(Numero),
    FOREIGN KEY (NumeroCuentaDestino) REFERENCES Cuentas(Numero),
    CONSTRAINT chk_cuentas_diferentes CHECK (NumeroCuentaOrigen <> NumeroCuentaDestino)
);

select * from Usuarios;

select * from Cuentas;

select * from Transacciones;