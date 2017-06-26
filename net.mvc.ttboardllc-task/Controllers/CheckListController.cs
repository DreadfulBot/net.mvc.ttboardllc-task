using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using net.mvc.ttboardllc_task.Models;

namespace net.mvc.ttboardllc_task.Controllers
{
    public class CheckListController : Controller
    {
        private readonly ICheckListRepository _checkListRepository;
        private readonly IStatusRepository _sr;
        private readonly ITaskCheckListRepository _tsr;
        private readonly ITaskRepository _tr;

        public CheckListController(ICheckListRepository checkListRepository, 
            IStatusRepository statusRepository, 
            ITaskCheckListRepository taskCheckListRepository,
            ITaskRepository taskRepository)
        {
            _checkListRepository = checkListRepository;
            _sr = statusRepository;
            _tsr = taskCheckListRepository;
            _tr = taskRepository;
        }

        public ViewResult List(int? offset, int? limit)
        {
            if (offset == null)
                offset = 0;

            if (limit == null)
                limit = 10;

            var checkLists = _checkListRepository.GetAll(offset.Value, limit.Value);
            var statuses = _sr.GetAll();

            var clvm = new CheckListViewModel()
            {
                Checklists = checkLists,
                Statuses = statuses
            };

            return View(clvm);
        }

        public ViewResult Edit(int? id, string callback)
        {
            if (id == null)
                return View();

            var model = new CheckListEditViewModel()
            {
                Callback = callback,
                CheckList = _checkListRepository.GetById(id.Value),
                Statuses = _sr.GetAll()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CheckList checkList)
        {
            var isValid = TryValidateModel(checkList);
            if (isValid)
            {
                _checkListRepository.Update(checkList);
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            _checkListRepository.Delete(id);
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult AddLastTask(int checkListId)
        {
            var taskId = _tr.GetLastId();
            _tsr.AttachTask(taskId, checkListId);

            return RedirectToAction("Edit", new RouteValueDictionary(
                new { controller = "CheckList", action = "Edit", Id = checkListId }));
        }

        [HttpPost]
        public ActionResult UpdateStatus(int taskId, int newStatusId)
        {
            ModelState.Clear();
            _checkListRepository.UpdateStatus(taskId, newStatusId);
            return Json(new { success = true, responseText = "Статус успешно обновлен" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ChangeState(int taskId, int checkListId, bool isChecked)
        {
            _tsr.ChangeState(taskId, checkListId, isChecked);
            return Json(new { success = true, responseText = "Состояние успешно обновлено" }, JsonRequestBehavior.AllowGet);
        }

        public ViewResult Create(string callback)
        {
            var model = new CheckListCreateViewModel()
            {
                Checklist = new CheckList(),
                Statuses = _sr.GetAll(),
                Callback = callback
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CheckList checkList)
        {
            var isValid = TryValidateModel(checkList);
            if (isValid)
            {
                _checkListRepository.Submit(checkList);
            }

            return RedirectToAction("List");
        }

    }
}