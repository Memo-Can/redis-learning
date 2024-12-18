using System.Text;
using System.Text.Json;
using IDistributedCacheApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace IDistributedCacheApp.Web.Controllers;

public class ProductController : Controller
{
    private readonly IDistributedCache _distributedCache;

    public ProductController(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
    }

    public async Task<IActionResult> Index()
    {
        //Redis be able to keep various of value type. SetString normaly keep to value as string but absuluteExpiration object changes to value type as hash that expiration value also keeps it.
        await _distributedCache.SetStringAsync("name","memo-can", 
            new DistributedCacheEntryOptions(){
             AbsoluteExpiration=DateTime.Now.AddMinutes(5)
         });


        //Sample is indicates that how to send an object to redis
        var user = new User{
            Id= Guid.NewGuid(),
            Name="Memo",
            SureName="Can"
        };

        var jsonUser=JsonSerializer.Serialize(user);

        await _distributedCache.SetStringAsync("user:1",jsonUser,  new DistributedCacheEntryOptions(){
             AbsoluteExpiration=DateTime.Now.AddMinutes(1)
         });

        //The data also be send bu byte array
        await _distributedCache.SetAsync("user:2",Encoding.UTF8.GetBytes(jsonUser),  new DistributedCacheEntryOptions(){
            AbsoluteExpiration=DateTime.Now.AddMinutes(1)
        });

        return View();
    }

    public async Task<IActionResult> Show()
    {
        ViewBag.name=await _distributedCache.GetStringAsync("name");
        ViewBag.user= JsonSerializer.Deserialize<User>(await _distributedCache.GetStringAsync("user:1"));
        ViewBag.image=await _distributedCache.GetAsync("image");

        return View();
    }

    public async Task<IActionResult> ImageUrl()
    {
        try
        {
            return File(await _distributedCache.GetAsync("image"),"image/png");
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IActionResult> Remove()
    {
        await _distributedCache.RemoveAsync("name");
        await _distributedCache.RemoveAsync("user:1");

        return View();
    }


    public async Task<IActionResult> ImageCache()
    {
        try
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(),"IDistributedCacheApp.Web/wwwroot/images/sample.png");
            var imageByte= System.IO.File.ReadAllBytes(path);

            await _distributedCache.SetAsync("image",imageByte);
            return View();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
