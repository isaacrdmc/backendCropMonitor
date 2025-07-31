namespace CropMonitor.DTOs.Cultivos
{
    public class CultivoDetailDto
    {
        public int CultivoID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string ImagenURL { get; set; }
        public string RequisitosClima { get; set; }
        public string RequisitosAgua { get; set; }
        public string RequisitosLuz { get; set; }
        public bool EsFavorito { get; set; } // También para el detalle, por el ícono de corazón
        public List<string> Temporadas { get; set; } // Para mostrar las estaciones asociadas
    }
}