namespace Vulyk.Common
{
    public static class DefaultUsers
    {
        public static readonly ApplicationUser AdministratorUser = new ApplicationUser
        {
            EmailConfirmed = true,
            UserName = "vulyk.messenger@gmail.com",
            NormalizedUserName = "VULYK.MESSENGER@GMAIL.COM",
            Email = "vulyk.messenger@gmail.com",
            NormalizedEmail = "VULYK.MESSENGER@GMAIL.COM",
            FullName = "Mykyta Titarenko",
            Id = DefaultIds.AdministratorUserId,
            PhoneNumber = "+380953589545"
        };
    }
}
