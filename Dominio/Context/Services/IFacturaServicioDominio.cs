using Dominio.Context.Entidades;
using Dominio.Context.Entidades.Articulos;
using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Context.Entidades.Finanzas;

namespace Dominio.Context.Services
{
    public interface IFacturaServicioDominio
    {
        List<FacturaDetalle> ObtenerFacturaDetalle(Articulo articulo, List<FacturaDetalle> facturasDetalle, 
            Vendedor vendedor, out string mensajeValidacion);

        List<FacturaDetalle> CalcularFacturasDetalle(List<FacturaDetalle> facturasDetallEntidad, 
            IEnumerable<Articulo> articulosEntidad, IEnumerable<Vendedor> vendedoresEntidad);

        List<FacturaDetalle> CalcularTotales(List<FacturaDetalle> facturasDetalle);
        void CrearFactura(Batch batch, FacturaEncabezado facturaEncabezado, 
            List<FacturaDetalle> facturaDetalle, List<FormasPago> formasPago, List<Articulo> articulos, Cliente cliente, RegimenFiscal regimenFiscal);

        bool ValidarFactura(List<FormasPago> formasPago, Cliente cliente, out string mensajeValidacion);
    }
}
