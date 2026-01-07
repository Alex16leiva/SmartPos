using Aplicacion.DTOs;
using Aplicacion.DTOs.Vendedores;

namespace Aplicacion.Services.VendedorSevices
{
    public interface IVendedorApplicationService
    {
        SearchResult<VendedorDTO> ObtenerVendedoresPaginado(ObtenerVendedor request);
        VendedorDTO ActualizarVendedor(ActualizarVenderor request);

        VendedorDTO CrearVendedor(CrearVenderor request);

        List<VendedorDTO> ObtenerVendedores(ObtenerVendedor request);

        VendedorDTO EliminarVendedor(ObtenerVendedor request);
    }   
}
