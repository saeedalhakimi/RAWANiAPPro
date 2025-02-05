using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using RAWANi.WEBAPi.Application.Abstractions;
using RAWANi.WEBAPi.Application.Data.DbContexts;
using RAWANi.WEBAPi.Application.MEDiatR.AuthMDIR.CommandHandlers;
using RAWANi.WEBAPi.Application.Models;
using RAWANi.WEBAPi.Application.Repository;
using RAWANi.WEBAPi.Application.Services;
using RAWANi.WEBAPi.Filters;
using RAWANi.WEBAPi.Infrastructure.Data.DataWrapperFactory;
using RAWANi.WEBAPi.Infrastructure.Repository.Posts;
using RAWANi.WEBAPi.Infrastructure.Services;
using RAWANi.WEBAPi.Middlewares;
using RAWANi.WEBAPi.Services;
using Serilog;
using Serilog.Formatting.Json;
using System.Text;
using System.Threading.RateLimiting;

try
{
    // Configure Serilog
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build())
        .Enrich.FromLogContext()
        .WriteTo.Async(a => a.File(
            path: "Logs/log-.txt",
            rollingInterval: RollingInterval.Day,
            restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information,
            formatter: new JsonFormatter())) // Only asynchronous file sink
        .WriteTo.Debug() // Debug sink for development convenience
        .CreateLogger();

    Log.Information("Starting the web application setup at {Time}", DateTime.Now);

    var builder = WebApplication.CreateBuilder(args);

    // Use Serilog for logging
    builder.Host.UseSerilog();

    //Factories
    Log.Information("Initializing database connection factory...");
    builder.Services.AddScoped<IDatabaseConnectionFactory, SqlDatabaseConnectionFactory>();

    Log.Information("Initializing file service...");
    // Register the FileService
    builder.Services.AddScoped<IFileService>(provider =>
    {
        var uploadsFolderPath = Path.Combine(builder.Environment.WebRootPath, "uploads");
        return new FileService(uploadsFolderPath);
    });

    // Add rate limiting services
    Log.Information("Setting up rate limiting policies...");

    builder.Services.AddRateLimiter(options =>
    {
        // Environment-specific rate limits
        var isProduction = builder.Environment.IsProduction();
        var permitLimit = isProduction ? 100 : 1000;

        // Define a fixed window rate limiter policy
        options.AddPolicy("fixed", context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = TimeSpan.FromMinutes(1),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 10
                }));

        // Customize the response when the rate limit is exceeded
        options.OnRejected = (context, _) =>
        {
            // Log when rate limit is exceeded
            Log.Warning("Rate limit exceeded for {PartitionKey} at {Time}. Returning 429 Too Many Requests.",
                        context.HttpContext.User.Identity?.Name ?? context.HttpContext.Request.Headers.Host.ToString(), DateTime.Now);

            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.HttpContext.Response.Headers.RetryAfter = "60"; // Retry after 60 seconds
            context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.");
            return new ValueTask();
        };
    });

    // Add health checks services
    Log.Information("Setting up health check services...");
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<DataContext>(); // Check database connectivity
    /*.AddCheck<UserRepositoryHealthCheck>("user_repository_health_check");*/ // Add custom health check

    //servces for the DI container

    Log.Information("Registering services into DI container...");
    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ApiExceptionHandler>();
    });

    builder.Services.AddScoped<ILoggMessagingService, LoggMessagingService>();
    builder.Services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));
    builder.Services.AddScoped<IErrorHandler, ErrorHandler>();

    //Repositories
    Log.Information("Registering Repositories...");
    builder.Services.AddScoped<IPostRepository, PostRepository>();

    // Register MediatR

    Log.Information("Setting up MediatR services for CQRS...");
    builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(RegisterIdentityCommandHandler).Assembly));

    // DataContext
    Log.Information("Configuring database context...");
    builder.Services.AddDbContext<DataContext>(options =>
                   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Register Identity with RoleManager
    Log.Information("Configuring Identity with RoleManager...");
    builder.Services.AddIdentityCore<IdentityUser>(options =>
    {
        // Configure Identity options if needed
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
    })
        .AddRoles<IdentityRole>() // Add support for roles
        .AddEntityFrameworkStores<DataContext>() // Use the DataContext for Identity storage
        .AddRoleManager<RoleManager<IdentityRole>>() // Register RoleManager
        .AddUserManager<UserManager<IdentityUser>>() // Register UserManager
        .AddDefaultTokenProviders();


    // Versioning
    Log.Information("Initializing API versioning...");
    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new UrlSegmentApiVersionReader();
    })
               .AddApiExplorer(options =>
               {
                   options.GroupNameFormat = "'v'VVV";
                   options.SubstituteApiVersionInUrl = true;
               });

    // Jwt Authentication
    Log.Information("Initializing JWT authentication...");
    var jwtSettings = new JwtSettings();
    builder.Configuration.Bind(nameof(JwtSettings), jwtSettings);

    var jwtSection = builder.Configuration.GetSection(nameof(JwtSettings));
    builder.Services.Configure<JwtSettings>(jwtSection);

    if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key))
    {
        Log.Fatal("JwtSettings configuration is missing or invalid.");
        throw new InvalidOperationException("JwtSettings configuration is missing or invalid.");
    }

    builder.Services.AddSingleton<IJwtService, JwtService>();

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
            };
        });

    Log.Information("Configuring authorization policies...");
    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
        options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    });

    // Register the email service
    Log.Information("Registering email service...");
    builder.Services.AddSingleton<IEmailService>(new EmailService(
        smtpServer: builder.Configuration["EmailSettings:SmtpServer"],
        smtpPort: int.Parse(builder.Configuration["EmailSettings:SmtpPort"]),
        smtpUsername: builder.Configuration["EmailSettings:SmtpUsername"],
        smtpPassword: builder.Configuration["EmailSettings:SmtpPassword"]
    ));

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    Log.Information("Configuring middleware pipeline");

    // Serve static files from the "wwwroot" folder
    app.UseStaticFiles();

    // Serve static files from the "uploads" folder
    var uploadsFolderPath = Path.Combine(builder.Environment.WebRootPath, "uploads");
    if (!Directory.Exists(uploadsFolderPath))
    {
        Directory.CreateDirectory(uploadsFolderPath);
    }

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(uploadsFolderPath),
        RequestPath = "/uploads"
    });


    app.UseMiddleware<ExceptionMiddleware>();
    app.UseHttpsRedirection();

    // Map the health check endpoint
    app.MapHealthChecks("/health");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    Log.Information("Starting the web application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application startup failed");
}
finally
{
    Log.Information("Shutting down the web application");
    Log.CloseAndFlush();
}