namespace Vulyk.Web.ViewModels.UserManagement
{
    public class UsersViewModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public const int PageSize = 30;
    }
}