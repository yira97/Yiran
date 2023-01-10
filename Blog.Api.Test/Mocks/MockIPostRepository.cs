using Blog.Api.Entities;
using Blog.Api.Entities.Nested;
using Blog.Api.Repositories;
using Moq;

namespace Blog.Api.Test.Mocks;

public class MockIPostRepository
{
    public static List<DomainEntity> Domains => new List<DomainEntity>()
    {
        new DomainEntity()
        {
            Id = "domain_id_1",
            Name = "domain_name_1",
            CreatedById = "user_1",
            CreatedAt = DateTime.Parse("01/12/2018 01:01:01"),
            UpdatedById = "user_1",
            UpdatedAt = DateTime.Parse("08/18/2018 01:01:01"),
        },
        new DomainEntity()
        {
            Id = "domain_id_2",
            Name = "domain_name_2",
            CreatedById = "user_1",
            CreatedAt = DateTime.Parse("01/12/2018 01:01:01"),
            UpdatedById = "user_1",
            UpdatedAt = DateTime.Parse("08/18/2018 01:01:01"),
        }
    };

    public static List<PostEntity> Posts => new List<PostEntity>()
    {
        new PostEntity()
        {
            Id = "post_id_1",
            Title = "post_title_1",
            SubTitle = "post_subtitle_1",
            Slug = "post_slug_1",
            Topic = "topic_1",
            Category = "category_1",
            CreatedById = "user_1",
            CreatedAt = DateTime.Parse("01/12/2018 01:01:01"),
            UpdatedById = "user_1",
            UpdatedAt = DateTime.Parse("08/18/2018 01:01:01"),
            Language = "zh",
            isPublic = true,
            DomainId = "domain_1",
            Content = new PostContent()
            {
                Cover = new ImageWithCaption()
                {
                    ResourceId = "static_resource_id_1",
                    Caption = "caption_cover"
                },
                Blocks = new List<PostContentBlock>
                {
                    new PostContentBlock()
                    {
                        Paragraph = "paragraph_1"
                    },
                    new PostContentBlock()
                    {
                        Image = new ImageWithCaption
                        {
                            ResourceId = "static_resource_id_2",
                            Caption = "caption_image"
                        }
                    },
                    new PostContentBlock()
                    {
                        Images = new List<ImageWithCaption>()
                        {
                            new ImageWithCaption()
                            {
                                ResourceId = "static_resource_id_3",
                                Caption = "caption_images_1"
                            },
                            new ImageWithCaption()
                            {
                                ResourceId = "static_resource_id_4",
                                Caption = "caption_images_2"
                            },
                        }
                    },
                }
            }
        }
    };

    public static Mock<IPostRepository> GetMock()
    {
        var mock = new Mock<IPostRepository>();

        mock.Setup(m => m.ListAllDomains()).Returns(() =>
        {
            return Task.FromResult(Domains.Select(d => d.DomainDto()));
        });

        mock.Setup(m => m.GetDomainInfo(It.IsAny<string>())).Returns((string id) =>
        {
            return Task.FromResult(Domains.Find(d => d.Id == id)?.DomainDto());
        });

        return mock;
    }
}