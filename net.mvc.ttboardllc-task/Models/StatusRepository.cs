using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using net.mvc.ttboardllc_task.Models;

namespace net.mvc.ttboardllc_task.Models
{
    public class StatusRepository : IStatusRepository
    {
        private readonly string _table = "statuses";

        private readonly string[] _columns = new[]
        {
            "id", "name"
        };

        private static readonly string _connectionString = System.Configuration.ConfigurationManager.
            ConnectionStrings["LocalDb"].ConnectionString;

        private readonly SQLWorker _sw = SQLWorker.getInstance(_connectionString);

        public IEnumerable<Status> GetAll()
        {
            var data = _sw.Select(_table, new string[] {"id"}, new string[] {});

            List<Status> result = new List<Status>();
            foreach (DataRow row in data.Rows)
            {
                result.Add(GetById(Convert.ToInt32(row["id"])));
            }
            return result;
        }

        public string GetNameById(int id)
        {
            throw new NotImplementedException();
        }

        public string GetIdByName(string name)
        {
            throw new NotImplementedException();
        }

        public Status GetById(int id)
        {
            var data = _sw.Select(_table, _columns, new string[] {id.ToString()});

            if (data.Rows.Count == 0)
                return null;

            if (data.Rows.Count > 1)
                throw new Exception("2 одинаковых id");

            return  new Status()
            {
                Id = String.IsNullOrEmpty(data.Rows[0]["id"].ToString()) ?
                    -1 : Convert.ToInt32(data.Rows[0]["id"]),
                Name = String.IsNullOrEmpty(data.Rows[0]["name"].ToString()) ?
                    String.Empty : data.Rows[0]["name"].ToString()
            };
        }
    }
}