using Contracts;
using MassTransit;
using BiddingService.Models;
using MongoDB.Entities;

namespace BiddingService.Consumers;

public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
{
  public async Task Consume(ConsumeContext<AuctionDeleted> context)
  {
    Console.WriteLine("--> Consuming auction deleted: " + context.Message.Id);

    await DB.DeleteAsync<Auction>(context.Message.Id);

  }
}
