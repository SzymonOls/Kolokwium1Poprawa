using System.Data.Common;
using Kolokwium1Poprawa.DTOs;
using Kolokwium1Poprawa.Exceptions;
using Microsoft.Data.SqlClient;

namespace Kolokwium1Poprawa.Services;

public class DbService : IDbService
{
    private readonly string _connectionString;

    public DbService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
    }

    public async Task<ProjectDto> GetProjectAsync(int projectId)
    {
        var query = @"
                    SELECT pp.ProjectId, pp.Objective, pp.StartDate, pp.EndDate, a.Name, a.OriginDate, i.InstitutionId, i.Name, i.FoundedYear, s.FirstName, s.LastName, s.HireDate, sa.Role
                    FROM Preservation_Project pp
                    join Artifact a on pp.ArtifactId = a.ArtifactId
                    join Institution i on a.InstitutionId = i.InstitutionId
                    join Staff_Assignment sa on sa.ProjectId = pp.ProjectId
                    join Staff s on sa.StaffId = s.StaffId
                    where pp.ProjectId = @ProjectId;";
        
        
        await using SqlConnection connection = new(_connectionString);
        await using SqlCommand command = new();
        
        command.Connection = connection;
        command.CommandText = query;
        await connection.OpenAsync();
        
        command.Parameters.AddWithValue("@ProjectId", projectId);
        var reader = await command.ExecuteReaderAsync();

        ProjectDto? projects = null;

        while (await reader.ReadAsync())
        {
            if (projects == null)
            {
                projects = new ProjectDto()
                {
                    ProjectId = reader.GetInt32(0),
                    Objective = reader.GetString(1),
                    StartDate = reader.GetDateTime(2),
                    EndDate = await reader.IsDBNullAsync(3) ? null : reader.GetDateTime(3),
                    Artifact = new ArtifactDto()
                    {
                        Name = reader.GetString(4),
                        OriginDate = reader.GetDateTime(5),
                        Institution = new InstitutionDto()
                        {
                            InstitutionId = reader.GetInt32(6),
                            Name = reader.GetString(7),
                            FoundedYear = reader.GetInt32(8),
                        },
                    },
                    StaffAssignments = new List<StaffAssignmentsDto>()
                };
            }

            var Assignments = new StaffAssignmentsDto()
            {
                FirstName = reader.GetString(9),
                LastName = reader.GetString(10),
                HireDate = reader.GetDateTime(11),
                Role = reader.GetString(12),
            };
            
            projects.StaffAssignments.Add(Assignments);
        }

        if (projects is null)
        {
            throw new NotFoundException("No projects found");
        }
        
        return projects;
    }


    public async Task AddProjectAsync(ProjectRequestDto request)
    {
        await using SqlConnection connection = new(_connectionString);
        await using SqlCommand command = new SqlCommand();
        
        command.Connection = connection;
        await connection.OpenAsync();
        
        DbTransaction transaction = connection.BeginTransaction();
        command.Transaction = transaction as SqlTransaction;

        try
        {
            command.Parameters.Clear();
            command.CommandText = "select ArtifactId from Artifact where ArtifactId = @ArtifactId;";
            command.Parameters.AddWithValue("@ArtifactId", request.Artifact.ArtifactId);

            var artifactIdObj = await command.ExecuteScalarAsync();
            if (artifactIdObj is not null)
                throw new ArgumentException("Artifact with this id already already exists");

            command.Parameters.Clear();
            command.CommandText = "select ProjectId from Preservation_Project where ProjectId = @ProjectId;";
            command.Parameters.AddWithValue("@ProjectId", request.Project.ProjectId);

            var projectIdObj = await command.ExecuteScalarAsync();
            if (projectIdObj is not null)
                throw new ArgumentException("Project with this id already exists");

            
            command.Parameters.Clear();
            command.CommandText = @"insert into Preservation_Project
                                    values (@ProjectId, @ArtifactId, @StartDate, @EndDate, @Objective);";
            command.Parameters.AddWithValue("@ProjectId", request.Project.ProjectId);
            command.Parameters.AddWithValue("@ArtifactId", request.Artifact.ArtifactId);
            command.Parameters.AddWithValue("@StartDate", request.Project.StartDate);
            command.Parameters.AddWithValue("@EndDate", DBNull.Value);
            command.Parameters.AddWithValue("@Objective", request.Project.Objective);
            
            
            command.Parameters.Clear();
            command.CommandText = @"insert into Artifact 
                                    values (@ArtifactId, @Name, @OriginDate, @InstitutionId);";
            command.Parameters.AddWithValue("@ArtifactId", request.Artifact.ArtifactId);
            command.Parameters.AddWithValue("@Name", request.Artifact.ArtifactName);
            command.Parameters.AddWithValue("@OriginDate", request.Artifact.OriginDate);
            command.Parameters.AddWithValue("@InstitutionId", request.Artifact.InstitutionId);


            


            await command.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}