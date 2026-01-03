using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;

namespace SmartPos.DTOs.Articulos
{
    public class ArticuloRequest : RequestBase  
    {
        public ArticulosDTO Articulo { get; set; }
        public InventarioMovimientoDTO InventarioMovimiento { get; set; }
    }

    public class ArticuloResponse : ResponseBase
    {
        public List<InventarioMovimientoDTO> InventarioMovimientos { get; set; }
    }
}
