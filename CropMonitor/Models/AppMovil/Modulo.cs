using CropMonitor.Models.AppMovil;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.AppMovil
{
    [Table("Modulos")]
    public class Modulo
    {
        [Key]
        public int ModuloID { get; set; }

        [Required]
        [StringLength(255)]
        public string NombreModulo { get; set; }

        [StringLength(50)]
        public string Estado { get; set; } // Ej: 'Activo', 'Inactivo'

        public int? DiasEnFuncionamiento { get; set; }

        public int? CantidadCultivosActual { get; set; }

        public int? CantidadCultivosMax { get; set; }

        [Required]
        public int UsuarioID { get; set; }

        [ForeignKey("UsuarioID")]
        public Usuario Usuario { get; set; }

        // Propiedades de navegación
        public ICollection<Sensor> Sensores { get; set; }
    }
}