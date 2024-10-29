using Microsoft.OpenApi.Models;
using System.Xml.Linq;
using System;
using Microsoft.Extensions.Options;
using SendingEmailWithASPNETCore.Configuration;
using SendingEmailWithASPNETCore.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<ICustomMailService, CustomMailService>();
builder.Services.AddScoped<ICustomMailService, CustomMailService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Example API",
        Version = "v1",
        Description = "An example of an ASP.NET Core Web API",
        Contact = new OpenApiContact
        {
            Name = "Alfin Ellsyan",
            Email = "djalfin8@gmail.com",
            Url = new Uri("https://example.com/contact"),
        },
    });
});

builder.Services.Configure<MailSetting>(builder.Configuration.GetSection("MailSetting"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    });
}

builder.Services.AddControllers();

app.UseSwagger(options =>
{
    options.SerializeAsV2 = true;
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
