using DapperSqlLearning.Api.Data;

namespace DapperSqlLearning.Api.Extensions;

public static class ApplicationDatabaseExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        const int maxAttempts = 20;
        var delay = TimeSpan.FromSeconds(3);

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var repository = scope.ServiceProvider.GetRequiredService<IProductRepository>();
                await repository.InitializeAsync(CancellationToken.None);
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                app.Logger.LogWarning(
                    ex,
                    "Database is not ready yet (attempt {Attempt}/{MaxAttempts}). Retrying in {DelaySeconds}s.",
                    attempt,
                    maxAttempts,
                    delay.TotalSeconds);

                await Task.Delay(delay);
            }
        }
    }
}
