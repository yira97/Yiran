using Blog.Admin.Helper;
using Blog.Admin.Models;
using Blog.Domain.Enums;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Evrane.Core.ObjectStorage;
using Evrane.Core.ObjectStorage.S3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly BlogService _blogService;
        private readonly S3HttpClient _s3HttpClient;

        public StorageController(ILogger<StorageController> logger, BlogService blogService, S3HttpClient s3HttpClient)
        {
            _blogService = blogService;
            _s3HttpClient = s3HttpClient;
        }

        [HttpGet("{resourceId}", Name = "GetTempGetInfo")]
        public async Task<ActionResult<GetInfo>> GetTempGetInfo(string resourceId)
        {
            var (tokenUpdated, accessTokenDto) =
                await _blogService.EnsureAccessToken(CookieHelper.GetAccessTokenFromCookie(HttpContext));
            if (tokenUpdated)
            {
                CookieHelper.WriteAccessTokenToCookie(HttpContext, accessTokenDto);
            }

            var info = await _blogService.GetTempGetInfo(resourceId, accessTokenDto.AccessToken);

            return info;
        }


        [HttpPost("upload/post-cover", Name = "UploadPostCover")]
        public async Task<ActionResult> UploadPostCover(IFormFile file)
        {
            var (tokenUpdated, accessTokenDto) =
                await _blogService.EnsureAccessToken(CookieHelper.GetAccessTokenFromCookie(HttpContext));
            if (tokenUpdated)
            {
                CookieHelper.WriteAccessTokenToCookie(HttpContext, accessTokenDto);
            }

            var info = await _blogService.GetPutInfo(new StaticResourceUpdateDto(
                    (int)StaticResourceAction.ADD_POST_COVER,
                    file.Length,
                    file.FileName
                )
                , accessTokenDto.AccessToken);

            await _s3HttpClient.PutSignedUrl(info.Url, file.OpenReadStream(), file.Length, file.ContentType);

            var tempInfo = await _blogService.GetTempGetInfo(info.ResourceId, accessTokenDto.AccessToken);


            return Ok(tempInfo);
        }


        [HttpPost("upload/post-content-image", Name = "UploadPostContentImage")]
        public async Task<ActionResult<PutInfo>> UploadPostContentImage(IFormFile file)
        {
            var (tokenUpdated, accessTokenDto) =
                await _blogService.EnsureAccessToken(CookieHelper.GetAccessTokenFromCookie(HttpContext));
            if (tokenUpdated)
            {
                CookieHelper.WriteAccessTokenToCookie(HttpContext, accessTokenDto);
            }

            var info = await _blogService.GetPutInfo(new StaticResourceUpdateDto(
                    (int)StaticResourceAction.ADD_POST_CONTENT_IMAGE,
                    file.Length,
                    file.FileName
                )
                , accessTokenDto.AccessToken);

            await _s3HttpClient.PutSignedUrl(info.Url, file.OpenReadStream(), file.Length, file.ContentType);

            var tempInfo = await _blogService.GetTempGetInfo(info.ResourceId, accessTokenDto.AccessToken);

            return Ok(tempInfo);
        }
    }
}