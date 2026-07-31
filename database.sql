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
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260613005203_InitialSync', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260613005238_AddPointHistorySystem', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [Bookings] ADD [TotalPrice] decimal(18,2) NOT NULL DEFAULT 0.0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260613005528_AddTotalPriceToBooking', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260613010053_AddLoyaltyAndTotalPrice', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [ServiceReviews] (
    [Id] nvarchar(50) NOT NULL,
    [BookingId] varchar(50) NOT NULL,
    [OverallRating] float NOT NULL,
    [CleanlinessRating] int NOT NULL,
    [SpeedRating] int NOT NULL,
    [StaffRating] int NOT NULL,
    [Comment] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ServiceReviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ServiceReviews_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
);

CREATE UNIQUE INDEX [IX_ServiceReviews_BookingId] ON [ServiceReviews] ([BookingId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260620050630_AddServiceReviewV2', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [ServiceReviews] ADD [BranchId] nvarchar(50) NOT NULL DEFAULT N'';

ALTER TABLE [ServiceReviews] ADD [CustomerId] nvarchar(50) NOT NULL DEFAULT N'';

CREATE TABLE [Attendances] (
    [Id] varchar(50) NOT NULL,
    [UserId] varchar(50) NOT NULL,
    [BranchId] varchar(50) NOT NULL,
    [AttendanceDate] datetime2 NOT NULL,
    [CheckInTime] datetime2 NULL,
    [CheckOutTime] datetime2 NULL,
    [Status] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_Attendances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Attendances_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Attendances_Users] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
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

CREATE INDEX [IX_Attendances_BranchId] ON [Attendances] ([BranchId]);

CREATE INDEX [IX_Attendances_UserId] ON [Attendances] ([UserId]);

CREATE INDEX [IX_Equipments_BranchId] ON [Equipments] ([BranchId]);

CREATE INDEX [IX_MaintenanceTasks_BranchId] ON [MaintenanceTasks] ([BranchId]);

CREATE INDEX [IX_MaintenanceTasks_EquipmentId] ON [MaintenanceTasks] ([EquipmentId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260628094101_AddAttendance_Recreated', N'10.0.8');

COMMIT;
GO

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

BEGIN TRANSACTION;
CREATE TABLE [ServicePackages] (
    [Id] varchar(50) NOT NULL,
    [Name] nvarchar(150) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Price] decimal(18,2) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL DEFAULT ((getutcdate())),
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ServicePackages] PRIMARY KEY ([Id])
);

CREATE TABLE [PackageServices] (
    [PackageId] varchar(50) NOT NULL,
    [ServiceId] varchar(50) NOT NULL,
    CONSTRAINT [PK_PackageServices] PRIMARY KEY ([PackageId], [ServiceId]),
    CONSTRAINT [FK_PackageServices_ServicePackages_PackageId] FOREIGN KEY ([PackageId]) REFERENCES [ServicePackages] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PackageServices_WashServices_ServiceId] FOREIGN KEY ([ServiceId]) REFERENCES [WashServices] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_PackageServices_ServiceId] ON [PackageServices] ([ServiceId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260628183946_AddServicePackages', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [Promotions] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Code] nvarchar(50) NOT NULL,
    [DiscountPercent] int NOT NULL,
    [MaxUsage] int NULL,
    [CurrentUsage] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Promotions] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260630041451_AddPromotions', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
CREATE TABLE [StaffProfiles] (
    [UserId] varchar(50) NOT NULL,
    [Salary] decimal(18,2) NOT NULL,
    [LeaveDays] int NOT NULL,
    [UsedLeaveDays] int NOT NULL,
    CONSTRAINT [PK_StaffProfiles] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_StaffProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260701003739_AddStaffProfile', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [MembershipTiers] ADD [MaxBookingDays] int NOT NULL DEFAULT 0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260714015755_AddMaxBookingDays', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [ServiceReviews] ADD [Reply] nvarchar(2000) NULL;

CREATE TABLE [Notifications] (
    [Id] nvarchar(50) NOT NULL,
    [UserId] varchar(50) NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [Type] nvarchar(50) NOT NULL,
    [IsRead] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260714073511_AddNotificationsAndReviewReply', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
DROP TABLE [Promotions];

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260714130425_RemovePromotions_AddVouchers', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260714133849_AddBannersTable', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260714155453_IncreaseBookingNotesLength', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
DECLARE @var nvarchar(max);
SELECT @var = QUOTENAME([d].[name])
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Bookings]') AND [c].[name] = N'Notes');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Bookings] DROP CONSTRAINT ' + @var + ';');
ALTER TABLE [Bookings] ALTER COLUMN [Notes] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260714160216_RemoveBookingNotesLengthLimit', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [Banners] ADD [IsHidden] bit NOT NULL DEFAULT CAST(0 AS bit);

ALTER TABLE [Banners] ADD [PlatformType] nvarchar(max) NOT NULL DEFAULT N'';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260714165357_AddBannerPlatformAndHidden', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;

IF COL_LENGTH('dbo.Banners', 'VoucherId') IS NULL
BEGIN
    ALTER TABLE dbo.Banners ADD VoucherId varchar(50) NULL;
END



DECLARE @ConstraintName nvarchar(200);

-- Drop constraint for Title
SET @ConstraintName = (SELECT Name FROM sys.default_constraints WHERE PARENT_OBJECT_ID = OBJECT_ID('dbo.Banners') AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns WHERE NAME = N'Title' AND object_id = OBJECT_ID(N'dbo.Banners')));
IF @ConstraintName IS NOT NULL EXEC('ALTER TABLE dbo.Banners DROP CONSTRAINT ' + @ConstraintName);
IF COL_LENGTH('dbo.Banners', 'Title') IS NOT NULL ALTER TABLE dbo.Banners DROP COLUMN Title;

-- Drop constraint for RedirectUrl
SET @ConstraintName = (SELECT Name FROM sys.default_constraints WHERE PARENT_OBJECT_ID = OBJECT_ID('dbo.Banners') AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns WHERE NAME = N'RedirectUrl' AND object_id = OBJECT_ID(N'dbo.Banners')));
IF @ConstraintName IS NOT NULL EXEC('ALTER TABLE dbo.Banners DROP CONSTRAINT ' + @ConstraintName);
IF COL_LENGTH('dbo.Banners', 'RedirectUrl') IS NOT NULL ALTER TABLE dbo.Banners DROP COLUMN RedirectUrl;

