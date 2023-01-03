using Blog.Api.Entities;
using Blog.Api.Enums;
using Blog.Api.Models;
using Blog.Api.Repositories;
using Moq;

namespace Blog.Api.Test.Mocks;

public class MockIStaticResourceRepository
{
    public static List<StaticResourceEntity> StaticResources => new List<StaticResourceEntity>()
    {
        new StaticResourceEntity
        {
            Id = "static_resource_id_1",
            Key = "static_resource_key_1_cover",
            CreatedById = "user_1",
            CreatedAt = DateTime.Parse("01/12/2018 01:01:01"),
            UpdatedById = "user_1",
            UpdatedAt = DateTime.Parse("08/18/2018 01:01:01"),
            Action = (int)StaticResourceAction.ADD_POST_COVER,
            Category = (int)StaticResourceCategory.POST_COVER,
            ReferenceId = "post_id_1",
            OriginalFileName = "file_1.jpg",
            FileSize = 1024,
        },
        new StaticResourceEntity
        {
            Id = "static_resource_id_2",
            Key = "static_resource_key_2_block_image",
            CreatedById = "user_1",
            CreatedAt = DateTime.Parse("01/12/2018 01:01:01"),
            UpdatedById = "user_1",
            UpdatedAt = DateTime.Parse("08/18/2018 01:01:01"),
            Action = (int)StaticResourceAction.ADD_POST_CONTENT_IMAGE,
            Category = (int)StaticResourceCategory.POST_CONTENT_IMAGE,
            ReferenceId = "post_id_1",
            OriginalFileName = "file_2.jpg",
            FileSize = 1024,
        },
        new StaticResourceEntity
        {
            Id = "static_resource_id_3",
            Key = "static_resource_key_3",
            CreatedById = "user_1",
            CreatedAt = DateTime.Parse("01/12/2018 01:01:01"),
            UpdatedById = "user_1",
            UpdatedAt = DateTime.Parse("08/18/2018 01:01:01"),
            Action = (int)StaticResourceAction.ADD_POST_CONTENT_IMAGE,
            Category = (int)StaticResourceCategory.POST_CONTENT_IMAGE,
            ReferenceId = "post_id_1",
            OriginalFileName = "file_3.jpg",
            FileSize = 1024,
        },
        new StaticResourceEntity
        {
            Id = "static_resource_id_4",
            Key = "static_resource_key_4",
            CreatedById = "user_1",
            CreatedAt = DateTime.Parse("01/12/2018 01:01:01"),
            UpdatedById = "user_1",
            UpdatedAt = DateTime.Parse("08/18/2018 01:01:01"),
            Action = (int)StaticResourceAction.ADD_POST_CONTENT_IMAGE,
            Category = (int)StaticResourceCategory.POST_CONTENT_IMAGE,
            ReferenceId = "post_id_1",
            OriginalFileName = "file_4.jpg",
            FileSize = 1024,
        }
    };

    public static StaticResourceUpdateDto RandomUpdateDto()
    {
        var random = new Random();
        var actions = new List<StaticResourceAction>()
        {
            StaticResourceAction.ADD_POST_COVER,
            StaticResourceAction.ADD_POST_CONTENT_IMAGE
        };
        var action = (int)actions[random.Next(actions.Count)];

        return new StaticResourceUpdateDto(
            action,
            100,
            ""
        );
    }

    public static Mock<IStaticResourceRepository> GetMock()
    {
        var mock = new Mock<IStaticResourceRepository>();

        var newCreated = new StaticResourceEntity
        {
            Id = "static_resource_id_temp",
            Key = "static_resource_key_temp",
            CreatedById = "user_1",
            CreatedAt = DateTime.Parse("01/12/2018 01:01:01"),
            UpdatedById = "user_1",
            UpdatedAt = DateTime.Parse("08/18/2018 01:01:01"),
            Action = (int)StaticResourceAction.ADD_POST_COVER,
            Category = (int)StaticResourceCategory.POST_COVER,
            ReferenceId = "post_id_1",
            OriginalFileName = "file_temp.jpg",
            FileSize = 1024,
        };

        mock.Setup(m => m.Create(It.IsAny<StaticResourceUpdateDto>(), It.IsAny<string>())).Returns(
            () => { return newCreated.StaticResourceArchiveDto(); });

        mock.Setup(m => m.Get(It.IsAny<string>())).Returns((string id) =>
        {
            var result = StaticResources.Find(r => r.Id == id)!.StaticResourceArchiveDto();
            return Task.FromResult(result);
        });


        return mock;
    }
}