using Dominio.Context.Entidades.Finanzas;
using Infraestructura.Context.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Contexto.Mapping.Finanzas
{
    internal class BatchMap : EntityMap<Batch>
    {
        public override void Configure(EntityTypeBuilder<Batch> builder)
        {
         
        
            builder.HasKey(r => r.BatchId);
            builder.ToTable("Batch", "Finanzas");
            builder.Property(r => r.BatchId).HasColumnName("BatchId").IsRequired().IsUnicode(false).HasMaxLength(25);
            builder.Property(r => r.CajaId).HasColumnName("CajaId").IsRequired();
            builder.Property(r => r.Estado).HasColumnName("Estado").IsRequired();
            builder.Property(r => r.FechaApertura).HasColumnName("FechaApertura");
            builder.Property(r => r.FechaCierre).HasColumnName("FechaCierre");
            builder.Property(r => r.CierreTotal).HasColumnName("CierreTotal");
            builder.Property(r => r.SubTotal).HasColumnName("SubTotal");
            builder.Property(r => r.Impuesto).HasColumnName("Impuesto");
            builder.Property(r => r.TotalVenta).HasColumnName("TotalVenta");
            builder.Property(r => r.VentasCredito).HasColumnName("VentasCredito");
            builder.Property(r => r.PagoACuenta).HasColumnName("PagoACuenta");
            builder.Property(r => r.Devoluciones).HasColumnName("Devoluciones");
            builder.Property(r => r.CantidadClientes).HasColumnName("CantidadClientes");
            builder.Property(r => r.TotalFormaDePago).HasColumnName("TotalFormaDePago");
            builder.Property(r => r.CambioTotal).HasColumnName("CambioTotal");
            builder.Property(r => r.Descuento).HasColumnName("Descuento");
            builder.Property(r => r.CostoTotal).HasColumnName("CostoTotal");
            builder.Property(r => r.Comision).HasColumnName("Comision");

            builder.HasOne(r => r.Caja).WithMany(r => r.Batches).HasForeignKey(r => r.CajaId);
        
            base.Configure(builder);
        }
    }
}