using System;
using Swashbuckle.AspNetCore.SwaggerGen;
using CumulativeSchoolDatabase.Models;
using CumulativeSchoolDatabase.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Swagger API help pages
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Teacher API",
        Version = "v1",
        Description = "API for managing teacher information",
    });
});

// Database
builder.Services.AddScoped<SchoolDbContext>();

// API for now, but should be added as separate Teacher Service,Student Service,Course Service
builder.Services.AddScoped<TeacherAPIController>();
builder.Services.AddScoped<StudentsAPIController>();
builder.Services.AddScoped<CoursesAPIController>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Use the Swagger documentation endpoint
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Teacher API V1");
        c.RoutePrefix = string.Empty; // To serve Swagger UI at the root of the app
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

