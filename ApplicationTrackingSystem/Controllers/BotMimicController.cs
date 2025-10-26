using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ApplicationTrackingSystem.Core.Interfaces;

namespace ApplicationTrackingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "BotMimic")]
    public class BotMimicController : ControllerBase
    {
        private readonly IBotMimicService _botMimicService;

        public BotMimicController(IBotMimicService botMimicService)
        {
            _botMimicService = botMimicService;
        }

        [HttpPost("process-all")]
        public async Task<ActionResult> ProcessAllTechnicalApplications()
        {
            try
            {
                await _botMimicService.ProcessTechnicalApplicationsAsync();
                return Ok(new { message = "All technical applications processed successfully" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("process/{applicationId}")]
        public async Task<ActionResult> ProcessSpecificApplication(int applicationId)
        {
            try
            {
                await _botMimicService.ProcessSpecificApplicationAsync(applicationId);
                return Ok(new { message = $"Application {applicationId} processed successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (System.InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
