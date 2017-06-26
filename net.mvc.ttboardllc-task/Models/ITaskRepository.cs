using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.mvc.ttboardllc_task.Models
{
    public interface ITaskRepository : IDisposable
    {
        IEnumerable<Task> GetAll(int offset, int limit);
        Task GetById(int id);
        void Submit(Task task);
        int GetLastId();
        void Update(Task task);
        void Delete(int id);
        void UpdateStatus(int taskId, int newStatusId);
    }
}
