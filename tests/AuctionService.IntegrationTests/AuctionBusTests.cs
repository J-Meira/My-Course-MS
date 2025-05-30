namespace AuctionService.IntegrationTests;

[Collection("Shared collection")]
public class AuctionBusTests(CustomWebAppFactory factory) : IAsyncLifetime
{
  private readonly CustomWebAppFactory _factory = factory;
  private readonly Fixture _fixture = new();
  private readonly ITestHarness _testHarness = factory.GetTestHarness();
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
  public async Task CreateAuction_WithAuthenticationAndValidAuction_ShouldPublishAuctionCreated()
  {
    //arrange
    var seller = "test";
    var recordToCreate = _fixture.Create<CreateAuctionDto>();
    _client.SetFakeJwtBearerToken(AuthHelper.GetBearerToken(seller));

    //act
    var response = await _client.PostAsJsonAsync("/api/auctions", recordToCreate);

    //assert
    response.EnsureSuccessStatusCode();
    Assert.True(await _testHarness.Published.Any<AuctionCreated>());
  }
}
