using CropMonitor.Models.AppMovil;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.AppMovil
{
    [Table("LecturasSensores")]
    public class LecturaSensor
    {
        [Key]
        public int LecturaID { get; set; }

        [Required]
        public int SensorID { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Valor { get; set; }

        [ForeignKey("SensorID")]
        public Sensor Sensor { get; set; }
    }
}