using fs_2025_a_api_demo_002.Endpoints;
using fs_2025_a_api_demo_002.Startup;
using fs_2025_assessment_1_75026.Endpoints;
using fs_2025_assessment_1_75026.Services;
using Microsoft.Azure.Cosmos;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the StationService
builder.Services.AddSingleton<IStationService, StationService>();

// Register MemoryCache
builder.Services.AddMemoryCache();

// Register the background service to update station data periodically

builder.Services.AddHostedService<StationUpdateBackgroundService>();


// Register CosmosClient

builder.Services.AddSingleton(s =>
{
    var config = s.GetRequiredService<IConfiguration>();
    return new CosmosClient(
        config["CosmosDb:Endpoint"],
        config["CosmosDb:Key"]
    );
});

builder.Services.AddSingleton<CosmosStationService>();


builder.AddDependencies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.AddWeatherEndPoints();
app.AddRootEndPoints();
app.AddBookEndPoints();
app.AddCourseEndPoints();

// V1 Endpoints
app.AddStationEndPoints();

// V2 Endpoints
app.AddStationV2EndPoints(); 


app.Run();


