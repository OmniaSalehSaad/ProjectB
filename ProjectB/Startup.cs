using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace ProjectB
{
   public static class Startup
    {
        //for manual instrumentation
        public static readonly ActivitySource MyActivitySource = new ActivitySource("ProjectB.Startup.*");
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
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddHttpClient("ProjectA").ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5119/"));
             builder.Services.AddOpenTelemetryTracing(builder =>
             {
                 builder.AddSource("ProjectBh")
                 .SetSampler(new AlwaysOnSampler())
               
                 .SetResourceBuilder(
                         ResourceBuilder.CreateDefault()
                           .AddService(serviceName: "OpenTelemetry.RampUp.ProjectB.*", serviceVersion: "0.0.2"))
                 .AddAspNetCoreInstrumentation()
                 //############################################################################################################
                 // when enable the line below, the spans have correctly relationship 
                 .AddHttpClientInstrumentation() 
                 //#################################################################################################################
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
                endpoints.MapGet("/", async context =>
                {
                    // Adding tags for debugging & tracing
                    Activity.Current.AddTag("Trace_ID", Activity.Current.TraceId.ToString());
                    Activity.Current.AddTag("Span_ID", Activity.Current.SpanId.ToString());
                    Activity.Current.AddTag("Parent_Span_ID", Activity.Current.ParentSpanId.ToString());

                    // Making call to /test endpoint in ProjectA (can be found in Startup.cs in ProjectA)
                    var client = context.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient("ProjectA");
                    var content = client.GetStringAsync("/test");
                    await context.Response.WriteAsync(await content);
                });
            });
        }
}
}
