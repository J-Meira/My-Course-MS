using AuctionService.Consumers;
using AuctionService.Data;
using AuctionService.Data.Repositories;
using AuctionService.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
  opt.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddMassTransit(x =>
{
  x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
  {
    o.QueryDelay = TimeSpan.FromSeconds(10);
    o.UsePostgres();
    o.UseBusOutbox();
  });

  x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();

  x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));


  x.UsingRabbitMq((context, cfg) =>
  {
    cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
    {
      host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
      host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
    });
    cfg.ConfigureEndpoints(context);
  });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.Authority = builder.Configuration["IdentityServerURL"];
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters.ValidateAudience = false;
    options.TokenValidationParameters.NameClaimType = "username";
  });

builder.Services.AddGrpc();
builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<GrpcAuctionService>();

try
{
  await DbInitializer.InitDb(app);
}
catch (Exception e)
{
  Console.WriteLine(e);
}

app.Run();

public partial class Program { }
