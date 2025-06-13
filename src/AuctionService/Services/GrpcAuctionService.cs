using AuctionService.Data;
using Grpc.Core;

namespace AuctionService.Services;

public class GrpcAuctionService(AuctionDbContext dbContext, ILogger<GrpcAuctionService> logger) : GrpcAuction.GrpcAuctionBase
{
  private readonly AuctionDbContext _dbContext = dbContext;
  private readonly ILogger<GrpcAuctionService> _logger = logger;

  public override async Task<GrpcAuctionResponse> GetAuction(GetAuctionRequest request, ServerCallContext context)
  {
    _logger.LogInformation("======> Receive request for  Auction with id: {id}", request.Id);
    var auction = await _dbContext.Auctions.FindAsync(Guid.Parse(request.Id));

    return auction is null
      ? throw new RpcException(new Status(StatusCode.NotFound, "Auction not found"))
      : new GrpcAuctionResponse
      {
        Auction = new GrpcAuctionModel
        {
          Id = auction.Id.ToString(),
          ReservePrice = auction.ReservePrice,
          Seller = auction.Seller,
          AuctionEnd = auction.AuctionEnd.ToString()
        }
      };
  }

}
