using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using Microsoft.Ajax.Utilities;
using Microsoft.Build.Framework;

namespace net.mvc.ttboardllc_task.Models
{
    public class TaskRepository : ITaskRepository
    {
        private readonly string _table = "tasks";

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

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Task> GetAll(int offset, int limit)
        {
            var data = _sw.Select(_table, new string[] {"id"}, new string[] {});

            List<Task> result = new List<Task>();
            foreach (DataRow row in data.Rows)
            {
                result.Add(GetById(Convert.ToInt32(row["id"])));
            }
            return result;
        }

        public Task GetById(int id)
        {
            var data = _sw.Select(_table, _columns, new string[] {id.ToString()});

            if (data.Rows.Count == 0)
                return null;

            if (data.Rows.Count > 1)
                throw new Exception("2 одинаковых id");

            return new Task()
            {
                CreatedAt = String.IsNullOrEmpty(data.Rows[0]["date_created"].ToString()) ?
                        new DateTime(1970, 01, 01) : Convert.ToDateTime(data.Rows[0]["date_created"].ToString()),
                Description = data.Rows[0]["description"].ToString(),
                Title = String.IsNullOrEmpty(data.Rows[0]["title"].ToString()) ?
                    "" : data.Rows[0]["title"].ToString(),
                Id = Int32.Parse(data.Rows[0]["id"].ToString()),
                ModifiedAt = String.IsNullOrEmpty(data.Rows[0]["date_modified"].ToString()) ?
                        new DateTime(1970, 01, 01) : Convert.ToDateTime(data.Rows[0]["date_modified"].ToString()),
                Status = String.IsNullOrEmpty(data.Rows[0]["status_id"].ToString()) ?
                        -1 : Convert.ToInt32(data.Rows[0]["status_id"].ToString())
            };
        }

        public int GetLastId()
        {
            var data = _sw.Select(_table, "max(id)", String.Empty);
            return Convert.ToInt32(data.Rows[0][0]);
        }

        public void Update(Task task)
        {
            string modifiedAt = DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day +
                               " " + DateTime.Now.Hour + (":") + DateTime.Now.Minute + (":") + DateTime.Now.Second;

            _sw.Update(
                _table, 
                _fillable, 
                new string[]
                {
                    task.Title,
                    task.Description,
                    task.CreatedAt.ToString("yyyy.MM.dd hh:mm:ss"),
                    modifiedAt,
                    task.Status.ToString()
                },
                new string[]
                {
                    "Id"
                },
                new string[]
                {
                    task.Id.ToString()
                }
            );
        }

        public void Delete(int id)
        {
            _sw.Delete(_table, new string[] { "Id" }, new string[] { id.ToString() });
        }

        public void UpdateStatus(int taskId, int newStatusId)
        {
            //3 параметра: имя таблицы, имя столбца, новое значение столбца, условие
            _sw.Update(_table, "status_id", newStatusId.ToString(), "id = " + taskId );
        }

        public void Submit(Task task)
        {
            string createdAt = DateTime.Now.Year + "." + DateTime.Now.Month + "." + DateTime.Now.Day +
                               " " + DateTime.Now.Hour + (":") + DateTime.Now.Minute + (":") + DateTime.Now.Second;
            string updatedAt = createdAt;

            _sw.Insert(_table, 
                _fillable, 
                new []
                {
                    task.Title,
                    task.Description,
                    createdAt,
                    updatedAt,
                    task.Status.ToString()
                } );
        }
    }
}