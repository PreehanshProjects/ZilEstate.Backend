using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ZilEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAgencyAndRentalType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgencyId",
                table: "Properties",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinimumStayDays",
                table: "Properties",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerNight",
                table: "Properties",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerWeek",
                table: "Properties",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RentalType",
                table: "Properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Properties",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Agencies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "text", nullable: true),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AgencyId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Agencies_AgencyId",
                        column: x => x.AgencyId,
                        principalTable: "Agencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Agencies",
                columns: new[] { "Id", "Address", "CreatedAt", "Description", "Email", "IsVerified", "LogoUrl", "Name", "Phone", "Website" },
                values: new object[,]
                {
                    { 1, "Grand Baie La Croisette, Grand Baie", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Specializing in high-end luxury villas and beachfront estates across Mauritius. Our team of experts provides personalized service to help you find your dream home.", "info@luxurymauritius.mu", true, "https://images.unsplash.com/photo-1560518883-ce09059eeffa?w=200", "Luxury Mauritius Properties", "+230 5250 1000", "https://luxurymauritius.mu" },
                    { 2, "Ebene Cybercity, Ebene", new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Your trusted partner for residential and commercial properties in Mauritius. We cover all districts and offer a wide range of options for every budget.", "contact@paradiseisland.mu", true, "https://images.unsplash.com/photo-1560520653-9e0e4c89eb11?w=200", "Paradise Island Real Estate", "+230 5450 2000", "https://paradiseisland.mu" }
                });

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { -20.277999999999999, 57.368000000000002 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AgencyId", "MinimumStayDays", "PricePerNight", "PricePerWeek", "RentalType", "UserId", "ViewCount" },
                values: new object[] { 1, null, null, null, 0, null, 0 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AgencyId", "MinimumStayDays", "PricePerNight", "PricePerWeek", "RentalType", "UserId", "ViewCount" },
                values: new object[] { 2, null, null, null, 0, null, 0 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AgencyId", "MinimumStayDays", "PricePerNight", "PricePerWeek", "RentalType", "UserId", "ViewCount" },
                values: new object[] { 1, null, null, null, 0, null, 0 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AgencyId", "MinimumStayDays", "PricePerNight", "PricePerWeek", "RentalType", "UserId", "ViewCount" },
                values: new object[] { null, null, null, null, 0, null, 0 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AgencyId", "MinimumStayDays", "PricePerNight", "PricePerWeek", "RentalType", "UserId", "ViewCount" },
                values: new object[] { 2, null, null, null, 0, null, 0 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AgencyId", "Latitude", "Longitude", "MinimumStayDays", "PricePerNight", "PricePerWeek", "RentalType", "UserId", "ViewCount" },
                values: new object[] { 1, -20.277999999999999, 57.368000000000002, null, null, null, 0, null, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_AgencyId",
                table: "Properties",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_UserId",
                table: "Properties",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AgencyId",
                table: "Users",
                column: "AgencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Agencies_AgencyId",
                table: "Properties",
                column: "AgencyId",
                principalTable: "Agencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Users_UserId",
                table: "Properties",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Agencies_AgencyId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Users_UserId",
                table: "Properties");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Agencies");

            migrationBuilder.DropIndex(
                name: "IX_Properties_AgencyId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_UserId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "AgencyId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MinimumStayDays",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PricePerNight",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PricePerWeek",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "RentalType",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Properties");

            migrationBuilder.UpdateData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { -20.2761, 57.362699999999997 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Latitude", "Longitude" },
                values: new object[] { -20.2761, 57.362699999999997 });
        }
    }
}
