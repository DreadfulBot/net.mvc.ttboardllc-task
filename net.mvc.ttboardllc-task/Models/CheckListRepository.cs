using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Microsoft.Practices.ObjectBuilder2;

namespace net.mvc.ttboardllc_task.Models
{
    public class CheckListRepository : ICheckListRepository
    {
        private readonly string _table = "checklists";

        private readonly string[] _columns = new[]
        {
            "id", "title", "description", "date_created", "date_modified", "status_id"
        };

        private readonly string[] _fillable = new[]
        {
            "title", "description", "date_created", "date_modified", "status_id"
        };

        private static readonly string _connectionString = System.Configuration.ConfigurationManager.
            ConnectionStrings["LocalDb"].ConnectionString;

        private readonly SQLWorker _sw = SQLWorker.getInstance(_connectionString);

        private readonly ITaskCheckListRepository _tcr;

        public CheckListRepository()
        {
            _tcr = new TaskCheckListRepository();
        }

        public IEnumerable<CheckList> GetAll(int offset, int limit)
        {
            var data = _sw.Select(_table, new string[] {"id"}, new string[] {});

            List<CheckList> result = new List<CheckList>();
            foreach (DataRow row in data.Rows)
            {
                result.Add(GetById(Convert.ToInt32(row["id"])));
            }
            return result;
        }

        public CheckList GetById(int id)
        {
            var checkListData = _sw.Select(_table, _columns, new string[] {id.ToString()});
            var taskData = _tcr.GetTasks(id);
            var checks = _tcr.GetChecks(id);

            if (checkListData.Rows.Count == 0)
                return null;

            if (checkListData.Rows.Count > 1)
                throw new Exception("2 одинаковых id");

            return new CheckList()
            {
                CreatedAt = String.IsNullOrEmpty(checkListData.Rows[0]["date_created"].ToString())
                    ? new DateTime(1970, 01, 01)
                    : Convert.ToDateTime(checkListData.Rows[0]["date_created"].ToString()),
                Description = checkListData.Rows[0]["description"].ToString(),
                Title = String.IsNullOrEmpty(checkListData.Rows[0]["title"].ToString())
                    ? ""
                    : checkListData.Rows[0]["title"].ToString(),
                Id = Int32.Parse(checkListData.Rows[0]["id"].ToString()),
                ModifiedAt = String.IsNullOrEmpty(checkListData.Rows[0]["date_modified"].ToString())
                    ? new DateTime(1970, 01, 01)
                    : Convert.ToDateTime(checkListData.Rows[0]["date_modified"].ToString()),
                Status = String.IsNullOrEmpty(checkListData.Rows[0]["status_id"].ToString())
                    ? -1
                    : Convert.ToInt32(checkListData.Rows[0]["status_id"].ToString()),
                Tasks = taskData,
                Checks = checks
            };
        }

        public void Submit(CheckList checkList)
        {
            string createdAt = DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day +
                                " " + DateTime.Now.Hour + (":") + DateTime.Now.Minute + (":") + DateTime.Now.Second;

            string updatedAt = createdAt;

            _sw.Insert(
                _table,
                _fillable, 
                new string[]
                {
                    checkList.Title,
                    checkList.Description,
                    createdAt,
                    updatedAt,
                    checkList.Status.ToString()
                });
        }

        public int GetLastId()
        {
            throw new NotImplementedException();
        }

        public void Update(CheckList checkList)
        {
            string modifiedAt = DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day +
                              " " + DateTime.Now.Hour + (":") + DateTime.Now.Minute + (":") + DateTime.Now.Second;
            
            _sw.Update(
                _table,
                _fillable,
                new string[]
                {
                    checkList.Title,
                    checkList.Description,
                    checkList.CreatedAt.ToString("yyyy.MM.dd hh:mm:ss"),
                    modifiedAt,
                    checkList.Status.ToString()
                },
                new string[] { "id" },
                new string[] {checkList.Id.ToString()});
        }

        public void Delete(int id)
        {
            var tasks = _tcr.GetTasks(id);

            tasks.ForEach(c =>
            {
                _tcr.DetachTask(c.Id, id);
            });


            _sw.Delete(_table, new string[] {"id"}, new string[] {id.ToString()});

        }

        public void UpdateStatus(int checkListId, int newStatusId)
        {
            _sw.Update(_table, "status_id", newStatusId.ToString(), "id = " + checkListId);
        }
    }
}