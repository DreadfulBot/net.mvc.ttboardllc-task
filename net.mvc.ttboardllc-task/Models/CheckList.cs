using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace net.mvc.ttboardllc_task.Models
{
    public class CheckList
    {
        public int Id { get; set; }

        [Microsoft.Build.Framework.Required]
        [DisplayName("Заголовок")]
        public string Title { get; set; }

        [Microsoft.Build.Framework.Required]
        [DisplayName("Описание")]
        public string Description { get; set; }

        [Microsoft.Build.Framework.Required]
        [DisplayName("Создано")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{yyyy.dd.MM hh:mm:ss}")]
        public DateTime CreatedAt { get; set; }

        [Microsoft.Build.Framework.Required]
        [DisplayName("Изменено")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{yyyy.dd.MM hh:mm:ss}")]
        public DateTime ModifiedAt { get; set; }

        [Required]
        [DisplayName("Статус")]
        public int Status { get; set; }

        public IEnumerable<Task> Tasks { get; set; }

        // int - task_id, bool - checked/unchecked
        public Dictionary<int, bool> Checks { get; set; } 
    }
}