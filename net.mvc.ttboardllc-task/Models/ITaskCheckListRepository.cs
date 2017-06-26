using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.mvc.ttboardllc_task.Models
{
    public interface ITaskCheckListRepository
    {
        IEnumerable<Task> GetTasks(int checkListId);
        Dictionary<int, bool> GetChecks(int checkListId); 
        void DetachTask(int taskId, int checkListId);
        void AttachTask(int taskId, int checkListId);
        void ChangeState(int taskId, int checkListId, bool isChecked);
        void DetachFromAll(int taskId);
    }
}
