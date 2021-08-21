using JwtSample;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite("Data Source=MyDB.db"));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapPost("/Login", async (UserViewModel userViewModel, UserManager<ApplicationUser> userManager) =>
{
    var user = await userManager.FindByEmailAsync(userViewModel.Email);
    if (user == null)
        return Results.Unauthorized();

    var isValid = await userManager.CheckPasswordAsync(user, userViewModel.Password);
    if (isValid)
        return Results.Ok(GenerateToken(user));

    return Results.Unauthorized();
});

app.MapPost("/Register", async (UserViewModel userViewModel, UserManager<ApplicationUser> userManager) =>
{
    var user = new ApplicationUser() { UserName = userViewModel.Email, Email = userViewModel.Email };
    var result = await userManager.CreateAsync(user, userViewModel.Password);
    if (result.Succeeded)
        return Results.Ok(GenerateToken(user));

    return Results.BadRequest(result.Errors.Select(x => x.Description));
});

app.MapGet("/", () => "Hello World!");

app.MapGet("/Secured", () => "Secured Page").RequireAuthorization();

app.Run();

string GenerateToken(ApplicationUser user)
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var tokenDescriptor = new SecurityTokenDescriptor()
    {
        Subject = new ClaimsIdentity(new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        }),
        Expires = DateTime.Now.AddHours(1),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(app.Configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256)
    };
    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}