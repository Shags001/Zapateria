using System.ComponentModel.DataAnnotations;

namespace ZapateriaWinForms.Models
{
    public class Producto
    {
        [Key]
        public int ID_Producto { get; set; }
        public string Nombre_Producto { get; set; } = string.Empty;
        public string Talla { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public decimal Precio_Unitario { get; set; }
        public string Material { get; set; } = string.Empty;
        public int Stock { get; set; }
        public int ID_Categoria { get; set; }
        public int ID_Proveedor { get; set; }
        public string Imagen_Producto { get; set; } = string.Empty;
        public string Codigo_Barra { get; set; } = string.Empty;
    }
}
