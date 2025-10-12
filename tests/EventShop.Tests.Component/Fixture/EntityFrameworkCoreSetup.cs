using EventShop.Web.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Memoria.EventSourcing;
using Memoria.EventSourcing.Store.EntityFrameworkCore;
using Memoria.EventSourcing.Store.EntityFrameworkCore.Identity;

namespace EventShop.Tests.Component.Fixture;

public static class EntityFrameworkCoreSetup
{
    public static IDomainService CreateDomainService(FakeTimeProvider timeProvider, IHttpContextAccessor createHttpContextAccessor)
    {
        var dbContextOptions = CreateContextOptions();
        var dbContext = new ApplicationDbContext(dbContextOptions, timeProvider, createHttpContextAccessor);
        return new EntityFrameworkCoreDomainService(dbContext);
    }

    private static DbContextOptions<IdentityDomainDbContext> CreateContextOptions()
    {
        var builder = new DbContextOptionsBuilder<IdentityDomainDbContext>();
        builder.UseInMemoryDatabase("EventShop");
        return builder.Options;
    }
}
