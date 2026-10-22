using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DemoMVC.Migrations
{
    /// <inheritdoc />
    public partial class FacultyMajorStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Faculties",
                columns: table => new
                {
                    FacultyCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculties", x => x.FacultyCode);
                });

            migrationBuilder.CreateTable(
                name: "TrainingSystems",
                columns: table => new
                {
                    TrainingSystemCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSystems", x => x.TrainingSystemCode);
                });

            migrationBuilder.CreateTable(
                name: "Majors",
                columns: table => new
                {
                    MajorCode = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FacultyCode = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Majors", x => x.MajorCode);
                    table.ForeignKey(
                        name: "FK_Majors_Faculties_FacultyCode",
                        column: x => x.FacultyCode,
                        principalTable: "Faculties",
                        principalColumn: "FacultyCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EnrollmentYear = table.Column<int>(type: "int", nullable: false),
                    TrainingSystemCode = table.Column<string>(type: "nvarchar(2)", nullable: false),
                    MajorCode = table.Column<string>(type: "nvarchar(2)", nullable: false),
                    FacultyCode = table.Column<string>(type: "nvarchar(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Faculties_FacultyCode",
                        column: x => x.FacultyCode,
                        principalTable: "Faculties",
                        principalColumn: "FacultyCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_Majors_MajorCode",
                        column: x => x.MajorCode,
                        principalTable: "Majors",
                        principalColumn: "MajorCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Students_TrainingSystems_TrainingSystemCode",
                        column: x => x.TrainingSystemCode,
                        principalTable: "TrainingSystems",
                        principalColumn: "TrainingSystemCode",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Faculties",
                columns: new[] { "FacultyCode", "Name" },
                values: new object[,]
                {
                    { "CD", "Khoa Cơ điện" },
                    { "CNTT", "Khoa Công nghệ thông tin" },
                    { "DKNL", "Khoa Dầu khí và Năng lượng" },
                    { "GDQP", "Khoa Giáo dục Quốc phòng" },
                    { "KHCB", "Khoa Khoa học cơ bản" },
                    { "KHKTDC", "Khoa Khoa học và Kỹ thuật Địa chất" },
                    { "KTQTKD", "Khoa Kinh tế và Quản trị kinh doanh" },
                    { "LLCT", "Khoa Lý luận Chính trị" },
                    { "MO", "Khoa Mỏ" },
                    { "MT", "Khoa Môi trường" },
                    { "TDBD", "Khoa Trắc địa - Bản đồ và Quản lý đất đai" },
                    { "XD", "Khoa Xây dựng" }
                });

            migrationBuilder.InsertData(
                table: "TrainingSystems",
                columns: new[] { "TrainingSystemCode", "Name" },
                values: new object[] { "21", "Đại học chính quy" });

            migrationBuilder.InsertData(
                table: "Majors",
                columns: new[] { "MajorCode", "FacultyCode", "Name" },
                values: new object[] { "05", "CNTT", "Công nghệ thông tin" });

            migrationBuilder.CreateIndex(
                name: "IX_Majors_FacultyCode",
                table: "Majors",
                column: "FacultyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Students_FacultyCode",
                table: "Students",
                column: "FacultyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Students_MajorCode",
                table: "Students",
                column: "MajorCode");

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentCode",
                table: "Students",
                column: "StudentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_TrainingSystemCode",
                table: "Students",
                column: "TrainingSystemCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Majors");

            migrationBuilder.DropTable(
                name: "TrainingSystems");

            migrationBuilder.DropTable(
                name: "Faculties");
        }
    }
}
