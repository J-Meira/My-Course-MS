using Contracts;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using NotificationService.Hubs;

namespace NotificationService.Consumers;

public class AuctionCreatedConsumer(ILogger<AuctionCreatedConsumer> logger, IHubContext<NotificationHub> hubContext) : IConsumer<AuctionCreated>
{
  private readonly ILogger<AuctionCreatedConsumer> _logger = logger;
  private readonly IHubContext<NotificationHub> _hubContext = hubContext;

  public async Task Consume(ConsumeContext<AuctionCreated> context)
  {
    _logger.LogInformation("--> Consuming auction created: {Id}", context.Message.Id);

    await _hubContext.Clients.All.SendAsync("AuctionCreated", context.Message);
  }
}
