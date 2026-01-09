using Dominio.Context.Entidades.Finanzas;
using Infraestructura.Context.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Contexto.Mapping.Finanzas
{
    internal class CuentasPorCobrarHistorialMap : EntityMap<CuentasPorCobrarHistorial>
    {
        public override void Configure(EntityTypeBuilder<CuentasPorCobrarHistorial> builder)
        {

            builder.ToTable("CuentasPorCobrarHistorial", "Finanzas");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("Id").IsRequired().HasComputedColumnSql();
            builder.Property(r => r.CuentaPorCobrarID).HasColumnName("CuentaPorCobrarID");
            builder.Property(r => r.BatchId).HasColumnName("BatchId");
            builder.Property(r => r.Monto).HasColumnName("Monto");
            builder.Property(r => r.PagoID).HasColumnName("PagoID");
            builder.Property(r => r.Comentario).HasColumnName("Comentario");
            builder.Property(r => r.Fecha).HasColumnName("Fecha");
            builder.Property(r => r.TipoDeHistorico).HasColumnName("TipoDeHistorico");

            builder.HasOne(r => r.Batch).WithMany(r => r.CuentasPorCobrarHistorial).HasForeignKey(r => r.BatchId);

            builder.HasOne(r => r.CuentasPorCobrar).WithMany(r => r.CuentasPorCobrarHistorial).HasForeignKey(r => r.CuentaPorCobrarID);
            base.Configure(builder);
        }
    }
}