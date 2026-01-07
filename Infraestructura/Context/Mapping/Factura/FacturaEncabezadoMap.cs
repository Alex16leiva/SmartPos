using Dominio.Context.Entidades.FacturaAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Context.Mapping.Factura
{
    internal class FacturaEncabezadoMap : EntityMap<FacturaEncabezado>
    {
        public override void Configure(EntityTypeBuilder<FacturaEncabezado> builder)
        {
            builder.HasKey(r => r.FacturaId);
            builder.ToTable("FacturaEncabezado", "dbo");
            builder.Property(r => r.FacturaId).HasColumnName("FacturaId").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.BatchId).HasColumnName("BatchId").IsRequired().IsUnicode(false).HasMaxLength(25);
            builder.Property(r => r.CajaId).HasColumnName("CajaId").IsRequired();
            builder.Property(r => r.ClienteId).HasColumnName("ClienteId").IsRequired().IsUnicode(false).HasMaxLength(25);
            builder.Property(r => r.Total).HasColumnName("Total");
            builder.Property(r => r.Impuesto).HasColumnName("Impuesto");
            builder.Property(r => r.Comentario).HasColumnName("Comentario").IsRequired().IsUnicode(false).HasMaxLength(225);
            builder.Property(r => r.CampoPersonalizado1).HasColumnName("CampoPersonalizado1").IsRequired().IsUnicode(false).HasMaxLength(70);
            builder.Property(r => r.CampoPersonalizado2).HasColumnName("CampoPersonalizado2").IsRequired().IsUnicode(false).HasMaxLength(70);
            builder.Property(r => r.LLamadaId).HasColumnName("LLamadaId").IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.LlamadaTipo).HasColumnName("LlamadaTipo").IsRequired().IsUnicode(false).HasMaxLength(40);
            builder.Property(r => r.CAI).HasColumnName("CAI").IsUnicode(false).HasMaxLength(70);
            builder.Property(r => r.Correlativo).HasColumnName("Correlativo").IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.FechaCreacion).HasColumnName("FechaCreacion");

            builder.Property(r => r.NumeroReferencia).HasColumnName("NumeroReferencia").IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.CajeroId).HasColumnName("CajeroId");

            //builder.(r => r.Cliente).WithMany(r => r.FacturaEncabezado).HasForeignKey(r => r.ClienteId);
            base.Configure(builder);
        }
    }
}
