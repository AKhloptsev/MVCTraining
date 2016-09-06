using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MVCTraining.Models;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVCTraining.Controllers
{
    public class RoleController : Controller
    {
        private ApplicationDbContext context;
        private UserManager<ApplicationUser> usersManager;
        private RoleManager<IdentityRole> rolesManager;

        public RoleController()
        {
            context = new ApplicationDbContext();
            usersManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            rolesManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
        }

        // GET: Role
        public ActionResult Index()
        {
            return View(rolesManager.Roles.Where((a) => !a.Name.Equals("Admin", StringComparison.InvariantCultureIgnoreCase)));
        }

        #region Manage Roles
        //
        // GET: /Role/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Role/Create
        [HttpPost]
        public async Task<ActionResult> Create(RoleViewModel roleViewModel)
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(roleViewModel.Name);
                var roleResult = await rolesManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    ModelState.AddModelError("", roleResult.Errors.First().ToString());
                    return View();
                }

                return RedirectToAction("ManageRoles");
            }
            else
            {
                return View();
            }
        }

        //
        // GET: /Role/Edit/Admin
        public async Task<ActionResult> Edit(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var role = await rolesManager.FindByNameAsync(name);
            if (role == null)
            {
                return HttpNotFound();
            }

            return View(role);
        }

        //
        // POST: /Role/Edit/User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                var result = await rolesManager.UpdateAsync(role);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }

                return RedirectToAction("ManageRoles");
            }
            else
            {
                return View();
            }
        }

        //
        // GET: /Role/Delete/User
        public async Task<ActionResult> Delete(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var role = await rolesManager.FindByNameAsync(name);
            if (role == null)
            {
                return HttpNotFound();
            }

            return View(role);
        }

        //
        // POST: /Role/Delete/User
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string name)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(name))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var role = await rolesManager.FindByNameAsync(name);
                var result = await rolesManager.DeleteAsync(role);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First().ToString());
                    return View();
                }

                return RedirectToAction("ManageRoles");
            }
            else
            {
                return View();
            }
        }
        #endregion
    }
}