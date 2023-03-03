using Elsa.Common.Features;
using Elsa.Extensions;
using Elsa.Features.Abstractions;
using Elsa.Features.Attributes;
using Elsa.Features.Services;
using Elsa.Http.ContentWriters;
using Elsa.Http.Handlers;
using Elsa.Http.Implementations;
using Elsa.Http.Options;
using Elsa.Http.Parsers;
using Elsa.Http.Providers;
using Elsa.Http.Services;
using Elsa.JavaScript.Features;
using Elsa.Liquid.Features;
using Elsa.Workflows.Management.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Elsa.Http.Features;

/// <summary>
/// Installs services related to HTTP services and activities.
/// </summary>
[DependsOn(typeof(MemoryCacheFeature))]
[DependsOn(typeof(LiquidFeature))]
[DependsOn(typeof(JavaScriptFeature))]
public class HttpFeature : FeatureBase
{
    /// <inheritdoc />
    public HttpFeature(IModule module) : base(module)
    {
    }

    /// <summary>
    /// A delegate to configure <see cref="HttpActivityOptions"/>.
    /// </summary>
    public Action<HttpActivityOptions>? ConfigureHttpOptions { get; set; }

    /// <summary>
    /// A delegate that is invoked when authorizing an inbound HTTP request.
    /// </summary>
    public Func<IServiceProvider, IHttpEndpointAuthorizationHandler> HttpEndpointAuthorizationHandler { get; set; } = ActivatorUtilities.GetServiceOrCreateInstance<AllowAnonymousHttpEndpointAuthorizationHandler>;
    
    /// <summary>
    /// A delegate that is invoked when an HTTP workflow faults. 
    /// </summary>
    public Func<IServiceProvider, IHttpEndpointWorkflowFaultHandler> HttpEndpointWorkflowFaultHandler { get; set; } = ActivatorUtilities.GetServiceOrCreateInstance<DefaultHttpEndpointWorkflowFaultHandler>;

    /// <summary>
    /// A delegate to configure the <see cref="HttpClient"/> used when by the <see cref="SendHttpRequest"/> activity.
    /// </summary>
    public Action<IServiceProvider, HttpClient> HttpClient { get; set; } = (_, _) => { };

    /// <summary>
    /// A delegate to configure the <see cref="HttpClientBuilder"/> for <see cref="HttpClient"/>.
    /// </summary>
    public Action<IHttpClientBuilder> HttpClientBuilder { get; set; } = _ => { };

    /// <inheritdoc />
    public override void Configure()
    {
        Module.UseWorkflowManagement(management =>
        {
            management.AddVariableTypes(new[]
            {
                typeof(RouteData),
                typeof(HttpRequest),
                typeof(HttpResponse)
            }, "HTTP");
            
            management.AddActivitiesFrom<HttpFeature>();
        });
    }

    /// <inheritdoc />
    public override void Apply()
    {
        var configureOptions = ConfigureHttpOptions ?? (options =>
        {
            options.BasePath = "/workflows";
            options.BaseUrl = new Uri("http://localhost");
        });

        Services.Configure(configureOptions);

        var httpClientBuilder = Services.AddHttpClient<SendHttpRequest>(HttpClient);
        HttpClientBuilder(httpClientBuilder);

        Services
            .AddSingleton<IRouteMatcher, RouteMatcher>()
            .AddSingleton<IRouteTable, RouteTable>()
            .AddSingleton<IAbsoluteUrlProvider, DefaultAbsoluteUrlProvider>()
            .AddSingleton<IHttpBookmarkProcessor, HttpBookmarkProcessor>()
            .AddNotificationHandlersFrom<UpdateRouteTable>()
            .AddHttpContextAccessor()

            // Content parsers.
            .AddSingleton<IHttpContentParser, StringHttpContentParser>()
            .AddSingleton<IHttpContentParser, JsonHttpContentParser>()
            .AddSingleton<IHttpContentParser, XmlHttpContentParser>()

            // HTTP content factories.
            .AddSingleton<IHttpContentFactory, TextContentFactory>()
            .AddSingleton<IHttpContentFactory, JsonContentFactory>()
            .AddSingleton<IHttpContentFactory, XmlContentFactory>()
            .AddSingleton<IHttpContentFactory, FormUrlEncodedHttpContentFactory>()

            // Activity property options providers.
            .AddSingleton<IActivityPropertyOptionsProvider, WriteHttpResponseContentTypeOptionsProvider>()

            // Add Http endpoint handlers
            .AddSingleton(HttpEndpointWorkflowFaultHandler)
            ;
    }
}