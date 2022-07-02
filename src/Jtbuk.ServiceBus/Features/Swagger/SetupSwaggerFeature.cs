namespace Jtbuk.ServiceBus.Features.Swagger;

public static class SetupSwaggerFeature
{
    public static void UseSwaggerFeature(this IServiceCollection services)
    {
        services.AddSwaggerGen();
    }

    public static void AddSwaggerFeature(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
    }
}