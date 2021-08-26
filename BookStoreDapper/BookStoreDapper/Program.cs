
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped(_ => new SqliteConnection("Data Source=Books.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "BookStore API", Description = "BookStore API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1"));
}

await CreateTablesAsync(app.Services);

app.MapGet("/Books", async (SqliteConnection connection) =>
{
    var query = "SELECT * FROM Books";
    var books = await connection.QueryAsync<Book>(query);
    return Results.Ok(books);
});

app.MapGet("/Books/{id}", async (int id, SqliteConnection connection) =>
{
    var query = "SELECT * FROM Books WHERE Id = @id";
    var book = await connection.QuerySingleOrDefaultAsync<Book>(query, new { id });
    if (book == null)
        return Results.NotFound();

    return Results.Ok(book);
});

app.MapPost("/Books", async (Book book, SqliteConnection connection) =>
{
    var query = "INSERT INTO Books(Title, Description, Publisher, Language, ISBN, Authors, Subjects) " +
    "VALUES(@Title, @Description, @Publisher, @Language, @ISBN, @Authors, @Subjects) RETURNING *";

    var newBook = await connection.QuerySingleAsync<Book>(query, book);
    return Results.Ok(newBook);
});

app.MapPut("/Books/{id}", async (int id, Book book, SqliteConnection connection) =>
{
    var selectQuery = "SELECT * FROM Books WHERE Id = @id";
    var existingBook = await connection.QuerySingleOrDefaultAsync<Book>(selectQuery, new { id });
    if (existingBook == null)
        return Results.NotFound();

    var updateQuery = "UPDATE Books SET Title = @Title, Description = @Description, Publisher = @Publisher," +
    "Language = @Language, ISBN = @ISBN, Authors = @Authors, Subjects = @Subjects WHERE Id=@Id RETURNING *";

    // We don't want to change the id (in case the user has submited the id from body)
    book.Id = id;

    var updatedBook = await connection.QuerySingleOrDefaultAsync<Book>(updateQuery, book);
    return Results.Ok(updatedBook);
});

app.MapDelete("/Books/{id}", async (int id, SqliteConnection connection) =>
{
    var query = "DELETE FROM Books WHERE Id = @id";
    var rowsAffected = await connection.ExecuteAsync(query, new { id });
    if (rowsAffected == 0)
        return Results.NotFound();

    return Results.Ok();
});

app.Run();

async Task CreateTablesAsync(IServiceProvider services)
{
    var query = "CREATE TABLE IF NOT EXISTS Books (" +
            "Id INTEGER PRIMARY KEY AUTOINCREMENT," +
            "Title TEXT NOT NULL," +
            "Description TEXT NOT NULL," +
            "Publisher TEXT NOT NULL," +
            "Language TEXT NOT NULL," +
            "ISBN TEXT NOT NULL," +
            "Authors TEXT NOT NULL," +
            "Subjects TEXT NOT NULL" +
            ");";

    using var db = services.CreateScope().ServiceProvider.GetRequiredService<SqliteConnection>();
    await db.ExecuteAsync(query);
}

class Book
{
    public int Id { get; set; }
    public string Title {  get; set; }
    public string Description { get; set; }
    public string Publisher { get; set; }
    public string Language { get; set; }
    public string ISBN { get; set; }
    public string Authors { get; set; }
    public string Subjects { get; set; }
}
