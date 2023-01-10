using Blog.Admin.Models;
using Blog.Domain.Models;
using Blog.Domain.Services.Client;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Admin.Controllers
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

        public IActionResult Index(string id)
        {
            return View();
        }
    }
}