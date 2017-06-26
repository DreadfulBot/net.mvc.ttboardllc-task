using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.ModelBinding;

namespace net.mvc.ttboardllc_task.Models
{
    public class Task
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Заголовок")]
        public string Title { get; set; }
        [Required]
        [DisplayName("Описание")]
        public string Description { get; set; }
        [Required]
        [DisplayName("Статус")]
        public int Status { get; set; }
        [Required]
        [DisplayName("Создано")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{yyyy.dd.MM hh:mm:ss}")]
        public DateTime CreatedAt { get; set; }
        [DisplayName("Изменено")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{yyyy.dd.MM hh:mm:ss}")]
        public DateTime ModifiedAt { get; set; }
    }
}