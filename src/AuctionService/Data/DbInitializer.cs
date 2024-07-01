
using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data
{
    public class DbInitializer
    {
        public static void Initialize(WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            SeedData(scope.ServiceProvider.GetService<AuctionDbContext>());
        }

        private static void SeedData(AuctionDbContext auctionDbContext)
        {
            auctionDbContext.Database.Migrate(); 

            if(auctionDbContext.Auctions.Any())
            {
                return;
            }

            var auction = new Auction()
            {
                Id = Guid.NewGuid(),
                ReservePrice = 1000,
                Seller = "John Doe",
                Winner = "Jane Smith",
                SoldAmount = 1500,
                CurrentHighBid = 1600,
                AuctionEnd = DateTime.UtcNow.AddDays(7),
                Status = Status.Live,
                Item = new Item
                {
                    Id = Guid.NewGuid(),
                    Make = "Ford",
                    Model = "F150",
                    Color = "Red",
                    Mileage = 10000,
                    Year = 2020
                }
            };

            auctionDbContext.Add(auction);
            auctionDbContext.SaveChanges();
        }
    }
}
