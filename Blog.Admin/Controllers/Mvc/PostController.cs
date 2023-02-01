using System.Text.Json;
using Blog.Admin.Helper;
using Blog.Admin.Middlewares;
using Blog.Admin.Models;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.Admin.Controllers.Mvc
{
    public class PostController : Controller
    {
        private readonly ILogger<PostController> _logger;
        private readonly BlogService _blogService;

        public PostController(ILogger<PostController> logger, BlogService blogService)
        {
            _logger = logger;
            _blogService = blogService;
        }


        public async Task<IActionResult> Create()
        {
            var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
            if (string.IsNullOrEmpty(accessToken.AccessToken)) return RedirectToAction("Register", "Account");

            var domainId = HttpContext.GetDomainIdFromHttpContextItems();
            if (string.IsNullOrEmpty(domainId)) return RedirectToAction("Index", "Domain");
            var domain = await _blogService.GetDomainAsync(domainId);

            var vm = new CreatePostViewModel();
            vm.DomainId = domainId;
            foreach (var category in domain.Categories)
            {
                vm.DomainCategories.Add(new SelectListItem(category.Name, category.Id));
            }

            foreach (var topic in domain.Topics)
            {
                vm.DomainTopics.Add(new SelectListItem(topic.Name, topic.Id));
            }

            ViewData["Levels"] = new BreadcrumbsDto(Links: new[]
            {
                new NavigationDto("Post", Url.Action("Index", "Post", new { domainId })!, false),
                new NavigationDto($"Create Post", Url.Action("Create", "Post", new { domainId })!, true)
            });

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel vm)
        {
            var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
            if (string.IsNullOrEmpty(accessToken.AccessToken)) return RedirectToAction("Login", "Account");
            var postContent = JsonSerializer.Deserialize<PostContentDto>(vm.PostContentJson);
            var updateDto = new PostUpdateDto(Title: vm.Title, SubTitle: vm.SubTitle, Slug: vm.Slug, Topic: vm.Topic,
                Category: vm.Category, Language: vm.Language, IsPublic: vm.IsPublic, DomainId: vm.DomainId,
                Content: postContent!);
            var post = await _blogService.CreatePost(updateDto, accessToken.AccessToken);

            return RedirectToAction("Index", new { id = post.Id });
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
            var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
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

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditPostViewModel vm)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Delete(DeletePostDto deletePostDto)
        {
            var accessToken = HttpContext.GetAccessTokenInfoFromHttpContextItems();
            if (string.IsNullOrEmpty(accessToken.RefreshToken)) return RedirectToAction("Register", "Account");

            await _blogService.DeletePost(deletePostDto.PostId,
                accessToken.AccessToken!);

            return RedirectToAction("Index", new { domainId = deletePostDto.DomainId });
        }
    }
}