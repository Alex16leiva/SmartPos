using Aplicacion.DTOs.Finanzas;

namespace Aplicacion.Services.Finanzas
{
    public interface IFinanzasApplicationService
    {
        Task<BatchDTO> CrearBatch(BatchRequest request);
        Task<BatchDTO> ObtenerBatch(BatchRequest request);
    }
}
