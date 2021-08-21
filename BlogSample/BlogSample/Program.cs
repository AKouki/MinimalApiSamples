using BlogSample;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BlogDbContext>(options => options.UseSqlite("Data Source=MyBlog.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo() { Title = "Blog API", Description = "Blog API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog API v1"));
}

app.MapGet("/Blogs", async (BlogDbContext db) => await db.Blogs.ToListAsync());

app.MapGet("/Blogs/{name}", async (string name, BlogDbContext db) =>
{
    var blog = await db.Blogs.SingleOrDefaultAsync(b => b.Name == name);
    if (blog == null)
        return Results.NotFound();

    return Results.Ok(blog);
});

app.MapPost("/Blogs", async (Blog blog, BlogDbContext db) =>
{
    var existingBlog = await db.Blogs.FirstOrDefaultAsync(b => b.Name == blog.Name);
    if (existingBlog != null)
        return Results.BadRequest(new { Error = "Another blog with the same name already exists" });

    var newBlog = new Blog()
    {
        Name = blog.Name
    };

    db.Blogs.Add(newBlog);
    await db.SaveChangesAsync();

    return Results.Ok(newBlog);
});

app.MapPut("/Blogs/{name}", async (string name, Blog blog, BlogDbContext db) =>
{
    var blogToEdit = await db.Blogs.FirstOrDefaultAsync(b => b.Name == name);
    if (blogToEdit == null)
        return Results.NotFound();

    blogToEdit.Name = blog.Name;

    db.Blogs.Update(blogToEdit);
    await db.SaveChangesAsync();

    return Results.Ok(blogToEdit);
});

app.MapDelete("/Blogs/{name}", async (string name, BlogDbContext db) =>
{
    var blog = await db.Blogs.FirstOrDefaultAsync(b => b.Name == name);
    if (blog == null)
        return Results.NotFound();

    db.Blogs.Remove(blog);
    await db.SaveChangesAsync();

    return Results.Ok();
});

app.MapGet("/Blogs/{blogName}/Posts/{slug}", async (string blogName, string slug, BlogDbContext db) =>
{
    var blog = await db.Blogs.FirstOrDefaultAsync(b => b.Name == blogName);
    if (blog == null)
        return Results.NotFound();

    var post = await db.Posts.FirstOrDefaultAsync(p => p.Slug == slug && p.BlogId == blog.Id);
    if (post == null)
        return Results.NotFound();

    return Results.Ok(post);
});

app.MapGet("/Blogs/{blogName}/Posts", async (string blogName, BlogDbContext db) =>
{
    var blog = await db.Blogs.FirstOrDefaultAsync(b => b.Name == blogName);
    if (blog == null)
        return Results.NotFound();

    var posts = await db.Posts.Where(p => p.BlogId == blog.Id).ToListAsync();
    return Results.Ok(posts);
});

app.MapGet("/Blogs/{blogName}/Category/{category}", async (string blogName, string category, BlogDbContext db) =>
{
    var blog = await db.Blogs.FirstOrDefaultAsync(b => b.Name == blogName);
    if (blog == null)
        return Results.NotFound();

    var posts = await db.Posts.Where(p => p.Category == category && p.BlogId == blog.Id).ToListAsync();
    return Results.Ok(posts);
});

app.MapGet("/Blogs/{blogName}/Author/{authorName}", async (string blogName, string authorName, BlogDbContext db) =>
{
    var blog = await db.Blogs.FirstOrDefaultAsync(b => b.Name == blogName);
    if (blog == null)
        return Results.NotFound();

    var posts = await db.Posts.Where(p => p.AuthorName == authorName && p.BlogId == blog.Id).ToListAsync();
    return Results.Ok(posts);
});

app.MapPost("/Blogs/{blogName}/Posts", async (string blogName, Post post, BlogDbContext db) =>
{
    var blog = await db.Blogs.FirstOrDefaultAsync(b => b.Name == blogName);
    if (blog == null)
        return Results.NotFound();

    bool slugExists = await db.Posts.AnyAsync(p => p.BlogId == blog.Id && p.Slug == post.Slug);
    if (slugExists)
        return Results.BadRequest(new { Error = $"This blog already contains another post with slug: {post.Slug}" });

    var newPost = new Post()
    {
        Title = post.Title,
        Content = post.Content,
        ThumbnailUrl = post.ThumbnailUrl,
        AuthorName = post.AuthorName,
        CreatedAt = DateTime.Now,
        Category = post.Category,
        Slug = post.Slug,
        ReadTime = post.ReadTime,
        BlogId = blog.Id
    };

    db.Posts.Add(newPost);
    await db.SaveChangesAsync();

    return Results.Ok(newPost);
});

app.MapPut("/Blogs/{blogName}/Posts/{slug}", async (string blogName, string slug, Post post, BlogDbContext db) =>
{
    var blog = await db.Blogs.FirstOrDefaultAsync(b => b.Name == blogName);
    if (blog == null)
        return Results.NotFound();

    var postToEdit = await db.Posts.FirstOrDefaultAsync(p => p.BlogId == blog.Id && p.Slug == slug);
    if (postToEdit == null)
        return Results.NotFound();

    bool slugExists = await db.Posts.AnyAsync(p => p.BlogId == blog.Id && p.Slug == post.Slug);
    if (slugExists)
        return Results.BadRequest(new { Error = $"This blog already contains another post with slug: {post.Slug}" });

    postToEdit.Title = post.Title;
    postToEdit.Content = post.Content;
    postToEdit.ThumbnailUrl = post.ThumbnailUrl;
    postToEdit.AuthorName = post.AuthorName;
    postToEdit.UpdatedAt = DateTime.Now;
    postToEdit.Category = post.Category;
    postToEdit.Slug = post.Slug;
    postToEdit.ReadTime = post.ReadTime;

    db.Posts.Update(postToEdit);
    await db.SaveChangesAsync();

    return Results.Ok(postToEdit);
});

app.MapDelete("/Blogs/{blogName}/Posts/{slug}", async (string blogName, string slug, BlogDbContext db) =>
{
    var blog = await db.Blogs.FirstOrDefaultAsync(b => b.Name == blogName);
    if (blog == null)
        return Results.NotFound();

    var post = await db.Posts.FirstOrDefaultAsync(p => p.BlogId == blog.Id && p.Slug == slug);
    if (post == null)
        return Results.NotFound();

    db.Posts.Remove(post);
    await db.SaveChangesAsync();

    return Results.Ok();
});


app.Run();
