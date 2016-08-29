using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System;
using log4net;
using MVCTraining.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Web;

namespace MVCTraining
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Container container = new Container();
            RegisterDependecies(container);
            
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        private void RegisterDependecies(Container container)
        {
            container.RegisterSingleton<ILog>(LogManager.GetLogger("RollingFileAppender"));
            container.Register<IUserStore<ApplicationUser>>(() => new UserStore<ApplicationUser>(new ApplicationDbContext()));
            container.RegisterPerWebRequest(() => HttpContext.Current.GetOwinContext().Authentication);
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
        }
    }
}