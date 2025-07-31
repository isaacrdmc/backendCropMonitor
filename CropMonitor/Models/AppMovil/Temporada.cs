using CropMonitor.Models.AppMovil;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.AppMovil
{
    [Table("Temporadas")]
    public class Temporada
    {
        [Key]
        public int TemporadaID { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreTemporada { get; set; }

        // Propiedades de navegación
        public ICollection<CultivosTemporada> CultivosTemporadas { get; set; }
    }
}