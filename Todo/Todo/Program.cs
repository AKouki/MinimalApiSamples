using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Todo;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlite("Data Source=MyTodos.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Todo API", Description = "Todo API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1"));
}

app.MapGet("/Todos", async (TodoDbContext db) => await db.Todos.ToListAsync());

app.MapGet("/Todos/{id}", async (int id, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo == null)
        return Results.NotFound();

    return Results.Ok(todo);
});

app.MapGet("/Todos/Completed", async (TodoDbContext db) => await db.Todos.Where(x => x.IsCompleted == true).ToListAsync());

app.MapGet("/Todos/Uncompleted", async (TodoDbContext db) => await db.Todos.Where(x => x.IsCompleted == false).ToListAsync());

app.MapPost("/Todos", async (TodoItem todo, TodoDbContext db) =>
{
    var newTodo = new TodoItem()
    {
        Title = todo.Title,
        Description = todo.Description,
        IsCompleted = todo.IsCompleted
    };

    db.Todos.Add(newTodo);
    await db.SaveChangesAsync();

    return Results.Ok(newTodo);
});

app.MapPut("/Todos/{id}", async (int id, TodoItem todo, TodoDbContext db) =>
{
    var todoToEdit = await db.Todos.FindAsync(id);
    if (todoToEdit == null)
        return Results.NotFound();

    todoToEdit.Title = todo.Title;
    todoToEdit.Description = todo.Description;
    todoToEdit.IsCompleted = todo.IsCompleted;

    db.Todos.Update(todoToEdit);
    await db.SaveChangesAsync();

    return Results.Ok(todoToEdit);
});

app.MapPut("/Todos/{id}/MarkCompleted/{value}", async (int id, bool value, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo == null)
        return Results.NotFound();

    todo.IsCompleted = value;

    db.Todos.Update(todo);
    await db.SaveChangesAsync();

    return Results.Ok(todo);
});

app.MapDelete("/Todos/{id}", async (int id, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo == null)
        return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.Run();
