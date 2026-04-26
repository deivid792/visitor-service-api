using System.ClientModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;
using Resend;
using Visitor.Cfg.Infrastructure.Cache;
using VisitorService.aplication.Interface;
using VisitorService.Application.Interfaces;
using VisitorService.Application.UseCases.Users.Commands;
using VisitorService.Application.UseCases.Users.Commands.CreateManager;
using VisitorService.Application.UseCases.Visits.Commands;
using VisitorService.Application.UseCases.Visits.Queries;
using VisitorService.Domain.Interfaces;
using VisitorService.Domain.Services;
using VisitorService.Infrastructure.Context;
using VisitorService.Infrastructure.Repositories;
using VisitorService.Infrastructure.Services;
using VisitorService.Interfaces.Extensions;
using VisitorService.Interfaces.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Controllers
builder.Services.AddControllers();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<AppDbContext>();

// Repositories
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVisitRepository, VisitRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();
builder.Services.AddTransient<IEmailService, EmailService>();


// Handlers / Use Cases
builder.Services.AddScoped<IcreateVisitHandler, CreateVisitHandler>();
builder.Services.AddScoped<IGetAllVisitsHandler, GetAllVisitsHandler>();
builder.Services.AddScoped<IGetTodayApprovedVisitsHandler, GetTodayApprovedVisitsHandler>();
builder.Services.AddScoped<IloginHandler, LoginHandler>();
builder.Services.AddScoped<IRegisterVisitorHandler, RegisterVisitorHandler>();
builder.Services.AddScoped<IUpdateVisitStatusHandler, UpdateVisitStatusHandler>();
builder.Services.AddScoped<IVisitCheckInHandler, VisitCheckInHandler>();
builder.Services.AddScoped<IVisitCheckOutHandler, VisitCheckOutHandler>();
builder.Services.AddScoped<ICreateManagerHandler, CreateManagerHandler>();

// Email Settings
builder.Services.AddOptions();
builder.Services.AddHttpClient<IResend, ResendClient>();
builder.Services.Configure<ResendClientOptions>( options =>
{
    options.ApiToken = builder.Configuration["ResendSettings:ApiToken"];
} );
builder.Services.AddTransient<IResend, ResendClient>();

// JWT Auth
builder.Services.AddJwtAuthentication(builder.Configuration);

//HeaderPropagation
builder.Services.AddHeaderPropagation(options => { options.Headers.Add("Authorization"); });

var groqApiKey = builder.Configuration["GroqApiKey"];

// LLM Config
var openAIClient = new OpenAIClient(new ApiKeyCredential(groqApiKey), new OpenAIClientOptions { Endpoint = new Uri("https://api.groq.com/openai/v1") });
builder.Services.AddChatClient(openAIClient.GetChatClient("llama-3.3-70b-versatile").AsIChatClient())
    .UseFunctionInvocation();

builder.Services.AddChatClient(openAIClient.GetChatClient("llama-3.3-70b-versatile").AsIChatClient())
    .UseFunctionInvocation(configure: options =>
    {
        options.MaximumIterationsPerRequest = 3;
    });

// Cache and History
builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(o => { o.Configuration = "localhost:6379"; });
builder.Services.AddScoped<ChatHistoryService>();
builder.Services.AddScoped<ChatService>();

// MCP
builder.Services.AddMcpClient("http://localhost:5000/sse");

System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var app = builder.Build();

// Exceptions
app.UseGlobalExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

// Authentication
app.UseAuthentication();
app.UseAuthorization();

// Propagation
app.UseHeaderPropagation();

// Map Controllers
app.MapControllers();

app.Run();


