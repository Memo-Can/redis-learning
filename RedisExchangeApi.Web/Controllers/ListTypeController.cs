using Microsoft.AspNetCore.Mvc;
using RedisExchangeApi.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeApi.Web.Controllers
{
    public class ListTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;
        private readonly string listKey = "cars";

        public ListTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(0);
        }

        public async Task<ActionResult> Index()
        {
            var cars = new List<string>();

            if (await _db.KeyExistsAsync(listKey))
            {
                var redisArray = await _db.ListRangeAsync(listKey);
                redisArray.ToList().ForEach(car=>{
                    cars.Add(car.ToString());
                });
            }

            return View(cars);
        }

        [HttpPost]
        public async Task<ActionResult> Add(string name)
        {
            await _db.ListRightPushAsync(listKey, name);

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Delete(string name)
        {
            await _db.ListRemoveAsync(listKey,name);
            return RedirectToAction("Index");
        }
    }
}
