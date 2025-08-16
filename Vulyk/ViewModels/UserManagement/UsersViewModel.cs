namespace Vulyk.ViewModels.User
{
    public class UsersViewModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public const int PageSize = 30;
    }
}