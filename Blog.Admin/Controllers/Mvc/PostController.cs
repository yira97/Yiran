using System.Text.Json;
using Blog.Admin.Helper;
using Blog.Admin.Middlewares;
using Blog.Admin.Models;
using Blog.Admin.Services;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace Blog.Admin.Controllers.Mvc
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly BlogService _blogService;
        private readonly CommonLocalizationService _commonLocalizationService;
        private readonly IOptions<RequestLocalizationOptions> _localizationOptions;

        public PostController(ILogger<PostController> logger, BlogService blogService,
            CommonLocalizationService commonLocalizationService,
            IOptions<RequestLocalizationOptions> localizationOptions)
        {
            _logger = logger;
            _blogService = blogService;
            _commonLocalizationService = commonLocalizationService;
            _localizationOptions = localizationOptions;
        }

        public async Task<IActionResult> Index(
            int pageSize = 20,
            string pageToken = "",
            int orderBy = 0,
            bool ascending = false,
            bool publicOnly = false,
            string? categoryId = null,
            string? topicId = null
        )
        {
            var domainId = HttpContext.GetDomainIdFromHttpContextItems();
            if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

            var result = await _blogService.ListPosts(
                pageSize: pageSize,
                pageToken: pageToken,
                orderBy: orderBy,
                ascending: ascending,
                publicOnly: publicOnly,
                categoryId: categoryId,
                topicId: topicId,
                domainId: domainId
            );

            var vm = new PostViewModel();
            vm.QueryResult = result;

            ViewData[ViewHelper.ViewData.ActiveNav] = RouteHelper.Controller.Post;

            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
            if (string.IsNullOrEmpty(accessToken.AccessToken))
                return RedirectToAction("Index", "SignIn", new { Area = "Account" });

            var domainId = HttpContext.GetDomainIdFromHttpContextItems();
            if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");

            var domain = await _blogService.GetDomainAsync(domainId);

            var vm = new PostCreateViewModel();
            // except input data
            vm.DomainId = domainId;
            foreach (var category in domain.Categories)
            {
                vm.DomainCategories.Add(new SelectListItem(category.Name, category.Id));
            }

            foreach (var topic in domain.Topics)
            {
                vm.DomainTopics.Add(new SelectListItem(topic.Name, topic.Id));
            }

            vm.SupportLanguages = _localizationOptions.Value.SupportedCultures!.Select(culture => new SelectListItem(
                text: culture.DisplayName, value: culture.Name));

            // input data
            vm.PostCreateFormData.DomainId = domainId;

            // breadcumbs
            ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
            {
                new NavigationDto(_commonLocalizationService.Get("文章"), Url.Action("Index", "Post", new { domainId })!),
                new NavigationDto(_commonLocalizationService.Get("创建文章"),
                    Url.Action("Create", "Post", new { domainId })!)
            });

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostCreateViewModel vm)
        {
            var input = vm.PostCreateFormData;

            var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
            if (string.IsNullOrEmpty(accessToken.AccessToken))
                return RedirectToAction("Index", "SignIn", new { Area = "Account" });

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var postContent = JsonSerializer.Deserialize<PostContentDto>(input.PostContentJson, options);

            var updateDto = new PostUpdateDto(
                Title: input.Title,
                SubTitle: input.SubTitle,
                Slug: input.Slug,
                Topic: input.Topic,
                Category: input.Category,
                Language: input.Language,
                IsPublic: input.IsPublic,
                DomainId: input.DomainId,
                Content: postContent!
            );
            var post = await _blogService.CreatePost(updateDto, accessToken.AccessToken);

            return RedirectToAction("CreateResult",
                new { DomainId = vm.DomainId, status = "succeed", postId = post.Id, postTitle = post.Title });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
            if (string.IsNullOrEmpty(accessToken.AccessToken))
                return RedirectToAction("Index", "SignIn", new { Area = "Account" });

            var domainId = HttpContext.GetDomainIdFromHttpContextItems();
            if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");
            var domain = await _blogService.GetDomainAsync(domainId);

            var post = await _blogService.GetPost(id, accessToken.AccessToken);

            var vm = new PostEditViewModel();
            // except input model
            vm.Id = id;
            vm.Title = post.Title;
            vm.SubTitle = post.SubTitle;
            vm.Slug = post.Slug;
            vm.Topic = post.Topic;
            vm.Category = post.Category;
            vm.Language = post.Language;
            vm.IsPublic = post.isPublic;
            vm.DomainId = domainId;
            foreach (var category in domain.Categories)
            {
                vm.DomainCategories.Add(new SelectListItem(category.Name, category.Id));
            }

            foreach (var topic in domain.Topics)
            {
                vm.DomainTopics.Add(new SelectListItem(topic.Name, topic.Id));
            }

            vm.SupportLanguages = _localizationOptions.Value.SupportedCultures!.Select(culture => new SelectListItem(
                text: culture.DisplayName, value: culture.Name));

            // input model
            vm.PostEditFormData.Title = post.Title;
            vm.PostEditFormData.SubTitle = post.SubTitle;
            vm.PostEditFormData.Slug = post.Slug;
            vm.PostEditFormData.Topic = post.Topic;
            vm.PostEditFormData.Category = post.Category;
            vm.PostEditFormData.Language = post.Language;
            vm.PostEditFormData.IsPublic = post.isPublic;
            vm.PostEditFormData.DomainId = domainId;

            // breadcumbs
            ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
            {
                new NavigationDto(_commonLocalizationService.Get("文章"), Url.Action("Index", "Post", new { domainId })!),
                new NavigationDto($"{_commonLocalizationService.Get("编辑文章")} ({post.Title})",
                    Url.Action("Edit", "Post", new { domainId, id })!)
            });

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, PostEditViewModel vm)
        {
            var input = vm.PostEditFormData;

            var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
            if (string.IsNullOrEmpty(accessToken.AccessToken))
                return RedirectToAction("Index", "SignIn", new { Area = "Account" });

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var postContent = JsonSerializer.Deserialize<PostContentDto>(input.PostContentJson, options);

            var updateDto = new PostUpdateDto(
                Title: input.Title,
                SubTitle: input.SubTitle,
                Slug: input.Slug,
                Topic: input.Topic,
                Category: input.Category,
                Language: input.Language,
                IsPublic: input.IsPublic,
                Content: postContent!
            );
            var updated = await _blogService.EditPost(id, updateDto, accessToken.AccessToken);

            return RedirectToAction("EditResult",
                new { DomainId = vm.DomainId, status = "succeed", postId = id, postTitle = updated.Title });
        }

        [HttpGet]
        public IActionResult EditResult(string status, string postId, string postTitle)
        {
            var domainId = HttpContext.GetDomainIdFromHttpContextItems();

            var vm = new ActionResultViewModel();
            if (status == "succeed")
            {
                vm.Title = "Complete";
                vm.Message = $"You have updated post ({postTitle}) successfully";
                vm.ResultType = Enums.ActionResultType.Succeed;
                vm.Buttons = new List<ActionButtonInfoDto>()
                {
                    new ActionButtonInfoDto(
                        Name: "Back",
                        Href: Url.Action("Index", new { domainId })!,
                        Type: Enums.ActionButtonType.Common),
                    new ActionButtonInfoDto(
                        Name: "View",
                        Href: Url.Action("Edit", new { domainId, id = postId })!,
                        Type: Enums.ActionButtonType.Primary),
                };
            }
            else
            {
                vm.Title = "Error";
                vm.Message = $"Unknown Error";
                vm.ResultType = Enums.ActionResultType.Error;
                vm.Buttons = new List<ActionButtonInfoDto>()
                {
                    new ActionButtonInfoDto(
                        Name: "Back",
                        Href: Url.Action("Index", new { domainId })!,
                        Type: Enums.ActionButtonType.Common),
                };
            }

            return View("ActionResult", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeletePostDto deletePostDto)
        {
            var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
            if (string.IsNullOrEmpty(accessToken.RefreshToken))
                return RedirectToAction("Index", "SignIn", new { Area = "Account" });

            await _blogService.DeletePost(deletePostDto.PostId,
                accessToken.AccessToken!);

            return RedirectToAction("Index", new { domainId = deletePostDto.DomainId });
        }

        [HttpGet]
        public IActionResult CreateResult(string status, string postId, string postTitle)
        {
            var domainId = HttpContext.GetDomainIdFromHttpContextItems();

            var vm = new ActionResultViewModel();
            if (status == "succeed")
            {
                vm.Title = "Complete";
                vm.Message = $"You have created post ({postTitle}) successfully";
                vm.ResultType = Enums.ActionResultType.Succeed;
                vm.Buttons = new List<ActionButtonInfoDto>()
                {
                    new ActionButtonInfoDto(
                        Name: "Back",
                        Href: Url.Action("Index", new { domainId })!,
                        Type: Enums.ActionButtonType.Common),
                    new ActionButtonInfoDto(
                        Name: "View",
                        Href: Url.Action("Edit", new { domainId, id = postId })!,
                        Type: Enums.ActionButtonType.Primary),
                };
            }
            else
            {
                vm.Title = "Error";
                vm.Message = $"Unknown Error";
                vm.ResultType = Enums.ActionResultType.Error;
                vm.Buttons = new List<ActionButtonInfoDto>()
                {
                    new ActionButtonInfoDto(
                        Name: "Back",
                        Href: Url.Action("Index", new { domainId })!,
                        Type: Enums.ActionButtonType.Common),
                };
            }

            return View("ActionResult", vm);
        }
    }
}