using AuctionService.Data;
using AuctionService.Data.Repositories;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuctionsController(
    IAuctionRepository repository,
    IMapper mapper,
    IPublishEndpoint publishEndpoint
  ) : ControllerBase
  {
    private readonly IAuctionRepository _repository = repository;
    private readonly IMapper _mapper = mapper;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAll(string date)
    {
      return await _repository.GetAuctionsAsync(date);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetById(Guid id)
    {
      var auction = await _repository.GetAuctionByIdAsync(id);

      return auction is null
        ? NotFound()
        : auction;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> Create(CreateAuctionDto auctionDto)
    {
      var auction = _mapper.Map<Auction>(auctionDto);

      auction.Seller = User.Identity.Name;

      _repository.AddAuction(auction);

      var newAuction = _mapper.Map<AuctionDto>(auction);

      await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

      var result = await _repository.SaveChangesAsync();

      return !result ?
        BadRequest("Could not save changes to the DB") :
        CreatedAtAction(nameof(GetById),
          new { auction.Id }, _mapper.Map<AuctionDto>(auction));
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
      var auction = await _repository.GetAuctionEntityAsync(id);

      if (auction is null) return NotFound();

      if (auction.Seller != User.Identity?.Name) return Forbid();

      auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
      auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
      auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
      auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
      auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;
      auction.UpdatedAt = DateTime.UtcNow;

      await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

      var result = await _repository.SaveChangesAsync();

      return result ?
       Ok() :
       BadRequest("Problem saving changes");

    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
      var auction = await _repository.GetAuctionEntityAsync(id);

      if (auction is null) return NotFound();

      if (auction.Seller != User.Identity?.Name) return Forbid();

      _repository.RemoveAuction(auction);

      await _publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

      var result = await _repository.SaveChangesAsync();

      return result ?
       Ok() :
       BadRequest("Could not update DB");

    }


  }
}
