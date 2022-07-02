using Jtbuk.ServiceBus.Features.Applications;

namespace Jtbuk.ServiceBus.Features.Users;


public static class SetupUsersFeature
{
    public static void AddUsersFeature(this WebApplication app)
    {
        //Add a user
        app.MapPost("api/users", CreateUserAction.Invoke);
        //Update a user
        //Get a user
        //Get a list of users
        //Delete a user
    }
}