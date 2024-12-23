using Microsoft.AspNetCore.Mvc;
using RedisExchangeApi.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeApi.Web.Controllers
{
    public class SortedSetTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;
        private readonly string listKey = "sortedsetnames";

        public SortedSetTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(2);
        }

        public async Task<IActionResult> Index()
        {
            var names = new HashSet<string>();

            if(await _db.KeyExistsAsync(listKey))
            {
                _db.SortedSetScan(listKey).ToList().ForEach(name=>{
                    names.Add(name.ToString());
                });
            }

            return View(names);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string name, int score)
        {
            await _db.SortedSetAddAsync(listKey, name, score);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string name)
        {
            await _db.SortedSetRemoveAsync(listKey, name);

            return RedirectToAction("Index");
            
        }
    }
}
