using Aplicacion.DTOs.Factura;
using Aplicacion.Helpers;
using CrossCutting.Identity;
using Dominio.Context.Entidades;
using Dominio.Context.Entidades.Articulos;
using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Context.Entidades.Finanzas;
using Dominio.Context.Services;
using Dominio.Core;
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

        public FacturaResponse AgregarArticuloAFactura(FacturaRequest request)
        {
            List<string> articulos = request.FacturaDetalle.Select(r => r.ArticuloId).ToList();
            articulos.Add(request.ArticuloId);

            var articulosEntidad = _genericRepository.GetFiltered<Articulo>(r => articulos.Contains(r.ArticuloId));

            if (articulosEntidad.IsNull() || !articulosEntidad.Where(r => r.ArticuloId.ToUpper() == request.ArticuloId.ToUpper()).Any())
            {
                return new FacturaResponse
                {
                    FacturaDetalle = request.FacturaDetalle,
                    Message = $"El articulo {request.ArticuloId}, no existe"
                };
            }

            string mensajeValidacion = string.Empty;

            Vendedor vendedor = new Vendedor
            {
                VendedorId = request.Vendedor.VendedorId,
                Nombre = request?.Vendedor?.Nombre
            };

            List<FacturaDetalle> facturasDetalleEntidad = MapFacturaDetalleDeDtoAEntidad(request.FacturaDetalle, articulosEntidad);

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
                Comentario = r.Comentario.ValueOrEmpty(),
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

        public FacturaResponse CalcularFacturaDetalle(FacturaRequest request)
        {
            List<string> articulos = request.FacturaDetalle.Select(r => r.ArticuloId).ToList();

            List<int> vendedores = request.FacturaDetalle.Select(r => r.VendedorId).ToList();

            IEnumerable<Articulo> articulosEntidad = _genericRepository.GetFiltered<Articulo>(r => articulos.Contains(r.ArticuloId));

            IEnumerable<Vendedor> VendedoresEntidad = _genericRepository.GetFiltered<Vendedor>(r => vendedores.Contains(r.VendedorId));

            List<FacturaDetalle> facturasDetallEntidad = MapFacturaDetalleDeDtoAEntidad(request.FacturaDetalle, articulosEntidad);

            facturasDetallEntidad = _facturaServicioDominio.CalcularFacturasDetalle(facturasDetallEntidad, articulosEntidad, VendedoresEntidad);

            return new FacturaResponse
            {
                FacturaDetalle = MapFacturaDetalleDeEntidadADto(facturasDetallEntidad)
            };
        }

        public async Task<FacturaResponse> CrearFactura(FacturaRequest request)
        {
            Batch batch = await _genericRepository.GetSingleAsync<Batch>(r => r.BatchId == request.FacturaEncabezado.BatchId);

            if (batch.EstaCerrado())
            {
                return new FacturaResponse
                {
                    Message = "No se puede crear la factura porque el batch está cerrado, genere un nuevo batch"
                };
            }

            RegimenFiscal regimenFiscal = await _genericRepository.GetSingleAsync<RegimenFiscal>(r => r.Activo);

            List<string> articulosFacturados = request.FacturaDetalle.Select(r => r.ArticuloId).ToList();

            IEnumerable<Articulo> articulos = await _genericRepository.GetFilteredAsync<Articulo>(r => articulosFacturados.Contains(r.ArticuloId));

            Cliente cliente = new Cliente();

            if (request.FacturaEncabezado.ClienteId.HasValue())
            {
                cliente = await _genericRepository.GetSingleAsync<Cliente>(item =>  item.NumeroCuenta == request.FacturaEncabezado.ClienteId );
            }

            string correlativoFacturaId = IdentityFactory.CreateIdentity().NextCorrelativeIdentity("FA");
            request.FacturaEncabezado.LLamadaId = request.FacturaEncabezado.FacturaId.ValueOrEmpty();
            request.FacturaEncabezado.FacturaId = correlativoFacturaId;

            if (request.FacturaEncabezado.EsDevolucion)
            {
                var facturaOriginal = await _genericRepository.GetSingleAsync<FacturaEncabezado>(r => r.FacturaId == request.FacturaEncabezado.LLamadaId);

                facturaOriginal.LLamadaId = correlativoFacturaId;
                facturaOriginal.LlamadaTipo = "ReferenciaDevolucion";
            }

            FacturaEncabezado facturaEncabezado = MaterializarFacturaEncabezadoDtoAEntidad(request.FacturaEncabezado, regimenFiscal);

            List<FacturaDetalle> facturaDetalle = MaterializarFacturaDetalleDtoAEntidad(request.FacturaDetalle, articulos);

            List<FormasPago> formasPago = request.FormasPagos.Select(r =>
                new FormasPago
                {
                    Cobro = r.Cobro,
                    Codigo = r.Codigo,
                    Descripcion = r.Descripcion,
                    FormaPagoId = r.FormaPagoId,
                    MostrarCobro = r.MostrarCobro,
                    TipoPago = r.TipoPago
                }).ToList();

            _facturaServicioDominio.CrearFactura(batch, facturaEncabezado, facturaDetalle, formasPago, articulos.ToList(), cliente);
            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("CrearFactura");

            
            _genericRepository.UnitOfWork.Commit(transactionInfo);

    
            facturaEncabezado.FechaTransaccion = transactionInfo.FechaTransaccion;
            return new FacturaResponse
            {
                FacturaDetalle = MaterializarFacturaDetalleEntidadADto(facturaDetalle),
                FacturaEncabezado = MaterializarFacturaEncabezadoEntidadADto(facturaEncabezado, regimenFiscal)
            };
        }

        private FacturaEncabezadoDTO MaterializarFacturaEncabezadoEntidadADto(FacturaEncabezado facturaEncabezado, RegimenFiscal regimenFiscal)
        {
            return new FacturaEncabezadoDTO
            {
                FacturaId = facturaEncabezado.FacturaId,
                BatchId = facturaEncabezado.BatchId,
                CAI = facturaEncabezado.CAI,
                Correlativo = facturaEncabezado.Correlativo,
                Desde = regimenFiscal.Desde,
                Hasta = regimenFiscal.Hasta,
                FechaLimiteEmision = regimenFiscal.FechaLimiteEmision,
                CajaId = facturaEncabezado.CajaId,
                Cambio = facturaEncabezado.Cambio,
                CampoPersonalizado1 = facturaEncabezado.CampoPersonalizado1,
                CampoPersonalizado2 = facturaEncabezado.CampoPersonalizado2,
                ClienteId = facturaEncabezado.ClienteId,
                Comentario = facturaEncabezado.Comentario,
                Descuento = facturaEncabezado.Descuento,
                Impuesto = facturaEncabezado.Impuesto,
                LLamadaId = facturaEncabezado.LLamadaId,
                LlamadaTipo = facturaEncabezado.LlamadaTipo,
                SubTotal = facturaEncabezado.SubTotal,
                Total = facturaEncabezado.Total,
                FechaTransaccion = facturaEncabezado.FechaTransaccion,
                FechaCreacion = facturaEncabezado.FechaCreacion
            };
        }

        private List<FacturaDetalleDTO> MaterializarFacturaDetalleEntidadADto(List<FacturaDetalle> facturaDetalle)
        {
            return facturaDetalle.Select(r => new
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

        private List<FacturaDetalle> MaterializarFacturaDetalleDtoAEntidad(List<FacturaDetalleDTO> facturaDetalle, IEnumerable<Articulo> articulosEntidad)
        {
            return facturaDetalle.Select(r => new
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

        private FacturaEncabezado MaterializarFacturaEncabezadoDtoAEntidad(FacturaEncabezadoDTO facturaEncabezado, RegimenFiscal regimenFiscal)
        {
            return new FacturaEncabezado
            {
                FacturaId = facturaEncabezado.FacturaId,
                BatchId = facturaEncabezado.BatchId,
                CajaId = facturaEncabezado.CajaId,
                ClienteId = facturaEncabezado.ClienteId,
                Total = facturaEncabezado.Total,
                Impuesto = facturaEncabezado.Impuesto,
                CampoPersonalizado1 = facturaEncabezado.CampoPersonalizado1,
                CampoPersonalizado2 = facturaEncabezado.CampoPersonalizado2,
                Comentario = facturaEncabezado.Comentario,
                Cambio = facturaEncabezado.Cambio,
                SubTotal = facturaEncabezado.SubTotal,
                Descuento = facturaEncabezado.Descuento,
                LLamadaId = facturaEncabezado.LLamadaId,
                LlamadaTipo = facturaEncabezado.LlamadaTipo,
                CAI = regimenFiscal.IsNotNull() ? regimenFiscal.CAI : string.Empty,
                Correlativo = regimenFiscal.IsNotNull() ? regimenFiscal.ObtenerCorrelativoNuevo() : string.Empty,
                Desde = regimenFiscal.IsNotNull() ? regimenFiscal.Desde : string.Empty,
                Hasta = regimenFiscal.IsNotNull() ? regimenFiscal.Hasta : string.Empty,
                FechaLimiteEmision = regimenFiscal.IsNotNull() ? regimenFiscal.FechaLimiteEmision : DateTime.Now,
                FechaCreacion = facturaEncabezado.FechaCreacion,
                EsDevolucion = facturaEncabezado.EsDevolucion
            };
        }
    }
}
