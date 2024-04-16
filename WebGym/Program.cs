using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gym.Model;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebGym.Options;
using System.Data;


var builder = WebApplication.CreateBuilder();

//Connect Database
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DBContext>(options => options.UseNpgsql(connection));




//Add Authorization and Authentication
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireClaim(ClaimTypes.Role, "admin"));

});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true
        };
    });
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();






// APP

// LOG IN
app.MapPost("/login", async (User loginData, DBContext db) =>
{
  
    // находим пользователя 
    User? person = await db.Users.FirstOrDefaultAsync(p => p.Email == loginData.Email && p.Password == loginData.Password);
    // если пользователь не найден, отправляем статусный код 401
    if (person is null) return Results.Unauthorized();
    if (person.IsBanned) return Results.Problem("This user is banned", statusCode: 403); //forbidden


    //set claims
    var claims = new List<Claim>()
    {
        new Claim(ClaimTypes.NameIdentifier, person.Id.ToString()),
        new Claim(ClaimTypes.Email, person.Email)
    };

    if (person.Email == "admin")
    {
        claims.Add(new Claim(ClaimTypes.Role, "admin"));
    }
    else
    {
        claims.Add(new Claim(ClaimTypes.Role, "user"));
    
    }
    // создаем JWT-токен
    var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.Now.AddMinutes(1), //DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)) ; 
    var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

    return Results.Json(encodedJwt); // return token
});



// DELETE USER BY ID
app.MapDelete("/users/{id:int}", [Authorize(Policy = "RequireAdminRole")]
async (int id, DBContext db) =>
{
    // Get the user by id
    User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

    if (user == null) return Results.NotFound(new { message = String.Format( "No such user {0}", id) });

    db.Users.Remove(user);
    await db.SaveChangesAsync();
    //return Results.Json(user);
    return Results.NoContent(); // return 204
});


// ADD USER 
app.MapPost("/users", [Authorize(Policy = "RequireAdminRole")]
async (User user, DBContext db) =>
{
    await db.Users.AddAsync(user);
    await db.SaveChangesAsync();
    // return Results.Json(user);
    return Results.Created($"/api/users/{user.Id}", user); // return 201
});


//BAN USER BY ID
app.MapPut("/users/ban/{id:int}", [Authorize(Policy = "RequireAdminRole")]
async (int id, DBContext db) =>
{
   
    var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    // если пользователь найден, изменяем его данные и отправляем обратно клиенту
    user.IsBanned = !user.IsBanned;
    await db.SaveChangesAsync();
    return Results.Ok(user);
});


// GET ALL USERS
app.MapGet("/users", [Authorize(Policy = "RequireAdminRole")]
async (DBContext db) =>
{
    var users = await db.Users.Where(u => u.Email != "admin").ToListAsync();
    return Results.Ok(users);
});

app.MapGet("/users/banned", [Authorize(Policy = "RequireAdminRole")]
async (DBContext db) =>
{
    var users = await db.Users.Where(u => u.IsBanned).ToListAsync();
    return Results.Ok(users);
});


app.MapGet("/users/unbanned", [Authorize(Policy = "RequireAdminRole")]
    async (DBContext db) =>
    {
    var users = await db.Users.Where(u => !u.IsBanned && u.Email != "admin").ToListAsync();
    return Results.Ok(users);
});



// WELCOME PAGE
app.Map("/", (HttpContext context) =>
{
    var user = context.User;

    if (user is not null && user.Identity.IsAuthenticated)
    {
        var userEmail = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        return Results.Json(new { message = $"Welcome, {userEmail}!" });
    }
    else
    {
        return Results.Json(new { message = "User is NOT authenticated" });
    }
});

app.Run();











//var builder = WebApplication.CreateBuilder();
//string connection = builder.Configuration.GetConnectionString("DefaultConnection");
//// добавляем контекст ApplicationContext в качестве сервиса в приложение
//builder.Services.AddDbContext<DBContext>(options => options.UseNpgsql(connection));
//var app = builder.Build();

//app.MapGet("/", (DBContext db) => db.Users.ToList());

//app.MapGet("/api/users/{id:int}", async (int id, DBContext db) =>
//{
//    // получаем пользователя по id
//    User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

//    // если не найден, отправляем статусный код и сообщение об ошибке
//    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

//    // если пользователь найден, отправляем его
//    return Results.Json(user);
//});

//app.MapDelete("/api/users/{id:int}", async (int id, DBContext db) =>
//{
//    // получаем пользователя по id
//    User? user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);

//    // если не найден, отправляем статусный код и сообщение об ошибке
//    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

//    // если пользователь найден, удаляем его
//    db.Users.Remove(user);
//    await db.SaveChangesAsync();
//    return Results.Json(user);
//});

