using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ApplicationTrackingSystem.Core.DTOs;
using ApplicationTrackingSystem.Core.Interfaces;

namespace ApplicationTrackingSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApplicationController : ControllerBase
    {
        private readonly IApplicationService _applicationService;

        public ApplicationController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost]
        [Authorize(Roles = "Applicant")]
        public async Task<ActionResult<ApplicationResponse>> CreateApplication([FromBody] CreateApplicationRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var application = await _applicationService.CreateApplicationAsync(userId, request);
                return CreatedAtAction(nameof(GetApplicationById), new { id = application.Id }, application);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationResponse>> GetApplicationById(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var application = await _applicationService.GetApplicationByIdAsync(id, userId, userRole);
                return Ok(application);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (System.UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-applications")]
        public async Task<ActionResult<IEnumerable<ApplicationResponse>>> GetMyApplications()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var applications = await _applicationService.GetMyApplicationsAsync(userId, userRole);
                return Ok(applications);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update-status")]
        [Authorize(Roles = "Admin,BotMimic")]
        public async Task<ActionResult<ApplicationResponse>> UpdateApplicationStatus([FromBody] UpdateApplicationStatusRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var application = await _applicationService.UpdateApplicationStatusAsync(userId, userRole, request);
                return Ok(application);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (System.UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
