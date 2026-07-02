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
    WHERE [MigrationId] = N'20260415144321_Initsss'
)
BEGIN
    CREATE TABLE [Machines] (
        [CodeMachine] nvarchar(20) NOT NULL,
        [NomMachine] nvarchar(50) NOT NULL,
        [Actif] bit NOT NULL,
        CONSTRAINT [PK_Machines] PRIMARY KEY ([CodeMachine])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415144321_Initsss'
)
BEGIN
    CREATE TABLE [Produits] (
        [CodeArticle] nvarchar(20) NOT NULL,
        [NomProduit] nvarchar(50) NOT NULL,
        [Designation] nvarchar(100) NOT NULL,
        [TailleEchantillonnage] int NOT NULL,
        [Cadence] int NOT NULL,
        CONSTRAINT [PK_Produits] PRIMARY KEY ([CodeArticle])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415144321_Initsss'
)
BEGIN
    CREATE TABLE [TypeDefauts] (
        [Id] int NOT NULL IDENTITY,
        [NomDefaut] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NOT NULL,
        [CauseProbable] nvarchar(500) NOT NULL,
        [Solution] nvarchar(500) NOT NULL,
        [Frequence] int NOT NULL,
        [ImagePath] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_TypeDefauts] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415144321_Initsss'
)
BEGIN
    CREATE TABLE [Utilisateurs] (
        [Id] int NOT NULL IDENTITY,
        [FirstName] nvarchar(50) NOT NULL,
        [LastName] nvarchar(50) NOT NULL,
        [Email] nvarchar(100) NOT NULL,
        [Role] int NOT NULL,
        [Password] nvarchar(max) NOT NULL,
        [Actif] bit NOT NULL,
        CONSTRAINT [PK_Utilisateurs] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415144321_Initsss'
)
BEGIN
    CREATE TABLE [ResultatControles] (
        [Id] nvarchar(36) NOT NULL,
        [DateControle] datetime2 NOT NULL,
        [CodeMachine] nvarchar(20) NOT NULL,
        [CodeArticle] nvarchar(20) NOT NULL,
        [UtilisateurId] int NOT NULL,
        [NumOF] nvarchar(50) NOT NULL,
        [Quantite] int NOT NULL,
        [Cadence] int NOT NULL,
        [NbEchantillons] int NOT NULL,
        [StatutLot] nvarchar(20) NOT NULL,
        [SolutionGlobale] nvarchar(500) NULL,
        [TypeDefaut1Id] int NULL,
        [TypeDefaut2Id] int NULL,
        [NbDefautsTest1] int NOT NULL,
        [NbDefautsTest2] int NOT NULL,
        [Defaut1] nvarchar(200) NULL,
        [Defaut2] nvarchar(200) NULL,
        CONSTRAINT [PK_ResultatControles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ResultatControles_Machines_CodeMachine] FOREIGN KEY ([CodeMachine]) REFERENCES [Machines] ([CodeMachine]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultatControles_Produits_CodeArticle] FOREIGN KEY ([CodeArticle]) REFERENCES [Produits] ([CodeArticle]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultatControles_TypeDefauts_TypeDefaut1Id] FOREIGN KEY ([TypeDefaut1Id]) REFERENCES [TypeDefauts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultatControles_TypeDefauts_TypeDefaut2Id] FOREIGN KEY ([TypeDefaut2Id]) REFERENCES [TypeDefauts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultatControles_Utilisateurs_UtilisateurId] FOREIGN KEY ([UtilisateurId]) REFERENCES [Utilisateurs] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415144321_Initsss'
)
BEGIN
    CREATE INDEX [IX_ResultatControles_CodeArticle] ON [ResultatControles] ([CodeArticle]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415144321_Initsss'
)
BEGIN
    CREATE INDEX [IX_ResultatControles_CodeMachine] ON [ResultatControles] ([CodeMachine]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415144321_Initsss'
)
BEGIN
    CREATE INDEX [IX_ResultatControles_TypeDefaut1Id] ON [ResultatControles] ([TypeDefaut1Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415144321_Initsss'
)
BEGIN
    CREATE INDEX [IX_ResultatControles_TypeDefaut2Id] ON [ResultatControles] ([TypeDefaut2Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415144321_Initsss'
)
BEGIN
    CREATE INDEX [IX_ResultatControles_UtilisateurId] ON [ResultatControles] ([UtilisateurId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260415144321_Initsss'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260415144321_Initsss', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416134948_FKProduitOptionnelle'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260416134948_FKProduitOptionnelle', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416135502_FKProduitOptionnellee'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260416135502_FKProduitOptionnellee', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416135756_FKProduitOptionnelleeo'
)
BEGIN
    ALTER TABLE [ResultatControles] DROP CONSTRAINT [FK_ResultatControles_Produits_CodeArticle];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416135756_FKProduitOptionnelleeo'
)
BEGIN
    DROP INDEX [IX_ResultatControles_CodeArticle] ON [ResultatControles];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416135756_FKProduitOptionnelleeo'
)
BEGIN
    ALTER TABLE [ResultatControles] ADD [ProduitCodeArticle] nvarchar(20) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416135756_FKProduitOptionnelleeo'
)
BEGIN
    CREATE INDEX [IX_ResultatControles_ProduitCodeArticle] ON [ResultatControles] ([ProduitCodeArticle]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416135756_FKProduitOptionnelleeo'
)
BEGIN
    ALTER TABLE [ResultatControles] ADD CONSTRAINT [FK_ResultatControles_Produits_ProduitCodeArticle] FOREIGN KEY ([ProduitCodeArticle]) REFERENCES [Produits] ([CodeArticle]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260416135756_FKProduitOptionnelleeo'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260416135756_FKProduitOptionnelleeo', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417133529_Fixlongeur'
)
BEGIN
    ALTER TABLE [Machines] ADD [Code] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417133529_Fixlongeur'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260417133529_Fixlongeur', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417134708_Fixcodemachine'
)
BEGIN
    DECLARE @var sysname;
    SELECT @var = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Machines]') AND [c].[name] = N'Code');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Machines] DROP CONSTRAINT [' + @var + '];');
    ALTER TABLE [Machines] DROP COLUMN [Code];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417134708_Fixcodemachine'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260417134708_Fixcodemachine', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260417142009_Fixcodemachin'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260417142009_Fixcodemachin', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE TABLE [DestinatairesNotification] (
        [Id] int NOT NULL IDENTITY,
        [UtilisateurId] int NULL,
        [Role] nvarchar(50) NULL,
        [Canal] tinyint NOT NULL,
        [NiveauMinimum] tinyint NOT NULL,
        [EstActif] bit NOT NULL,
        [Destinataire] nvarchar(255) NOT NULL,
        CONSTRAINT [PK_DestinatairesNotification] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DestinatairesNotification_Utilisateurs_UtilisateurId] FOREIGN KEY ([UtilisateurId]) REFERENCES [Utilisateurs] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE TABLE [Seuils] (
        [Id] int NOT NULL IDENTITY,
        [CodeMachine] nvarchar(20) NOT NULL,
        [CodeArticle] nvarchar(20) NULL,
        [TypeDefaut1Id] int NULL,
        [SeuilPourcentage] decimal(5,2) NOT NULL,
        [SeuilQuantite] int NULL,
        [EstActif] bit NOT NULL,
        [DateCreation] datetime2 NOT NULL,
        [DateModification] datetime2 NULL,
        [CreePar] int NULL,
        CONSTRAINT [PK_Seuils] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Seuils_Machines_CodeMachine] FOREIGN KEY ([CodeMachine]) REFERENCES [Machines] ([CodeMachine]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Seuils_Produits_CodeArticle] FOREIGN KEY ([CodeArticle]) REFERENCES [Produits] ([CodeArticle]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Seuils_TypeDefauts_TypeDefaut1Id] FOREIGN KEY ([TypeDefaut1Id]) REFERENCES [TypeDefauts] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE TABLE [Alertes] (
        [Id] int NOT NULL IDENTITY,
        [SeuilId] int NOT NULL,
        [CodeMachine] nvarchar(20) NOT NULL,
        [CodeArticle] nvarchar(20) NULL,
        [TypeDefaut1Id] int NULL,
        [TypeDefaut2Id] int NULL,
        [TauxDetecte] decimal(5,2) NOT NULL,
        [QuantiteDefauts] int NOT NULL,
        [QuantiteTotale] int NOT NULL,
        [Niveau] tinyint NOT NULL,
        [Statut] tinyint NOT NULL,
        [Message] nvarchar(500) NULL,
        [DateAlerte] datetime2 NOT NULL,
        [DateResolution] datetime2 NULL,
        [ResolueParId] int NULL,
        [CommentaireResolution] nvarchar(1000) NULL,
        CONSTRAINT [PK_Alertes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Alertes_Machines_CodeMachine] FOREIGN KEY ([CodeMachine]) REFERENCES [Machines] ([CodeMachine]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Alertes_Seuils_SeuilId] FOREIGN KEY ([SeuilId]) REFERENCES [Seuils] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Alertes_TypeDefauts_TypeDefaut1Id] FOREIGN KEY ([TypeDefaut1Id]) REFERENCES [TypeDefauts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Alertes_TypeDefauts_TypeDefaut2Id] FOREIGN KEY ([TypeDefaut2Id]) REFERENCES [TypeDefauts] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Alertes_Utilisateurs_ResolueParId] FOREIGN KEY ([ResolueParId]) REFERENCES [Utilisateurs] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE TABLE [HistoriqueNotifications] (
        [Id] int NOT NULL IDENTITY,
        [AlerteId] int NOT NULL,
        [UtilisateurId] int NULL,
        [Canal] tinyint NOT NULL,
        [Destinataire] nvarchar(255) NOT NULL,
        [Sujet] nvarchar(255) NULL,
        [Corps] nvarchar(max) NOT NULL,
        [Statut] tinyint NOT NULL,
        [DateEnvoi] datetime2 NULL,
        [DateLecture] datetime2 NULL,
        [ErreurMessage] nvarchar(500) NULL,
        [NbTentatives] int NOT NULL,
        CONSTRAINT [PK_HistoriqueNotifications] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HistoriqueNotifications_Alertes_AlerteId] FOREIGN KEY ([AlerteId]) REFERENCES [Alertes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_HistoriqueNotifications_Utilisateurs_UtilisateurId] FOREIGN KEY ([UtilisateurId]) REFERENCES [Utilisateurs] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_Alertes_CodeMachine] ON [Alertes] ([CodeMachine]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_Alertes_DateAlerte] ON [Alertes] ([DateAlerte]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_Alertes_ResolueParId] ON [Alertes] ([ResolueParId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_Alertes_SeuilId] ON [Alertes] ([SeuilId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_Alertes_Statut] ON [Alertes] ([Statut]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_Alertes_TypeDefaut1Id] ON [Alertes] ([TypeDefaut1Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_Alertes_TypeDefaut2Id] ON [Alertes] ([TypeDefaut2Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_DestinatairesNotification_UtilisateurId] ON [DestinatairesNotification] ([UtilisateurId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_HistoriqueNotifications_AlerteId] ON [HistoriqueNotifications] ([AlerteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_HistoriqueNotifications_Statut] ON [HistoriqueNotifications] ([Statut]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_HistoriqueNotifications_UtilisateurId] ON [HistoriqueNotifications] ([UtilisateurId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_Seuils_CodeArticle] ON [Seuils] ([CodeArticle]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_Seuils_CodeMachine] ON [Seuils] ([CodeMachine]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    CREATE INDEX [IX_Seuils_TypeDefaut1Id] ON [Seuils] ([TypeDefaut1Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515133826_Ajout_Notifications'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260515133826_Ajout_Notifications', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515143407_AddMachineIdToAlerte'
)
BEGIN
    ALTER TABLE [Alertes] ADD [MachineId] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260515143407_AddMachineIdToAlerte'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260515143407_AddMachineIdToAlerte', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519104045_InitAlert'
)
BEGIN
    DROP INDEX [IX_HistoriqueNotifications_UtilisateurId] ON [HistoriqueNotifications];
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HistoriqueNotifications]') AND [c].[name] = N'UtilisateurId');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [HistoriqueNotifications] DROP CONSTRAINT [' + @var1 + '];');
    EXEC(N'UPDATE [HistoriqueNotifications] SET [UtilisateurId] = 0 WHERE [UtilisateurId] IS NULL');
    ALTER TABLE [HistoriqueNotifications] ALTER COLUMN [UtilisateurId] int NOT NULL;
    ALTER TABLE [HistoriqueNotifications] ADD DEFAULT 0 FOR [UtilisateurId];
    CREATE INDEX [IX_HistoriqueNotifications_UtilisateurId] ON [HistoriqueNotifications] ([UtilisateurId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519104045_InitAlert'
)
BEGIN
    ALTER TABLE [HistoriqueNotifications] ADD [MessageErreur] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260519104045_InitAlert'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260519104045_InitAlert', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521085954_InitA'
)
BEGIN
    ALTER TABLE [Alertes] DROP CONSTRAINT [FK_Alertes_TypeDefauts_TypeDefaut1Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521085954_InitA'
)
BEGIN
    ALTER TABLE [Alertes] DROP CONSTRAINT [FK_Alertes_TypeDefauts_TypeDefaut2Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521085954_InitA'
)
BEGIN
    DROP INDEX [IX_Alertes_TypeDefaut1Id] ON [Alertes];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521085954_InitA'
)
BEGIN
    DROP INDEX [IX_Alertes_TypeDefaut2Id] ON [Alertes];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521085954_InitA'
)
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Seuils]') AND [c].[name] = N'SeuilQuantite');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Seuils] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Seuils] DROP COLUMN [SeuilQuantite];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521085954_InitA'
)
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Alertes]') AND [c].[name] = N'MachineId');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Alertes] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Alertes] DROP COLUMN [MachineId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521085954_InitA'
)
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Alertes]') AND [c].[name] = N'TypeDefaut1Id');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Alertes] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [Alertes] DROP COLUMN [TypeDefaut1Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521085954_InitA'
)
BEGIN
    DECLARE @var5 sysname;
    SELECT @var5 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Alertes]') AND [c].[name] = N'TypeDefaut2Id');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Alertes] DROP CONSTRAINT [' + @var5 + '];');
    ALTER TABLE [Alertes] DROP COLUMN [TypeDefaut2Id];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521085954_InitA'
)
BEGIN
    DECLARE @var6 sysname;
    SELECT @var6 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Alertes]') AND [c].[name] = N'Message');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Alertes] DROP CONSTRAINT [' + @var6 + '];');
    EXEC(N'UPDATE [Alertes] SET [Message] = N'''' WHERE [Message] IS NULL');
    ALTER TABLE [Alertes] ALTER COLUMN [Message] nvarchar(500) NOT NULL;
    ALTER TABLE [Alertes] ADD DEFAULT N'' FOR [Message];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521085954_InitA'
)
BEGIN
    CREATE TABLE [CommentairesAlerte] (
        [Id] int NOT NULL IDENTITY,
        [AlerteId] int NOT NULL,
        [AuteurId] int NOT NULL,
        [NomAuteur] nvarchar(100) NOT NULL,
        [Contenu] nvarchar(2000) NOT NULL,
        [DateCreation] datetime2 NOT NULL,
        CONSTRAINT [PK_CommentairesAlerte] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_CommentairesAlerte_Alertes_AlerteId] FOREIGN KEY ([AlerteId]) REFERENCES [Alertes] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521085954_InitA'
)
BEGIN
    CREATE INDEX [IX_CommentairesAlerte_AlerteId] ON [CommentairesAlerte] ([AlerteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260521085954_InitA'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260521085954_InitA', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525093730_AddPredictionDefaut'
)
BEGIN
    DECLARE @var7 sysname;
    SELECT @var7 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HistoriqueNotifications]') AND [c].[name] = N'MessageErreur');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [HistoriqueNotifications] DROP CONSTRAINT [' + @var7 + '];');
    ALTER TABLE [HistoriqueNotifications] DROP COLUMN [MessageErreur];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525093730_AddPredictionDefaut'
)
BEGIN
    CREATE TABLE [PredictionsDefauts] (
        [Id] int NOT NULL IDENTITY,
        [DatePrediction] datetime2 NOT NULL,
        [ResultatControleId] nvarchar(36) NOT NULL,
        [EstDefectueux] bit NOT NULL,
        [Probabilite] float NOT NULL,
        [TypeDefautPreditId] int NULL,
        [NiveauRisque] nvarchar(20) NOT NULL,
        [UtilisateurId] int NULL,
        CONSTRAINT [PK_PredictionsDefauts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PredictionsDefauts_ResultatControles_ResultatControleId] FOREIGN KEY ([ResultatControleId]) REFERENCES [ResultatControles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PredictionsDefauts_TypeDefauts_TypeDefautPreditId] FOREIGN KEY ([TypeDefautPreditId]) REFERENCES [TypeDefauts] ([Id]),
        CONSTRAINT [FK_PredictionsDefauts_Utilisateurs_UtilisateurId] FOREIGN KEY ([UtilisateurId]) REFERENCES [Utilisateurs] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525093730_AddPredictionDefaut'
)
BEGIN
    CREATE INDEX [IX_PredictionsDefauts_ResultatControleId] ON [PredictionsDefauts] ([ResultatControleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525093730_AddPredictionDefaut'
)
BEGIN
    CREATE INDEX [IX_PredictionsDefauts_TypeDefautPreditId] ON [PredictionsDefauts] ([TypeDefautPreditId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525093730_AddPredictionDefaut'
)
BEGIN
    CREATE INDEX [IX_PredictionsDefauts_UtilisateurId] ON [PredictionsDefauts] ([UtilisateurId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260525093730_AddPredictionDefaut'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260525093730_AddPredictionDefaut', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603105335_Addforgienkeydedefauts'
)
BEGIN
    ALTER TABLE [PredictionsDefauts] DROP CONSTRAINT [FK_PredictionsDefauts_ResultatControles_ResultatControleId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603105335_Addforgienkeydedefauts'
)
BEGIN
    ALTER TABLE [PredictionsDefauts] DROP CONSTRAINT [FK_PredictionsDefauts_TypeDefauts_TypeDefautPreditId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603105335_Addforgienkeydedefauts'
)
BEGIN
    ALTER TABLE [PredictionsDefauts] DROP CONSTRAINT [FK_PredictionsDefauts_Utilisateurs_UtilisateurId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603105335_Addforgienkeydedefauts'
)
BEGIN
    DROP INDEX [IX_PredictionsDefauts_ResultatControleId] ON [PredictionsDefauts];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603105335_Addforgienkeydedefauts'
)
BEGIN
    DROP INDEX [IX_PredictionsDefauts_TypeDefautPreditId] ON [PredictionsDefauts];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603105335_Addforgienkeydedefauts'
)
BEGIN
    DROP INDEX [IX_PredictionsDefauts_UtilisateurId] ON [PredictionsDefauts];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603105335_Addforgienkeydedefauts'
)
BEGIN
    EXEC sp_rename N'[PredictionsDefauts].[UtilisateurId]', N'LatenceMs', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603105335_Addforgienkeydedefauts'
)
BEGIN
    DECLARE @var8 sysname;
    SELECT @var8 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PredictionsDefauts]') AND [c].[name] = N'ResultatControleId');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [PredictionsDefauts] DROP CONSTRAINT [' + @var8 + '];');
    ALTER TABLE [PredictionsDefauts] ALTER COLUMN [ResultatControleId] nvarchar(max) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603105335_Addforgienkeydedefauts'
)
BEGIN
    DECLARE @var9 sysname;
    SELECT @var9 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PredictionsDefauts]') AND [c].[name] = N'Probabilite');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [PredictionsDefauts] DROP CONSTRAINT [' + @var9 + '];');
    ALTER TABLE [PredictionsDefauts] ALTER COLUMN [Probabilite] decimal(5,4) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603105335_Addforgienkeydedefauts'
)
BEGIN
    ALTER TABLE [PredictionsDefauts] ADD [FeaturesJson] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603105335_Addforgienkeydedefauts'
)
BEGIN
    ALTER TABLE [PredictionsDefauts] ADD [ModelVersion] nvarchar(20) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603105335_Addforgienkeydedefauts'
)
BEGIN
    ALTER TABLE [PredictionsDefauts] ADD [ShapExplicationJson] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260603105335_Addforgienkeydedefauts'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260603105335_Addforgienkeydedefauts', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605093950_Addlots'
)
BEGIN
    CREATE INDEX [IX_PredictionsDefauts_TypeDefautPreditId] ON [PredictionsDefauts] ([TypeDefautPreditId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605093950_Addlots'
)
BEGIN
    ALTER TABLE [PredictionsDefauts] ADD CONSTRAINT [FK_PredictionsDefauts_TypeDefauts_TypeDefautPreditId] FOREIGN KEY ([TypeDefautPreditId]) REFERENCES [TypeDefauts] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605093950_Addlots'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260605093950_Addlots', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605140215_Addlotsss'
)
BEGIN
    ALTER TABLE [ResultatControles] ADD [LotId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605140215_Addlotsss'
)
BEGIN
    ALTER TABLE [Machines] ADD [Statut] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605140215_Addlotsss'
)
BEGIN
    ALTER TABLE [Alertes] ADD [LotId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605140215_Addlotsss'
)
BEGIN
    CREATE TABLE [Lot] (
        [Id] int NOT NULL IDENTITY,
        [NumeroLot] nvarchar(50) NOT NULL,
        [MachineId] nvarchar(20) NOT NULL,
        [ProduitId] nvarchar(20) NOT NULL,
        [OperateurId] int NOT NULL,
        [DateDebut] datetime2 NOT NULL,
        [DateFin] datetime2 NULL,
        [QuantitePrevue] int NOT NULL,
        [QuantiteProduite] int NULL,
        [Statut] nvarchar(20) NOT NULL,
        [Commentaire] nvarchar(500) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NULL,
        CONSTRAINT [PK_Lot] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Lot_Machines_MachineId] FOREIGN KEY ([MachineId]) REFERENCES [Machines] ([CodeMachine]) ON DELETE CASCADE,
        CONSTRAINT [FK_Lot_Produits_ProduitId] FOREIGN KEY ([ProduitId]) REFERENCES [Produits] ([CodeArticle]) ON DELETE CASCADE,
        CONSTRAINT [FK_Lot_Utilisateurs_OperateurId] FOREIGN KEY ([OperateurId]) REFERENCES [Utilisateurs] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605140215_Addlotsss'
)
BEGIN
    CREATE INDEX [IX_ResultatControles_LotId] ON [ResultatControles] ([LotId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605140215_Addlotsss'
)
BEGIN
    CREATE INDEX [IX_Alertes_LotId] ON [Alertes] ([LotId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605140215_Addlotsss'
)
BEGIN
    CREATE INDEX [IX_Lot_MachineId] ON [Lot] ([MachineId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605140215_Addlotsss'
)
BEGIN
    CREATE INDEX [IX_Lot_OperateurId] ON [Lot] ([OperateurId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605140215_Addlotsss'
)
BEGIN
    CREATE INDEX [IX_Lot_ProduitId] ON [Lot] ([ProduitId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605140215_Addlotsss'
)
BEGIN
    ALTER TABLE [Alertes] ADD CONSTRAINT [FK_Alertes_Lot_LotId] FOREIGN KEY ([LotId]) REFERENCES [Lot] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605140215_Addlotsss'
)
BEGIN
    ALTER TABLE [ResultatControles] ADD CONSTRAINT [FK_ResultatControles_Lot_LotId] FOREIGN KEY ([LotId]) REFERENCES [Lot] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260605140215_Addlotsss'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260605140215_Addlotsss', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    ALTER TABLE [Alertes] DROP CONSTRAINT [FK_Alertes_Machines_CodeMachine];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    ALTER TABLE [Alertes] DROP CONSTRAINT [FK_Alertes_Seuils_SeuilId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    ALTER TABLE [HistoriqueNotifications] DROP CONSTRAINT [FK_HistoriqueNotifications_Utilisateurs_UtilisateurId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    DECLARE @var10 sysname;
    SELECT @var10 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Alertes]') AND [c].[name] = N'TauxDetecte');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Alertes] DROP CONSTRAINT [' + @var10 + '];');
    ALTER TABLE [Alertes] ALTER COLUMN [TauxDetecte] decimal(18,2) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    DROP INDEX [IX_Alertes_Statut] ON [Alertes];
    DECLARE @var11 sysname;
    SELECT @var11 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Alertes]') AND [c].[name] = N'Statut');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Alertes] DROP CONSTRAINT [' + @var11 + '];');
    ALTER TABLE [Alertes] ALTER COLUMN [Statut] int NOT NULL;
    CREATE INDEX [IX_Alertes_Statut] ON [Alertes] ([Statut]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    DECLARE @var12 sysname;
    SELECT @var12 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Alertes]') AND [c].[name] = N'SeuilId');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [Alertes] DROP CONSTRAINT [' + @var12 + '];');
    ALTER TABLE [Alertes] ALTER COLUMN [SeuilId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    DECLARE @var13 sysname;
    SELECT @var13 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Alertes]') AND [c].[name] = N'Niveau');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Alertes] DROP CONSTRAINT [' + @var13 + '];');
    ALTER TABLE [Alertes] ALTER COLUMN [Niveau] int NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    DECLARE @var14 sysname;
    SELECT @var14 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Alertes]') AND [c].[name] = N'Message');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Alertes] DROP CONSTRAINT [' + @var14 + '];');
    ALTER TABLE [Alertes] ALTER COLUMN [Message] nvarchar(max) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    DECLARE @var15 sysname;
    SELECT @var15 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Alertes]') AND [c].[name] = N'CommentaireResolution');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [Alertes] DROP CONSTRAINT [' + @var15 + '];');
    ALTER TABLE [Alertes] ALTER COLUMN [CommentaireResolution] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    DROP INDEX [IX_Alertes_CodeMachine] ON [Alertes];
    DECLARE @var16 sysname;
    SELECT @var16 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Alertes]') AND [c].[name] = N'CodeMachine');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [Alertes] DROP CONSTRAINT [' + @var16 + '];');
    ALTER TABLE [Alertes] ALTER COLUMN [CodeMachine] nvarchar(450) NOT NULL;
    CREATE INDEX [IX_Alertes_CodeMachine] ON [Alertes] ([CodeMachine]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    DECLARE @var17 sysname;
    SELECT @var17 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Alertes]') AND [c].[name] = N'CodeArticle');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [Alertes] DROP CONSTRAINT [' + @var17 + '];');
    ALTER TABLE [Alertes] ALTER COLUMN [CodeArticle] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    ALTER TABLE [Alertes] ADD [NbNonConformesConsecutifs] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    ALTER TABLE [Alertes] ADD [NumOF] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    ALTER TABLE [Alertes] ADD CONSTRAINT [FK_Alertes_Seuils_SeuilId] FOREIGN KEY ([SeuilId]) REFERENCES [Seuils] ([Id]) ON DELETE SET NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    ALTER TABLE [HistoriqueNotifications] ADD CONSTRAINT [FK_HistoriqueNotifications_Utilisateurs_UtilisateurId] FOREIGN KEY ([UtilisateurId]) REFERENCES [Utilisateurs] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260609095947_Addemailnotifications'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260609095947_Addemailnotifications', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260610094431_Addemailnotificatio'
)
BEGIN
    ALTER TABLE [HistoriqueNotifications] DROP CONSTRAINT [FK_HistoriqueNotifications_Utilisateurs_UtilisateurId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260610094431_Addemailnotificatio'
)
BEGIN
    DROP INDEX [IX_HistoriqueNotifications_UtilisateurId] ON [HistoriqueNotifications];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260610094431_Addemailnotificatio'
)
BEGIN
    DECLARE @var18 sysname;
    SELECT @var18 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HistoriqueNotifications]') AND [c].[name] = N'Sujet');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [HistoriqueNotifications] DROP CONSTRAINT [' + @var18 + '];');
    EXEC(N'UPDATE [HistoriqueNotifications] SET [Sujet] = N'''' WHERE [Sujet] IS NULL');
    ALTER TABLE [HistoriqueNotifications] ALTER COLUMN [Sujet] nvarchar(255) NOT NULL;
    ALTER TABLE [HistoriqueNotifications] ADD DEFAULT N'' FOR [Sujet];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260610094431_Addemailnotificatio'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260610094431_Addemailnotificatio', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260610094827_Addemailnotificati'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260610094827_Addemailnotificati', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260610115722_Addemailnotific'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260610115722_Addemailnotific', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622140823_AddNumLotMatiereToResultatControle'
)
BEGIN
    ALTER TABLE [ResultatControles] ADD [NumLotMatiere] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260622140823_AddNumLotMatiereToResultatControle'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260622140823_AddNumLotMatiereToResultatControle', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624084215_AddNumLotMatiereToResultat'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260624084215_AddNumLotMatiereToResultat', N'9.0.2');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260629101218_FixGestionDynamiqueAcces'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260629101218_FixGestionDynamiqueAcces', N'9.0.2');
END;

COMMIT;
GO

