using Aplicacion.DTOs;
using Aplicacion.DTOs.FormasPagos;
using Aplicacion.Helpers;
using Dominio.Context.Entidades.Finanzas;
using Dominio.Core;
using Dominio.Core.Extensions;
using Infraestructura.Context;
using ServicioAplicacion.DTOs.FormasPagos;

namespace Aplicacion.Services.FPagos
{
    public class FormasPagoApplicationService : IFormasPagoApplicationService
    {
        private readonly IGenericRepository<IDataContext> _genericRepository;

        public FormasPagoApplicationService(IGenericRepository<IDataContext> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public SearchResult<FormasPagoDTO> ObtenerFormasDePagoPaginado(FormaPagoRequest request)
        {
            DynamicFilter dynamicFilter = DynamicFilterFactory.CreateDynamicFilter(request.QueryInfo);
            var formasPago = _genericRepository.GetPagedAndFiltered<FormasPago>(dynamicFilter);

            return new SearchResult<FormasPagoDTO>
            {
                ItemCount = formasPago.ItemCount,
                PageCount = formasPago.PageCount,
                PageIndex = formasPago.PageIndex,
                TotalItems = formasPago.TotalItems,
                Items = (from qry in formasPago.Items as IEnumerable<FormasPago>
                         select new FormasPagoDTO
                         {
                             FormaPagoId = qry.FormaPagoId,
                             Codigo = qry.Codigo,
                             Descripcion = qry.Descripcion,
                             TipoPago = qry.TipoPago,
                             OrdenMostrar = qry.OrdenMostrar,
                             TiposDePago = ObtenerTiposPagos(qry.TipoPago),
                         }).ToList()
            };
        }

        public List<FormasPagoDTO> ObtenerFormasDePago(FormaPagoRequest request)
        {
            IEnumerable<FormasPago> formasPago = _genericRepository.GetAll<FormasPago>();

            return formasPago.Select(r =>
            new FormasPagoDTO
            {
                FormaPagoId = r.FormaPagoId,
                Codigo = r.Codigo,
                Descripcion = r.Descripcion,
                TipoPago = r.TipoPago,
                OrdenMostrar = r.OrdenMostrar,
                TiposDePago = ObtenerTiposPagos(r.TipoPago),
            }).ToList();
        }

        private string ObtenerTiposPagos(int tipoPago)
        {
            var tiposPagos = ObtenerTiposPagos();

            var pago = tiposPagos.FirstOrDefault(r => r.Id == tipoPago);

            return pago.IsNotNull() ? pago.Tipo : string.Empty;
        }

        public FormasPagoDTO CrearFormaPago(FormaPagoRequest request)
        {
            FormasPago formaPago = _genericRepository.GetSingle<FormasPago>(r => r.Descripcion == request.FormasPago.Descripcion);

            if (formaPago != null)
            {
                return new FormasPagoDTO
                {
                    Message = $"La forma de pago {request.FormasPago.Descripcion}, ya existe"
                };
            }

            FormasPago codigoPago = _genericRepository.GetSingle<FormasPago>(r => r.Codigo == request.FormasPago.Codigo);

            if (codigoPago != null)
            {
                return new FormasPagoDTO
                {
                    Message = $"El código {request.FormasPago.Codigo}, ya existe"
                };
            }

            FormasPago nuevaformadePago = new FormasPago
            {
                Codigo = request.FormasPago.Codigo,
                Descripcion = request.FormasPago.Descripcion,
                TipoPago = request.FormasPago.TipoPago,
                OrdenMostrar = request.FormasPago.OrdenMostrar
            };

            _genericRepository.Add(nuevaformadePago);

            Commit(request.RequestUserInfo, "CrearFormaPago");

            return request.FormasPago;
        }

        public FormasPagoDTO ActualizarFormaPago(FormaPagoRequest request)
        {
            FormasPago existeCodigo = _genericRepository.GetSingle<FormasPago>(r => r.FormaPagoId != request.FormasPago.FormaPagoId 
                                        && r.Codigo == request.FormasPago.Codigo);
            if (existeCodigo != null)
            {
                return new FormasPagoDTO
                {
                    Message = $"El código para la forma de pago {request.FormasPago.Codigo} ya existe."
                };
            }

            FormasPago formaPago = _genericRepository.GetSingle<FormasPago>(r => r.FormaPagoId == request.FormasPago.FormaPagoId);

            if (formaPago == null)
            {
                return new FormasPagoDTO
                {
                    Message = $"La forma de pago {request.FormasPago.Descripcion}, no existe"
                };
            }

            formaPago.Codigo = request.FormasPago.Codigo;
            formaPago.Descripcion = request.FormasPago.Descripcion;
            formaPago.TipoPago = request.FormasPago.TipoPago;
            formaPago.OrdenMostrar = request.FormasPago.OrdenMostrar;

            Commit(request.RequestUserInfo, "ActualizarFormaPago");

            return request.FormasPago;
        }

        public List<PagosTiposDTO> ObtenerTiposPagos()
        {
            return new List<PagosTiposDTO>()
            {
                new PagosTiposDTO(){ Id=1, Tipo="Efectivo"},
                new PagosTiposDTO(){ Id=3, Tipo="Tarjeta"},
                new PagosTiposDTO(){ Id=4, Tipo="Credito"},
            }.ToList();

        }

        private void Commit(RequestUserInfo requestUserInfo, string descripcionTransaccion)
        {
            TransactionInfo transactionInfo = requestUserInfo.CrearTransactionInfo(descripcionTransaccion);

            _genericRepository.UnitOfWork.Commit(transactionInfo);
        }
    }
}
