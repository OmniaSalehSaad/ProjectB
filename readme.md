# Introduction
this repo created to show bug in auto instrumntation in .NET open telemetry.
we have two project A and B. the Both project use open telemtery tracing. project B use ASPNet core instrumentation. projectB call projectA . Project A enable ASPNet core and http client instrumentation.
The issue appear when projectB call project A, projectA span has invalid span parent id as in following image:
![enter image description here](https://github.com/OmniaSalehSaad/ProjectB/blob/master/Images/Screenshot%202022-08-24%20141620.png)
in the above image span of projectA should be child of span of projectB but the relationship doesn't appear correctly.
But when ProjectB using ASPNet core and http client instrumentation. the relationship  between spans appeared correctly.

![enter image description here](https://github.com/OmniaSalehSaad/ProjectB/blob/master/Images/Screenshot%202022-08-24%20142453.png)

there is a new span  HTTP GET appeared which created from HttpClient instrumentation.


# Running

Visual Studio
1-clone the repo using: https://github.com/OmniaSalehSaad/ProjectB.git
2-install the required packages. click right on project solution, choose manage NuGet packages then check 'include prerelease'. install the following packages if they are not installed:
 - OpenTelemetry.Exporter.Console 
 - OpenTelemetry.Extensions.Hosting
 - OpenTelemetry.Exporter.Jaeger
 - OpenTelemetry.Instrumentation.AspNet
 - OpenTelemetry.Instrumentation.Http
3-choose run projectA and projectB 
![enter image description here](https://github.com/OmniaSalehSaad/ProjectB/upload)

4- download Jaeger [Jaeger – Download Jaeger (jaegertracing.io)](https://www.jaegertracing.io/download/)  and then run jager using following command
> ./jaeger-all-in-one

you can open jaeger using http://localhost:16686/. 
5-start running, projectA will be on https://localhost:7215/ and projectB will be on http://localhost:5119


