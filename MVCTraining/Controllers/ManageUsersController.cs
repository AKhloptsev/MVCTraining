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
                var model = new ManageUsersViewModel();
                model.Id = user.Id;
                model.Name = user.UserName;
                model.Email = user.Email;
                model.Roles = user.Roles.ToList();
                model.Claims = user.Claims.ToList();
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

            return View(user);
        }

        //
        // POST: /ManageUsers/Edit/User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var result = await ApplicationUser.UpdateApplicationUser(usersManager, user);
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