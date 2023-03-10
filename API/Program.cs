using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container. Orders are not important.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StoreContext>(opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline. Orders are important.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();


// this part is to create a database and seed the data when we run the code
var scope = app.Services.CreateScope();
// The idea behind this is we need to get hold of our DBContext service by creating a scope and store it inside the scope variable. From there we can create a variable to store our context in.
// get hold of the StoreContext
var context = scope.ServiceProvider.GetRequiredService<StoreContext>();

// we also want to get hold of a logger so that we can log any error that we get. The type we specify in the ILogger is the class that we're executing (Program)
var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

// Now we need to execute some code against our context. This is the code that could go wrong and it could generate an exception if we haven't got all of our ducks lined up as we need to. So we do this in try catch block
try
{
    context.Database.Migrate(); //Applies any pending migrations for the context to the database. Will create the database if it does not already exist.
    DbInitializer.Initialize(context);
}
catch (Exception ex)
{
    logger.LogError(ex, "A problem occurred during migration");
}


app.Run();
