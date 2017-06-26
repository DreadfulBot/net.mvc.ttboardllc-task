using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace net.mvc.ttboardllc_task.Models
{
    public class CheckListEditViewModel
    {
        public CheckList CheckList;
        public IEnumerable<Status> Statuses;
        public string Callback;
        public Dictionary<string, string> Labels = new Dictionary<string, string>()
        {
            { "Id", "Идентификатор" },
            { "Title", "Заголовок" },
            { "Description", "Описание" },
            { "Status", "Статус" },
            { "CreatedAt", "Добавлена" },
            { "ModifiedAt", "Изменена" }
        };
    }
}