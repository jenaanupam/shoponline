using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace Infrastructure.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUser = new ApplicationUser { UserName = "admin@synt.com", Email = "admin@synt.com" };
            await userManager.CreateAsync(defaultUser, "Pass@word1");
        }
    }
}
