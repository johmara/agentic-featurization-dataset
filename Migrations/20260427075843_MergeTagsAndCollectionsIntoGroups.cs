using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReferenceManager.Migrations
{
    /// <inheritdoc />
    public partial class MergeTagsAndCollectionsIntoGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaperGroup",
                columns: table => new
                {
                    GroupsId = table.Column<int>(type: "INTEGER", nullable: false),
                    PapersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaperGroup", x => new { x.GroupsId, x.PapersId });
                    table.ForeignKey(
                        name: "FK_PaperGroup_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaperGroup_Papers_PapersId",
                        column: x => x.PapersId,
                        principalTable: "Papers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaperGroup_PapersId",
                table: "PaperGroup",
                column: "PapersId");

            // Migrate Collections → Groups (preserving IDs and descriptions)
            migrationBuilder.Sql(
                "INSERT INTO Groups (Id, Name, Description) SELECT Id, Name, Description FROM Collections;");

            // Migrate Tags → Groups (IDs offset by max Collections.Id, no description)
            migrationBuilder.Sql(
                "INSERT INTO Groups (Id, Name, Description) " +
                "SELECT (SELECT COALESCE(MAX(Id), 0) FROM Collections) + Id, Name, NULL FROM Tags;");

            // Migrate PaperCollection → PaperGroup
            migrationBuilder.Sql(
                "INSERT INTO PaperGroup (GroupsId, PapersId) SELECT CollectionsId, PapersId FROM PaperCollection;");

            // Migrate PaperTag → PaperGroup (with same ID offset applied to tag IDs)
            migrationBuilder.Sql(
                "INSERT INTO PaperGroup (GroupsId, PapersId) " +
                "SELECT (SELECT COALESCE(MAX(Id), 0) FROM Collections) + TagsId, PapersId FROM PaperTag;");

            migrationBuilder.DropTable(name: "PaperCollection");
            migrationBuilder.DropTable(name: "PaperTag");
            migrationBuilder.DropTable(name: "Collections");
            migrationBuilder.DropTable(name: "Tags");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaperGroup");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaperCollection",
                columns: table => new
                {
                    CollectionsId = table.Column<int>(type: "INTEGER", nullable: false),
                    PapersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaperCollection", x => new { x.CollectionsId, x.PapersId });
                    table.ForeignKey(
                        name: "FK_PaperCollection_Collections_CollectionsId",
                        column: x => x.CollectionsId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaperCollection_Papers_PapersId",
                        column: x => x.PapersId,
                        principalTable: "Papers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaperTag",
                columns: table => new
                {
                    PapersId = table.Column<int>(type: "INTEGER", nullable: false),
                    TagsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaperTag", x => new { x.PapersId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_PaperTag_Papers_PapersId",
                        column: x => x.PapersId,
                        principalTable: "Papers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaperTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaperCollection_PapersId",
                table: "PaperCollection",
                column: "PapersId");

            migrationBuilder.CreateIndex(
                name: "IX_PaperTag_TagsId",
                table: "PaperTag",
                column: "TagsId");
        }
    }
}
