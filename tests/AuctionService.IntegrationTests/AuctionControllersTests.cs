using System.Net.Http.Json;
using AuctionService.Data;
using AuctionService.DTOs;
using Microsoft.Extensions.DependencyInjection;

using System.Net;
using AuctionService.IntegrationTests.Helpers;
using AuctionService.IntegrationTests.Fixtures;
using AutoFixture;

namespace AuctionService.IntegrationTests;

[Collection("Shared collection")]
public class AuctionControllersTests(CustomWebAppFactory factory) : IAsyncLifetime
{
  private readonly CustomWebAppFactory _factory = factory;
  private readonly Fixture _fixture = new();
  private readonly HttpClient _client = factory.CreateClient();
  private readonly string _auctionId = "afbee524-5972-4075-8800-7d1f9d7b0a0c";

  public Task InitializeAsync() => Task.CompletedTask;

  public Task DisposeAsync()
  {
    using var scope = _factory.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AuctionDbContext>();
    DbHelper.RestartDbForTests(db);
    return Task.CompletedTask;
  }

  [Fact]
  public async Task GetAuctions_WithNoParams_ShouldReturnAllAuctions()
  {
    // act
    var response = await _client
      .GetFromJsonAsync<List<AuctionDto>>("/api/auctions");

    // assert
    Assert.Equal(3, response.Count);
  }

  [Fact]
  public async Task GetAuctionById_WithValidId_ShouldReturnTheAuction()
  {
    // act
    var response = await _client
      .GetFromJsonAsync<AuctionDto>($"/api/auctions/{_auctionId}");

    // assert
    Assert.Equal(_auctionId, response.Id.ToString());
    Assert.Equal("GT", response.Model);
  }

  [Fact]
  public async Task GetAuctionById_WithInvalidId_ShouldReturnNotFound()
  {
    // act
    var response = await _client.GetAsync($"/api/auctions/{Guid.NewGuid()}");

    // assert
    Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
  }

  [Fact]
  public async Task GetAuctionById_WithInvalidGuid_ShouldReturnBadRequest()
  {
    //act
    var response = await _client.GetAsync($"/api/auctions/invalid-guid");

    //assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task CreateAuction_WithoutAuthentication_ShouldReturnUnauthorized()
  {
    //arrange
    var auctionToCreate = _fixture.Create<CreateAuctionDto>();
    //act
    var response = await _client.PostAsJsonAsync("/api/auctions", auctionToCreate);

    //assert
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task CreateAuction_WithAuthenticationAndInvalidAuction_ShouldReturnBadRequest()
  {
    //arrange
    var seller = "test";
    var recordToCreate = new CreateAuctionDto
    {
      Model = "test",
    };
    _client.SetFakeJwtBearerToken(AuthHelper.GetBearerToken(seller));

    //act
    var response = await _client.PostAsJsonAsync("/api/auctions", recordToCreate);

    //assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task CreateAuction_WithAuthenticationAndValidAuction_ShouldReturnCreated()
  {
    //arrange
    var seller = "test";
    var recordToCreate = _fixture.Create<CreateAuctionDto>();
    _client.SetFakeJwtBearerToken(AuthHelper.GetBearerToken(seller));

    //act
    var response = await _client.PostAsJsonAsync("/api/auctions", recordToCreate);
    var createdAuction = await response.Content.ReadFromJsonAsync<AuctionDto>();
    var responseBody = await response.Content.ReadFromJsonAsync<AuctionDto>();

    //assert
    response.EnsureSuccessStatusCode();
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    Assert.Equal(seller, responseBody.Seller);
  }

  [Fact]
  public async Task UpdateAuction_WithValidUpdateDtoAndUser_ShouldReturn200()
  {
    // arrange
    var seller = "test";
    var recordToCreate = _fixture.Create<CreateAuctionDto>();
    _client.SetFakeJwtBearerToken(AuthHelper.GetBearerToken(seller));
    var createResponse = await _client.PostAsJsonAsync("/api/auctions", recordToCreate);
    var auction = await createResponse.Content.ReadFromJsonAsync<AuctionDto>();
    var auctionId = auction.Id;
    var recordToUpdate = new UpdateAuctionDto
    {
      Make = auction.Make,
      Model = auction.Model,
      Year = auction.Year,
      Color = "Red",
      Mileage = auction.Mileage,
    };

    // act
    var response = await _client.PutAsJsonAsync($"api/auctions/{auctionId}", recordToUpdate);

    // assert
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
  }

  [Fact]
  public async Task UpdateAuction_WithValidUpdateDtoAndInvalidUser_ShouldReturn403()
  {
    // arrange 
    var seller = "test";
    var recordToCreate = _fixture.Create<CreateAuctionDto>();
    _client.SetFakeJwtBearerToken(AuthHelper.GetBearerToken(seller));
    var createResponse = await _client.PostAsJsonAsync("/api/auctions", recordToCreate);
    var auction = await createResponse.Content.ReadFromJsonAsync<AuctionDto>();
    var auctionId = auction.Id;
    var recordToUpdate = new UpdateAuctionDto
    {
      Make = auction.Make,
      Model = auction.Model,
      Year = auction.Year,
      Color = "Red",
      Mileage = auction.Mileage,
    };
    var seller2 = "test2";
    _client.SetFakeJwtBearerToken(AuthHelper.GetBearerToken(seller2));

    // act
    var response = await _client.PutAsJsonAsync($"api/auctions/{auctionId}", recordToUpdate);

    // assert
    Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
  }

  [Fact]
  public async Task UpdateAuction_WithInvalidUpdateDto_ShouldReturn400()
  {
    // arrange
    var seller = "test";
    var recordToCreate = _fixture.Create<CreateAuctionDto>();
    _client.SetFakeJwtBearerToken(AuthHelper.GetBearerToken(seller));
    var createResponse = await _client.PostAsJsonAsync("/api/auctions", recordToCreate);
    var auction = await createResponse.Content.ReadFromJsonAsync<AuctionDto>();
    var auctionId = auction.Id;
    var recordToUpdate = new UpdateAuctionDto
    {
      Make = auction.Make,
      Model = auction.Model,
      Year = auction.Year,
      Mileage = auction.Mileage,
    };

    //act
    var response = await _client.PutAsJsonAsync($"api/auctions/{auctionId}", recordToUpdate);

    //assert
    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }
}

