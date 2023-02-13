namespace anigure.Helpers;

public static class RoleHelpers
{
    public static readonly List<string> RoleNames = new()
    {
        Admin,
        User
    };

    public const string Admin = "admin";
    public const string User = "user";
}