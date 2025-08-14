namespace Vulyk.ViewModels
{
    public class UsersViewModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public const int PageSize = 30;
    }
}