-- Drop constraint for Position
SET @ConstraintName = (SELECT Name FROM sys.default_constraints WHERE PARENT_OBJECT_ID = OBJECT_ID('dbo.Banners') AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns WHERE NAME = N'Position' AND object_id = OBJECT_ID(N'dbo.Banners')));
IF @ConstraintName IS NOT NULL EXEC('ALTER TABLE dbo.Banners DROP CONSTRAINT ' + @ConstraintName);
IF COL_LENGTH('dbo.Banners', 'Position') IS NOT NULL ALTER TABLE dbo.Banners DROP COLUMN Position;

-- Drop constraint for IsActive
SET @ConstraintName = (SELECT Name FROM sys.default_constraints WHERE PARENT_OBJECT_ID = OBJECT_ID('dbo.Banners') AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns WHERE NAME = N'IsActive' AND object_id = OBJECT_ID(N'dbo.Banners')));
IF @ConstraintName IS NOT NULL EXEC('ALTER TABLE dbo.Banners DROP CONSTRAINT ' + @ConstraintName);
IF COL_LENGTH('dbo.Banners', 'IsActive') IS NOT NULL ALTER TABLE dbo.Banners DROP COLUMN IsActive;

-- Drop constraint for CreatedAt
SET @ConstraintName = (SELECT Name FROM sys.default_constraints WHERE PARENT_OBJECT_ID = OBJECT_ID('dbo.Banners') AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns WHERE NAME = N'CreatedAt' AND object_id = OBJECT_ID(N'dbo.Banners')));
IF @ConstraintName IS NOT NULL EXEC('ALTER TABLE dbo.Banners DROP CONSTRAINT ' + @ConstraintName);
IF COL_LENGTH('dbo.Banners', 'CreatedAt') IS NOT NULL ALTER TABLE dbo.Banners DROP COLUMN CreatedAt;

-- Drop constraint for UpdatedAt
SET @ConstraintName = (SELECT Name FROM sys.default_constraints WHERE PARENT_OBJECT_ID = OBJECT_ID('dbo.Banners') AND PARENT_COLUMN_ID = (SELECT column_id FROM sys.columns WHERE NAME = N'UpdatedAt' AND object_id = OBJECT_ID(N'dbo.Banners')));
IF @ConstraintName IS NOT NULL EXEC('ALTER TABLE dbo.Banners DROP CONSTRAINT ' + @ConstraintName);
IF COL_LENGTH('dbo.Banners', 'UpdatedAt') IS NOT NULL ALTER TABLE dbo.Banners DROP COLUMN UpdatedAt;



IF COL_LENGTH('dbo.Banners', 'PlatformType') IS NULL
BEGIN
    ALTER TABLE dbo.Banners ADD PlatformType nvarchar(max) NOT NULL DEFAULT 'Web';
END



IF COL_LENGTH('dbo.Banners', 'IsHidden') IS NULL
BEGIN
    ALTER TABLE dbo.Banners ADD IsHidden bit NOT NULL DEFAULT 0;
END



IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Banners_Vouchers_VoucherId')
BEGIN
    ALTER TABLE dbo.Banners ADD CONSTRAINT FK_Banners_Vouchers_VoucherId FOREIGN KEY (VoucherId) REFERENCES dbo.Vouchers(Id);
END


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260715010900_SyncBannersTable', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
DROP TABLE dbo.Banners;

CREATE TABLE [Banners] (
    [Id] int NOT NULL IDENTITY,
    [ImageUrl] nvarchar(max) NOT NULL,
    [VoucherId] varchar(50) NULL,
    [PlatformType] nvarchar(max) NOT NULL,
    [IsHidden] bit NOT NULL,
    CONSTRAINT [PK_Banners] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Banners_Vouchers_VoucherId] FOREIGN KEY ([VoucherId]) REFERENCES [Vouchers] ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260715015040_RecreateBannersWithIdentity', N'10.0.8');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [MembershipTiers] ADD [MinMaintainPoints] int NOT NULL DEFAULT 0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260715015219_AddMinMaintainPointsToMembershipTier', N'10.0.8');

COMMIT;
GO

