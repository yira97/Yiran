using Blog.Api.Controllers;
using Blog.Api.Models;
using Blog.Api.Test.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blog.Api.Test;

public class DomainControllerTest
{
    private static DomainController MakeDomainController()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        var factory = serviceProvider.GetService<ILoggerFactory>()!;
        var logger = factory.CreateLogger<DomainController>();

        var repositoryWrapperMock = MockIUnitOfWork.GetMock();


        var domainController = new DomainController(logger, repositoryWrapperMock.Object);

        return domainController;
    }

    [Fact]
    public async Task WhenListDomain_ThenAllDomainReturn()
    {
        var domainController = MakeDomainController();

        var result = await domainController.List();

        var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);

        Assert.Equal(okResult.StatusCode, StatusCodes.Status200OK);

        var ds = Assert.IsAssignableFrom<IEnumerable<DomainDto>>(okResult.Value).ToList();
        Assert.Equal(ds.Count, MockIPostRepository.Domains.Count);
    }

    [Fact]
    public async Task WhenGetExistDomain_DomainReturn()
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        var factory = serviceProvider.GetService<ILoggerFactory>()!;
        var logger = factory.CreateLogger<DomainController>();

        var repositoryWrapperMock = MockIUnitOfWork.GetMock();


        var domainController = new DomainController(logger, repositoryWrapperMock.Object);

        var random = new Random();

        var id = MockIPostRepository.Domains[random.Next(MockIPostRepository.Domains.Count)].Id;

        var result = await domainController.Get(id);

        var okResult = Assert.IsAssignableFrom<OkObjectResult>(result.Result);

        Assert.Equal(okResult.StatusCode, StatusCodes.Status200OK);

        var d = Assert.IsAssignableFrom<DomainDto>(okResult.Value);
        Assert.Equal(d.Id, id);
        Assert.NotEmpty(d.Name);
    }

    [Fact]
    public async Task WhenGetNotExistDomain_Return404()
    {
        var domainController = MakeDomainController();

        var result = await domainController.Get(Guid.NewGuid().ToString());

        Assert.IsAssignableFrom<NotFoundResult>(result.Result);
    }
}