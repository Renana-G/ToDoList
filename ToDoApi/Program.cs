using TodoApi;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddDbContext<ToDoDbContext>();


// builder.Services.AddCors(options =>
// {
//     options.AddPolicy("AllowAll",
//         builder =>
//         {
//             builder.WithOrigins("http://localhost:3000", "https://todolist-pe48.onrender.com/") 
//                 .AllowAnyMethod()
//                 .AllowAnyHeader()
//                 .AllowCredentials();
//         });
// });

builder.Services.AddCors();
 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
    {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

// builder.Services.AddAuthentication().AddJwtBearer("LocalAuthIssuer");
builder.Services.AddAuthorization();


var app = builder.Build();

//swagger 
// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

//cors
// app.UseCors("AllowAll");
 app.UseCors(builder => builder
 .AllowAnyOrigin()
 .AllowAnyMethod()
 .AllowAnyHeader()
);
app.UseAuthentication();
app.UseAuthorization();

//routs

app.MapPost("/login", (User loginUser, ToDoDbContext context)=>{

    var user = context.Users.FirstOrDefault(user => user.Email==loginUser.Email && user.Password==loginUser.Password);
   
    if(user!=null){
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];
        var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString())
             }),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials
            (new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);
        var stringToken = tokenHandler.WriteToken(token);
        return Results.Ok(stringToken);
    }
    return Results.Unauthorized();
});

app.MapPost("/register", async(User user, ToDoDbContext context)=>{
    context.Users.Add(user);
    await context.SaveChangesAsync();
    return Results.Created($"new user created with user id: {user.Id}", user);
});

app.MapGet("/task", async(ToDoDbContext context) =>{
    //var str = user?.Identity?.ToString();
    var tasks = await context.Items.ToListAsync();//.Where(task=> task.UserId==user.).ToList();
    return Results.Ok(tasks);
} );
//.RequireAuthorization();

app.MapPost("/task", async (Item item, ToDoDbContext context) =>{
    context.Items.Add(item);
    await context.SaveChangesAsync();
    return Results.Created($"new task created with id:{item.Id}", item);
} );
//.RequireAuthorization();


app.MapPut("/task/{id}", async (int id, Item updatedItem, ToDoDbContext context) => {

    var existingItem = context.Items.FirstOrDefault(i => i.Id == id);

    if (existingItem == null)
    {
        return Results.NotFound("Item not found");
    }

    existingItem.Name = updatedItem.Name!=null?  updatedItem.Name : existingItem.Name;
    existingItem.IsComplete = updatedItem.IsComplete!=null? updatedItem.IsComplete: existingItem.IsComplete;

    context.Items.Update(existingItem);

    // Save changes to the database
    await context.SaveChangesAsync();

    return Results.Ok($"Task #{existingItem.Id} updated successfully");

});
//.RequireAuthorization();

app.MapDelete("/task/{id}", async(int id, ToDoDbContext context) => {
    
    var existingItem = context.Items.FirstOrDefault(i => i.Id == id);

    if(existingItem == null)
    {
        return Results.NotFound("Item not found");
    }

    // Remove the existing item from the context
    context.Items.Remove(existingItem);

    // Save changes to the database
    await context.SaveChangesAsync();

    return Results.Ok($"Task #{existingItem.Id} deleted successfully");
});
//.RequireAuthorization();


app.MapGet("/", () => "The app is running");

app.Run();
