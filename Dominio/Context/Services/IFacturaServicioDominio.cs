using Dominio.Context.Entidades;
using Dominio.Context.Entidades.Articulos;
using Dominio.Context.Entidades.FacturaAgg;

namespace Dominio.Context.Services
{
    public interface IFacturaServicioDominio
    {
        List<FacturaDetalle> ObtenerFacturaDetalle(Articulo articulo, List<FacturaDetalle> facturasDetalle, 
            Vendedor vendedor, out string mensajeValidacion);

        List<FacturaDetalle> CalcularFacturasDetalle(List<FacturaDetalle> facturasDetallEntidad, IEnumerable<Articulo> articulosEntidad, IEnumerable<Vendedor> vendedoresEntidad);

        List<FacturaDetalle> CalcularTotales(List<FacturaDetalle> facturasDetalle);

        //void CrearFactura(Batch batch, FacturaEncabezado facturaEncabezado,
        //    List<FacturaDetalle> facturaDetalle, List<FormasPago> formasPagos, List<Articulo> articulos, Cliente cliente);
    }
}
