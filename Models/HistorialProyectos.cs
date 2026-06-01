using System.ComponentModel.DataAnnotations;

namespace Proyecto_SkyInit.Models
{
    public class HistorialProyecto
    {
        [Key]
        public int HistorialID { get; set; }
        public int ProyectoID { get; set; }
        public int UsuarioID { get; set; }
        public string? Cambio { get; set; }
        public DateTime? FechaCambio { get;set; }
        //Navegacion
        public Proyecto? Proyecto { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
