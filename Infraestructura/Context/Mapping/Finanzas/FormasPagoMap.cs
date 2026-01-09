using Dominio.Context.Entidades.Finanzas;
using Infraestructura.Context.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Contexto.Mapping.Factura
{
    internal class FormasPagoMap : EntityMap<FormasPago>
    {
        public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<FormasPago> builder)
        {
            builder.ToTable("FormasPago", "Finanzas");
            builder.HasKey(r => r.FormaPagoId);

            builder.Property(r => r.FormaPagoId).HasColumnName("FormaPagoId").IsRequired().HasComputedColumnSql();
            builder.Property(r => r.Codigo).HasColumnName("Codigo").IsRequired().IsUnicode(false).HasMaxLength(20);
            builder.Property(r => r.Descripcion).HasColumnName("Descripcion").IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(r => r.TipoPago).HasColumnName("TipoPago");
            builder.Property(r => r.OrdenMostrar).HasColumnName("OrdenMostrar");
        
            base.Configure(builder);
        }
    }
}