using AutoMapper;
using BiddingService.DTOs;
using BiddingService.Models;
using BiddingService.Services;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;

namespace BiddingService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BidsController(IMapper mapper, IPublishEndpoint publishEndpoint, GrpcAuctionClient grpcClient) : ControllerBase
{
  private readonly IMapper _mapper = mapper;
  private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
  private readonly GrpcAuctionClient _grpcClient = grpcClient;

  [Authorize]
  [HttpPost]
  public async Task<ActionResult<BidDto>> PlaceBid(string auctionId, int amount)
  {
    var auction = await DB.Find<Auction>().OneAsync(auctionId);

    if (auction == null)
    {
      auction = _grpcClient.GetAuction(auctionId);
      if (auction == null) return BadRequest("Cannot accept bids on this auction at this time");
    }

    var bidderName = User.Identity?.Name;

    if (auction.Seller == bidderName)
    {
      return BadRequest("Cannot bid on your own auction");
    }

    var bid = new Bid
    {
      Amount = amount,
      AuctionId = auctionId,
      Bidder = bidderName
    };

    if (auction.AuctionEnd < DateTime.UtcNow)
    {
      bid.BidStatus = BidStatus.Finished;
    }

    if (bid.BidStatus != BidStatus.Finished)
    {
      var highBid = await DB.Find<Bid>()
        .Match(a => a.AuctionId == auctionId)
        .Sort(b => b.Descending(x => x.Amount))
        .ExecuteFirstAsync();

      if (highBid != null && amount > highBid.Amount || highBid == null)
      {
        bid.BidStatus = amount >= auction.ReservePrice
          ? BidStatus.Accepted
          : BidStatus.AcceptedBelowReserve;
      }

      if (highBid != null && amount <= highBid.Amount)
      {
        bid.BidStatus = BidStatus.TooLow;
      }
    }

    await DB.SaveAsync(bid);

    await _publishEndpoint.Publish(_mapper.Map<BidPlaced>(bid));

    return Ok(_mapper.Map<BidDto>(bid));
  }

  [HttpGet("{auctionId}")]
  public async Task<ActionResult<List<BidDto>>> GetBidsForAuction(string auctionId)
  {
    var bids = await DB.Find<Bid>()
      .Match(a => a.AuctionId == auctionId)
      .Sort(b => b.Descending(x => x.BidTime))
      .ExecuteAsync();

    return bids.Select(_mapper.Map<BidDto>).ToList();
  }

}
