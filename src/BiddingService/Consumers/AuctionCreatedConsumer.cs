using Contracts;
using MassTransit;
using BiddingService.Models;
using MongoDB.Entities;

namespace BiddingService.Consumers;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
{
  public async Task Consume(ConsumeContext<AuctionCreated> context)
  {
    Console.WriteLine("--> Consuming auction created: " + context.Message.Id);

    var auction = new Auction
    {
      ID = context.Message.Id.ToString(),
      Seller = context.Message.Seller,
      ReservePrice = context.Message.ReservePrice,
      AuctionEnd = context.Message.AuctionEnd
    };

    await auction.SaveAsync();

    // return Task.CompletedTask;
  }
}
