using AuctionService.Controllers;
using AuctionService.Data.Repositories;
using AuctionService.DTOs;
using AuctionService.Entities;
using AuctionService.Helpers;
using AuctionService.UnitTests.Helpers;
using AutoFixture;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AuctionService.UnitTests;
public class AuctionControllerTest
{
  private readonly AuctionsController _controller;
  private readonly Fixture _fixture = new();
  private readonly IMapper _mapper;
  private readonly Mock<IAuctionRepository> _repository = new();
  private readonly Mock<IPublishEndpoint> _publishEndpoint = new();

  public AuctionControllerTest()
  {
    var mockMapper = new MapperConfiguration(cfg =>
    {
      cfg.AddMaps(typeof(MappingProfiles).Assembly);
    }).CreateMapper().ConfigurationProvider;

    _mapper = new Mapper(mockMapper);

    _controller = new AuctionsController(_repository.Object, _mapper, _publishEndpoint.Object)
    {
      ControllerContext = new ControllerContext()
      {
        HttpContext = new DefaultHttpContext
        {
          User = Authorization.GetClaims()
        }
      }
    };
  }

  [Fact]
  public async Task GetAuctions_WithNoParams_Returns10Auctions()
  {
    // arrange
    var auctions = _fixture.CreateMany<AuctionDto>(10).ToList();
    _repository.Setup(repo => repo.GetAuctionsAsync(null)).ReturnsAsync(auctions);

    // act
    var result = await _controller.GetAll(null);

    // assert
    Assert.Equal(10, result.Value.Count);
    Assert.IsType<ActionResult<List<AuctionDto>>>(result);
  }

  [Fact]
  public async Task GetAuctionById_WithValidGuid_Returns1Auction()
  {
    // arrange
    var auction = _fixture.Create<AuctionDto>();
    _repository.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

    // act
    var result = await _controller.GetById(auction.Id);

    // assert
    Assert.Equal(auction, result.Value);
    Assert.IsType<ActionResult<AuctionDto>>(result);
  }

  [Fact]
  public async Task GetAuctionById_WithInvalidGuid_ReturnsNotFound()
  {
    // arrange
    _repository.Setup(repo => repo.GetAuctionByIdAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

    // act
    var result = await _controller.GetById(Guid.NewGuid());

    // assert
    Assert.IsType<NotFoundResult>(result.Result);
  }

  [Fact]
  public async Task CreateAuction_WithValidAuction_Returns201Created()
  {
    // arrange
    var auctionToCreate = _fixture.Create<CreateAuctionDto>();
    _repository.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
    _repository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

    // act
    var result = await _controller.Create(auctionToCreate);
    var createdResult = result.Result as CreatedAtActionResult;

    // assert
    Assert.NotNull(createdResult);
    Assert.Equal("GetById", createdResult.ActionName);
    Assert.IsType<AuctionDto>(createdResult.Value);
    Assert.IsType<CreatedAtActionResult>(result.Result);
  }

  [Fact]
  public async Task CreateAuction_WithInvalidAuction_Returns400BadRequest()
  {
    // arrange
    var auctionToCreate = _fixture.Create<CreateAuctionDto>();
    _repository.Setup(repo => repo.AddAuction(It.IsAny<Auction>()));
    _repository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);

    // act
    var result = await _controller.Create(auctionToCreate);

    // assert
    Assert.IsType<BadRequestObjectResult>(result.Result);
  }

  [Fact]
  public async Task UpdateAuction_WithValidAuction_Returns204NoContent()
  {
    // arrange
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
    auction.Seller = "test";
    var auctionToUpdate = _fixture.Create<UpdateAuctionDto>();
    _repository.Setup(repo => repo.GetAuctionEntityAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
    _repository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

    // act
    var result = await _controller.UpdateAuction(auction.Id, auctionToUpdate);

    // assert
    Assert.IsType<OkResult>(result);
  }

  [Fact]
  public async Task UpdateAuction_WithInvalidAuction_Returns400BadRequest()
  {
    // arrange
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
    auction.Seller = "test";
    var auctionToUpdate = _fixture.Create<UpdateAuctionDto>();
    _repository.Setup(repo => repo.GetAuctionEntityAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
    _repository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(false);

    // act    
    var result = await _controller.UpdateAuction(auction.Id, auctionToUpdate);

    // assert
    Assert.IsType<BadRequestObjectResult>(result);
  }

  [Fact]
  public async Task UpdateAuction_WithInvalidId_Returns404NotFound()
  {
    // arrange
    var auctionToUpdate = _fixture.Create<UpdateAuctionDto>();
    _repository.Setup(repo => repo.GetAuctionEntityAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

    // act
    var result = await _controller.UpdateAuction(Guid.NewGuid(), auctionToUpdate);

    // assert 
    Assert.IsType<NotFoundResult>(result);
  }

  [Fact]
  public async Task UpdatedAuction_WithDifferentSeller_Returns403Forbidden()
  {
    // arrange
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
    auction.Seller = "seller1";
    var auctionToUpdate = _fixture.Create<UpdateAuctionDto>();
    _repository.Setup(repo => repo.GetAuctionEntityAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

    //act
    var result = await _controller.UpdateAuction(auction.Id, auctionToUpdate);

    //assert
    Assert.IsType<ForbidResult>(result);
  }

  [Fact]
  public async Task DeleteAuction_WithValidId_Returns204NoContent()
  {
    // arrange
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
    auction.Seller = "test";
    _repository.Setup(repo => repo.GetAuctionEntityAsync(It.IsAny<Guid>())).ReturnsAsync(auction);
    _repository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(true);

    // act
    var result = await _controller.DeleteAuction(auction.Id);

    // assert
    Assert.IsType<OkResult>(result);
  }

  [Fact]
  public async Task DeleteAuction_WithInvalidId_Returns404NotFound()
  {
    // arrange
    _repository.Setup(repo => repo.GetAuctionEntityAsync(It.IsAny<Guid>())).ReturnsAsync(value: null);

    // act
    var result = await _controller.DeleteAuction(Guid.NewGuid());

    // assert
    Assert.IsType<NotFoundResult>(result);
  }

  [Fact]
  public async Task DeleteAuction_WithDifferentSeller_Returns403Forbidden()
  {
    // arrange
    var auction = _fixture.Build<Auction>().Without(x => x.Item).Create();
    auction.Item = _fixture.Build<Item>().Without(x => x.Auction).Create();
    auction.Seller = "seller1";
    _repository.Setup(repo => repo.GetAuctionEntityAsync(It.IsAny<Guid>())).ReturnsAsync(auction);

    // act
    var result = await _controller.DeleteAuction(auction.Id);

    // assert
    Assert.IsType<ForbidResult>(result);
  }

}
