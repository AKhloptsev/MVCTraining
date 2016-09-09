using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVCTraining.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCTraining.Controllers
{
    public class ManageUsersController : Controller
    {
        private ApplicationDbContext context;
        private UserManager<ApplicationUser> usersManager;
        private RoleManager<IdentityRole> rolesManager;

        public ManageUsersController()
        {
            context = new ApplicationDbContext();
            usersManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            rolesManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }

        // GET: Users
        public ActionResult Index()
        {
            var models = new List<ManageUsersViewModel>();
            var users = usersManager.Users.Where((a) => !a.UserName.Equals("admin@gmail.com", StringComparison.InvariantCultureIgnoreCase));
            foreach (var user in users)
            {
                var model = new ManageUsersViewModel
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    Roles = usersManager.GetRoles(user.Id).ToList(),
                    Claims = user.Claims.ToList()
                };

                models.Add(model);
            }

            return View(models);
        }

        #region Manage Users

        //
        // GET: /ManageUsers/Edit/User
        public async Task<ActionResult> Edit(string userId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await usersManager.FindByIdAsync(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await usersManager.GetRolesAsync(user.Id);
            var allRolesIdentity = rolesManager.Roles.ToList().Where(x => !x.Name.Equals("Admin"));
            var allRoles = allRolesIdentity
                .Select(c => new
                {
                    RoleId = c.Id,
                    RoleName = c.Name
                })
                .ToList();

            var roleIds = from allRole in allRoles
                          from userRole in userRoles
                          where allRole.RoleName.Equals(userRole)
                          select allRole.RoleId;

            var selectList = new MultiSelectList(allRoles, "RoleId", "RoleName", roleIds.ToList());

            var editUser = new ManageUsersViewModel(user, userRoles.ToList(), selectList, roleIds.ToList());

            return View(editUser);
        }

        //
        // POST: /ManageUsers/Edit/User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ManageUsersViewModel user)
        {
            if (ModelState.IsValid)
            {
                var result = await ApplicationUser.UpdateApplicationUser(usersManager, rolesManager, user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }

                return RedirectToAction("Index", "ManageUsers");
            }
            else
            {
                return View();
            }
        }

        //
        // GET: /ManageUsers/Delete/User
        public async Task<ActionResult> Delete(string userId)
        {
            if (String.IsNullOrEmpty(userId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await usersManager.FindByIdAsync(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            return View(user);
        }

        //
        // POST: /ManageUsers/Delete/User
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string userId)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(userId))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await usersManager.FindByIdAsync(userId);
                var result = await usersManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }

                return RedirectToAction("ManageUsers");
            }
            else
            {
                return View();
            }
        }
        #endregion
    }
}