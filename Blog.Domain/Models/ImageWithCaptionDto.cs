using Evrane.Core.ObjectStorage;

namespace Blog.Domain.Models;

public record ImageWithCaptionDto(string ResourceId, string Caption, ImageGetInfoDto? GetInfo = null);