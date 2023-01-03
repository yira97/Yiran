using Blog.Api.Repositories;
using Blog.Api.Services;
using Moq;

namespace Blog.Api.Test.Mocks;

public class MockIUnitOfWork
{
    public static Mock<IUnitOfWork> GetMock()
    {
        var mock = new Mock<IUnitOfWork>();
        var postRepoMock = MockIPostRepository.GetMock();
        var staticResourceRepoMock = MockIStaticResourceRepository.GetMock();

        mock.Setup(m => m.PostRepository).Returns(() => postRepoMock.Object);
        mock.Setup(m => m.StaticResourceRepository).Returns(() => staticResourceRepoMock.Object);
        mock.Setup(m => m.CompleteAsync()).Callback(() => { });

        return mock;
    }
}