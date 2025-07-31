using CropMonitor.Models.AppMovil;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.AppMovil
{
    [Table("Cultivos")]
    public class Cultivo
    {
        [Key]
        public int CultivoID { get; set; }

        [Required]
        [StringLength(255)]
        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public string ImagenURL { get; set; }

        [StringLength(255)]
        public string RequisitosClima { get; set; }

        [StringLength(255)]
        public string RequisitosAgua { get; set; }

        [StringLength(255)]
        public string RequisitosLuz { get; set; }

        // Propiedades de navegación
        public ICollection<CultivosTemporada> CultivosTemporadas { get; set; }
        public ICollection<RecetasCultivo> RecetasCultivos { get; set; }
        public ICollection<Sensor> Sensores { get; set; }
        public ICollection<TipsCultivo> TipsCultivos { get; set; }
        public ICollection<Favorito> Favoritos { get; set; }
        public ICollection<Notificacion> Notificaciones { get; set; }
        // Se relaciona con GuiaCultivo de la sección WebApp
        public ICollection<CropMonitor.Models.WebApp.GuiaCultivo> GuiaCultivos { get; set; }
    }
}