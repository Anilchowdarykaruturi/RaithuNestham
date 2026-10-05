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
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE TABLE [Crops] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [TeluguName] nvarchar(max) NOT NULL,
        [Season] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Crops] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE TABLE [Fertilizers] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Type] nvarchar(max) NOT NULL,
        [RecommendedFor] nvarchar(max) NOT NULL,
        [UsageInstructions] nvarchar(max) NOT NULL,
        [IsOrganic] bit NOT NULL,
        CONSTRAINT [PK_Fertilizers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE TABLE [GovernmentSchemes] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [Description] nvarchar(max) NOT NULL,
        [Eligibility] nvarchar(max) NOT NULL,
        [Benefits] nvarchar(max) NOT NULL,
        [ApplicationProcess] nvarchar(max) NOT NULL,
        [OfficialWebsite] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_GovernmentSchemes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE TABLE [Pesticides] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        [TargetPest] nvarchar(max) NOT NULL,
        [RecommendedCrop] nvarchar(max) NOT NULL,
        [UsageInstructions] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Pesticides] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] int NOT NULL IDENTITY,
        [Username] nvarchar(450) NOT NULL,
        [PasswordHash] nvarchar(max) NOT NULL,
        [Role] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE TABLE [WeatherLogs] (
        [Id] int NOT NULL IDENTITY,
        [Village] nvarchar(max) NOT NULL,
        [Temperature] decimal(18,2) NOT NULL,
        [Humidity] decimal(18,2) NOT NULL,
        [Rainfall] decimal(18,2) NOT NULL,
        [WeatherCondition] nvarchar(max) NOT NULL,
        [RecordedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_WeatherLogs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE TABLE [Farmers] (
        [Id] int NOT NULL IDENTITY,
        [UserId] int NOT NULL,
        [FullName] nvarchar(max) NOT NULL,
        [PhoneNumber] nvarchar(max) NOT NULL,
        [Village] nvarchar(max) NOT NULL,
        [Mandal] nvarchar(max) NOT NULL,
        [District] nvarchar(max) NOT NULL,
        [State] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Farmers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Farmers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE TABLE [AIChats] (
        [Id] int NOT NULL IDENTITY,
        [FarmerId] int NOT NULL,
        [Question] nvarchar(max) NOT NULL,
        [Answer] nvarchar(max) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_AIChats] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AIChats_Farmers_FarmerId] FOREIGN KEY ([FarmerId]) REFERENCES [Farmers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE TABLE [Fields] (
        [Id] int NOT NULL IDENTITY,
        [FarmerId] int NOT NULL,
        [FieldName] nvarchar(max) NOT NULL,
        [AreaInAcres] decimal(18,2) NOT NULL,
        [SoilType] nvarchar(max) NOT NULL,
        [IrrigationType] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Fields] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Fields_Farmers_FarmerId] FOREIGN KEY ([FarmerId]) REFERENCES [Farmers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE TABLE [CropRecords] (
        [Id] int NOT NULL IDENTITY,
        [FieldId] int NOT NULL,
        [CropId] int NOT NULL,
        [PlantingDate] datetime2 NOT NULL,
        [HarvestDate] datetime2 NULL,
        [ExpectedYield] decimal(18,2) NULL,
        CONSTRAINT [PK_CropRecords] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CropRecords_Crops_CropId] FOREIGN KEY ([CropId]) REFERENCES [Crops] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_CropRecords_Fields_FieldId] FOREIGN KEY ([FieldId]) REFERENCES [Fields] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_AIChats_FarmerId] ON [AIChats] ([FarmerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CropRecords_CropId] ON [CropRecords] ([CropId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_CropRecords_FieldId] ON [CropRecords] ([FieldId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Farmers_UserId] ON [Farmers] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Fields_FarmerId] ON [Fields] ([FarmerId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Users_Username] ON [Users] ([Username]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923204343_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260923204343_InitialCreate', N'9.0.20');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923223652_RaithuNesthamDb'
)
BEGIN
    DECLARE @var sysname;
    SELECT @var = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WeatherLogs]') AND [c].[name] = N'Temperature');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [WeatherLogs] DROP CONSTRAINT [' + @var + '];');
    ALTER TABLE [WeatherLogs] ALTER COLUMN [Temperature] decimal(10,2) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923223652_RaithuNesthamDb'
)
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WeatherLogs]') AND [c].[name] = N'Rainfall');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [WeatherLogs] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [WeatherLogs] ALTER COLUMN [Rainfall] decimal(10,2) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923223652_RaithuNesthamDb'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WeatherLogs]') AND [c].[name] = N'Humidity');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [WeatherLogs] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [WeatherLogs] ALTER COLUMN [Humidity] decimal(10,2) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923223652_RaithuNesthamDb'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260923223652_RaithuNesthamDb', N'9.0.20');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923224246_RaithuNestham'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Fields]') AND [c].[name] = N'AreaInAcres');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Fields] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Fields] ALTER COLUMN [AreaInAcres] decimal(10,2) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260923224246_RaithuNestham'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260923224246_RaithuNestham', N'9.0.20');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928231917_AddGovernmentSchemes'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260928231917_AddGovernmentSchemes', N'9.0.20');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928235700_AddEquipmentSubsidies'
)
BEGIN
    CREATE TABLE [EquipmentSubsidies] (
        [Id] int NOT NULL IDENTITY,
        [EquipmentName] nvarchar(max) NOT NULL,
        [Category] nvarchar(max) NOT NULL,
        [SchemeName] nvarchar(max) NOT NULL,
        [EligibleFarmers] nvarchar(max) NOT NULL,
        [SubsidyDetails] nvarchar(max) NOT NULL,
        [MaximumSubsidy] nvarchar(max) NOT NULL,
        [ApplicationProcess] nvarchar(max) NOT NULL,
        [OfficialWebsite] nvarchar(max) NOT NULL,
        [State] nvarchar(max) NOT NULL,
        [LastVerified] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_EquipmentSubsidies] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260928235700_AddEquipmentSubsidies'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260928235700_AddEquipmentSubsidies', N'9.0.20');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002063026_SyncModelSnapshot'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261002063026_SyncModelSnapshot', N'9.0.20');
END;

COMMIT;
GO

