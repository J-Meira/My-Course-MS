using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Consumers;

public class AuctionFinishedConsumer(ILogger<AuctionFinishedConsumer> logger, IHubContext<NotificationHub> hubContext) : IConsumer<AuctionFinished>
{
  private readonly ILogger<AuctionFinishedConsumer> _logger = logger;
  private readonly IHubContext<NotificationHub> _hubContext = hubContext;

  public async Task Consume(ConsumeContext<AuctionFinished> context)
  {
    _logger.LogInformation("--> Consuming auction finished: {Id}", context.Message.AuctionId);

    await _hubContext.Clients.All.SendAsync("AuctionFinished", context.Message);
  }
}
