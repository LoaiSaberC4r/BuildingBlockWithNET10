using AcademyOps.Api.Configuration;
using AcademyOps.Application;
using AcademyOps.Infrastructure;
using AcademyOps.Infrastructure.EntityFrameworkCore.SqlServer;
using AcademyOps.Infrastructure.EntityFrameworkCore.SqlServer.Persistence;
using Asp.Versioning;
using BuildingBlock.Api.Bootstrap;
using BuildingBlock.Api.Logging;
using BuildingBlock.Api.OpenApi;
using BuildingBlock.Api.ProblemDetails;

EnvironmentFileLoader.LoadIfDevelopment(args);

var builder = WebApplication.CreateBuilder(args);

builder.AddBuildingBlockSerilog("AcademyOps.Api");

builder.Services
    .AddControllers()
    .AddApplicationPart(typeof(BuildingBlock.Api.ProblemDetailsMappingMvc).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddBuildingBlockSwagger(options =>
{
    options.ApiTitle = "AcademyOps API";
});
builder.Services.AddBuildingBlockLocalization(builder.Configuration);
builder.Services.AddBuildingBlockProblemDetails();
builder.Services.AddAcademyOpsCors(builder.Configuration);
builder.Services.AddAcademyOpsApplication();
builder.Services.AddAcademyOpsInfrastructure(builder.Configuration);
builder.Services.AddAcademyOpsSqlServerPersistence(builder.Configuration);
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<AcademyOpsDbContext>();

var app = builder.Build();

app.UseBuildingBlockSerilog();
app.UseHttpsRedirection();
app.UseBuildingBlockLocalization();
app.UseRouting();
app.UseCors(CorsPolicyNames.Default);
app.UseSwagger(options =>
{
    options.RouteTemplate = "swagger/{documentName}/swagger.json";
});
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = "swagger";
    options.SwaggerEndpoint("./v1/swagger.json", "AcademyOps API v1");
});

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
