
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data.Repositories;
public class AuctionRepository(AuctionDbContext context, IMapper mapper) : IAuctionRepository
{
  private readonly AuctionDbContext _context = context;
  private readonly IMapper _mapper = mapper;

  public void AddAuction(Auction auction)
  {
    _context.Auctions.Add(auction);
  }

  public async Task<AuctionDto> GetAuctionByIdAsync(Guid id)
  {
    return await _context.Auctions
      .Include(x => x.Item)
      .ProjectTo<AuctionDto>(_mapper.ConfigurationProvider)
      .FirstOrDefaultAsync(x => x.Id == id);
  }

  public async Task<Auction> GetAuctionEntityAsync(Guid id)
  {
    return await _context.Auctions
      .Include(x => x.Item)
      .FirstOrDefaultAsync(x => x.Id == id);
  }

  public async Task<List<AuctionDto>> GetAuctionsAsync(string date)
  {
    var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

    if (!string.IsNullOrEmpty(date))
    {
      query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
    }

    return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider).ToListAsync();
  }

  public void RemoveAuction(Auction auction)
  {
    _context.Auctions.Remove(auction);
  }

  public async Task<bool> SaveChangesAsync()
  {
    return await _context.SaveChangesAsync() > 0;
  }
}
