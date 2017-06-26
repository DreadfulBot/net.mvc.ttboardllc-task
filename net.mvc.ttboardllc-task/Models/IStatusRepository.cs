using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.mvc.ttboardllc_task.Models
{
    public interface IStatusRepository
    {
        IEnumerable<Status> GetAll();
        string GetNameById(int id);
        string GetIdByName(string name);
        Status GetById(int id);
    }
}
