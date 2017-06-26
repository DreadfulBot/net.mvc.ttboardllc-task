using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace net.mvc.ttboardllc_task.Models
{
    public interface ICheckListRepository
    {
        IEnumerable<CheckList> GetAll(int offset, int limit);
        CheckList GetById(int id);
        void Submit(CheckList checkList);
        int GetLastId();
        void Update(CheckList checkList);
        void Delete(int id);
        void UpdateStatus(int checkListId, int newStatusId);
    }
}
