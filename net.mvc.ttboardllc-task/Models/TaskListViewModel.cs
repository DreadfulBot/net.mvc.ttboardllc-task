using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace net.mvc.ttboardllc_task.Models
{
    public class TaskListViewModel
    {
        public IEnumerable<Task> Tasks;
        public Dictionary<string, string> Labels = new Dictionary<string, string>()
        {
            { "Id", "Идентификатор" },
            { "Title", "Заголовок" },
            { "Description", "Описание" },
            { "Status", "Статус" },
            { "CreatedAt", "Добавлена" },
            { "ModifiedAt", "Изменена" }
        };

        public IEnumerable<Status> Statuses;
    }
}