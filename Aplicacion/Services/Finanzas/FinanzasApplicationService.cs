using Aplicacion.DTOs.Finanzas;
using Aplicacion.Helpers;
using Azure.Core;
using CrossCutting.Identity;
using Dominio.Context.Entidades.Finanzas;
using Dominio.Core;
using Dominio.Core.Extensions;
using Infraestructura.Context;

namespace Aplicacion.Services.Finanzas
{
    public class FinanzasApplicationService : IFinanzasApplicationService
    {
        private readonly IGenericRepository<IDataContext> _genericRepository;

        public FinanzasApplicationService(IGenericRepository<IDataContext> genericRepository)
        {
            _genericRepository = genericRepository;
        }

        public async Task<BatchDTO> ObtenerOBuscarBatchActivoAsync(BatchRequest request)
        {
            // 1. Buscar si hay uno abierto
            var batchActual = await _genericRepository.GetSingleAsync<Batch>(b => b.CajaId == request.CajaId && b.Estado == "Nuevo");

            if (batchActual.IsNotNull())
                return MapBatchEntidadADto(batchActual);

            // 2. Si no existe, crear el nuevo lote (Transacción única)
            var nuevoBatchId = IdentityFactory.CreateIdentity().NextCorrelativeIdentity("BC");

            var nuevoBatch = new Batch
            {
                BatchId = nuevoBatchId,
                CajaId = request.CajaId,
                Estado = "Nuevo",
                FechaApertura = DateTime.Now,
                SubTotal = 0,
                TotalVenta = 0,
                CantidadClientes = 0
            };

            _genericRepository.Add(nuevoBatch);
            TransactionInfo transactionInfo = request.RequestUserInfo.CrearTransactionInfo("AgregarRol");
            _genericRepository.UnitOfWork.Commit(transactionInfo);

            return MapBatchEntidadADto(nuevoBatch);
        }

        private BatchDTO MapBatchEntidadADto(Batch entidad)
        {
            return new BatchDTO
            {
                CajaId = entidad.CajaId,
                BatchId = entidad.BatchId,
                CambioTotal = entidad.CambioTotal,
                CantidadClientes = entidad.CantidadClientes,
                CierreTotal = entidad.CierreTotal,
                Comision = entidad.Comision,
                CostoTotal = entidad.CostoTotal,
                Descuento = entidad.Descuento,
                Devoluciones = entidad.Devoluciones,
                Estado = entidad.Estado,
                FechaApertura = entidad.FechaApertura,
                FechaCierre = entidad.FechaCierre,
                FechaTransaccion = entidad.FechaTransaccion,
                Impuesto = entidad.Impuesto,
                ModificadoPor = entidad.ModificadoPor,
                PagoACuenta = entidad.PagoACuenta,
                TotalFormaDePago = entidad.TotalFormaDePago,
                SubTotal = entidad.SubTotal,
                TotalVenta = entidad.TotalVenta,
                VentasCredito = entidad.VentasCredito,
            };
        }
    }
}
