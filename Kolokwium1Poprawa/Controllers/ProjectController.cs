using Kolokwium1Poprawa.DTOs;
using Kolokwium1Poprawa.Exceptions;
using Kolokwium1Poprawa.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium1Poprawa.Controllers;

[Route("api/")]
[ApiController]
public class ProjectController : ControllerBase
{
    private readonly IDbService _dbService;

    public ProjectController(IDbService dbService)
    {
        _dbService = dbService;
    }

    [HttpGet("projects/{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        try
        {
            var res = await _dbService.GetProjectAsync(id);
            return Ok(res);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost("artifacts")]
    public async Task<IActionResult> AddProject([FromBody] ProjectRequestDto request)
    {
        try
        {
            await _dbService.AddProjectAsync(request);
            return Ok("Project added");
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}