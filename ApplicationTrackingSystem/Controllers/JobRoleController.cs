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
    public class JobRoleController : ControllerBase
    {
        private readonly IJobRoleService _jobRoleService;

        public JobRoleController(IJobRoleService jobRoleService)
        {
            _jobRoleService = jobRoleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobRoleResponse>>> GetAllJobRoles()
        {
            try
            {
                var jobRoles = await _jobRoleService.GetAllJobRolesAsync();
                return Ok(jobRoles);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<JobRoleResponse>> GetJobRoleById(int id)
        {
            try
            {
                var jobRole = await _jobRoleService.GetJobRoleByIdAsync(id);
                return Ok(jobRole);
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<JobRoleResponse>> CreateJobRole([FromBody] CreateJobRoleRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var jobRole = await _jobRoleService.CreateJobRoleAsync(userId, request);
                return CreatedAtAction(nameof(GetJobRoleById), new { id = jobRole.Id }, jobRole);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
