

using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace ProjectA
{
    public static class Startup
    {

        public static readonly ActivitySource MyActivitySource = new("ProjectA.Startup.*");

        public static WebApplication InitializeApp(String[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder);
            var app = builder.Build();
            Configure(app);
            return app;
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.Hierarchical;

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddOpenTelemetryTracing(builder =>
            {
                builder.AddSource("ProjectA.Startup.*")
                  .SetResourceBuilder(
                        ResourceBuilder.CreateDefault()
                          .AddService(serviceName: "OpenTelemetry.RampUp.ProjectA.*", serviceVersion: "0.0.1"))
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddJaegerExporter()
                .AddConsoleExporter();
            });
        }

        private static void Configure(WebApplication app)
        {

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.UseEndpoints(endpoints =>
            {
             
                endpoints.MapGet("/test", async context =>
                {

                    Activity.Current.AddTag("Trace_ID", Activity.Current.TraceId.ToString());

                    Activity.Current.AddTag("Span_ID", Activity.Current.SpanId.ToString());
                    Activity.Current.AddTag("Parent_Span_ID", Activity.Current.ParentSpanId.ToString());
                    await context.Response.WriteAsync("ProjectA TEST");
                });
            });
        }


    }
}
