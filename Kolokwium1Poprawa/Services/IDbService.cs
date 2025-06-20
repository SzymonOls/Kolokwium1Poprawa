using Kolokwium1Poprawa.DTOs;

namespace Kolokwium1Poprawa.Services;

public interface IDbService
{ 
    Task<ProjectDto> GetProjectAsync(int id);
    Task AddProjectAsync(ProjectRequestDto project);
}