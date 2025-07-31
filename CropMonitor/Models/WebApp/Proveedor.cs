using CropMonitor.Models.WebApp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.WebApp
{
    [Table("Proveedores")]
    public class Proveedor
    {
        [Key]
        public int ProveedorID { get; set; }

        [StringLength(255)]
        public string NombreEmpresa { get; set; }

        [StringLength(255)]
        public string Contacto { get; set; }

        [StringLength(50)]
        public string Telefono { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        public string Direccion { get; set; }

        // Propiedades de navegación
        public ICollection<Compra> Compras { get; set; }
    }
}