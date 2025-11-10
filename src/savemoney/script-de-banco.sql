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

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251009031727_update-login', N'8.0.20');
    GO

    COMMIT;
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
        [ConsumoMensal] float NOT NULL,
        CONSTRAINT [PK_ConversoresEnergia] PRIMARY KEY ([Id])
    );
    GO

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251009051930_update-conversor-energia', N'8.0.20');
    GO

    COMMIT;
    GO

    BEGIN TRANSACTION;
    GO

    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ConversoresEnergia]') AND [c].[name] = N'ConsumoMensal');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ConversoresEnergia] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [ConversoresEnergia] ALTER COLUMN [ConsumoMensal] float NULL;
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

    CREATE TABLE [Aportes] (
        [Id] int NOT NULL IDENTITY,
        [Valor] decimal(18,2) NOT NULL,
        [DataAporte] datetime2 NOT NULL,
        [MetaFinanceiraId] int NOT NULL,
        CONSTRAINT [PK_Aportes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Aportes_MetasFinanceiras_MetaFinanceiraId] FOREIGN KEY ([MetaFinanceiraId]) REFERENCES [MetasFinanceiras] ([Id]) ON DELETE CASCADE
    );
    GO

    CREATE INDEX [IX_Aportes_MetaFinanceiraId] ON [Aportes] ([MetaFinanceiraId]);
    GO

    CREATE INDEX [IX_MetasFinanceiras_UsuarioId] ON [MetasFinanceiras] ([UsuarioId]);
    GO

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251016190214_AddMetasFinanceirasEAportes', N'8.0.20');
    GO

    COMMIT;
    GO

    BEGIN TRANSACTION;
    GO

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251028035537_SincronizandoTabelasPosMerge', N'8.0.20');
    GO

    COMMIT;
    GO

    BEGIN TRANSACTION;
    GO

    CREATE TABLE [Category] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [IsPredefined] bit NOT NULL,
        CONSTRAINT [PK_Category] PRIMARY KEY ([Id])
    );
    GO

    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'IsPredefined') AND [object_id] = OBJECT_ID(N'[Category]'))
        SET IDENTITY_INSERT [Category] ON;
    INSERT INTO [Category] ([Id], [Name], [IsPredefined])
    VALUES (1, N'Alimentação', CAST(1 AS bit)),
    (2, N'Lazer', CAST(1 AS bit)),
    (3, N'Transporte', CAST(1 AS bit)),
    (4, N'Moradia', CAST(1 AS bit)),
    (5, N'Despesas Operacionais', CAST(1 AS bit));
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Name', N'IsPredefined') AND [object_id] = OBJECT_ID(N'[Category]'))
        SET IDENTITY_INSERT [Category] OFF;
    GO

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251029004414_AddCategory', N'8.0.20');
    GO

    COMMIT;
    GO

    BEGIN TRANSACTION;
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

    CREATE INDEX [IX_Budget_UserId] ON [Budget] ([UserId]);
    GO

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251029012721_AddBudget', N'8.0.20');
    GO

    COMMIT;
    GO

    BEGIN TRANSACTION;
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

    CREATE INDEX [IX_BudgetCategory_BudgetId] ON [BudgetCategory] ([BudgetId]);
    GO

    CREATE INDEX [IX_BudgetCategory_CategoryId] ON [BudgetCategory] ([CategoryId]);
    GO

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251029013304_AddBudgetCategory', N'8.0.20');
    GO

    COMMIT;
    GO

    BEGIN TRANSACTION;
    GO

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251105003227_budgetCateoryReceita', N'8.0.20');
    GO

    COMMIT;
    GO

    BEGIN TRANSACTION;
    GO

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251105003455_AddRecurringFieldsAndBudgetCategoryToReceita', N'8.0.20');
    GO

    COMMIT;
    GO

    BEGIN TRANSACTION;
    GO

    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Receitas]') AND [c].[name] = N'Categoria');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Receitas] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Receitas] DROP COLUMN [Categoria];
    GO

    ALTER TABLE [Receitas] ADD [BudgetCategoryId] int NOT NULL DEFAULT 0;
    GO

    ALTER TABLE [Receitas] ADD [Frequency] int NOT NULL DEFAULT 0;
    GO

    ALTER TABLE [Receitas] ADD [Interval] int NOT NULL DEFAULT 0;
    GO

    ALTER TABLE [Receitas] ADD [IsRecurring] bit NOT NULL DEFAULT CAST(0 AS bit);
    GO

    ALTER TABLE [Receitas] ADD [RecurrenceEndDate] datetime2 NULL;
    GO

    ALTER TABLE [Receitas] ADD [RecurrenceOccurrences] int NULL;
    GO

    CREATE INDEX [IX_Receitas_BudgetCategoryId] ON [Receitas] ([BudgetCategoryId]);
    GO

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20251106002717_tableReceita', N'8.0.20');
    GO

    COMMIT;
    GO

