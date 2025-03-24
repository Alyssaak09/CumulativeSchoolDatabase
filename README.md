# ASP.NET Core School Database System Application 
This school project connects our server to a MySQL Database with MySql.Data.MySqlClient.

- Models/SchoolDbContext.cs
   * A class which represents the connection to the database. Be mindful of the connection string fields!

- Controllers/TeacherAPIController.cs, Controllers/StudentsAPIController.cs, Controllers/CoursesAPIController
  * An API Controller which allows us to access information about Teachers,Students,Courses
  
- Program.cs
  * Configuration of the application

## Testing
-Test the ListTeacherNames API responds with information about teachers.
  * GET api/Teacher/ListTeacherNames through cURL and MVC 

## Common Errors
- Make sure "MySQL.Data" is installed in your project
  * If not installed, go to "Tools" > "Nuget Package Manager" > "Manage Nuget Packages for Solution" > "Browse" > type "MySQL.Data" > "Install"

- Make sure "Swashbuckle.AspNetCore.Swagger" is installed in your project
  * If not installed, go to "Tools" > "Nuget Package Manager" > "Manage Nuget Packages for Solution" > "Browse" > type "Swashbuckle.AspNetCore.Swagger" > "Install"

- Make sure "Swashbuckle.AspNetCore.SwaggerGen" is installed in your project
  * If not installed, go to "Tools" > "Nuget Package Manager" > "Manage Nuget Packages for Solution" > "Browse" > type "Swashbuckle.AspNetCore.SwaggerGen" > "Install"

- Make sure the view folder name 'TeacherPage' needs to match the the first two name of the Controller 'TeacherPageController' name
  
- Update Program.cs and add the Database 'builder.Services.AddScoped<SchoolDbContext>();' and API 'builder.Services.AddScoped<TeacherAPIController>();','builder.Services.AddScoped<StudentsAPIController>();', 'builder.Services.AddScoped<CoursesAPIController>();'
  
- Update Layout.cshtml in the shared folder and add a li class and make sure asp-controller matchs the view folder name

## Tools 
Tools used in this project is:
- MicroSoft Visual Studio 'ASP.NET CORE Web App (Model-View-Controller) #C
- MAMP
- Swagger
- cURL
