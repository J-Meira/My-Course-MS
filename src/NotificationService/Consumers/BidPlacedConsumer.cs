using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Consumers;

public class BidPlacedConsumer(ILogger<BidPlacedConsumer> logger, IHubContext<NotificationHub> hubContext) : IConsumer<BidPlaced>
{
  private readonly ILogger<BidPlacedConsumer> _logger = logger;
  private readonly IHubContext<NotificationHub> _hubContext = hubContext;

  public async Task Consume(ConsumeContext<BidPlaced> context)
  {
    _logger.LogInformation("--> Consuming bid placed for auction: {Id}", context.Message.AuctionId);

    await _hubContext.Clients.All.SendAsync("BidPlaced", context.Message);
  }
}
