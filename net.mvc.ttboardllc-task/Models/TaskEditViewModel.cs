using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace net.mvc.ttboardllc_task.Models
{
    public class TaskEditViewModel
    {
        public Task Task;
        public IEnumerable<Status> Statuses;
        public string Callback;
    }
}