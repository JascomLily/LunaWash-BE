using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LunaWash.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SyncBannersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Banners', 'VoucherId') IS NULL
BEGIN
    ALTER TABLE dbo.Banners ADD VoucherId varchar(50) NULL;
END
");
            migrationBuilder.Sql(@"
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
");
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Banners', 'PlatformType') IS NULL
BEGIN
    ALTER TABLE dbo.Banners ADD PlatformType nvarchar(max) NOT NULL DEFAULT 'Web';
END
");
            migrationBuilder.Sql(@"
IF COL_LENGTH('dbo.Banners', 'IsHidden') IS NULL
BEGIN
    ALTER TABLE dbo.Banners ADD IsHidden bit NOT NULL DEFAULT 0;
END
");
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Banners_Vouchers_VoucherId')
BEGIN
    ALTER TABLE dbo.Banners ADD CONSTRAINT FK_Banners_Vouchers_VoucherId FOREIGN KEY (VoucherId) REFERENCES dbo.Vouchers(Id);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
