using AutoMapper;
using Vulyk.Application.Services.User;
using Vulyk.Web.Common;
using Vulyk.Web.ViewModels.UserManagement;

namespace Vulyk.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RoleNames.Administrator)]
    public class UserController : Controller
    {

        private readonly IUserService _userService;

        private readonly IMapper _mapper;

        public UserController(IMapper mapper, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["ChoosedPage"] = "AdminPanel";

            var getUsersResult = await _userService.GetUsers(1, UsersViewModel.PageSize);
            if (!getUsersResult.IsSuccess)
            {
                foreach (var error in getUsersResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Message);
                }
            }

            return View(_mapper.Map<UsersViewModel>(getUsersResult.Value));
        }

        public async Task<IActionResult> LoadUsers(int page)
        {
            return Json(await _userService.GetUsers(page, UsersViewModel.PageSize));
        }
    }
}
