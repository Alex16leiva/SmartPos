using Aplicacion.DTOs.Factura;
using Dominio.Context.Entidades;
using Dominio.Context.Entidades.Articulos;
using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Context.Services;
using Dominio.Core.Extensions;
using Infraestructura.Context;

namespace Aplicacion.Services.Factura
{
    public class FacturaApplicationService : IFacturaApplicationService
    {
        private readonly IGenericRepository<IDataContext> _genericRepository;
        private readonly IFacturaServicioDominio _facturaServicioDominio;
        public FacturaApplicationService(IGenericRepository<IDataContext> genericRepository,
            IFacturaServicioDominio facturaServicioDominio) 
        {
            _genericRepository = genericRepository;
            _facturaServicioDominio = facturaServicioDominio;
        }

        public FacturaResponse AgregarArticuloAFactura(AgregarArticuloRequest request)
        {
            List<string> articulos = request.FacturasDetalle.Select(r => r.ArticuloId).ToList();
            articulos.Add(request.ArticuloId);

            var articulosEntidad = _genericRepository.GetFiltered<Articulo>(r => articulos.Contains(r.ArticuloId));

            if (articulosEntidad.IsNull() || !articulosEntidad.Where(r => r.ArticuloId.ToUpper() == request.ArticuloId.ToUpper()).Any())
            {
                return new FacturaResponse
                {
                    FacturaDetalle = request.FacturasDetalle,
                    Message = $"El articulo {request.ArticuloId}, no existe"
                };
            }

            string mensajeValidacion = string.Empty;

            Vendedor vendedor = new Vendedor
            {
                VendedorId = request.Vendedor.VendedorId,
                Nombre = request.Vendedor?.Nombre
            };

            List<FacturaDetalle> facturasDetalleEntidad = MapFacturaDetalleDeDtoAEntidad(request.FacturasDetalle, articulosEntidad);

            Articulo articulo = articulosEntidad.FirstOrDefault(r => r.ArticuloId.ToUpper() == request.ArticuloId.ToUpper());

            List<FacturaDetalle> facturasDetalle = _facturaServicioDominio.ObtenerFacturaDetalle(articulo, facturasDetalleEntidad, vendedor, out mensajeValidacion);

            if (mensajeValidacion.HasValue())
            {
                return new FacturaResponse
                {
                    Message = mensajeValidacion,
                };
            }
            return new FacturaResponse
            {
                FacturaDetalle = MapFacturaDetalleDeEntidadADto(facturasDetalle)
            };
        }

        private List<FacturaDetalleDTO> MapFacturaDetalleDeEntidadADto(List<FacturaDetalle> facturasDetalle)
        {
            return facturasDetalle.Select(r => new
            FacturaDetalleDTO
            {
                ArticuloId = r.ArticuloId,
                Cantidad = r.Cantidad,
                CantidadOriginal = r.CantidadOriginal,
                CantidadArticulos = r.CantidadArticulos,
                Comentario = r.Comentario,
                Comision = r.Comision,
                Costo = r.Costo,
                Descripcion = r.Descripcion,
                DescripcionExtendida = r.DescripcionExtendida,
                DescripcionImpuesto = r.DescripcionImpuesto,
                Descuento = r.Descuento,
                FacturaId = r.FacturaId,
                Id = r.Id,
                Impuesto = r.Impuesto,
                MostrarNegrita = r.MostrarNegrita,
                NombreVendedor = r.NombreVendedor,
                PagaImpuesto = r.PagaImpuesto,
                PorcentajeImpuesto = r.PorcentajeImpuesto,
                Precio = r.Precio,
                PrecioFinal = r.PrecioFinal,
                PrecioSinImpuesto = r.PrecioSinImpuesto,
                VendedorId = r.VendedorId,
                SubTotal = r.SubTotal,
                Total = r.Total,
            }).ToList();
        }

        private List<FacturaDetalle> MapFacturaDetalleDeDtoAEntidad(List<FacturaDetalleDTO> facturasDetalle, IEnumerable<Articulo> articulosEntidad)
        {
            return facturasDetalle.Select(r => new
            FacturaDetalle
            {
                ArticuloId = r.ArticuloId,
                Cantidad = r.Cantidad,
                CantidadArticulos = r.CantidadArticulos,
                CantidadOriginal = r.CantidadOriginal,
                Comentario = string.IsNullOrWhiteSpace(r.Comentario) ? string.Empty : r.Comentario,
                Comision = r.Comision,
                Costo = r.Costo,
                Descripcion = r.Descripcion,
                DescripcionExtendida = r.DescripcionExtendida,
                DescripcionImpuesto = r.DescripcionImpuesto,
                Descuento = r.Descuento,
                FacturaId = r.FacturaId,
                Id = r.Id,
                Impuesto = r.Impuesto,
                MostrarNegrita = r.MostrarNegrita,
                NombreVendedor = r.NombreVendedor,
                PagaImpuesto = r.PagaImpuesto,
                PorcentajeImpuesto = r.PorcentajeImpuesto,
                Precio = r.Precio,
                PrecioFinal = r.PrecioFinal,
                PrecioSinImpuesto = r.PrecioSinImpuesto,
                VendedorId = r.VendedorId,
                SubTotal = r.SubTotal,
                Total = r.Total,
                Articulo = articulosEntidad.FirstOrDefault(x => x.ArticuloId == r.ArticuloId)
            }).ToList();
        }


    }
}
