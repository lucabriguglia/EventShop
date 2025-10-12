using System.Diagnostics;
using EventShop.Tests.Component.Fixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Time.Testing;
using Memoria;
using Memoria.EventSourcing;
using Xunit;

namespace EventShop.Tests.Component;

public abstract class ComponentTestBase : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    protected readonly IDispatcher Dispatcher;
    protected IDomainService DomainService = null!;
    protected FakeTimeProvider TimeProvider = null!;
    private ActivitySource _activitySource = null!;
    private ActivityListener _activityListener = null!;

    protected ComponentTestBase(WebApplicationFactory<Program> factory)
    {
        var webApplicationFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                TimeProvider = new FakeTimeProvider();
                var httpContextAccessor = HttpContextSetup.CreateHttpContextAccessor();

                DomainService = EntityFrameworkCoreSetup.CreateDomainService(TimeProvider, httpContextAccessor);
                services.Replace(ServiceDescriptor.Scoped<IDomainService>(_ => DomainService));
            });
        });

        SetupActivity();

        var serviceProvider = webApplicationFactory.Services;
        var scope = serviceProvider.CreateScope();
        Dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();
    }

    private void SetupActivity()
    {
        _activitySource = new ActivitySource("TestSource");

        _activityListener = new ActivityListener();
        _activityListener.ShouldListenTo = _ => true;
        _activityListener.Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData;
        _activityListener.SampleUsingParentId = (ref ActivityCreationOptions<string> _) => ActivitySamplingResult.AllData;
        _activityListener.ActivityStarted = _ => { };
        _activityListener.ActivityStopped = _ => { };

        ActivitySource.AddActivityListener(_activityListener);

        _activitySource.StartActivity();
    }

    public void Dispose()
    {
        _activityListener.Dispose();
        _activitySource.Dispose();
        GC.SuppressFinalize(this);
    }
}
