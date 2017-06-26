using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace net.mvc.ttboardllc_task.Models
{
    public class CheckListCreateViewModel
    {
        public CheckList Checklist;
        public IEnumerable<Status> Statuses;
        public string Callback;
    }
}