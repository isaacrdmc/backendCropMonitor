using System.Collections.Generic;

namespace CropMonitor.DTOs.Recetas
{
    public class RecetaDetailDto
    {
        public int RecetaID { get; set; }
        public string NombreReceta { get; set; }
        public string Descripcion { get; set; }
        public string Instrucciones { get; set; }
        public List<CultivoEnRecetaDto> CultivosNecesarios { get; set; }
    }

    public class CultivoEnRecetaDto
    {
        public int CultivoID { get; set; }
        public string NombreCultivo { get; set; }
    }
}