//app.MapPost("/api/users", async (User user, DBContext db) =>
//{
//    // добавляем пользователя в массив
//    await db.Users.AddAsync(user);
//    await db.SaveChangesAsync();
//    return user;
//});

//app.MapPut("/api/users", async (User userData, DBContext db) =>
//{
//    // получаем пользователя по id
//    var user = await db.Users.FirstOrDefaultAsync(u => u.Id == userData.Id);

//    // если не найден, отправляем статусный код и сообщение об ошибке
//    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

//    // если пользователь найден, изменяем его данные и отправляем обратно клиенту
//    user.IsBanned = userData.IsBanned;


//    await db.SaveChangesAsync();
//    return Results.Json(user);
//});

//app.Run();






























//app.Run(async (context) =>
//{
//    var response = context.Response;
//    var request = context.Request;
//    var path = request.Path;

//    string expressionForGuid = @"^/api/users/\w{8}-\w{4}-\w{4}-\w{4}-\w{12}$";
//    if (path == "/api/users" && request.Method == "GET")
//    {
//        await GetAllPeople(response);
//    }
//    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "GET")
//    {
//        // получаем id из адреса url
//        string? id = path.Value?.Split("/")[3];
//        await GetPerson(id, response);
//    }
//    else if (path == "/api/users" && request.Method == "POST")
//    {
//        await CreatePerson(response, request);
//    }
//    else if (path == "/api/users" && request.Method == "PUT")
//    {
//        await UpdatePerson(response, request);
//    }
//    else if (Regex.IsMatch(path, expressionForGuid) && request.Method == "DELETE")
//    {
//        string? id = path.Value?.Split("/")[3];
//        await DeletePerson(id, response);
//    }
//    else
//    {
//      //  response.ContentType = "text/html; charset=utf-8";
//       /// await response.SendFileAsync("html/index.html");
//        await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
//        // await response.WriteAsJsonAsync(new { message = "GGGGGGGG" });
//    }
//});

//app.Run();



//// получение всех пользователей
//async Task GetAllPeople(HttpResponse response)
//{
//    await response.WriteAsJsonAsync(users);
//}
//// получение одного пользователя по id
//async Task GetPerson(string? id, HttpResponse response)
//{
//    // получаем пользователя по id
//    User? user = users.FirstOrDefault((u) => u.Id == id);
//    // если пользователь найден, отправляем его
//    if (user != null)
//        await response.WriteAsJsonAsync(user);
//    // если не найден, отправляем статусный код и сообщение об ошибке
//    else
//    {
//        response.StatusCode = 404;
//        await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
//    }
//}

//async Task DeletePerson(string? id, HttpResponse response)
//{
//    // получаем пользователя по id
//    User? user = users.FirstOrDefault((u) => u.Id == id);
//    // если пользователь найден, удаляем его
//    if (user != null)
//    {
//        users.Remove(user);
//        await response.WriteAsJsonAsync(user);
//    }
//    // если не найден, отправляем статусный код и сообщение об ошибке
//    else
//    {
//        response.StatusCode = 404;
//        await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
//    }
//}

//async Task CreatePerson(HttpResponse response, HttpRequest request)
//{
//    try
//    {
//        // получаем данные пользователя
//        var user = await request.ReadFromJsonAsync<User>();
//        if (user != null)
//        {
//            // устанавливаем id для нового пользователя
//            user.Id = Guid.NewGuid().ToString();
//            // добавляем пользователя в список
//            users.Add(user);
//            await response.WriteAsJsonAsync(user);
//        }
//        else
//        {
//            throw new Exception("Некорректные данные");
//        }
//    }
//    catch (Exception)
//    {
//        response.StatusCode = 400;
//        await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
//    }
//}

//async Task UpdatePerson(HttpResponse response, HttpRequest request)
//{
//    try
//    {
//        // получаем данные пользователя
//        User? userData = await request.ReadFromJsonAsync<User>();
//        if (userData != null)
//        {
//            // получаем пользователя по id
//            var user = users.FirstOrDefault(u => u.Id == userData.Id);
//            // если пользователь найден, изменяем его данные и отправляем обратно клиенту
//            if (user != null)
//            {
//                user.Age = userData.Age;
//                user.Name = userData.Name;
//                await response.WriteAsJsonAsync(user);
//            }
//            else
//            {
//                response.StatusCode = 404;
//                await response.WriteAsJsonAsync(new { message = "Пользователь не найден" });
//            }
//        }
//        else
//        {
//            throw new Exception("Некорректные данные");
//        }
//    }
//    catch (Exception)
//    {
//        response.StatusCode = 400;
//        await response.WriteAsJsonAsync(new { message = "Некорректные данные" });
//    }
//}