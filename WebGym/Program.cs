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
using System.Text.Json.Serialization;
using WebGym.Services;
using System.Net;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Net.Http.Headers;


var builder = WebApplication.CreateBuilder();

//Connect Database
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DBContext>(options => options.UseNpgsql(connection, options => options.MigrationsAssembly("WebGym")));
builder.Services.AddHostedService<CleanupService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddTransient<EmailService>();

//Add Authorization and Authentication
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireClaim(ClaimTypes.Role, "admin"));

});

builder.Configuration.AddUserSecrets<Program>();
var authOptions = new AuthOptions();
builder.Configuration.GetSection("AuthOptions").Bind(authOptions);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidIssuer = authOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = authOptions.Audience,
            ValidateLifetime = true,
            IssuerSigningKey = authOptions.GetSymmetricSecurityKey(),
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
app.MapPost("/login", async (User loginData, DBContext db, IPasswordHasher<User> passwordHasher) =>
{
    // Find the user
    User? person = await db.Users.FirstOrDefaultAsync(p => p.Email == loginData.Email);
    // If the user is not found, return 401 Unauthorized
    if (person is null) return Results.Unauthorized();
    if (person.IsBanned) return Results.Problem("This user is banned", statusCode: 403); //forbidden

    var verificationResult = passwordHasher.VerifyHashedPassword(person, person.Password, loginData.Password);
    if (verificationResult == PasswordVerificationResult.Failed)
    {
        return Results.Problem("Wrong password", statusCode: 401);
    }


    // Set claims
    var claims = new List<Claim>()
    {
        new Claim(ClaimTypes.NameIdentifier, person.Id.ToString()),
        new Claim(ClaimTypes.Email, person.Email),
        new Claim(ClaimTypes.Name, person.Name),
        new Claim(ClaimTypes.MobilePhone, person.PhoneNumber),
        new Claim("IsBanned", person.IsBanned.ToString()),
    };

    if (person.Email == "admin")
    {
        claims.Add(new Claim(ClaimTypes.Role, "admin"));
    }
    else
    {
        claims.Add(new Claim(ClaimTypes.Role, "user"));
    }

    // Create JWT-token
    var jwt = new JwtSecurityToken(
            issuer: authOptions.Issuer,
            audience: authOptions.Audience,
            claims: claims,
            expires: DateTime.Now.AddHours(2), //DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
            signingCredentials: new SigningCredentials(authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
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
app.MapPost("/users",
async (User user, DBContext db, IPasswordHasher<User> passwordHasher) =>
{
    user.Password = passwordHasher.HashPassword(user, user.Password);
    await db.Users.AddAsync(user);
    await db.SaveChangesAsync();
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

// BUY MEMBERSHIP
app.MapPut("/memberships/buy", 
    async (User user, DBContext dbContext) =>
{
    var userInDb = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
    if (userInDb == null) return Results.NotFound(new { message = "No such user" });
    userInDb.UserMemberships = user.UserMemberships;

    dbContext.Users.Update(userInDb);
    await dbContext.SaveChangesAsync();

   
    return Results.Ok();
});

//PAYMENT
app.MapPost("/users/payment", 
       async (int membershipId, DBContext dbContext) =>
       {
        // GET MEMBERSHIP
        var membership = await dbContext.Memberships.FirstOrDefaultAsync(m => m.Id == membershipId);
           // CREATE REQUEST
               var client = new HttpClient();

               var byteArray = Encoding.ASCII.GetBytes("385708:test_QjpJqSgi_4o7cPSsSDe667iS9sUBdafzKvBLjmGmdvU");
               client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                var content = new StringContent(
                    $@"{{
                    ""amount"": {{
                    ""value"": {membership.Price},
                    ""currency"": ""RUB""
                    }},
                    ""capture"": true,
                    ""confirmation"": {{
                    ""type"": ""redirect"",
                        ""return_url"": ""https://support.google.com/mail/answer/6304825?hl=en&co=GENIE.Platform%3DDesktop""
                    }}
                    }}", Encoding.UTF8);

               content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

               var request = new HttpRequestMessage
               {
                   Method = HttpMethod.Post,
                   RequestUri = new Uri("https://api.yookassa.ru/v3/payments"),
                   Headers =
            {
                { "Idempotence-Key", DateTime.Now.ToString() },
            },
                   Content = content
               };
           // SEND REQUEST
           try { 
               var response = await client.SendAsync(request);
               string content_link = await response.Content.ReadAsStringAsync();
               JsonDocument doc = JsonDocument.Parse(content_link);
               string url = doc.RootElement.GetProperty("confirmation").GetProperty("confirmation_url").GetString();


               if (response.IsSuccessStatusCode)
               {
                   return Results.Ok(new { confirmation_url = url });
               }
               else
               {
                   //Console.WriteLine(request.ToString());
                   //Console.WriteLine("Error: " + response.StatusCode);
                   return Results.Problem("Error occurred while making the payment.");
               }

           }

           catch (Exception ex)
           {
               //Console.WriteLine();
               //Console.WriteLine("PROBLEM!!!");
               //Console.WriteLine(ex.Message);
               return Results.Problem(ex.Message);
           }
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


//IS USER EXIST
app.MapGet("/users/exist/{email}",
    async (string email, DBContext db) =>
{
    var isExist = await db.Users.AnyAsync(u => u.Email == email);
    return Results.Ok(isExist);
});

//IS USER A MEMBER
app.MapGet("/users/member/{email}",
       async (string email, DBContext db) =>
       {
        var isMember = await db.Members.AnyAsync(u => u.Email == email);
        return Results.Ok(isMember);
    });


//GET ALL MEMBERSHIPS

app.MapGet("/memberships",
    async (DBContext db) =>
    {
        var memberships = await db.Memberships
        .Include(m => m.Freeze)
        .ToListAsync();
        return Results.Ok(memberships);
    });

//GET USER ACTIVE MEMBERSHIPS
app.MapGet("/memberships/active/{id:int}",
       async (int id, DBContext db) =>
       {
           var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
           var activeMemberships = await db.MembershipInstances
        .Include(m => m.Membership)
        .Where(m => m.UserId == id && m.Status == Status.Active)
        .ToListAsync();
        return Results.Ok(activeMemberships);
    });

//GET USER INACTIVE MEMBERSHIPS
app.MapGet("/memberships/notactive/{id:int}",
          async (int id, DBContext db) =>
          {
        var notActiveMemberships = await db.MembershipInstances
        .Include(m => m.Membership)
        .Where(m => m.UserId == id && m.Status == Status.Inactive)
        .ToListAsync();
        return Results.Ok(notActiveMemberships);
    });

// GET USER FROZEN MEMBERSHIPS
app.MapGet("/memberships/frozen/{id:int}",
             async (int id, DBContext db) =>
             {
        var frozenMemberships = await db.MembershipInstances
        .Include(m => m.Membership)
        .Include(m => m.ActiveFreeze)
        .Where(m => m.UserId == id && m.Status == Status.Frozen)
        .ToListAsync();
        return Results.Ok(frozenMemberships);
    });

//GET ALL FREEZES
app.MapGet("/freezes",
       async (DBContext db) =>
       {
        var freezes = await db.Freezes.ToListAsync();
        return Results.Ok(freezes);
    });

//DELETE MEMBERSHIP BY ID
app.MapDelete("/memberships/{id:int}", [Authorize(Policy = "RequireAdminRole")]
async (int id, DBContext db) =>
       {
        var membership = await db.Memberships.FirstOrDefaultAsync(m => m.Id == id);
        if (membership == null) return Results.NotFound(new { message = "No such membership" });

        db.Memberships.Remove(membership);
        await db.SaveChangesAsync();
        return Results.NoContent();
    });

//DELETE FREEZE BY ID
app.MapDelete("/freezes/{id:int}", [Authorize(Policy = "RequireAdminRole")]
async (int id, DBContext db) =>
{
    var freeze = await db.Freezes.FirstOrDefaultAsync(m => m.Id == id);
    if (freeze == null) return Results.NotFound(new { message = "No such freeze" });

    db.Freezes.Remove(freeze);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

//GET MEMBERSHIP BY ID
app.MapGet("/memberships/{id:int}",
       async (int id, DBContext db) =>
       {
           var membership = await db.Memberships
               .Include(m => m.Freeze) // Include the Freeze related to the Membership
               .FirstOrDefaultAsync(m => m.Id == id);
           if (membership == null) return Results.NotFound(new { message = "No such membership" });

        return Results.Ok(membership);
    });

//GET FREEZE BY ID
app.MapGet("/freezes/{id:int}",
       async (int id, DBContext db) =>
       {
        var freeze = await db.Freezes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (freeze == null) return Results.NotFound(new { message = "No such freeze" });

        return Results.Ok(freeze);
    });

//GET MEMBERSHIP INSTANCE BY ID

app.MapGet("/membershipinstances/{id:int}",
    async (int id, DBContext db) =>
    {
        var membershipInstance = await db.MembershipInstances
            .Include(mi => mi.Membership)
            .ThenInclude(m => m.Freeze)
            .Include(mi => mi.ActiveFreeze)
            .Include(mi => mi.User) // Include the User related to the MembershipInstance
            .FirstOrDefaultAsync(mi => mi.Id == id);
        if (membershipInstance == null) return Results.NotFound(new { message = "No such membership instance" });

        return Results.Ok(membershipInstance);
    });

//DOES MEMBERSHIP EXIST
app.MapPost("/memberships/exist", [Authorize(Policy = "RequireAdminRole")]
async (Membership membership, DBContext db) =>
          {
        var isExist = await db.Memberships.AnyAsync(m => m.Name == membership.Name && m.Price == membership.Price && m.Months == membership.Months && m.FreezeId == membership.FreezeId);
        return Results.Ok(isExist);
    });

//DOES FREEZE EXIST
app.MapPost("/freezes/exist", [Authorize(Policy = "RequireAdminRole")]
async (Freeze freeze, DBContext db) =>
{
    var isExist = await db.Freezes.AnyAsync(m => m.Name == freeze.Name && m.Price == freeze.Price && m.Days == freeze.Days);
    return Results.Ok(isExist);
});


//ADD MEMBERSHIP
app.MapPost("/memberships", [Authorize(Policy = "RequireAdminRole")]
async (Membership membership, DBContext db) =>
       {
        await db.Memberships.AddAsync(membership);
        await db.SaveChangesAsync();
        return Results.Created($"/api/memberships/{membership.Id}", membership);
    });

//ADD FREEZE
app.MapPost("/freezes", [Authorize(Policy = "RequireAdminRole")]
async (Freeze freeze, DBContext db) =>
{
    await db.Freezes.AddAsync(freeze);
    await db.SaveChangesAsync();
    return Results.Created($"/api/memberships/{freeze.Id}", freeze);
});

//EDIT MEMBERSHIP
app.MapPut("/memberships", [Authorize(Policy = "RequireAdminRole")]
async (Membership membership, DBContext db) =>
          {
        var membershipInDb = await db.Memberships.FirstOrDefaultAsync(m => m.Id == membership.Id);
        if (membershipInDb == null) return Results.NotFound(new { message = "No such membership" });

        membershipInDb.Name = membership.Name;
        membershipInDb.Price = membership.Price;
        membershipInDb.Months = membership.Months;
        membershipInDb.FreezeId = membership.FreezeId;
        await db.SaveChangesAsync();
        return Results.Ok(membershipInDb);
    });

// EDTI FREEZE
app.MapPut("/freezes", [Authorize(Policy = "RequireAdminRole")]
async (Freeze freeze, DBContext db) =>
{
    var freezeInDb = await db.Freezes.FirstOrDefaultAsync(m => m.Id == freeze.Id);
    if (freezeInDb == null) return Results.NotFound(new { message = "No such membership" });

    freezeInDb.Name = freeze.Name;
    freezeInDb.Price = freeze.Price;
    freezeInDb.Days = freeze.Days;
    freezeInDb.MinDays = freeze.MinDays;
    await db.SaveChangesAsync();
    return Results.Ok(freezeInDb);
});

// DOES ACTIVE MEMBERSHIP EXIST BY USER ID
app.MapGet("/user/memberships/exist/{id:int}",
          async (int id, DBContext db) =>
          {
           var isExist = await db.MembershipInstances.AnyAsync(m => m.UserId == id && m.Status == Status.Active);
           return Results.Ok(isExist);
       });


//ACTIVATE MEMBERSHIP BY ID
app.MapPut("/membershipinstances/activate/{id:int}",
       async (int id, DBContext db) =>
       {
           
           var membershipInstance = await db.MembershipInstances
       .Include(mi => mi.Membership)
       .ThenInclude(m => m.Freeze) // Include the Freeze related to the Membership
       .Include(mi => mi.User) // Include the User related to the MembershipInstance
       .FirstOrDefaultAsync(mi => mi.Id == id);
           if (membershipInstance == null) return Results.NotFound(new { message = "No such membership instance" });

        membershipInstance.StartDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        membershipInstance.EndDate = membershipInstance.StartDate.Value.AddMonths(membershipInstance.Membership.Months.Value);
           membershipInstance.Status = Status.Active;
           FreezeActive freezeActive = new FreezeActive
           {
               DaysLeft = membershipInstance.Membership.Freeze.Days.Value,
               MembershipInstanceId = membershipInstance.Id,
               //MembershipInstance = membershipInstance
           };
           db.ActiveFreezes.Add(freezeActive);

           await db.SaveChangesAsync();
           return Results.Ok(membershipInstance);
    });

// FREEZE MEMBERSHIP 

app.MapPost("/memberships/freeze", 
async (MembershipInstance updatedMembership, DBContext db) =>
{

    var existingMembership = await db.MembershipInstances
        .Include(m => m.ActiveFreeze) 
        .FirstOrDefaultAsync(m => m.Id == updatedMembership.Id);

    if (existingMembership == null)
    {
        return Results.NotFound(new { message = "No such membership instance" });
    }

    // Update the properties of the existing membership with the properties of the updated membership
    existingMembership.ActiveFreeze.StartDate = updatedMembership.ActiveFreeze.StartDate;
    existingMembership.ActiveFreeze.EndDate = updatedMembership.ActiveFreeze.EndDate;
    existingMembership.ActiveFreeze.DaysLeft = updatedMembership.ActiveFreeze.DaysLeft;
    existingMembership.Status = updatedMembership.Status;
    existingMembership.EndDate = updatedMembership.EndDate;

    // Save the changes to the database
    await db.SaveChangesAsync();

    return Results.Ok(existingMembership);

});

// CANCEL FREEZE
app.MapDelete("/memberships/cancel/freeze/{id:int}", 
    async(int id, DBContext db) =>
{
    var membershipInstance = await db.MembershipInstances
        .Include(m => m.ActiveFreeze)
        .FirstOrDefaultAsync(m => m.Id == id);

    if (membershipInstance == null) return Results.NotFound(new { message = "No such membership instance" });
    int delta = (membershipInstance.ActiveFreeze.EndDate - DateTime.Today.ToUniversalTime()).Value.Days;

    TimeSpan duration = TimeSpan.FromDays(delta);
    membershipInstance.EndDate = membershipInstance.EndDate.Value - duration;
    membershipInstance.ActiveFreeze.DaysLeft += delta;
    membershipInstance.Status = Status.Active;
    db.SaveChanges();

    return Results.Ok(membershipInstance);

});



// RESET PASSWORD

app.MapGet("/users/resetpassword/{token}",
    async (DBContext db, HttpContext context) =>
    {
        var token = WebUtility.UrlDecode(context.Request.RouteValues["token"].ToString());
        
       
        if (string.IsNullOrEmpty(token))
        {
            await context.Response.WriteAsync("Token is missing");
            return;
            //return Results.Problem("Token is missing", statusCode: 400);
        }
        string hashedToken = null;
        try
        { 
          hashedToken = TokenGenerator.HashToken(token);
        }
        catch (FormatException)
        {
            await context.Response.WriteAsync("Invalid token");
            return;
           // return Results.Problem("Invalid token", statusCode: 400);
           
        }
        
        User? user = await db.Users.FirstOrDefaultAsync(u => u.ResetToken == hashedToken);
        if (user == null)
        {
            await context.Response.WriteAsync("Invalid token");
            return;
           // return Results.Problem("Invalid token", statusCode: 400);
        }

        if (user.ResetTokenCreationTime.Value.AddHours(24) < DateTime.UtcNow)
        {
            await context.Response.WriteAsync("Token has expired");
            return;
           // return Results.Problem("Token has expired", statusCode: 400);
        }
        context.Response.ContentType = "text/html";
        await context.Response.SendFileAsync(Path.Combine(app.Environment.ContentRootPath, "html", "password.html"));
       // return Results.Ok();


    });

app.MapPost("/users/resetpassword",
    async (HttpContext context, DBContext db, EmailService emailService) =>
    {
        string email = await new StreamReader(context.Request.Body).ReadToEndAsync();
        email = email.Trim('"');
        Console.WriteLine(email);
        User? user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            // User not found

           return Results.Problem("User not found", statusCode: 404);
        }

        // Generate a random token
        var token = TokenGenerator.GenerateToken();

        // Hash the token and store it in the database
        var hashedToken = TokenGenerator.HashToken(token);
        await TokenGenerator.SaveUserToken(user, hashedToken, db);
      
       // EmailService emailService = new EmailService();

        await emailService.SendEmailAsync(email, "Reset Password",
            $"Hello!<br><br> This message was sent to you since<br><br> you've been trying to reset" +
            $"<br><br>your password in your gym profile.<br><br> " +
            $"Go to this link to reset password: <a href='http://192.168.56.1:5119/users/resetpassword/{WebUtility.UrlEncode(token)}'>CREATE NEW PASSWORD</a>" +
            $"<br><br><br> Or ignore this message if this is a mistake.<br><br>" +
            $"<br><br>With respect, your gym admin!");
        return Results.Ok();

    }

    );




app.MapPost("users/new-password", async (HttpContext context, DBContext db, IPasswordHasher<User> passwordHasher) =>
{
   
    var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();

    var doc = JsonDocument.Parse(requestBody);
    var password = doc.RootElement.GetProperty("password").GetString();
    var token = doc.RootElement.GetProperty("token").GetString();

    token = WebUtility.UrlDecode(token);

    if (string.IsNullOrEmpty(token))
    {
        return Results.Problem("Token is missing", statusCode: 400);
    }
    string hashedToken = null;
    try
    {
        hashedToken = TokenGenerator.HashToken(token);
    }
    catch (FormatException)
    {
        return Results.Problem("Invalid token", statusCode: 400);

    }

    User? user = await db.Users.FirstOrDefaultAsync(u => u.ResetToken == hashedToken);
    if (user == null)
    {
        return Results.Problem("Invalid token", statusCode: 400);
    }
    
    user.Password = passwordHasher.HashPassword(user, password);

   
    user.ResetToken = null;
    user.ResetTokenCreationTime = null;
    db.Users.Update(user);
    await db.SaveChangesAsync();

    return Results.Ok();
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