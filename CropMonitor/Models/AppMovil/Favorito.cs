using CropMonitor.Models.AppMovil;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CropMonitor.Models.AppMovil
{
    [Table("Favoritos")]
    public class Favorito
    {
        [Key]
        public int FavoritoID { get; set; }

        [Required]
        public int UsuarioID { get; set; }

        [Required]
        public int CultivoID { get; set; }

        public DateTime FechaAgregado { get; set; } = DateTime.Now;

        [ForeignKey("UsuarioID")]
        public Usuario Usuario { get; set; }

        [ForeignKey("CultivoID")]
        public Cultivo Cultivo { get; set; }
    }
}