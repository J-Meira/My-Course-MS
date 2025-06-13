

using AuctionService;
using BiddingService.Models;
using Grpc.Net.Client;

namespace BiddingService.Services;

public class GrpcAuctionClient(ILogger<GrpcAuctionClient> logger, IConfiguration config)
{
  private readonly ILogger<GrpcAuctionClient> _logger = logger;
  private readonly IConfiguration _config = config;

  public Auction GetAuction(string id)
  {
    _logger.LogInformation("=====> Getting Auction with id: {id}", id);

    var channel = GrpcChannel.ForAddress(_config["GrpcAuction"]);
    var client = new GrpcAuction.GrpcAuctionClient(channel);
    var request = new GetAuctionRequest { Id = id };

    try
    {
      var response = client.GetAuction(request);
      return new Auction
      {
        ID = response.Auction.Id,
        Seller = response.Auction.Seller,
        ReservePrice = response.Auction.ReservePrice,
        AuctionEnd = DateTime.Parse(response.Auction.AuctionEnd),
      };
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Error getting Auction with id: {id}", id);
      return null;
    }
  }
}
