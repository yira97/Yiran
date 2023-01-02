using Blog.Api.Entities.Nested;
using Blog.Core.ObjectStorage;

namespace Blog.Api.Models;

public static class PostDtoExtensions
{
    public delegate Task<ImageGetInfoDto> ImageGetInfoSource(string resourceId);

    public static async Task<PostDto> WithResourceGetInfo(this PostDto src, ImageGetInfoSource imageGetInfoSource)
    {
        var contentCover = src.Content.Cover;

        if (contentCover != null)
        {
            contentCover = contentCover with
            {
                GetInfo = await imageGetInfoSource(contentCover.ResourceId)
            };
        }

        var contentBlocks = src.Content.Blocks.ToList();

        for (var i = 0; i < contentBlocks.Count; i++)
        {
            var block = contentBlocks[i];

            var blockImage = block.Image;
            if (blockImage != null)
            {
                blockImage = blockImage with
                {
                    GetInfo = await imageGetInfoSource(blockImage.ResourceId)
                };

                contentBlocks[i] = contentBlocks[i] with
                {
                    Image = blockImage
                };
                continue;
            }

            var blockImages = block.Images?.ToList();
            if (blockImages != null)
            {
                for (int j = 0; i < blockImages.Count; j++)
                {
                    blockImages[j] = blockImages[j] with
                    {
                        GetInfo = await imageGetInfoSource(blockImages[j].ResourceId)
                    };
                }

                contentBlocks[i] = contentBlocks[i] with
                {
                    Images = blockImages
                };
                continue;
            }
        }

        var post = src with
        {
            Content = new PostContentDto(contentCover, contentBlocks)
        };

        return post;
    }
}