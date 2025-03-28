using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigiNumberApplicationApi.TelegramBot.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sudo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<long>(type: "INTEGER", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sudo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ChatId = table.Column<long>(type: "INTEGER", maxLength: 20, nullable: false),
                    PhoneNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    IsVerify = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VirtualNumberDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 6, nullable: false),
                    CountryName = table.Column<string>(type: "TEXT", nullable: false),
                    Flag = table.Column<string>(type: "TEXT", maxLength: 60, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualNumberDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VirtualSessionDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApiId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ApiHash = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Password2Fa = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualSessionDetails", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VirtualNumber",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CountryCode = table.Column<string>(type: "TEXT", maxLength: 6, nullable: false),
                    Number = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    EStatusOrder = table.Column<string>(type: "TEXT", nullable: false),
                    VirtualSessionDetailsId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualNumber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VirtualNumber_VirtualSessionDetails_VirtualSessionDetailsId",
                        column: x => x.VirtualSessionDetailsId,
                        principalTable: "VirtualSessionDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderVirtualNumber",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<string>(type: "TEXT", maxLength: 15, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    VirtualNumberId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderVirtualNumber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderVirtualNumber_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderVirtualNumber_VirtualNumber_VirtualNumberId",
                        column: x => x.VirtualNumberId,
                        principalTable: "VirtualNumber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderVirtualNumber_UserId",
                table: "OrderVirtualNumber",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderVirtualNumber_VirtualNumberId",
                table: "OrderVirtualNumber",
                column: "VirtualNumberId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sudo_ChatId",
                table: "Sudo",
                column: "ChatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_ChatId",
                table: "User",
                column: "ChatId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VirtualNumber_Number",
                table: "VirtualNumber",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VirtualNumber_VirtualSessionDetailsId",
                table: "VirtualNumber",
                column: "VirtualSessionDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderVirtualNumber");

            migrationBuilder.DropTable(
                name: "Sudo");

            migrationBuilder.DropTable(
                name: "VirtualNumberDetails");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "VirtualNumber");

            migrationBuilder.DropTable(
                name: "VirtualSessionDetails");
        }
    }
}
