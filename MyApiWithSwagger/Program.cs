using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Services.Implementations;
using Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MyConnection")
    ));
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBorrowRecordService, BorrowRecordService>();
builder.Services.AddScoped<IReaderService, ReaderService>();


// Add Swashbuckle Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Library Management System",
        Version = "v1",
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
    });
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
