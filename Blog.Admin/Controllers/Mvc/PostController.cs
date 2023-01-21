using Blog.Admin.Helper;
using Blog.Admin.Middlewares;
using Blog.Admin.Models;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;

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


        public IActionResult Create()
        {
            var vm = new CreatePostViewModel();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel vm)
        {
            var updateDto = new PostUpdateDto(Title: vm.Title, SubTitle: vm.SubTitle, Slug: vm.Slug, Topic: vm.Topic,
                Category: vm.Category, Language: vm.Language, IsPublic: vm.IsPublic, DomainId: vm.DomainId,
                Content: new PostContentDto(Cover: vm.Cover, Blocks: vm.Blocks));
            var post = await _blogService.CreatePost(updateDto);

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
    }
}