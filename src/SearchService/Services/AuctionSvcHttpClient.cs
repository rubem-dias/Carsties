using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Services
{
    public class AuctionSvcHttpClient
    {
        private IConfiguration _config;
        private HttpClient _httpClient;

        AuctionSvcHttpClient(HttpClient httpClient, IConfiguration config)
        {
            _config = config;
            _httpClient = httpClient;
        }

        public async Task<List<Item>> GetItemsForSearchDb()
        {
            var lastUpdated = await DB.Find<Item, string>()
                .Sort(x => x.Descending(a => a.UpdatedAt))
                .Project(a => a.UpdatedAt.ToString())
                .ExecuteFirstAsync();

             return await _httpClient.GetFromJsonAsync<List<Item>>($"{_config["AuctionSvcUrl"]}/api/auctions?date={lastUpdated}");
        }
    }
}
