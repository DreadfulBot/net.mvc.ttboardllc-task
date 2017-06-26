using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace net.mvc.ttboardllc_task.Models
{
    public class TaskCheckListRepository : ITaskCheckListRepository
    {
        private readonly string _table = "task_checklist";

        private readonly string[] _columns = new string[]
        {
            "task_id",
            "checklist_id",
            "is_checked",
            "date_created"
        };

        private readonly string[] _fillable = new string[]
        {
            "task_id",
            "checklist_id",
            "is_checked",
            "date_created"
        };

        private static readonly string _connectionString = System.Configuration.ConfigurationManager.
            ConnectionStrings["LocalDb"].ConnectionString;

        private readonly SQLWorker _sw = SQLWorker.getInstance(_connectionString);

        private ITaskRepository _tr;

        public TaskCheckListRepository()
        {
            _tr = new TaskRepository();
        }

        public IEnumerable<Task> GetTasks(int checkListId)
        {
            var data = _sw.Select(_table, new string[] { "checklist_id", "task_id"}, new string[] {checkListId.ToString()});

            var result = new List<Task>();
            foreach (DataRow row in data.Rows)
            {
                result.Add(_tr.GetById(Convert.ToInt32(row["task_id"])));
            }

            return result;
        }

        public Dictionary<int, bool> GetChecks(int checkListId)
        {
            var data = _sw.Select(_table, new string[] {"checklist_id", "task_id", "is_checked"},
                new string[] {checkListId.ToString()})
                ;

            var result = new Dictionary<int, bool>();
            foreach (DataRow row in data.Rows)
            {
                    result.Add(Convert.ToInt32(row["task_id"]), row["is_checked"].ToString() != "0");
            }

            return result;
        }

        public void DetachTask(int taskId, int checkListId)
        {
            var data = _sw.Select(_table, new string[] {"task_id", "checklist_id"}, new string[] {taskId.ToString(), checkListId.ToString()});
            if(data.Rows.Count == 0)
                throw new Exception($"Связи {taskId} с {checkListId} не существует");

            _sw.Delete(_table, $"task_id='{taskId}' AND checklist_id='{checkListId}'");
        }

        public void AttachTask(int taskId, int checkListId)
        {
            var data = _sw.Select(_table, new string[] {"task_id", "checklist_id" }, new string[] { taskId.ToString(), checkListId.ToString() });
            if(data.Rows.Count > 0)
                throw new Exception($"Связь {taskId} с {checkListId} уже существует");

            string createdAt = DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day +
                              " " + DateTime.Now.Hour + (":") + DateTime.Now.Minute + (":") + DateTime.Now.Second;


            _sw.Insert(_table, _columns, new string[] {taskId.ToString(), checkListId.ToString(), "0", createdAt});
        }

        public void ChangeState(int taskId, int checkListId, bool isChecked)
        {
            var data = _sw.Select(_table, new string[] { "task_id", "checklist_id" }, new string[] { taskId.ToString(), checkListId.ToString() });
            if(data.Rows.Count == 0)
                throw new Exception($"Связи {taskId} с {checkListId} не существует");

            _sw.Update(_table, "is_checked", isChecked == true ? "1" : "0", $"task_id='{taskId}' AND checklist_id='{checkListId}'");
        }

        public void DetachFromAll(int taskId)
        {
            _sw.Delete(_table, $"task_id = '{taskId}'");
        }
    }
}