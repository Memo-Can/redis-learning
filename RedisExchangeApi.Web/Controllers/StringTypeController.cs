using Microsoft.AspNetCore.Mvc;
using RedisExchangeApi.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeApi.Web.Controllers
{
    public class StringTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;

        public StringTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _db =_redisService.GetDb(0);
        }

        public async Task<IActionResult> Index()
        {
            //Creating string cache
            await _db.StringSetAsync("name", "memocan");
            await _db.StringSetAsync("visitor", 1);

            //Show string caches
            var name = await _db.StringGetAsync("name");
            var count = await _db.StringIncrementAsync("visitor", 1);

            ViewBag.name = name.HasValue ? name.ToString() : string.Empty;
            ViewBag.count = count > -1 ? count : 0;

            return View();
        }
    }
}
