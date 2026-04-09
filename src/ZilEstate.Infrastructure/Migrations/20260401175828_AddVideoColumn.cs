using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ZilEstate.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    District = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Bedrooms = table.Column<int>(type: "integer", nullable: true),
                    Bathrooms = table.Column<int>(type: "integer", nullable: true),
                    Parking = table.Column<int>(type: "integer", nullable: true),
                    SizeM2 = table.Column<double>(type: "double precision", nullable: true),
                    SizeArpents = table.Column<double>(type: "double precision", nullable: true),
                    Furnished = table.Column<bool>(type: "boolean", nullable: true),
                    SellerName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SellerPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    SellerWhatsApp = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SellerEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    LocationId = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    VideoUrl = table.Column<string>(type: "text", nullable: true),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    IsFeatured = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Properties_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropertyImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: false),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    PropertyId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyImages_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyId = table.Column<int>(type: "integer", nullable: false),
                    AuthorName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Question = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Answer = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AnsweredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropertyId = table.Column<int>(type: "integer", nullable: false),
                    AuthorName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "City", "District", "Latitude", "Longitude" },
                values: new object[,]
                {
                    { 1, "Port Louis", "Port Louis", -20.160900000000002, 57.497700000000002 },
                    { 2, "Curepipe", "Plaines Wilhems", -20.3156, 57.526400000000002 },
                    { 3, "Quatre Bornes", "Plaines Wilhems", -20.2654, 57.480400000000003 },
                    { 4, "Vacoas-Phoenix", "Plaines Wilhems", -20.297999999999998, 57.4953 },
                    { 5, "Rose Hill", "Plaines Wilhems", -20.239100000000001, 57.462899999999998 },
                    { 6, "Tamarin", "Black River", -20.328099999999999, 57.376899999999999 },
                    { 7, "Flic en Flac", "Black River", -20.2761, 57.362699999999997 },
                    { 8, "La Gaulette", "Black River", -20.497699999999998, 57.366199999999999 },
                    { 9, "Grand Baie", "Rivière du Rempart", -20.008700000000001, 57.583100000000002 },
                    { 10, "Pamplemousses", "Pamplemousses", -20.098600000000001, 57.575600000000001 },
                    { 11, "Centre de Flacq", "Flacq", -20.188300000000002, 57.714500000000001 },
                    { 12, "Mahébourg", "Grand Port", -20.407800000000002, 57.7029 },
                    { 13, "Souillac", "Savanne", -20.520099999999999, 57.5139 },
                    { 14, "Moka", "Moka", -20.2256, 57.5381 },
                    { 15, "Triolet", "Rivière du Rempart", -20.057500000000001, 57.542000000000002 },
                    { 16, "Chamarel", "Black River", -20.4299, 57.387799999999999 },
                    { 17, "Goodlands", "Rivière du Rempart", -19.994299999999999, 57.644399999999997 },
                    { 18, "Belle Mare", "Flacq", -20.1919, 57.767200000000003 },
                    { 19, "Blue Bay", "Grand Port", -20.448, 57.714799999999997 },
                    { 20, "Terre Rouge", "Pamplemousses", -20.145600000000002, 57.550800000000002 }
                });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "Bathrooms", "Bedrooms", "CreatedAt", "Description", "Furnished", "IsApproved", "IsFeatured", "Latitude", "LocationId", "Longitude", "Parking", "Price", "SellerEmail", "SellerName", "SellerPhone", "SellerWhatsApp", "SizeArpents", "SizeM2", "Status", "Title", "Type", "VideoUrl" },
                values: new object[,]
                {
                    { 1, 3, 4, new DateTime(2026, 3, 27, 0, 0, 0, 0, DateTimeKind.Utc), "Stunning 4-bedroom villa with private pool and sea view. Perfect for families looking for luxury living in the north of Mauritius.", true, true, true, -20.008700000000001, 9, 57.583100000000002, 2, 15000000m, "jmdupont@email.com", "Jean-Marc Dupont", "+230 5742 8834", "+23057428834", null, 350.0, 0, "Luxury Villa in Grand Baie", 1, null },
                    { 2, 1, 2, new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Bright 2-bedroom apartment on the 3rd floor with balcony. Close to supermarkets, schools, and the city center.", false, true, true, -20.2654, 3, 57.480400000000003, 1, 3500000m, "priya.r@email.com", "Priya Ramgoolam", "+230 5251 9012", "+23052519012", null, 85.0, 0, "Modern Apartment in Quatre Bornes", 2, null },
                    { 3, null, null, new DateTime(2026, 3, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Rare 15-arpent plot with panoramic sea view in the exclusive Tamarin area. Building permit available.", null, true, false, -20.328099999999999, 6, 57.376899999999999, null, 8500000m, null, "Antoine Leclercq", "+230 5888 2345", "+23058882345", 15.0, null, 0, "Land Plot in Tamarin - Sea View", 3, null },
                    { 4, 2, 3, new DateTime(2026, 3, 30, 0, 0, 0, 0, DateTimeKind.Utc), "Spacious 3-bedroom house available for long-term rental. Quiet neighborhood, fully furnished, with garden and covered parking.", true, true, true, -20.3156, 2, 57.526400000000002, 1, 35000m, "mc.bonnefoy@email.com", "Marie-Claire Bonnefoy", "+230 5314 6789", null, null, 200.0, 1, "House for Rent in Curepipe", 0, null },
                    { 5, null, null, new DateTime(2026, 3, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Prime commercial space in the heart of Port Louis business district. Ground floor, high visibility, ideal for retail or office.", null, true, false, -20.160900000000002, 1, 57.497700000000002, null, 45000m, "karim.b@business.mu", "Karim Bhunjun", "+230 5456 7890", "+23054567890", null, 120.0, 1, "Commercial Space in Port Louis", 4, null },
                    { 6, 4, 5, new DateTime(2026, 3, 31, 0, 0, 0, 0, DateTimeKind.Utc), "Exceptional beachfront villa with direct beach access. 5 bedrooms, private infinity pool, fully equipped kitchen and outdoor BBQ area.", true, true, true, -20.2761, 7, 57.362699999999997, 3, 25000000m, "sophie.b@lux.mu", "Sophie Bertrand", "+230 5567 2345", "+23055672345", null, 500.0, 0, "Beachfront Villa in Flic en Flac", 1, null }
                });

            migrationBuilder.InsertData(
                table: "PropertyImages",
                columns: new[] { "Id", "IsPrimary", "PropertyId", "Url" },
                values: new object[,]
                {
                    { 1, true, 1, "https://images.unsplash.com/photo-1613490493576-7fde63acd811?w=800" },
                    { 2, false, 1, "https://images.unsplash.com/photo-1613977257363-707ba9348227?w=800" },
                    { 3, true, 2, "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800" },
                    { 4, false, 2, "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800" },
                    { 5, true, 3, "https://images.unsplash.com/photo-1500382017468-9049fed747ef?w=800" },
                    { 6, true, 4, "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=800" },
                    { 7, true, 5, "https://images.unsplash.com/photo-1497366216548-37526070297c?w=800" },
                    { 8, true, 6, "https://images.unsplash.com/photo-1520250497591-112f2f40a3f4?w=800" },
                    { 9, false, 6, "https://images.unsplash.com/photo-1571003123894-1f0594d2b5d9?w=800" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_CreatedAt",
                table: "Properties",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_IsApproved",
                table: "Properties",
                column: "IsApproved");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_LocationId",
                table: "Properties",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Status",
                table: "Properties",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Properties_Type",
                table: "Properties",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyImages_PropertyId",
                table: "PropertyImages",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_PropertyId",
                table: "Questions",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_PropertyId",
                table: "Reviews",
                column: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyImages");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
