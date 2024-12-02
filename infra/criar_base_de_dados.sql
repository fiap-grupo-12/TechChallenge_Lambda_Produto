IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ByteMeBurger')
BEGIN
    CREATE DATABASE ByteMeBurger;
END;
GO

-- Bloco 2: Usar o banco de dados e criar tabelas
USE ByteMeBurger;
GO

-- Criação da tabela Categoria
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Categorias' AND xtype = 'U')
BEGIN
    CREATE TABLE Categorias (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nome NVARCHAR(MAX)
    );
END;

-- Criação da tabela Produto
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'Produtos' AND xtype = 'U')
BEGIN
    CREATE TABLE Produtos (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Nome NVARCHAR(MAX),
        Descricao NVARCHAR(MAX),
        Valor FLOAT NOT NULL,
        CategoriaProdutoId INT,
        FOREIGN KEY (CategoriaProdutoId) REFERENCES Categorias(Id)
    );
END;

-- Inserção de categorias, apenas se não existirem
IF NOT EXISTS (SELECT * FROM Categorias)
BEGIN
    INSERT INTO Categorias (Nome) VALUES ('Lanche');
    INSERT INTO Categorias (Nome) VALUES ('Acompanhamento');
    INSERT INTO Categorias (Nome) VALUES ('Bebida');
    INSERT INTO Categorias (Nome) VALUES ('Sobremesa');
END;
GO