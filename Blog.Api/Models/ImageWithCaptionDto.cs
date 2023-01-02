using Blog.Core.ObjectStorage;

namespace Blog.Api.Models;

public record ImageWithCaptionDto(string ResourceId, string Caption, ImageGetInfoDto? GetInfo = null);