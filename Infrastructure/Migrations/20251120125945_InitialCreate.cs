using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email_verified_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    provider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    provider_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    latitude = table.Column<decimal>(type: "decimal(10,8)", precision: 10, scale: 8, nullable: true),
                    longitude = table.Column<decimal>(type: "decimal(11,8)", precision: 11, scale: 8, nullable: true),
                    max_distance_km = table.Column<int>(type: "int", nullable: true),
                    email_verification_token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    email_verification_token_expires_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    table_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    record_id = table.Column<int>(type: "int", nullable: true),
                    old_values = table.Column<string>(type: "text", nullable: true),
                    new_values = table.Column<string>(type: "text", nullable: true),
                    ip_address = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_audit_logs_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_created_at",
                table: "audit_logs",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_table_name",
                table: "audit_logs",
                column: "table_name");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_table_record",
                table: "audit_logs",
                columns: new[] { "table_name", "record_id" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_user_id",
                table: "audit_logs",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_deleted_at",
                table: "users",
                column: "deleted_at");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_email_verification_token",
                table: "users",
                column: "email_verification_token",
                filter: "[email_verification_token] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_location",
                table: "users",
                columns: new[] { "latitude", "longitude" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
