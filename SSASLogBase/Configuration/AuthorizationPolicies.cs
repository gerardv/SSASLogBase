using Microsoft.AspNetCore.Authorization;

namespace SSASLogBase.Configuration
{
    public static class SkarpRockstarsAuthorizationPolicy
    {
        public static string Name => "Skarp Rockstars";

        public static void Build(AuthorizationPolicyBuilder builder) =>
            builder.RequireClaim("groups", "e2610d0d-a8a4-41a1-a9b8-4fe1dc4b2c34");
    }
}
