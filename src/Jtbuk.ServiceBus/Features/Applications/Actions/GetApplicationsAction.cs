using Jtbuk.ServiceBus.Data;
using Microsoft.EntityFrameworkCore;

namespace Jtbuk.ServiceBus.Features.Applications.Actions;

public record GetApplicationDto(string Name, Guid UniqueId, IEnumerable<GetApplicationRoleDto> Roles);
public record GetApplicationRoleDto(string Name, Guid UniqueId);
public class GetApplicationsAction
{
    public static async Task<List<GetApplicationDto>> Invoke(DatabaseContext context)
    {
        var dtos = await context.Applications
            .Select(a => new GetApplicationDto(
                a.Name,
                a.UniqueId,
                a.Roles.Select(r => new GetApplicationRoleDto(r.Name, r.UniqueId))
            ))
            .ToListAsync();

        return dtos;
    }
}
