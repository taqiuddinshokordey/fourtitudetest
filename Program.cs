using Microsoft.AspNetCore.Mvc;
using log4net;
using log4net.Config;
using System.IO;
using System.Linq;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Configure log4net
var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

// Load the .env file
Env.Load();

// Add services to the container.
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        // Create a logger instance
        var log = LogManager.GetLogger(typeof(Program));

        // Get the first error message from the model state
        var firstErrorMessage = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .Select(x => x.Value.Errors.FirstOrDefault()?.ErrorMessage)
            .FirstOrDefault();

        // Log the error message using log4net
        log.Error($"Validation failed: {firstErrorMessage}");

        // Custom response object with the error message in the 'message' field
        var response = new
        {
            result = 0,
            message = firstErrorMessage ?? "Validation failed."
        };

        // Return the custom bad request response with the modified structure
        return new BadRequestObjectResult(response);
    };
});

// Add Swagger for API documentation (optional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

