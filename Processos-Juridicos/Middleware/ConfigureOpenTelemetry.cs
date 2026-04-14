using OpenTelemetry.Metrics;

namespace Processos_Juridicos.Middleware;

public static class OpenTelemetryExtensions
{
    public static void ConfigureOpenTelemetry(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation();
                metrics.AddHttpClientInstrumentation();
                metrics.AddPrometheusExporter();
            });
    }
}
