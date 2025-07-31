using CropMonitor.Models.AppMovil;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.WebApp
{
    [Table("GuiaCultivo")]
    public class GuiaCultivo
    {
        [Key]
        public int GuiaID { get; set; }

        [Required]
        public int CultivoID { get; set; }

        public string Contenido { get; set; }

        public DateTime FechaPublicacion { get; set; } = DateTime.Now;

        [ForeignKey("CultivoID")]
        public Cultivo Cultivo { get; set; }
    }
}