using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_SkyInit.Models
{
    [Table("imagenesconstructora")]
    public class ImagenesConstructoras
    {
        [Key]
        public int ImagenId { get; set; }

        [Required]
        public int ConstructoraId { get; set; }

        [Required, StringLength(255)]
        public string URL { get; set; }

        public bool EsLogo { get; set; }

        //Navegacion//
        public Constructora Constructora { get; set; }
    }
}