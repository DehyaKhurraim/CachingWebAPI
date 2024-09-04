using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CachingWebApi.Data;
using CachingWebApi.Models;
using CachingWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CachingWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriversController: ControllerBase
    {
        private readonly  ILogger<DriversController> _logger;
        private readonly ICacheService _cacheService;
        private readonly AppDbContext  _context;

        public DriversController(ILogger<DriversController> logger, ICacheService cacheService, AppDbContext context)
        {
            _logger = logger;
            _cacheService = cacheService;
            _context = context;
        }   
        
        [HttpGet("drivers")]
        public async Task<IActionResult> Get() {
            var cacheData = _cacheService.GetData<List<Drivers>>("drivers");
            if(cacheData != null && cacheData.Count > 0){
                Console.WriteLine("Target Cache...");
                return Ok(cacheData);
            }
            cacheData = await _context.Drivers.ToListAsync();
            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData<List<Drivers>>("drivers", cacheData, expirationTime);
            Console.WriteLine("Target Database...");
            return Ok(cacheData);
        }

        [HttpPost("AddDriver")]
        public async Task<IActionResult> Post(Drivers value){
            var addedObject = await _context.Drivers.AddAsync(value);
            var expirationTime = DateTimeOffset.Now.AddSeconds(30);
            _cacheService.SetData<Drivers>($"driver{value.Id}", addedObject.Entity, expirationTime);
            await _context.SaveChangesAsync();
            return Ok(addedObject.Entity);
        }

        [HttpDelete("DeleteDriver")]
        public async Task<IActionResult> Delete(int id) {
            var exist = await _context.Drivers.FirstOrDefaultAsync(x => x.Id == id);    
            if(exist != null){
                _context.Drivers.Remove(exist);
                _cacheService.RemoveData($"driver{id}");
                await _context.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }
    }
}