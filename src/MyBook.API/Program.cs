using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using MyBook.API.Services;
using MyBook.DbContexts;
using MyBook.Services;
using System.Globalization;
using System.Reflection;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson();

builder.Services.Configure<AppSettings>(builder.Configuration);
var settings = builder.Configuration.Get<AppSettings>();
ArgumentNullException.ThrowIfNull(settings, nameof(settings));

builder.Services.AddDbContext<MyBookDbContext>(options =>
    options.UseSqlServer(settings.ConnectionStrings.MyBookDbContextConnection));

builder.Services.AddTransient<IPropertyMappingService, PropertyMappingService>();
builder.Services.AddTransient<IPropertyCheckerService, PropertyCheckerService>();

builder.Services.AddScoped<IMyBookRepository, MyBookRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

    setupAction.IncludeXmlComments(xmlCommentsFullPath);
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddRateLimiter(_ =>
{
    _.AddFixedWindowLimiter(policyName: settings.GlobalLimitOptions.RateLimitName, options =>
    {
        options.PermitLimit = settings.GlobalLimitOptions.PermitLimit;
        options.Window = TimeSpan.FromSeconds(settings.GlobalLimitOptions.Window);
        options.QueueLimit = settings.GlobalLimitOptions.QueueLimit;
    });
    _.OnRejected = (context, _) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken: _);

        return new ValueTask();
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler(appBuilder =>
    {
        appBuilder.Run(async context =>
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("An unexpected fault happened. Try again later.", context.RequestAborted);
        });
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRateLimiter();

app.MapControllers();

app.Run();
