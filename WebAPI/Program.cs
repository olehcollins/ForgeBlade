using Application;
using Application.Interfaces;
using Infrastructure;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;
using WebAPI;
using WebAPI.HealthCheck;

var builder = WebApplication.CreateBuilder(args);


// Conditional configure the Host and Logging services based on the environment
builder.Host.AddSerilogDocumentation(builder.Environment);

// Register your application services
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance
            = $"{context.HttpContext.Request.Method}: {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.Add("requestID", context.HttpContext.TraceIdentifier);
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();
builder.Services.AddApplication().AddInfrastructure();
builder.Services.AddSwaggerDocumentation(); // Use the custom Swagger extension method
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
// Configure Identity with EF Core
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
// Register your application services (from the Application layer) with their Infrastructure implementations
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddJwtConfiguration(builder.Configuration);
// Automatically retry failed requests up to 3 times, with increasing delays.
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(retryAttempt));
// Stops making requests if 5 consecutive failures occur, then waits 30 seconds before trying again.
var circuitBreakerPolicy = Policy
    .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
    .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
// For better resilience, combine a retry policy with a circuit breaker using PolicyWrap
var policyWrap = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

builder.Services.AddHttpClient("ResilientClient")
    .AddPolicyHandler(policyWrap);
builder.Services.AddHealthChecks().AddCheck<CustomHealthChecks>("Custom Health Check");
builder.Services.AddControllers();

var app = builder.Build();


// Middleware
app.UseExceptionHandler(_ => { }); // Exception Handler
app.UseHttpsRedirection(); // Enforce HTTPS early in the pipeline
app.UseSerilogDocumentation(app.Environment); // Log every request/response first
app.UseSwaggerDocumentation(app.Environment); // Register Swagger documentation routes
app.UseAuthentication();
app.UseAuthorization();
app.UseStatusCodePages(); // Use status code pages; update empty API responses

app.MapControllers();
app.MapCustomHealthChecks("/health");
// Minimal API endpoints
app.MapGet("/getUser", () => Results.BadRequest());
app.MapGet("/notFound", () => { throw new Exception("An exception occured"); });
app.MapGet("/goodRequest", () => Results.Ok("This is a good request"));


// End of pipeline
app.Run();