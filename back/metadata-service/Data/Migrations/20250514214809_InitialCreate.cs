using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MetadataService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "authors",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    birth_year = table.Column<short>(type: "smallint", nullable: true),
                    death_year = table.Column<short>(type: "smallint", nullable: true),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authors", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "songs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    year = table.Column<short>(type: "smallint", nullable: true),
                    duration_sec = table.Column<int>(type: "integer", nullable: true),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_songs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    approved = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "song_authors",
                columns: table => new
                {
                    song_id = table.Column<Guid>(type: "uuid", nullable: false),
                    author_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_song_authors", x => new { x.song_id, x.author_id, x.role });
                    table.ForeignKey(
                        name: "FK_song_authors_authors_author_id",
                        column: x => x.author_id,
                        principalTable: "authors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_song_authors_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "transcripts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    song_id = table.Column<Guid>(type: "uuid", nullable: false),
                    segment_index = table.Column<int>(type: "integer", nullable: false),
                    start_ms = table.Column<int>(type: "integer", nullable: false),
                    end_ms = table.Column<int>(type: "integer", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    confidence = table.Column<decimal>(type: "numeric", nullable: true),
                    checked_by_id = table.Column<Guid>(type: "uuid", nullable: true),
                    checked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_transcripts", x => x.id);
                    table.ForeignKey(
                        name: "FK_transcripts_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "song_tags",
                columns: table => new
                {
                    song_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tag_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_song_tags", x => new { x.song_id, x.tag_id });
                    table.ForeignKey(
                        name: "FK_song_tags_songs_song_id",
                        column: x => x.song_id,
                        principalTable: "songs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_song_tags_tags_tag_id",
                        column: x => x.tag_id,
                        principalTable: "tags",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_song_authors_author_id",
                table: "song_authors",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_song_tags_tag_id",
                table: "song_tags",
                column: "tag_id");

            migrationBuilder.CreateIndex(
                name: "ix_songs_title",
                table: "songs",
                column: "title");

            migrationBuilder.CreateIndex(
                name: "ux_tags_name",
                table: "tags",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_transcripts_song_segment",
                table: "transcripts",
                columns: new[] { "song_id", "segment_index" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "song_authors");

            migrationBuilder.DropTable(
                name: "song_tags");

            migrationBuilder.DropTable(
                name: "transcripts");

            migrationBuilder.DropTable(
                name: "authors");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DropTable(
                name: "songs");
        }
    }
}
