namespace Kolokwium1Poprawa.DTOs;

public class ProjectRequestDto
{
    public ArtifactRequestDto Artifact { get; set; } = new ArtifactRequestDto();
    public ProjectInfoDto Project { get; set; } = new ProjectInfoDto();
}

public class ArtifactRequestDto
{
    public int ArtifactId { get; set; }
    public string ArtifactName { get; set; } = string.Empty;
    public DateTime OriginDate { get; set; }
    public int InstitutionId { get; set; }
}

public class ProjectInfoDto
{
    public int ProjectId { get; set; }
    public string Objective { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}