using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var application = builder.Build();

// Configure the HTTP request pipeline.
if (application.Environment.IsDevelopment())
{
    application.UseSwagger();
    application.UseSwaggerUI();
}

application.UseHttpsRedirection();

application.UseAuthorization();

application.MapControllers();

await application.RunAsync();