using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using QtechOJT_Net9.Database;
using Scalar.AspNetCore;
using QtechOJT_Net9.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // our Vite/React dev URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// "Premature optimization is the root of all evil"
// Package Manager Console -> CLI command "Add-Migration [name], Update-Database"
// How did the migration file know to get the Main_Task class?
//  -Apparently, these are all abstracted and so have to be named appropriately upon declaration
//      because of the Context we set in the relevant DbContext (here KanbanDbContext)

builder.Services.AddDbContext<KanbanDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

        // This lets us change/update database entries as needed without warning,
        // By default (unlike in MySQL), SQL Server has it as a railguard.
        options.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
);

// For hashing using the standard Identity Framework library
//  Might have to reset everyone's password when transitioning from the old Javascript backend (since it uses BCrypt salts)
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Suggested by Claude
//  For attachment handling
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o =>
    o.MultipartBodyLengthLimit = 10 * 1024 * 1024); // 10 MB, mirrors multer limits

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference(); // to use Scalar, it is similar to Swagger. For OpenAPI documentation.
    app.MapOpenApi(); // for OpenAPI specs
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp"); // Cross Origins 

app.UseAuthorization();

app.MapControllers();


app.Run();
