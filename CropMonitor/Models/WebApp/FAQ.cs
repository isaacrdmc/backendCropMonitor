using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.WebApp
{
    [Table("FAQ")]
    public class FAQ
    {
        [Key]
        public int FAQID { get; set; }

        [StringLength(500)]
        public string Pregunta { get; set; }

        public string Respuesta { get; set; }
    }
}