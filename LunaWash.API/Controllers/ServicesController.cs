using System.Threading.Tasks;
using LunaWash.BLL.DTOs;
using LunaWash.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LunaWash.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceManagementService _serviceManagementService;

        public ServicesController(IServiceManagementService serviceManagementService)
        {
            _serviceManagementService = serviceManagementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _serviceManagementService.GetAllServicesAsync();
            return Ok(services);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(string id)
        {
            var service = await _serviceManagementService.GetServiceByIdAsync(id);
            if (service == null) return NotFound(new { message = "Service not found." });
            return Ok(service);
        }

        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] WashServiceCreateDto dto)
        {
            var service = await _serviceManagementService.CreateServiceAsync(dto);
            return CreatedAtAction(nameof(GetServiceById), new { id = service.Id }, service);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(string id, [FromBody] WashServiceUpdateDto dto)
        {
            var success = await _serviceManagementService.UpdateServiceAsync(id, dto);
            if (!success) return NotFound(new { message = "Service not found or already deleted." });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(string id)
        {
            var success = await _serviceManagementService.DeleteServiceAsync(id);
            if (!success) return NotFound(new { message = "Service not found or already deleted." });
            return NoContent();
        }

        [HttpPost("{id}/prices")]
        public async Task<IActionResult> AddOrUpdateServicePrice(string id, [FromBody] ServicePriceCreateUpdateDto dto)
        {
            var success = await _serviceManagementService.AddOrUpdateServicePriceAsync(id, dto);
            if (!success) return NotFound(new { message = "Service not found." });
            return Ok(new { message = "Service price updated successfully." });
        }
    }
}
