using log4net;
using MVCTraining.Utils;
using System.Web.Mvc;

namespace MVCTraining.Controllers
{
    public class HomeController : Controller
    {
        private ILog m_logger;

        public HomeController(ILog logger)
        {
            m_logger = logger;
        }

        [ClaimsAuthorize(Age = 20)]
        public ActionResult Index()
        {
            m_logger.Debug("Home/Index");
            return View();
        }

        public ActionResult About()
        {
            m_logger.Debug("Home/About");
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            m_logger.Debug("Home/Contact");
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}