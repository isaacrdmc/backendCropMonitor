using CropMonitor.Models.AppMovil;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.AppMovil
{
    [Table("TipsCultivos")]
    public class TipsCultivo
    {
        [Key]
        public int TipID { get; set; }

        [Required]
        public int CultivoID { get; set; }

        [Required]
        public string DescripcionTip { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public int? UsuarioID { get; set; } // Opcional: usuario que añadió el tip

        [ForeignKey("CultivoID")]
        public Cultivo Cultivo { get; set; }

        [ForeignKey("UsuarioID")]
        public Usuario Usuario { get; set; } // Clave foránea anulable
    }
}