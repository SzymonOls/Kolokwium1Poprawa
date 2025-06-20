namespace Kolokwium1Poprawa.DTOs;

public class ProjectDto
{
    public int ProjectId { get; set; }
    public string Objective { get; set; } = String.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public ArtifactDto Artifact { get; set; } = new ArtifactDto();
    public List<StaffAssignmentsDto> StaffAssignments { get; set; } = [];
}

public class ArtifactDto
{
    public string Name { get; set; } = String.Empty;
    public DateTime OriginDate { get; set; }
    public InstitutionDto Institution { get; set; } = new InstitutionDto();
}

public class InstitutionDto
{
    public int InstitutionId { get; set; }
    public string Name { get; set; } = String.Empty;
    public int FoundedYear { get; set; }
}

public class StaffAssignmentsDto
{
    public string FirstName { get; set; } = String.Empty;
    public string LastName { get; set; } = String.Empty;
    public DateTime HireDate { get; set; }
    public string Role { get; set; } = String.Empty;
}