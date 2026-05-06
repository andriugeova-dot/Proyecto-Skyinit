using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class Estadistica
    {
        [Key]
        public int EstadisticaID { get; set; }
        public int? PropiedadID { get; set; }
        public int? ProyectoID { get; set; }
        public int Visitas { get; set; }
        public int Favoritos { get; set; }
        public int Comentarios { get; set; }
        // Navegación
        public Propiedad? Propiedad { get; set; }
        public Proyecto? Proyecto { get; set; }
    }
}
