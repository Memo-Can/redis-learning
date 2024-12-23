using Microsoft.AspNetCore.Mvc;
using RedisExchangeApi.Web.Services;
using StackExchange.Redis;

namespace RedisExchangeApi.Web.Controllers
{
    public class SetTypeController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase _db;
        private readonly string listKey = "setnames";

        public SetTypeController(RedisService redisService)
        {
            _redisService = redisService;
            _db = _redisService.GetDb(1);
        }

        public async Task<IActionResult> Index()
        {
            var names = new HashSet<string>();

            if(await _db.KeyExistsAsync(listKey))
            {
                _db.SetMembers(listKey).ToList().ForEach(name=>{
                    names.Add(name.ToString());
                });
            }

            return View(names);
        }

        [HttpPost]
        public async Task<IActionResult> Add(string name)
        {
            await _db.KeyExpireAsync(listKey, DateTime.Now.AddSeconds(5));
            await _db.SetAddAsync(listKey, name);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string name)
        {
            await _db.SetRemoveAsync(listKey, name);

            return RedirectToAction("Index");

        }
    }
}
