using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
.LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.Authority = builder.Configuration["IdentityServerURL"];
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters.ValidateAudience = false;
    options.TokenValidationParameters.NameClaimType = "username";
  });

builder.Services.AddCors(options =>
{
  options.AddPolicy("customPolicy", policy =>
  {
    policy
      .WithOrigins(builder.Configuration["ClientUrl"])
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();
  });
});

var app = builder.Build();

app.UseCors("customPolicy");

app.MapReverseProxy();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
