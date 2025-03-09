using AuctionService.Entities;

namespace AuctionService.UnitTests;

public class AuctionEntityTests
{
  [Fact]
  // public void Method_Scenario_ExpectedResult
  public void HasReservePrice_ReservePriceGreaterThanZero_ReturnsTrue()
  {
    //arrange
    var auction = new Auction
    {
      Id = Guid.NewGuid(),
      ReservePrice = 100
    };

    //act
    var result = auction.HasReservePrice();

    //assert
    Assert.True(result);
  }

  [Fact]
  public void HasReservePrice_ReservePriceIsZero_ReturnsFalse()
  {
    //arrange
    var auction = new Auction
    {
      Id = Guid.NewGuid(),
      ReservePrice = 0
    };

    //act
    var result = auction.HasReservePrice();

    //assert
    Assert.False(result);
  }
}
