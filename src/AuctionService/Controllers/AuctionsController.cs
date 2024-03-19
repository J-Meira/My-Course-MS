using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuctionsController(AuctionDbContext context, IMapper mapper) : ControllerBase
  {
    private readonly AuctionDbContext _context = context;
    private readonly IMapper _mapper = mapper;


    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAll()
    {
      var auctions = await _context.Auctions
        .Include(x => x.Item)
        .OrderBy(x => x.Item.Make)
        .ToListAsync();

      return _mapper.Map<List<AuctionDto>>(auctions);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetById(Guid id)
    {
      var auction = await _context.Auctions
        .Include(x => x.Item)
        .FirstOrDefaultAsync(x => x.Id == id);

      return auction is null ?
       NotFound() :
       Ok(_mapper.Map<AuctionDto>(auction));
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> Create(CreateAuctionDto auctionDto)
    {
      var auction = _mapper.Map<Auction>(auctionDto);
      auction.Seller = "Test";
      _context.Auctions.Add(auction);

      var result = await _context.SaveChangesAsync() > 0;

      return !result ?
        BadRequest("Could not save changes to the DB") :
        CreatedAtAction(nameof(GetById),
          new { auction.Id }, _mapper.Map<AuctionDto>(auction));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
    {
      var auction = await _context.Auctions
        .Include(x => x.Item)
        .FirstOrDefaultAsync(x => x.Id == id);

      if (auction is null) return NotFound();

      auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
      auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
      auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
      auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
      auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

      var result = await _context.SaveChangesAsync() > 0;

      return result ?
       Ok() :
       BadRequest("Problem saving changes");

    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAuction(Guid id)
    {
      var auction = await _context.Auctions
        .FindAsync(id);

      if (auction is null) return NotFound();

      _context.Auctions.Remove(auction);

      var result = await _context.SaveChangesAsync() > 0;

      return result ?
       Ok() :
       BadRequest("Could not update DB");

    }


  }
}
