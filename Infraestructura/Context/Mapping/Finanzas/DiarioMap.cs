using Dominio.Context.Entidades.Finanzas;
using Infraestructura.Context.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infraestructura.Contexto.Mapping.Finanzas
{
    internal class DiarioMap : EntityMap<Diario>
    {
        public override void Configure(EntityTypeBuilder<Diario> builder)
        {

            builder.ToTable("Diario", "Finanzas");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("Id").IsRequired().HasComputedColumnSql();
            builder.Property(r => r.TipoTransaccionId).HasColumnName("TipoTransaccionId").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Referencia).HasColumnName("Referencia").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.CajaId).HasColumnName("CajaId").IsRequired();
            builder.Property(r => r.BatchId).HasColumnName("BatchId").IsRequired().IsUnicode(false).HasMaxLength(25);

            builder.HasOne(r => r.Batch).WithMany(r => r.Diario).HasForeignKey(r => r.BatchId);

            builder.HasOne(r => r.FacturaEncabezado).WithMany(r => r.Diario).HasForeignKey(r => r.Referencia);
        
            base.Configure(builder);
        }
    }
}