using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;

namespace ProjectB
{
    /*
     * To be implemented only in case httpclientinstrumentation is not used
     */
    public class TestMapPropagator : TextMapPropagator
    {
        private const string Version = "00";
        private const string TraceparentHeaderKey = "traceparent";

        public override ISet<string> Fields => new HashSet<string> { "traceparent", "tracestate" };

        public override PropagationContext Extract<T>(PropagationContext context, T carrier, Func<T, string, IEnumerable<string>> getter)
        {
            Console.WriteLine("############# Extract #############");
            return context;
        }

        /*
         * Inject the correct parent span id to the traceparent header only if the current span has a parent
         * Following this sample propagator: 
         * https://github.com/open-telemetry/opentelemetry-js/blob/5d9ed3faa519279f2c68209a8ea9e1c213a4899e/packages/opentelemetry-core/src/trace/W3CTraceContextPropagator.ts#L73
         */
        public override void Inject<T>(PropagationContext context, T carrier, Action<T, string, string> setter)
        {
            if (Activity.Current?.Parent is not null && context.ActivityContext.IsValid())
            {
                string actualParentSpanID = Activity.Current.Parent.SpanId.ToString();
                StringBuilder traceParentBuilder = new StringBuilder(Version);
                traceParentBuilder.Append("-").Append(context.ActivityContext.TraceId).Append("-").Append(actualParentSpanID).Append("-")
                    .Append("0").Append((int)context.ActivityContext.TraceFlags);
                // Change the key to TraceparentHeaderKey when implementing and testing
                setter(carrier, "testHeader", traceParentBuilder.ToString());
                Console.WriteLine("############# INJECT ############# " + traceParentBuilder.ToString());
            }
        }
    }
}
