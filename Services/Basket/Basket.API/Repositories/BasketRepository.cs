

using Basket.API.Entities;
using Basket.API.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API;

public class BasketRepository : IBasketRepository
{
    public readonly IDistributedCache _distributedCache;

    public BasketRepository(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
    }

    public async Task DeleteBasket(string userName)
    {   
        await _distributedCache.RemoveAsync(userName);;
    }

    public async Task<ShoppingCart> GetBasket(string userName)
    {
        var basket = await _distributedCache.GetStringAsync(userName);

        if (string.IsNullOrEmpty(basket))
        {
            return null!;
        }

        return JsonConvert.DeserializeObject<ShoppingCart>(basket)!;
    }

    public async Task<ShoppingCart> UpdateBasket(ShoppingCart basket)
    {
        await _distributedCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket));
        
        return await GetBasket(basket.UserName);
    }
}
