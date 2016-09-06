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

        public static async Task<IdentityResult> UpdateApplicationUser(UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            var oldUser = await manager.FindByIdAsync(user.Id);
            oldUser.UserName = user.UserName;
            oldUser.Email = user.Email;

            // remove old roles
            var roles = await manager.GetRolesAsync(oldUser.Id);
            await manager.RemoveFromRolesAsync(oldUser.Id, roles.ToArray());

            // add new roles
            roles = await manager.GetRolesAsync(user.Id);
            await manager.AddToRolesAsync(oldUser.Id, roles.ToArray());

            // remove old claims
            var claims = await manager.GetClaimsAsync(oldUser.Id);
            foreach (var claim in claims)
            {
                await manager.RemoveClaimAsync(oldUser.Id, claim);
            }

            // add new claims
            claims = await manager.GetClaimsAsync(user.Id);
            foreach (var claim in claims)
            {
                await manager.AddClaimAsync(oldUser.Id, claim);
            }

            var result = await manager.UpdateAsync(oldUser);

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