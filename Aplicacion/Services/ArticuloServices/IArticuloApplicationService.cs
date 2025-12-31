using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;
using SmartPos.DTOs.Articulos;

namespace Aplicacion.Services.ArticuloServices
{
    public interface IArticuloApplicationService
    {
        SearchResult<ArticulosDTO> ObtenerArticulos(ArticuloRequest request);
    }
}
