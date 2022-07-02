using Jtbuk.ServiceBus.Features.Applications;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;

namespace Jtbuk.IntegrationTests
{
    //https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-6.0
    public class IntegrationTests
    {        
        public async void CreateEndToEnd()
        {
            //Maybe do something like this for MassTransit?
            //https://stackoverflow.com/questions/57541541/configure-masstransit-for-testing-with-webapplicationfactorystartup
            var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder => { });
            var client = factory.CreateClient();

            var roles = new List<RegisterRoleDto>()
            {
                new("Role A", Guid.NewGuid()),
                new("Role B", Guid.NewGuid()),
            };
            var application = new RegisterApplicationDto("Jack App", Guid.NewGuid(), roles);

            await client.PostAsJsonAsync("api/register", application);
            //Register an app
            //Add a tenant
            //Add a user
            //Use entitlement to grant access for user
        }
    }
}