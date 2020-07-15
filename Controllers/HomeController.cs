using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Test_Bus.Data;
using Test_Bus.Models;
using Test_Bus.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Test_Bus.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<LinksHub> _hubContext;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IHubContext<LinksHub> hubContext)
        {
            _logger = logger;
            _context = context;
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {           
            var All_links = _context.LinksAvtobus.ToList();
            return View(All_links);
        }
                
        public async Task<IActionResult> Redirection(string hash)
        {
            var link = await _context.LinksAvtobus.FirstOrDefaultAsync(x => x.ShortUrl == Request.GetEncodedUrl());
            if (link == null)
            {
                return RedirectToAction(nameof(Index));
            }
            link.Count += 1;
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("RedirectLink", link.Id);
            return Redirect(link.LongUrl);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Link link)
        {
            if (ModelState.IsValid)
            {
                link.LongUrl = link.LongUrl.Trim('/');
                if (LinkExist(link.LongUrl))
                {
                    ViewBag.ShortUrl = (await _context.LinksAvtobus.FirstOrDefaultAsync(x => x.LongUrl == link.LongUrl)).ShortUrl;
                    return View();
                }
                link.ShortUrl = GetShortUrl();
                _context.Add(link);
                await _context.SaveChangesAsync();
                ViewBag.ShortUrl = link.ShortUrl;                
            }
            return View();
        }

        public async Task<IActionResult> Edit(int id)
        {
            var link = await _context.LinksAvtobus.FirstOrDefaultAsync(x => x.Id == id);
            return View(link);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Link link)
        {
            if (ModelState.IsValid)
            {
                link.LongUrl = link.LongUrl.Trim('/');
                if (LinkExist(link.LongUrl))
                {
                    return View(await _context.LinksAvtobus.FirstOrDefaultAsync(x => x.LongUrl == link.LongUrl));
                }                
                _context.Update(link);
                await _context.SaveChangesAsync();                
            }
            return View(link);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var link = await _context.LinksAvtobus.FirstOrDefaultAsync(x => x.Id == id);
            _context.Remove(link);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        public string GetShortUrl()
        {
            var letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            var rand = new Random();
            string result = "";          
            while (ShortUrlExist(result) || result == "")
            {
                result = $"{Request.Scheme}://{Request.Host}/";
                for (var i=0; i<6; i++)
                {
                    result += letters[rand.Next(letters.Length-1)];
                }
            }            
            return  result;
        }

        public bool ShortUrlExist(string shortUrl)
        {
            return _context.LinksAvtobus.Any(x => x.ShortUrl == shortUrl);
        }

        public bool LinkExist(string longUrl)
        {
            return _context.LinksAvtobus.Any(x => x.LongUrl == longUrl);
        }

    }
}
