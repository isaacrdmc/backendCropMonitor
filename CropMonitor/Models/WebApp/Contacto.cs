using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.WebApp
{
    [Table("Contacto")]
    public class Contacto
    {
        [Key]
        public int ContactoID { get; set; }

        [StringLength(255)]
        public string Nombre { get; set; }

        [StringLength(255)]
        public string Correo { get; set; }

        public string Mensaje { get; set; }

        public DateTime FechaEnvio { get; set; } = DateTime.Now;
    }
}