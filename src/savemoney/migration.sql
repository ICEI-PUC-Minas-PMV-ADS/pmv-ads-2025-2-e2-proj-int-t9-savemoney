IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [ConversoresEnergia] (
    [Id] int NOT NULL IDENTITY,
    [ValorBase] float NOT NULL,
    [TipoValor] nvarchar(max) NOT NULL,
    [Estado] nvarchar(max) NOT NULL,
    [Modalidade] nvarchar(max) NOT NULL,
    [BandeiraTarifaria] nvarchar(max) NOT NULL,
    [TipoDispositivo] nvarchar(max) NOT NULL,
    [TempoUso] nvarchar(max) NULL,
    [ConsumoMensal] float NULL,
    CONSTRAINT [PK_ConversoresEnergia] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Receita] (
    [Id] int NOT NULL IDENTITY,
    [Titulo] nvarchar(50) NOT NULL,
    [Valor] decimal(18,2) NOT NULL,
    [CurrencyType] nvarchar(max) NOT NULL,
    [DataInicio] datetime2 NOT NULL,
    [DataFim] datetime2 NOT NULL,
    [Recebido] bit NOT NULL,
    [IsRecurring] bit NOT NULL,
    [Recurrence] int NOT NULL,
    [RecurrenceCount] int NULL,
    CONSTRAINT [PK_Receita] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Usuario] (
    [Id] int NOT NULL IDENTITY,
    [Nome] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Documento] nvarchar(max) NOT NULL,
    [Senha] nvarchar(max) NOT NULL,
    [DataCadastro] datetime2 NOT NULL,
    [Perfil] int NOT NULL,
    [TipoUsuario] int NOT NULL,
    CONSTRAINT [PK_Usuario] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Budget] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [UserId] int NOT NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Budget] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Budget_Usuario_UserId] FOREIGN KEY ([UserId]) REFERENCES [Usuario] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Category] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [IsPredefined] bit NOT NULL,
    [UsuarioId] int NULL,
    CONSTRAINT [PK_Category] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Category_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [MetasFinanceiras] (
    [Id] int NOT NULL IDENTITY,
    [Titulo] nvarchar(100) NOT NULL,
    [Descricao] nvarchar(500) NULL,
    [ValorObjetivo] decimal(18,2) NOT NULL,
    [ValorAtual] decimal(18,2) NOT NULL,
    [DataLimite] datetime2 NULL,
    [UsuarioId] int NOT NULL,
    CONSTRAINT [PK_MetasFinanceiras] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MetasFinanceiras_Usuario_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [BudgetCategory] (
    [Id] int NOT NULL IDENTITY,
    [BudgetId] int NOT NULL,
    [CategoryId] int NOT NULL,
    [Limit] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_BudgetCategory] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BudgetCategory_Budget_BudgetId] FOREIGN KEY ([BudgetId]) REFERENCES [Budget] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BudgetCategory_Category_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Category] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Aportes] (
    [Id] int NOT NULL IDENTITY,
    [Valor] decimal(18,2) NOT NULL,
    [DataAporte] datetime2 NOT NULL,
    [MetaFinanceiraId] int NOT NULL,
    CONSTRAINT [PK_Aportes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Aportes_MetasFinanceiras_MetaFinanceiraId] FOREIGN KEY ([MetaFinanceiraId]) REFERENCES [MetasFinanceiras] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Despesa] (
    [Id] int NOT NULL IDENTITY,
    [Titulo] nvarchar(50) NOT NULL,
    [Valor] decimal(18,2) NOT NULL,
    [CurrencyType] nvarchar(max) NOT NULL,
    [DataInicio] datetime2 NOT NULL,
    [DataFim] datetime2 NOT NULL,
    [BudgetCategoryId] int NULL,
    [Recebido] bit NOT NULL,
    [IsRecurring] bit NOT NULL,
    [Recurrence] int NOT NULL,
    [RecurrenceCount] int NULL,
    CONSTRAINT [PK_Despesa] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Despesa_BudgetCategory_BudgetCategoryId] FOREIGN KEY ([BudgetCategoryId]) REFERENCES [BudgetCategory] ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsPredefined', N'Name', N'UsuarioId') AND [object_id] = OBJECT_ID(N'[Category]'))
    SET IDENTITY_INSERT [Category] ON;
INSERT INTO [Category] ([Id], [IsPredefined], [Name], [UsuarioId])
VALUES (1, CAST(1 AS bit), N'Alimentação', NULL),
(2, CAST(1 AS bit), N'Lazer', NULL),
(3, CAST(1 AS bit), N'Transporte', NULL),
(4, CAST(1 AS bit), N'Moradia', NULL),
(5, CAST(1 AS bit), N'Despesas Operacionais', NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsPredefined', N'Name', N'UsuarioId') AND [object_id] = OBJECT_ID(N'[Category]'))
    SET IDENTITY_INSERT [Category] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DataCadastro', N'Documento', N'Email', N'Nome', N'Perfil', N'Senha', N'TipoUsuario') AND [object_id] = OBJECT_ID(N'[Usuario]'))
    SET IDENTITY_INSERT [Usuario] ON;
INSERT INTO [Usuario] ([Id], [DataCadastro], [Documento], [Email], [Nome], [Perfil], [Senha], [TipoUsuario])
VALUES (1, '2025-11-18T13:59:23.3597713-03:00', N'000.000.000-00', N'admin@savemoney.com', N'Admin Savemoney', 0, N'123456', 0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'DataCadastro', N'Documento', N'Email', N'Nome', N'Perfil', N'Senha', N'TipoUsuario') AND [object_id] = OBJECT_ID(N'[Usuario]'))
    SET IDENTITY_INSERT [Usuario] OFF;
GO

CREATE INDEX [IX_Aportes_MetaFinanceiraId] ON [Aportes] ([MetaFinanceiraId]);
GO

CREATE INDEX [IX_Budget_UserId] ON [Budget] ([UserId]);
GO

CREATE INDEX [IX_BudgetCategory_BudgetId] ON [BudgetCategory] ([BudgetId]);
GO

CREATE INDEX [IX_BudgetCategory_CategoryId] ON [BudgetCategory] ([CategoryId]);
GO

CREATE INDEX [IX_Category_UsuarioId] ON [Category] ([UsuarioId]);
GO

CREATE INDEX [IX_Despesa_BudgetCategoryId] ON [Despesa] ([BudgetCategoryId]);
GO

CREATE INDEX [IX_MetasFinanceiras_UsuarioId] ON [MetasFinanceiras] ([UsuarioId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251118165923_InitialCreate', N'8.0.20');
GO

COMMIT;
GO

