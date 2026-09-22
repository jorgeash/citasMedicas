using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Kernel.Repositories;
using Xunit;

namespace Shared.Kernel.Tests;

public class RepositoryTests
{
    private class TestEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
        public DbSet<TestEntity> TestEntities { get; set; } = null!;
    }

    [Fact]
    public async Task AddRangeAsync_Should_AddEntities()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(databaseName: "AddRange_Should_Add").Options;
        using var context = new TestDbContext(options);
        var repo = new Repository<TestEntity>(context);

        var items = new List<TestEntity>
        {
            new TestEntity { Name = "A" },
            new TestEntity { Name = "B" },
            new TestEntity { Name = "C" }
        };

        await repo.AddRangeAsync(items);

        var count = await context.TestEntities.CountAsync();
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task GetOneByAsync_Should_ReturnEntity_When_FilterMatches()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(databaseName: "GetOneBy_Should_Return").Options;
        using var context = new TestDbContext(options);
        var repo = new Repository<TestEntity>(context);

        var items = new List<TestEntity>
        {
            new TestEntity { Name = "A" },
            new TestEntity { Name = "Target" },
            new TestEntity { Name = "C" }
        };

        await repo.AddRangeAsync(items);

        var found = await repo.GetOneByAsync(x => x.Name == "Target");
        Assert.NotNull(found);
        Assert.Equal("Target", found!.Name);
    }

    [Fact]
    public async Task GetPagedAsync_OrderByProperty_Should_OrderByName()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(databaseName: "GetPaged_OrderBy").Options;
        using var context = new TestDbContext(options);
        var repo = new Repository<TestEntity>(context);

        var items = new List<TestEntity>
        {
            new TestEntity { Name = "Charlie" },
            new TestEntity { Name = "Alpha" },
            new TestEntity { Name = "Bravo" }
        };

        await repo.AddRangeAsync(items);

        var paged = await repo.GetPagedAsync(1, 10, null, null, orderByProperty: "Name", orderByDescending: false);

        Assert.Equal(3, paged.TotalRecords);
        Assert.Equal("Alpha", paged.Data.First().Name);
    }
}
