using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LunaWash.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddBranchDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Branches]') AND name = N'Description')
                BEGIN
                    ALTER TABLE [Branches] ADD [Description] nvarchar(500) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Branches]') AND name = N'ImageUrl')
                BEGIN
                    ALTER TABLE [Branches] ADD [ImageUrl] nvarchar(max) NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Branches]') AND name = N'Latitude')
                BEGIN
                    ALTER TABLE [Branches] ADD [Latitude] float NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Branches]') AND name = N'Longitude')
                BEGIN
                    ALTER TABLE [Branches] ADD [Longitude] float NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Branches]') AND name = N'Description')
                BEGIN
                    ALTER TABLE [Branches] DROP COLUMN [Description];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Branches]') AND name = N'ImageUrl')
                BEGIN
                    ALTER TABLE [Branches] DROP COLUMN [ImageUrl];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Branches]') AND name = N'Latitude')
                BEGIN
                    ALTER TABLE [Branches] DROP COLUMN [Latitude];
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Branches]') AND name = N'Longitude')
                BEGIN
                    ALTER TABLE [Branches] DROP COLUMN [Longitude];
                END
            ");
        }
    }
}
