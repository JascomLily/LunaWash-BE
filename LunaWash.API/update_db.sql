BEGIN TRANSACTION;
ALTER TABLE [WashServices] ADD [IconName] nvarchar(100) NULL;

ALTER TABLE [WashServices] ADD [IsPopular] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [WashServices] ADD [ServiceType] nvarchar(50) NOT NULL DEFAULT N'Package';

ALTER TABLE [ServiceReviews] ADD [BranchId] nvarchar(50) NOT NULL DEFAULT N'';

ALTER TABLE [ServiceReviews] ADD [CustomerId] nvarchar(50) NOT NULL DEFAULT N'';

CREATE TABLE [BookingServices] (
    [Id] varchar(50) NOT NULL,
    [BookingId] varchar(50) NOT NULL,
    [WashServiceId] varchar(50) NOT NULL,
    [PriceAtTime] decimal(18,2) NOT NULL,
    [DurationAtTime] int NOT NULL,
    CONSTRAINT [PK_BookingServices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BookingServices_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BookingServices_WashServices_WashServiceId] FOREIGN KEY ([WashServiceId]) REFERENCES [WashServices] ([Id])
);

CREATE TABLE [Equipments] (
    [Id] nvarchar(50) NOT NULL,
    [BranchId] varchar(50) NOT NULL,
    [Name] nvarchar(150) NOT NULL,
    [Category] nvarchar(100) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [Priority] nvarchar(50) NOT NULL,
    [LastMaintenance] nvarchar(50) NULL,
    [NextMaintenance] nvarchar(50) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Equipments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Equipments_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ServiceFeatures] (
    [Id] varchar(50) NOT NULL,
    [WashServiceId] varchar(50) NOT NULL,
    [FeatureText] nvarchar(250) NOT NULL,
    [DisplayOrder] int NOT NULL,
    CONSTRAINT [PK_ServiceFeatures] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ServiceFeatures_WashServices_WashServiceId] FOREIGN KEY ([WashServiceId]) REFERENCES [WashServices] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [MaintenanceTasks] (
    [Id] nvarchar(50) NOT NULL,
    [EquipmentId] nvarchar(50) NOT NULL,
    [BranchId] varchar(50) NOT NULL,
    [TaskName] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Status] nvarchar(50) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_MaintenanceTasks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MaintenanceTasks_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MaintenanceTasks_Equipments_EquipmentId] FOREIGN KEY ([EquipmentId]) REFERENCES [Equipments] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_BookingServices_BookingId] ON [BookingServices] ([BookingId]);

CREATE INDEX [IX_BookingServices_WashServiceId] ON [BookingServices] ([WashServiceId]);

CREATE INDEX [IX_Equipments_BranchId] ON [Equipments] ([BranchId]);

CREATE INDEX [IX_MaintenanceTasks_BranchId] ON [MaintenanceTasks] ([BranchId]);

CREATE INDEX [IX_MaintenanceTasks_EquipmentId] ON [MaintenanceTasks] ([EquipmentId]);

CREATE INDEX [IX_ServiceFeatures_WashServiceId] ON [ServiceFeatures] ([WashServiceId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260628114113_AddDynamicServices', N'10.0.8');

COMMIT;
GO

