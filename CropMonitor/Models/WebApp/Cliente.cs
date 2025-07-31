using CropMonitor.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.WebApp
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        public int ClienteID { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        [StringLength(255)]
        public string Correo { get; set; }

        [StringLength(50)]
        public string Telefono { get; set; }

        public string Direccion { get; set; }

        // Propiedades de navegación
        public ICollection<Venta> Ventas { get; set; }
    }
}