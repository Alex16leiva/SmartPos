using Dominio.Context.Entidades.Finanzas;
using Infraestructura.Context.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Contexto.Mapping.Finanzas
{
    internal class FormaPagoDetalleMap : EntityMap<FormaPagoDetalle>
    {
        public override void Configure(EntityTypeBuilder<FormaPagoDetalle> builder)
        {

            builder.ToTable("FormaPagoDetalle", "Finanzas");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("Id").IsRequired().HasComputedColumnSql();
            builder.Property(r => r.BatchId).HasColumnName("BatchId").IsRequired().IsUnicode(false).HasMaxLength(25);
            builder.Property(r => r.FacturaId).HasColumnName("FacturaId").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.FormaPagoId).HasColumnName("FormaPagoId").IsRequired();
            builder.Property(r => r.PagoId).HasColumnName("PagoId").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Descripcion).HasColumnName("Descripcion").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Monto).HasColumnName("Monto").IsRequired();
            builder.Property(r => r.MontoExtranjero).HasColumnName("MontoExtranjero").IsRequired();

            builder.HasOne(r => r.FacturaEncabezado).WithMany(r => r.FormaPagoDetalle).HasForeignKey(r => r.FacturaId);
        
            base.Configure(builder);
        }
    }
}