using System.Globalization;
using Dapper;
using Microsoft.Data.Sqlite;
using Parcial1_P4_Yerferson.Models;

namespace Parcial1_P4_Yerferson.Services;

public class AutoresService(IConfiguration configuration)
{
    private readonly string _connectionString =
        configuration.GetConnectionString("AutoresConnection")
        ?? throw new InvalidOperationException(
            "Falta la conexion AutoresConnection.");

    private SqliteConnection CreateConnection() => new(_connectionString);

    public async Task InitializeAsync()
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS Autores (
                IdAutor INTEGER PRIMARY KEY AUTOINCREMENT,
                Nombres TEXT NOT NULL,
                Nacionalidad TEXT NOT NULL,
                FechaNacimiento TEXT NOT NULL,
                SueldoCentavos INTEGER NOT NULL
            );
            """;

        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql);
    }

    private static object Parameters(Autor autor) => new
    {
        autor.IdAutor,
        autor.Nombres,
        autor.Nacionalidad,
        FechaNacimiento = autor.FechaNacimiento.ToString(
            "yyyy-MM-dd", CultureInfo.InvariantCulture),
        SueldoCentavos = checked((long)(autor.Sueldo * 100m))
    };

    public async Task<int> SaveAsync(Autor autor)
    {
        const string sql = """
            INSERT INTO Autores
                (Nombres, Nacionalidad, FechaNacimiento, SueldoCentavos)
            VALUES
                (@Nombres, @Nacionalidad, @FechaNacimiento, @SueldoCentavos);
            SELECT last_insert_rowid();
            """;

        using var connection = CreateConnection();

        return await connection.ExecuteScalarAsync<int>(
            sql, Parameters(autor));
    }

    public async Task<bool> UpdateAsync(Autor autor)
    {
        const string sql = """
            UPDATE Autores
            SET Nombres = @Nombres,
                Nacionalidad = @Nacionalidad,
                FechaNacimiento = @FechaNacimiento,
                SueldoCentavos = @SueldoCentavos
            WHERE IdAutor = @IdAutor;
            """;

        using var connection = CreateConnection();

        return await connection.ExecuteAsync(
            sql, Parameters(autor)) > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql =
            "DELETE FROM Autores WHERE IdAutor = @IdAutor;";

        using var connection = CreateConnection();

        return await connection.ExecuteAsync(
            sql, new { IdAutor = id }) > 0;
    }

    public async Task<Autor?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT IdAutor, Nombres, Nacionalidad,
                   FechaNacimiento, SueldoCentavos
            FROM Autores
            WHERE IdAutor = @IdAutor;
            """;

        using var connection = CreateConnection();

        var row = await connection.QuerySingleOrDefaultAsync<AutorRow>(
            sql, new { IdAutor = id });

        return row is null ? null : ToAutor(row);
    }

    public async Task<IEnumerable<Autor>> GetListAsync()
    {
        const string sql = """
            SELECT IdAutor, Nombres, Nacionalidad,
                   FechaNacimiento, SueldoCentavos
            FROM Autores
            ORDER BY IdAutor;
            """;

        using var connection = CreateConnection();

        var rows = await connection.QueryAsync<AutorRow>(sql);

        return rows.Select(ToAutor).ToList();
    }

    private static Autor ToAutor(AutorRow row) => new(
        checked((int)row.IdAutor),
        row.Nombres,
        row.Nacionalidad,
        DateTime.ParseExact(
            row.FechaNacimiento,
            "yyyy-MM-dd",
            CultureInfo.InvariantCulture),
        row.SueldoCentavos / 100m);

    private sealed class AutorRow()
    {
        public long IdAutor { get; set; }
        public string Nombres { get; set; } = "";
        public string Nacionalidad { get; set; } = "";
        public string FechaNacimiento { get; set; } = "";
        public long SueldoCentavos { get; set; }
    }
}