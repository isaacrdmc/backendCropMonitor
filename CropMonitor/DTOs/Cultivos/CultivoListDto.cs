namespace CropMonitor.DTOs.Cultivos
{
    public class CultivoListDto
    {
        public int CultivoID { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string ImagenURL { get; set; }
        public bool EsFavorito { get; set; }
        public List<string> Temporadas { get; set; } // Para mostrar las estaciones asociadas
    }
}