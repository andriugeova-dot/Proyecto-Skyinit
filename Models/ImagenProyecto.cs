using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class ImagenProyecto
    {
        [Key]
        public int ImagenID { get; set; }
        public int ProyectoID { get; set; }
        public string URL { get; set; } = null!;
        //Navegacion
        public Proyecto? Proyecto { get; set; }
    }
}
