using System;
using Microsoft.Owin;
using Owin;
using MVCTraining.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

[assembly: OwinStartupAttribute(typeof(MVCTraining.Startup))]
namespace MVCTraining
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // Create Admin Role and creating a default Admin User     
            if (!roleManager.RoleExists("Admin"))
            {
                CreateAdminRole(roleManager);
            }

            var user = userManager.FindByName("admin@gmail.com");

            if (user == null)
            {
                var result = CreateAdminUser(userManager, out user);
                // Assign admin user to Admin Role
                if (result.Succeeded)
                {
                    userManager.AddToRole(user.Id, "Admin");
                }
            }

            AddDefaultRoles(roleManager);
        }

        private void AddDefaultRoles(RoleManager<IdentityRole> roleManager)
        {
            // creating Creating Manager role     
            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole();
                role.Name = "User";
                roleManager.Create(role);
            }

            // creating Creating Employee role     
            if (!roleManager.RoleExists("Agent"))
            {
                var role = new IdentityRole();
                role.Name = "Agent";
                roleManager.Create(role);
            }
        }

        private void CreateAdminRole(RoleManager<IdentityRole> roleManager)
        {
            // Create an Admin role   
            var role = new IdentityRole();
            role.Name = "Admin";
            roleManager.Create(role);
        }

        private IdentityResult CreateAdminUser(UserManager<ApplicationUser> userManager, out ApplicationUser user)
        {
            // Create an Admin user
            user = new ApplicationUser();
            user.UserName = "admin@gmail.com";
            user.Email = "admin@gmail.com";

            string adminPassword = "d653259";

            var result = userManager.Create(user, adminPassword);

            // Adding claim to admin
            var identityClaim = new IdentityUserClaim { ClaimType = "Year", ClaimValue = "1993" };
            user.Claims.Add(identityClaim);

            return result;
        }
    }
}
