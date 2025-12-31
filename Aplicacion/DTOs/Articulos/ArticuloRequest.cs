using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;

namespace SmartPos.DTOs.Articulos
{
    public class ArticuloRequest : RequestBase  
    {
        public ArticulosDTO Articulo { get; set; }
    }
}
