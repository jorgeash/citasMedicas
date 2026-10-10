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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    CREATE TABLE [Business_Company] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(200) NOT NULL,
        [Sigla] nvarchar(20) NOT NULL,
        [MainEmail] nvarchar(200) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Business_Company] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    CREATE TABLE [Business_CustomerType] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(100) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Business_CustomerType] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    CREATE TABLE [Business_Customer] (
        [Id] int NOT NULL IDENTITY,
        [Code] nvarchar(50) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [CustomerTypeId] int NOT NULL,
        [CompanyId] int NOT NULL,
        [LocationId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Business_Customer] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Business_Customer_Business_Company_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Business_Company] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Business_Customer_Business_CustomerType_CustomerTypeId] FOREIGN KEY ([CustomerTypeId]) REFERENCES [Business_CustomerType] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    CREATE TABLE [Business_CustomerContact] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(100) NOT NULL,
        [LastName] nvarchar(100) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [Phone1] nvarchar(30) NOT NULL,
        [Phone2] nvarchar(30) NULL,
        [CustomerId] int NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Business_CustomerContact] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Business_CustomerContact_Business_Customer_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Business_Customer] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'IsActive', N'MainEmail', N'Name', N'Sigla', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Business_Company]'))
        SET IDENTITY_INSERT [Business_Company] ON;
    EXEC(N'INSERT INTO [Business_Company] ([Id], [CreatedAt], [IsActive], [MainEmail], [Name], [Sigla], [UpdatedAt])
    VALUES (1, ''2026-10-01T12:00:00.0000000Z'', CAST(1 AS bit), N''contacto@clinicavidasalud.example'', N''Clinica Vida Salud S.A.'', N''CVS'', NULL),
    (2, ''2026-10-01T12:00:00.0000000Z'', CAST(1 AS bit), N''compras@distribuidoraandina.example'', N''Distribuidora Andina CIA LTDA'', N''DAN'', NULL),
    (3, ''2026-10-01T12:00:00.0000000Z'', CAST(1 AS bit), N''servicios.cliente@epstotal.example'', N''EPS-total Salud S.A.'', N''EPST'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'IsActive', N'MainEmail', N'Name', N'Sigla', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Business_Company]'))
        SET IDENTITY_INSERT [Business_Company] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'IsActive', N'Name', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Business_CustomerType]'))
        SET IDENTITY_INSERT [Business_CustomerType] ON;
    EXEC(N'INSERT INTO [Business_CustomerType] ([Id], [CreatedAt], [IsActive], [Name], [UpdatedAt])
    VALUES (1, ''2026-10-01T12:00:00.0000000Z'', CAST(1 AS bit), N''Empresa'', NULL),
    (2, ''2026-10-01T12:00:00.0000000Z'', CAST(1 AS bit), N''Persona Natural'', NULL),
    (3, ''2026-10-01T12:00:00.0000000Z'', CAST(1 AS bit), N''EPS / Aseguradora'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'IsActive', N'Name', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Business_CustomerType]'))
        SET IDENTITY_INSERT [Business_CustomerType] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CompanyId', N'CreatedAt', N'CustomerTypeId', N'Email', N'IsActive', N'LocationId', N'Name', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Business_Customer]'))
        SET IDENTITY_INSERT [Business_Customer] ON;
    EXEC(N'INSERT INTO [Business_Customer] ([Id], [Code], [CompanyId], [CreatedAt], [CustomerTypeId], [Email], [IsActive], [LocationId], [Name], [UpdatedAt])
    VALUES (1, N''CLI-0001'', 2, ''2026-10-01T12:00:00.0000000Z'', 1, N''compras@distribuidoraandina.example'', CAST(1 AS bit), 2, N''Distribuidora Andina CIA LTDA'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CompanyId', N'CreatedAt', N'CustomerTypeId', N'Email', N'IsActive', N'LocationId', N'Name', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Business_Customer]'))
        SET IDENTITY_INSERT [Business_Customer] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CustomerId', N'Email', N'FirstName', N'IsActive', N'LastName', N'Phone1', N'Phone2', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Business_CustomerContact]'))
        SET IDENTITY_INSERT [Business_CustomerContact] ON;
    EXEC(N'INSERT INTO [Business_CustomerContact] ([Id], [CreatedAt], [CustomerId], [Email], [FirstName], [IsActive], [LastName], [Phone1], [Phone2], [UpdatedAt])
    VALUES (1, ''2026-10-01T12:00:00.0000000Z'', 1, N''a.torres@distribuidoraandina.example'', N''Ana'', CAST(1 AS bit), N''Torres'', N''+593 2 2345678'', N''+593 99 1234567'', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CustomerId', N'Email', N'FirstName', N'IsActive', N'LastName', N'Phone1', N'Phone2', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Business_CustomerContact]'))
        SET IDENTITY_INSERT [Business_CustomerContact] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Business_Company_Sigla] ON [Business_Company] ([Sigla]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Business_Customer_Code] ON [Business_Customer] ([Code]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    CREATE INDEX [IX_Business_Customer_CompanyId] ON [Business_Customer] ([CompanyId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    CREATE INDEX [IX_Business_Customer_CustomerTypeId] ON [Business_Customer] ([CustomerTypeId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    CREATE INDEX [IX_Business_Customer_LocationId] ON [Business_Customer] ([LocationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    CREATE INDEX [IX_Business_CustomerContact_CustomerId] ON [Business_CustomerContact] ([CustomerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Business_CustomerType_Name] ON [Business_CustomerType] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010620_InitialBusiness'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261006010620_InitialBusiness', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006010725_ProbeDiff'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261006010725_ProbeDiff', N'9.0.0');
END;

COMMIT;
GO

