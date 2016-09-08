using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;

namespace MVCTraining.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            var yearClaim = Claims.FirstOrDefault(c => c.ClaimType == "Year");
            if (yearClaim != null)
                userIdentity.AddClaim(new Claim(yearClaim.ClaimType, yearClaim.ClaimValue));

            return userIdentity;
        }

        public static async Task<IdentityResult> UpdateApplicationUser(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> rolesManager, 
            ManageUsersViewModel user)
        {
            var oldUser = await userManager.FindByIdAsync(user.Id);
            oldUser.UserName = user.Name;
            oldUser.Email = user.Email;

            // remove old roles
            var roles = await userManager.GetRolesAsync(oldUser.Id);
            await userManager.RemoveFromRolesAsync(oldUser.Id, roles.ToArray());

            // add new roles
            foreach (var role in user.Roles)
            {
                var currentRole = await rolesManager.FindByNameAsync(role);
                await userManager.AddToRolesAsync(oldUser.Id, currentRole.Name);
            }

            // remove old claims
            var claims = await userManager.GetClaimsAsync(oldUser.Id);
            foreach (var claim in claims)
            {
                await userManager.RemoveClaimAsync(oldUser.Id, claim);
            }

            // add new claims
            foreach (var claim in user.Claims)
            {
                oldUser.Claims.Add(claim);
            }

            var result = await userManager.UpdateAsync(oldUser);

            return result;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}