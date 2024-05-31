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
using yoomoney_api.authorize;
using System.Net.Sockets;
using Microsoft.Extensions.Options;
using System.Globalization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http.HttpResults;
//using Newtonsoft.Json;
///using Newtonsoft.Json;


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

//GET ALL TRENERS
app.MapGet("/treners",
       async (DBContext db) =>
       {
        var treners = await db.Treners
           .Include(t => t.WorkDays)
            .ThenInclude(wd => wd.WorkHours)
        .ToListAsync();
        return Results.Ok(treners);
    });


//GET TRENERS WORKDAYS PER WEEK
//app.MapGet("/treners/workload/{id:int}",
//          async (int id, DBContext db) =>
//          {
//              Trener trener = db.Treners
//                  .Include(t => t.WorkDays)
//                      .ThenInclude(wd => wd.WorkHours)
//                  .Single(t => t.Id == id);

//              var workDaysAndHoursPerWeek = trener.WorkDays
//                    .GroupBy(wd => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(wd.Date, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
//                    .Select(g => new
//                    {
//                        Week = g.Key,
//                        WorkDaysCount = g.Count(),
//                        WorkHoursCount = g.SelectMany(wd => wd.WorkHours).Count()
//                    })
//                    .ToList();
//          });


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

//DELLETE TRENERS BY ID
app.MapDelete("/treners/{id:int}", [Authorize(Policy = "RequireAdminRole")]
async (int id, DBContext db) =>
{
    var trener = await db.Treners.FirstOrDefaultAsync(m => m.Id == id);
    if (trener == null) return Results.NotFound(new { message = "No such membership" });

    db.Treners.Remove(trener);
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


//GET TRENER BY ID
app.MapGet("/treners/{id:int}",
          async (int id, DBContext db) =>
          {
        var trener = await db.Treners
            .Include(t => t.WorkDays)
            .ThenInclude(wd => wd.WorkHours)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (trener == null) return Results.NotFound(new { message = "No such trener" });

        return Results.Ok(trener);
    });


//CHECK IF USER IS TRENER
app.MapGet("/treners/exist/{email}",
             async (string email, DBContext db) =>
             {
        var isExist = await db.Treners.AnyAsync(u => u.Email == email);
        return Results.Ok(isExist);
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

//ADD TRENER
app.MapPost("/treners", [Authorize(Policy = "RequireAdminRole")]
async (Trener trener, DBContext db, IPasswordHasher<User> passwordHasher) =>
{
    trener.Password = passwordHasher.HashPassword(trener, trener.Password);
    await db.Treners.AddAsync(trener);
    await db.SaveChangesAsync();
    return Results.Created($"/api/memberships/{trener.Id}", trener);
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


////EDIT TRENER
//app.MapPut("/treners", [Authorize(Policy = "RequireAdminRole")]
//async (Trener trener, DBContext db) =>
//{
//    var trenerInDb = await db.Treners.FirstOrDefaultAsync(m => m.Id == trener.Id);
//    if (trenerInDb == null) return Results.NotFound(new { message = "No such membership" });

//    trenerInDb.Name = trener.Name;
//    trenerInDb.PhoneNumber = trener.PhoneNumber;
//    trenerInDb.Email = trener.Email;
//    trenerInDb.Password = trener.Password;
//    trenerInDb.IsBanned = trener.IsBanned;
//    trenerInDb.ResetToken = trener.ResetToken;
//    trenerInDb.ResetTokenCreationTime = trener.ResetTokenCreationTime;
//    trenerInDb.Orders = trener.Orders;
//    trenerInDb.WorkDays = trener.WorkDays;
//    await db.SaveChangesAsync();
//    return Results.Ok(trener);
//    });

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
            $"Go to this link to reset password: <a href='https://stirred-lightly-cattle.ngrok-free.app/users/resetpassword/{WebUtility.UrlEncode(token)}'>CREATE NEW PASSWORD</a>" +
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





//PAYMENT
app.MapPost("/users/payment",
       async (int userId, int membershipId, Order order, DBContext dbContext) =>
       {

           Console.WriteLine();
           Console.WriteLine(order.Amount.Currency);
           // GET USER
           var user = await dbContext.Users.
           Include(u => u.Orders)
           .FirstOrDefaultAsync(u => u.Id == userId);

           //GET MEMBERSHIP
           var membership = await dbContext.Memberships.FirstOrDefaultAsync(m => m.Id == membershipId);

           // CREATE REQUEST
           var client = new HttpClient();

           var byteArray = Encoding.ASCII.GetBytes("385708:test_QjpJqSgi_4o7cPSsSDe667iS9sUBdafzKvBLjmGmdvU");
           client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

           //ADD PAYMENT REQUEST TO DB
           membership.Orders.Add(order);
           user.Orders.Add(order);  
           //dbContext.Orders.Add(order);
           dbContext.Users.Update(user); 
           dbContext.Memberships.Update(membership);
           dbContext.SaveChanges();
           var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
           string json = System.Text.Json.JsonSerializer.Serialize(order, options);
            Console.WriteLine();
            Console.WriteLine(json);
           HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

           //CREATE REQUEST

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
           
           var response = await client.SendAsync(request);

           if (response.IsSuccessStatusCode)
           {
               var content_with_payment = await response.Content.ReadAsStringAsync();
               var options_2 = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
               Console.WriteLine();
               Console.WriteLine(content_with_payment);
               Payment payment = JsonSerializer.Deserialize<Payment>(content_with_payment, options_2);
               order.Payment = payment;
               dbContext.Orders.Update(order);
               dbContext.SaveChanges();
               return Results.Ok(payment);
           }
           else
           {
               Console.WriteLine(response.StatusCode.ToString());
               return Results.Problem("Error occurred while making the payment.");
           }


       });

//CHECK PAYMENT STATUS
app.MapPost("/users/payment/notification",
       async (Notification notification, DBContext dbContext) =>
       {
           Console.WriteLine();
           Console.WriteLine(notification.Object.Status);
           Console.WriteLine("NOTIFICATION!!!!!!!!");
           if (notification.Object.Status == "succeeded")
           {
               var paymentInDb = await dbContext.Payments
                 .Include(p => p.Order)
                     .ThenInclude(o => o.User)
                 .Include(p => p.Order)
                     .ThenInclude(o => o.Membership)
                 .FirstOrDefaultAsync(p => p.Id == notification.Object.Id);
               if (paymentInDb == null) return Results.NotFound(new { message = "No such payment" });
               paymentInDb.Status = notification.Object.Status;
               paymentInDb.Paid = true;
               dbContext.Payments.Update(paymentInDb);

               var user = paymentInDb.Order.User;
               var membership = paymentInDb.Order.Membership;
               var membershipInstance = new MembershipInstance
               {
                   MembershipId = membership.Id,
                   UserId = user.Id,
               };
               if (user is Member member)
               {
                   member?.UserMemberships?.Add(membershipInstance);
                   dbContext.Members.Update(member);
               }
               else
               {
                   Member newMember = new Member(user);
                   newMember?.UserMemberships?.Add(membershipInstance);
                   dbContext.Users.Remove(user);
                   dbContext.Members.Add(newMember);
               }

               dbContext.SaveChanges();
           }
           return Results.Ok();
       });


//USER OR MEMBER BY EMAIL
app.MapGet("/users/status/{email}",
          async (string email, DBContext db) =>
          {
           var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
           if (user == null) return Results.NotFound(new { message = "No such user" });
           if(user is Member) return Results.Ok(true);
           return Results.Ok(false);
       });


//GET TRENER WORKHOURS BY DATE
app.MapGet("/treners/workhours/{id:int}",
             async (string dateString, int id, DBContext db) =>
             {
                 DateTime date = DateTime.Parse(dateString);
                 date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

                 var trener = await db.Treners
            .Include(t => t.WorkDays)
            .ThenInclude(wd => wd.WorkHours)
             .ThenInclude(wh => wh.WorkHourClients)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (trener == null) return Results.NotFound(new { message = "No such trener" });

        var workDay = trener.WorkDays.FirstOrDefault(wd => wd.Date.Date == date.Date);
        if (workDay == null) return Results.NotFound(new { message = "No such work day" });

        var workHours = workDay.WorkHours.OrderBy(wh => wh.Start);
                 return Results.Ok(workHours);
    });

//GET TRENER AVAILABLE HOURS BY DATE BY TRENER ID
app.MapGet("/treners/workhours/available/{id:int}",
             async (int id, string dateString, DBContext db) =>
             {
                 DateTime date = DateTime.Parse(dateString);
                 date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

                 DateTime now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);


                 var trener = await db.Treners
                     .Include(t => t.WorkDays)
                     .ThenInclude(wd => wd.WorkHours)
                     .FirstOrDefaultAsync(m => m.Id == id);
                 if (trener == null) return Results.NotFound(new { message = "No such trener" });


                 var workDay = trener.WorkDays.FirstOrDefault(wd => wd.Date.Date == date.Date);
                 if (workDay == null) return Results.NotFound(new { message = "No such work day" });

                 var workHours = workDay.WorkHours.Where(w => w.IsAvailable && w.Start > now).OrderBy(wh => wh.Start);
                 return Results.Ok(workHours);
             });



//ADD WORKHOUR TO TRENER'S WORKDAY BY ID
app.MapPost("/treners/{id:int}/workday",
    async(int id, string dateString, DBContext db) =>
    {
        DateTime date = DateTime.Parse(dateString);
        date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        Console.WriteLine(date);

        var trener = await db.Treners
           .Include(t => t.WorkDays)
           .ThenInclude(wd => wd.WorkHours)
           .FirstOrDefaultAsync(m => m.Id == id);
        if (trener == null) return Results.NotFound(new { message = "No such trener" });

        var workDay = await db.WorkDays
        .Include(wd => wd.WorkHours)
        .Include(wd => wd.Trener)
        .FirstOrDefaultAsync(m => m.TrenerId == id && m.Date.Date == date.Date);
        if (workDay == null)
        {
            workDay = new WorkDay
            {
                TrenerId = id,
                Date = date.Date

            };
            trener.WorkDays.Add(workDay);
        }

        bool workHourExists = workDay.WorkHours.Any(wh => wh.Start == date);
        if (workHourExists)
        {
            return Results.Conflict(new { message = "WorkHour already exists for the specified WorkDay." });
        }
        
        workDay.WorkHours.Add(
                new WorkHour
                {
                    Start = date,
                    WorkDayId = workDay.Id
                }
                );

        db.SaveChanges();
        return Results.Ok(workDay);
    });


//REMOVE WORKHOUR FROM WORKDAY
app.MapDelete("/treners/{id:int}/workday",
    async (int id, string dateString, DBContext db) =>
    {
        DateTime date = DateTime.Parse(dateString);
        date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        Console.WriteLine(date);

        var trener = await db.Treners
           .Include(t => t.WorkDays)
           .ThenInclude(wd => wd.WorkHours)
           .FirstOrDefaultAsync(m => m.Id == id);
        if (trener == null) return Results.NotFound(new { message = "No such trener" });

        var workDay = await db.WorkDays
        .Include(wd => wd.WorkHours)
        .ThenInclude(wh => wh.WorkHourClients)
        .Include(wd => wd.Trener)
        .FirstOrDefaultAsync(m => m.TrenerId == id && m.Date.Date == date.Date);

        if (workDay == null) return Results.NotFound(new { message = "No such workday" });

        WorkHour workHour = workDay.WorkHours.FirstOrDefault(m => m.Start == date);
        if (workHour != null)
        {
            if (workHour.WorkHourClients.Any()) { return Results.Conflict(new { message = "WorkHour has clients" });}
            workDay.WorkHours.Remove(workHour);
            db.SaveChanges();
            return Results.Ok();
        }

        return Results.NotFound(new { message = "No such workhour" });

    });


//BOOK WORKOUT BY CLIENT ID AND TRENER ID
app.MapPost("/treners/{trenerId:int}/workday/{memberId:int}",
async (int trenerId, int memberId, string dateString, DBContext db) =>
{
    DateTime date = DateTime.Parse(dateString);
    date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
   
    var member = await db.Members
    .Include(t=>t.UserMemberships)
    .Include(t=>t.Workouts).FirstOrDefaultAsync(m => m.Id == memberId);
    if (member == null) return Results.NotFound(new { message = "No such member" });

    if (member.Workouts.Any(w => w.Start == date)) return Results.Conflict(new { message = "You already have a workout for this time" });

    var trener = await db.Treners
          .Include(t => t.WorkDays)
          .ThenInclude(wd => wd.WorkHours)
          .ThenInclude(wh => wh.WorkHourClients)
          .FirstOrDefaultAsync(m => m.Id == trenerId);
    if (trener == null) return Results.NotFound(new { message = "No such trener" });


    var workDay = trener.WorkDays.FirstOrDefault(m => m.Date.Date == date.Date);
    if (workDay == null) return Results.NotFound(new { message = "No work day" });


    var workHour = workDay.WorkHours.FirstOrDefault(w=>w.Start == date);

    bool hasWorkout = workHour.WorkDay.WorkHours.Any(o => o.WorkHourClients.Any(o => o.Id == memberId));

    if (hasWorkout) return Results.Conflict();

    workHour.WorkHourClients.Add(member);
    Console.WriteLine();
    Console.WriteLine();
    foreach (var w in workHour.WorkHourClients)
    {
        Console.WriteLine(w.Name);
    }
   if(workHour.WorkHourClients.Count >= workHour.Capacity)
    {
        workHour.IsAvailable = false;
    }

    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine(workHour.WorkHourClients.Count);

    await db.SaveChangesAsync();
    return Results.Ok();
});


//GET USER WORKOUTS BY ID AND DATE
app.MapGet("/member/{id:int}/workouts",
             async (int id, string dateString, DBContext db) =>
             {
                 DateTime date = DateTime.Parse(dateString);
                 date = DateTime.SpecifyKind(date, DateTimeKind.Utc);

                 var member = await db.Members
                     .Include(t => t.Workouts)
                     .ThenInclude(t=>t.WorkDay)
                     .ThenInclude(wd=>wd.Trener)
                     .FirstOrDefaultAsync(m => m.Id == id);
                 Console.WriteLine();
                 Console.WriteLine();
                
                 if (member == null) return Results.NotFound(new { message = "member" });
                 Console.WriteLine(member.Id);

                 var workouts = member.Workouts.Where(wd=>wd.Start.Date == date.Date);
                 return Results.Ok(workouts);
             });


app.MapPost("/trener/{trenerId:int}/workdays/copy",
             async (int trenerId, string dateStringFrom, string dateStringTo, DBContext db) =>
             {

                 DateTime dateFrom = DateTime.Parse(dateStringFrom);
                 dateFrom = DateTime.SpecifyKind(dateFrom, DateTimeKind.Utc);

                 DateTime dateTo = DateTime.Parse(dateStringTo);
                 dateTo = DateTime.SpecifyKind(dateTo, DateTimeKind.Utc);

                 var trener = await db.Treners
                   .Include(t => t.WorkDays)
                   .ThenInclude(wd => wd.WorkHours)
                   .FirstOrDefaultAsync(m => m.Id == trenerId);
                 if (trener == null) return Results.NotFound(new { message = "No such trener" });

                 var workDay = trener.WorkDays.FirstOrDefault(wd => wd.Date.Date == dateFrom.Date && wd.TrenerId == trenerId);
                 if (workDay == null) return Results.NotFound(new { message = "No such workDay" });

                 var targetDay = db.WorkDays.FirstOrDefault(wd => wd.Date.Date == dateTo.Date && wd.TrenerId==trenerId);
                 if (targetDay != null)
                 {
                     if (targetDay.WorkHours.Any())
                     {
                         return Results.Conflict();
                     }
                 }
                 else
                 {

                     targetDay = new WorkDay
                     {
                         Date = dateTo,
                         TrenerId = workDay.TrenerId,
                     };
                     db.WorkDays.Add(targetDay);
                     await db.SaveChangesAsync();
                 }

                 foreach (var workHour in workDay.WorkHours)
                 {
                     var newStart = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day,
                               workHour.Start.Hour, workHour.Start.Minute, workHour.Start.Second);

                     targetDay.WorkHours.Add(new WorkHour
                     {
                         Start = DateTime.SpecifyKind(newStart, DateTimeKind.Utc),
                         WorkDayId = targetDay.Id   
                     });
                     db.WorkDays.Update(targetDay);
                 }

                

                 await db.SaveChangesAsync();

                 return Results.Ok();
             });


//GET WORKHOUR CLIENTS BY  WORKHOUR ID
app.MapGet("/workhour/{id:int}/clients",
             async (int id, DBContext db) =>
             {
                 
                var workHour = await db.WorkHours
                 .Include(wh => wh.WorkHourClients)
                 .FirstOrDefaultAsync(wh => wh.Id == id);

                 if (workHour == null) return Results.NotFound(new { message = "No such workhour" });
                 return Results.Ok(workHour.WorkHourClients);
             });


//CANCEL WORKOUT BY WORKOUT ID AND CLIENT ID
app.MapDelete("/workhour/{workHourId:int}/client/{clientId:int}",
    async (int workHourId, int clientId, DBContext db) =>
    {
        var workHour = await db.WorkHours
        .Include(wh => wh.WorkHourClients)
        .FirstOrDefaultAsync(wh => wh.Id == workHourId);

        if (workHour == null) return Results.NotFound(new { message = "No such workhour" });

        // Проверка, что время тренировки еще не наступило
        if (workHour.Start <= DateTime.UtcNow)
        {
            return Results.BadRequest(new { message = "Cannot cancel a workout that has already started" });
        }

        var client = workHour.WorkHourClients.FirstOrDefault(c => c.Id == clientId);
        if (client == null) return Results.NotFound(new { message = "No such client" });

        workHour.WorkHourClients.Remove(client);
        if (workHour.IsAvailable == false)
        {
            workHour.IsAvailable = true;
        }
        await db.SaveChangesAsync();
        return Results.Ok();
    });


//COPY WORKOUTS TO PARTICAULAR DAYS
app.MapPost("/trener/{trenerId:int}/workdays/copy/weekday",
    async (int trenerId, string dateStringFrom, DBContext db) =>
    {
        DateTime dateFrom = DateTime.Parse(dateStringFrom);
        dateFrom = DateTime.SpecifyKind(dateFrom, DateTimeKind.Utc);

        var trener = await db.Treners
            .Include(t => t.WorkDays)
            .ThenInclude(wd => wd.WorkHours)
            .FirstOrDefaultAsync(m => m.Id == trenerId);
        if (trener == null) return Results.NotFound(new { message = "No such trener" });

        var workDay = trener.WorkDays.FirstOrDefault(wd => wd.Date.Date == dateFrom.Date);
        if (workDay == null) return Results.NotFound(new { message = "No such workDay" });

        for (int i = 1; i < 28; i++)
        {
            DateTime dateTo = dateFrom.AddDays(i);
            if (dateTo.DayOfWeek == dateFrom.DayOfWeek)
            {
                var targetDay = trener.WorkDays.FirstOrDefault(wd => wd.Date.Date == dateTo.Date);
                if (targetDay != null)
                {
                    if (targetDay.WorkHours.Any())
                    {
                        continue; // Skip this day if it already has work hours
                    }
                }
                else
                {
                    targetDay = new WorkDay
                    {
                        Date = dateTo,
                        TrenerId = workDay.TrenerId,
                    };
                    db.WorkDays.Add(targetDay);
                    await db.SaveChangesAsync();
                }

                foreach (var workHour in workDay.WorkHours)
                {
                    var newStart = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day,
                        workHour.Start.Hour, workHour.Start.Minute, workHour.Start.Second);

                    targetDay.WorkHours.Add(new WorkHour
                    {
                        Start = DateTime.SpecifyKind(newStart, DateTimeKind.Utc),
                        WorkDayId = workHour.WorkDayId
                    });
                }

                db.WorkDays.Update(targetDay);
                await db.SaveChangesAsync();
            }
        }

        return Results.Ok();
    });

//APPLY TO PARTICULAR WEEKDAYS
app.MapPost("/treners/{trenerId:int}/workday/{memberId:int}/weekday",
    async (int trenerId, int memberId, string dateStringFrom, DBContext db) =>
    {
        string result = ""; 
        DateTime dateFrom = DateTime.Parse(dateStringFrom);
        dateFrom = DateTime.SpecifyKind(dateFrom, DateTimeKind.Utc);

        var member = await db.Members
            .Include(t => t.UserMemberships)
            .Include(t => t.Workouts).FirstOrDefaultAsync(m => m.Id == memberId);
        if (member == null) return Results.NotFound(new { message = "No such member" });

        var trener = await db.Treners
            .Include(t => t.WorkDays)
            .ThenInclude(wd => wd.WorkHours)
            .ThenInclude(wh => wh.WorkHourClients)
            .FirstOrDefaultAsync(m => m.Id == trenerId);
        if (trener == null) return Results.NotFound(new { message = "No such trener" });

        int i = 0;
        for (DateTime date = dateFrom; date <= dateFrom.AddMonths(1); date = date.AddDays(1))
        {
            if (date.DayOfWeek == dateFrom.DayOfWeek)
            {
                i++;
                var workDay = trener.WorkDays.FirstOrDefault(m => m.Date.Date == date.Date);
                if (workDay == null) {
                    result += $"Week {i}: It is not trener's workday; ";
                    continue; 
                }// Skip if there's no work day on this date

                var workHour = workDay.WorkHours.FirstOrDefault(w => w.Start == date);

                bool hasWorkout = workHour.WorkDay.WorkHours.Any(o => o.WorkHourClients.Any(o => o.Id == memberId));

                if (hasWorkout) {
                    result += $"Week {i}: You already have a workout here; ";
                    continue; 
                }// Skip if the member already has a workout at this time
                if (!workHour.IsAvailable) {
                    result += $"Week {i}: This time is not available; ";
                    continue; } // Skip if the work hour is not available

                workHour.WorkHourClients.Add(member);
                result += $"Week {i}: Success; ";
                if (workHour.WorkHourClients.Count >= workHour.Capacity)
                {
                    workHour.IsAvailable = false;
                }

                await db.SaveChangesAsync();
            }
        }

        return Results.Ok(result);
    });

app.Run();






