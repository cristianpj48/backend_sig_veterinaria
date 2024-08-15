using SIG_VETERINARIA.Repository;
using SIG_VETERINARIA.Services;
using SIG_VETERINARIA.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRepositoryServices();
builder.Services.AddServiceServices();
builder.Services.AddApplicationServices();
builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", opt =>
    {
        opt.WithOrigins("*", "")
        .AllowAnyHeader()
        .AllowCredentials()
        .AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("*"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
