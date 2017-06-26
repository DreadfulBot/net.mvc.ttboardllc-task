using System.Web.Mvc;

namespace net.mvc.ttboardllc_task.Controllers.Tasker
{
    public class IndexController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("List", "Task", new {offset = 0, limit = 10});
        }
    }
}