using Aplicacion.DTOs;
using Aplicacion.DTOs.Articulos;
using SmartPos.DTOs.Articulos;

namespace Aplicacion.Services.ArticuloServices
{
    public interface IArticuloApplicationService
    {
        SearchResult<ArticulosDTO> ObtenerArticulos(ArticuloRequest request);

        Task<ArticulosDTO> CrearArticuloAsync(ArticuloRequest request);

        Task<ArticulosDTO> ActualizarArticuloAsync(ArticuloRequest request);

        Task<ArticulosDTO> RegistrarMovimientoAsync(ArticuloRequest request);

        Task<ArticuloResponse> ObtenerKardexArticuloAsync(string articuloId);
    }
}
