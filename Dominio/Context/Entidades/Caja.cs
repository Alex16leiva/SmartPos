using Dominio.Context.Entidades.Finanzas;
using Dominio.Core;
using Dominio.Core.Extensions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Context.Entidades
{
    public class Caja : Entity
    {
        public int Id { get; set; }
        public int CajaId { get; set; }
        public string Descripcion { get; set; }
        public string BatchInicial { get; set; }
        public string BatchId { get; set; }
        public string NombreImpresora1 { get; set; }
        public bool HabilitarImpresora1 { get; set; }
        public string NombreImpresora2 { get; set; }
        public bool HabilitarImpresora2 { get; set; }
        public string NombreCashDrawer { get; set; }
        public bool HabilitarCashDrawer { get; set; }
        [ForeignKey("BatchId")]
        public virtual ICollection<Batch> Batches { get; set; }

        public void AgregarBatch(Batch nuevoBatch)
        {
            if (Batches.IsNull())
            {
                Batches = [];
            }

            Batches.Add(nuevoBatch);
        }

        public void ActualizarInfoBatch(string batchId)
        {
            if (BatchInicial.IsMissingValue())
            {
                BatchInicial = batchId;
            }
            BatchId = batchId;
        }
    }
}
