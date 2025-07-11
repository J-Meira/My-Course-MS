
using AuctionService.Data;
using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuctionService.IntegrationTests.Helpers;
public static class DbHelper
{
  public static void InitDbForTests(AuctionDbContext context)
  {
    context.Auctions.AddRange(GetAuctionsForTest());
    context.SaveChanges();
  }

  public static void RestartDbForTests(AuctionDbContext context)
  {
    context.RemoveRange(context.Auctions);
    context.SaveChanges();
    InitDbForTests(context);
  }

  public static void RemoveDbContext<T>(this IServiceCollection services)
  {
    var descriptor = services.SingleOrDefault(d =>
            d.ServiceType == typeof(DbContextOptions<AuctionDbContext>));

    if (descriptor != null) services.Remove(descriptor);
  }

  public static void EnsureCreated<T>(this IServiceCollection services)
  {
    var sp = services.BuildServiceProvider();

    using var scope = sp.CreateScope();
    var scopedServices = scope.ServiceProvider;
    var db = scopedServices.GetRequiredService<AuctionDbContext>();

    db.Database.Migrate();
    InitDbForTests(db);
  }

  private static List<Auction> GetAuctionsForTest()
  {
    return
    [
      // 1 Ford GT
      new Auction
        {
            Id = Guid.Parse("afbee524-5972-4075-8800-7d1f9d7b0a0c"),
            Status = Status.Live,
            ReservePrice = 20000,
            Seller = "bob",
            AuctionEnd = DateTime.UtcNow.AddDays(10),
            Item = new Item
            {
              Make = "Ford",
              Model = "GT",
              Color = "White",
              Mileage = 50000,
              Year = 2020,
              ImageUrl = "https://cdn.pixabay.com/photo/2016/05/06/16/32/car-1376190_960_720.jpg"
            }
        },
        // 2 Bugatti Veyron
        new Auction
        {
            Id = Guid.Parse("c8c3ec17-01bf-49db-82aa-1ef80b833a9f"),
            Status = Status.Live,
            ReservePrice = 90000,
            Seller = "alice",
            AuctionEnd = DateTime.UtcNow.AddDays(60),
            Item = new Item
            {
              Make = "Bugatti",
              Model = "Veyron",
              Color = "Black",
              Mileage = 15035,
              Year = 2018,
              ImageUrl = "https://cdn.pixabay.com/photo/2012/05/29/00/43/car-49278_960_720.jpg"
            }
        },
        // 3 Ford mustang
        new Auction
        {
            Id = Guid.Parse("bbab4d5a-8565-48b1-9450-5ac2a5c4a654"),
            Status = Status.Live,
            Seller = "bob",
            AuctionEnd = DateTime.UtcNow.AddDays(4),
            Item = new Item
            {
              Make = "Ford",
              Model = "Mustang",
              Color = "Black",
              Mileage = 65125,
              Year = 2023,
              ImageUrl = "https://cdn.pixabay.com/photo/2012/11/02/13/02/car-63930_960_720.jpg"
            }
        }

    ];
  }
}

