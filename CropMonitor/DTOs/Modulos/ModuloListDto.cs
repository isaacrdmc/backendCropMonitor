namespace CropMonitor.DTOs.Modulos
{
    public class ModuloListDto
    {
        public int ModuloID { get; set; }
        public string NombreModulo { get; set; }
        public string Estado { get; set; }
        public int DiasEnFuncionamiento { get; set; }
        public int CantidadCultivosActual { get; set; }
        public int CantidadCultivosMax { get; set; }
    }
}