using Microsoft.AspNetCore.Mvc;
using RedisExchangeApi.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeApi.Web.Controllers
{
    public class HashTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;
        private readonly string hashKey = "dictionary";

        public HashTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(3);
        }

        public async Task<IActionResult> Index()
        {
            var dictionaryList = new Dictionary<string,string>();

            if(await _db.KeyExistsAsync(hashKey))
            {
                _db.HashGetAll(hashKey).ToList().ForEach(x=>{
                    dictionaryList.Add(x.Name.ToString(), x.Value.ToString());
                });
            }

            return View(dictionaryList);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string name, string value)
        {
            await _db.HashSetAsync(hashKey, name, value);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string name)
        {
            await _db.HashDeleteAsync(hashKey, name);

            return RedirectToAction("Index");
        }
    }
}
