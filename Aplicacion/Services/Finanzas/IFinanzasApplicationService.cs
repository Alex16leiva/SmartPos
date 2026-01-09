using Aplicacion.DTOs.Finanzas;

namespace Aplicacion.Services.Finanzas
{
    public interface IFinanzasApplicationService
    {
        Task<BatchDTO> ObtenerOBuscarBatchActivoAsync(BatchRequest request);
    }
}
