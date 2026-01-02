using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dominio.Core
{
    public abstract class Entity
    {
        protected Entity()
        {
        }

        public Entity(string modificadoPor)
        {
            ModificadoPor = modificadoPor;
        }

        public string? ModificadoPor { get; set; }
        public DateTime FechaTransaccion { get; set; }
        public string DescripcionTransaccion { get; set; }
        [Timestamp] // Esto le dice a EF: "No incluyas esta columna en los INSERT o UPDATE"
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)] // Refuerza que SQL la calcula
        public byte[] RowVersion { get; set; }
        public Guid TransaccionUId { get; set; }
        public string TipoTransaccion { get; set; }
    }
}
