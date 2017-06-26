using System;
using System.Web.Mvc;
using net.mvc.ttboardllc_task.Models;

namespace net.mvc.ttboardllc_task.Controllers
{
    public class TaskController : Controller
    {
        private readonly IStatusRepository _sr;
        private readonly ITaskRepository _taskRepository;
        private readonly ITaskCheckListRepository _taskCheckListRepository;

        public TaskController(ITaskRepository taskRepository, 
            IStatusRepository statusRepository, 
            ITaskCheckListRepository taskCheckListRepository)
        {
            _taskRepository = taskRepository;
            _sr = statusRepository;
            _taskCheckListRepository = taskCheckListRepository;
        }

        public ViewResult Edit(int? id, string callback)
        {
            if(id == null)
                return View();

            TaskEditViewModel model = new TaskEditViewModel()
            {
                Callback = callback,
                Task = _taskRepository.GetById(id.Value),
                Statuses = _sr.GetAll()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Task task)
        {
            var isValid = TryValidateModel(task);
            if (isValid)
            {
                _taskRepository.Update(task);
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult UpdateStatus(int taskId, int newStatusId)
        {
            ModelState.Clear();
            _taskRepository.UpdateStatus(taskId, newStatusId);
            return Json(new { success = true, responseText = "Статус успешно обновлен" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            _taskCheckListRepository.DetachFromAll(id);
            _taskRepository.Delete(id);
            return RedirectToAction("List");
        }

        public ViewResult Create(string callback)
        {
            ViewBag.callback = callback;
            ViewBag.statuses = _sr.GetAll();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Task task, string callback)
        {
            var isValid = TryValidateModel(task);
            if (isValid)
            {
                _taskRepository.Submit(task);
            }

            if(!string.IsNullOrEmpty(callback))
                return Redirect(callback);
            else
                return RedirectToAction("List");
        }

        public ViewResult List(int? offset, int? limit)
        {
            if (offset == null)
                offset = 0;

            if (limit == null)
                limit = 10;

            var tasks = _taskRepository.GetAll(offset.Value, limit.Value);
            var statuses = _sr.GetAll();

            TaskListViewModel tvm = new TaskListViewModel()
            {
                Tasks = tasks,
                Statuses = statuses
            };

            return View(tvm);
        }
    }
}