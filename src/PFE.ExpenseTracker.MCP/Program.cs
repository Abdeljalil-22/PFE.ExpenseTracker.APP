using PFE.ExpenseTracker.Application;
using PFE.ExpenseTracker.Infrastructure;
using PFE.ExpenseTracker.MCP.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure our services
builder.Services.AddSingleton<GeminiService>();
builder.Services.AddScoped<IExpenseTrackerClient, ExpenseTrackerClient>();

// Add Application and Infrastructure layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Register stubs for MCP server dependencies
builder.Services.AddSingleton<PFE.ExpenseTracker.MCP.IExpenseService, PFE.ExpenseTracker.MCP.StubExpenseService>();
builder.Services.AddSingleton<PFE.ExpenseTracker.MCP.IBudgetService, PFE.ExpenseTracker.MCP.StubBudgetService>();
builder.Services.AddSingleton<PFE.ExpenseTracker.MCP.IAnalyticsService, PFE.ExpenseTracker.MCP.StubAnalyticsService>();
builder.Services.AddSingleton<PFE.ExpenseTracker.MCP.ICategoryService, PFE.ExpenseTracker.MCP.StubCategoryService>();
builder.Services.AddSingleton<PFE.ExpenseTracker.MCP.IMcpSecurityService, PFE.ExpenseTracker.MCP.StubMcpSecurityService>();
builder.Services.AddSingleton<PFE.ExpenseTracker.MCP.ExpenseTrackerMcpServer>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


var app = builder.Build();

// Add rate limiting middleware
app.UseMiddleware<PFE.ExpenseTracker.MCP.Middleware.RateLimitingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();
