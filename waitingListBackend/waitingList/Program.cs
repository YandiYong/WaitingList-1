using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using waitingList.Converters;
using waitingList.Data;
using waitingList.Services;

const string frontendCorsPolicy = "Frontend";

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        // API dates are accepted and returned only as dd/MM/yyyy.
        options.JsonSerializerOptions.Converters.Add(new StrictDateOnlyJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<WaitingListDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("WaitingListDatabase")));

// TimeProvider keeps local time consistent and makes the clock testable later.
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddScoped<CentreDataSeeder>();
builder.Services.AddScoped<ICentreService, CentreService>();
builder.Services.AddScoped<IVisitService, VisitService>();
builder.Services.AddScoped<ICheckInService, CheckInService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        var frontendOrigin = builder.Configuration["FrontendOrigin"]
            ?? "http://localhost:4200";

        policy.WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Build the local database and load test centres from Data/centres.json.
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<WaitingListDbContext>();
    await dbContext.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<CentreDataSeeder>().seedAsync();
}

// Local development uses HTTP to avoid development-certificate issues.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors(frontendCorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();
