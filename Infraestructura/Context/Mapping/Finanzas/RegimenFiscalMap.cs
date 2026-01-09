using Dominio.Context.Entidades.Finanzas;
using Infraestructura.Context.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Contexto.Mapping.Finanzas
{
    class RegimenFiscalMap : EntityMap<RegimenFiscal>
    {
        public override void Configure(EntityTypeBuilder<RegimenFiscal> builder)
        {
            builder.ToTable("RegimenFiscal", "Finanzas");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("Id").IsRequired().HasComputedColumnSql();
            builder.Property(r => r.Sucursal).HasColumnName("Sucursal").IsRequired().IsUnicode(false).HasMaxLength(6);
            builder.Property(r => r.CAI).HasColumnName("CAI").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Desde).HasColumnName("Desde").IsRequired();
            builder.Property(r => r.Hasta).HasColumnName("Hasta").IsRequired();
            builder.Property(r => r.CorrelativoActual).HasColumnName("CorrelativoActual").IsRequired();
            builder.Property(r => r.CantidadCaracteres).HasColumnName("CantidadCaracteres").IsRequired();
            builder.Property(r => r.Activo).HasColumnName("Activo").IsRequired();
            builder.Property(r => r.FechaLimiteEmision).HasColumnName("FechaLimiteEmision").IsRequired();
        
            base.Configure(builder);
        }
    }
}
