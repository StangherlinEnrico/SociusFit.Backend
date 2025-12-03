using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LikerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LikedUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    User1Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    User2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaxDistance = table.Column<int>(type: "int", nullable: false, defaultValue: 25),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RevokedTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevokedTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserCredentials",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCredentials", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ProfileComplete = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileSports",
                columns: table => new
                {
                    ProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileSports", x => new { x.ProfileId, x.SportId });
                    table.ForeignKey(
                        name: "FK_ProfileSports_Profiles_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileSports_Sports_SportId",
                        column: x => x.SportId,
                        principalTable: "Sports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Sports",
                columns: new[] { "Id", "CreatedAt", "Name" },
                values: new object[,]
                {
                    { new Guid("16987b1e-aaf1-728b-e3b8-f6644fe1437e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Arrampicata" },
                    { new Guid("203d490c-19a1-e430-9807-25d94b0203ec"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tennis" },
                    { new Guid("21bd7b7e-f9a2-8534-93a5-e5a07d47f872"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ginnastica" },
                    { new Guid("2439dc5f-df97-0d04-7bec-ce28583ce58f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fitness" },
                    { new Guid("4195f3bc-c3a3-073c-505b-6ab44e09aaf1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Boxe" },
                    { new Guid("4c81da5b-ed4a-26b1-8392-28f1a3d92f09"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Running" },
                    { new Guid("4fc9e18b-10dc-5f94-5704-322b08faa2a2"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Yoga" },
                    { new Guid("51809cf1-3dc8-72ca-2870-dbece5e52c58"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Basket" },
                    { new Guid("5d4db5e4-68cc-ff7a-0de2-7789d93eed47"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sci" },
                    { new Guid("73e73d70-c76f-0a28-781e-cafec1d9401f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pilates" },
                    { new Guid("79c2a2c2-365c-a9b6-192f-c86ce7d1b385"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Nuoto" },
                    { new Guid("83480abc-b2cb-09e0-861e-bf4edb230003"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Padel" },
                    { new Guid("8e6cb6ad-9a70-6895-a974-26b30acae6ab"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Escursionismo" },
                    { new Guid("998bc4df-0b46-659e-876e-327cb5c885f2"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Calcio" },
                    { new Guid("9d2f4a97-ffe4-3f99-bd4d-1cd3fb80865e"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Skateboard" },
                    { new Guid("a1d5be39-5b1a-ab57-ec2d-b1bb45795d37"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Danza" },
                    { new Guid("cac8b249-1d9f-694a-eef1-f09dfc92f579"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Arti Marziali" },
                    { new Guid("cc308f6c-960a-91dc-5cd0-1b9b76190df4"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pattinaggio" },
                    { new Guid("e15416f1-6e4a-b7c3-48c3-70e0a684361a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Surf" },
                    { new Guid("e4cb6107-b11e-4d8d-ed2e-24a1104f517f"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Snowboard" },
                    { new Guid("e91d6d84-be4f-c402-f94c-1ecf10c331e2"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pallavolo" },
                    { new Guid("eac96669-a004-51e6-f69f-113113a45e27"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CrossFit" },
                    { new Guid("f397af20-9c43-b574-b5ec-db2feaac528a"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ciclismo" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_Token",
                table: "DeviceTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_UserId",
                table: "DeviceTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceTokens_UserId_IsActive",
                table: "DeviceTokens",
                columns: new[] { "UserId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Likes_LikedUserId",
                table: "Likes",
                column: "LikedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_LikerUserId",
                table: "Likes",
                column: "LikerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_LikerUserId_LikedUserId",
                table: "Likes",
                columns: new[] { "LikerUserId", "LikedUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matches_CreatedAt",
                table: "Matches",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_User1Id",
                table: "Matches",
                column: "User1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_User2Id",
                table: "Matches",
                column: "User2Id");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MatchId",
                table: "Messages",
                column: "MatchId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MatchId_SentAt",
                table: "Messages",
                columns: new[] { "MatchId", "SentAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderId",
                table: "Messages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedAt",
                table: "Notifications",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId_Type",
                table: "Notifications",
                columns: new[] { "UserId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_Age",
                table: "Profiles",
                column: "Age");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_City",
                table: "Profiles",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_Latitude_Longitude",
                table: "Profiles",
                columns: new[] { "Latitude", "Longitude" });

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_UserId",
                table: "Profiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSports_ProfileId",
                table: "ProfileSports",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileSports_SportId",
                table: "ProfileSports",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_RevokedTokens_ExpiresAt",
                table: "RevokedTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_RevokedTokens_RevokedAt",
                table: "RevokedTokens",
                column: "RevokedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RevokedTokens_TokenId",
                table: "RevokedTokens",
                column: "TokenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RevokedTokens_UserId",
                table: "RevokedTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Sports_Name",
                table: "Sports",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCredentials_UserId",
                table: "UserCredentials",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceTokens");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "ProfileSports");

            migrationBuilder.DropTable(
                name: "RevokedTokens");

            migrationBuilder.DropTable(
                name: "UserCredentials");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "Sports");
        }
    }
}
