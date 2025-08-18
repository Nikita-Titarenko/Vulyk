namespace Vulyk.Common
{
    public static class DefaultRoles
    {
        public static readonly IdentityRole Administrator = new IdentityRole
        {
            Id = DefaultIds.AdministratorRoleId,
            Name = "Administrator",
            NormalizedName = "ADMINISTRATOR"
        };

        public static readonly IdentityRole User = new IdentityRole
        {
            Id = DefaultIds.UserRoleId,
            Name = "User",
            NormalizedName = "USER"
        };
    }
}
