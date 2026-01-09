using Dominio.Context.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Context.Mapping
{
    internal class CajaMap : EntityMap<Caja>
    {
        public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Caja> builder)
        {
            builder.HasKey(r => r.CajaId);
            builder.ToTable("Caja", "dbo");
            builder.Property(r => r.Id).HasColumnName("Id").IsRequired().HasComputedColumnSql();
            builder.Property(r => r.CajaId).HasColumnName("CajaId").IsRequired();
            builder.Property(r => r.Descripcion).HasColumnName("Descripcion").IsRequired().IsUnicode(false).HasMaxLength(25);
            builder.Property(r => r.BatchInicial).HasColumnName("BatchInicial").IsRequired().IsUnicode(false).HasMaxLength(25);
            builder.Property(r => r.BatchId).HasColumnName("BatchId").IsRequired().IsUnicode(false).HasMaxLength(25);
            builder.Property(r => r.NombreImpresora1).HasColumnName("NombreImpresora1").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.HabilitarImpresora1).HasColumnName("HabilitarImpresora1").IsRequired();
            builder.Property(r => r.NombreImpresora2).HasColumnName("NombreImpresora2").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.HabilitarImpresora2).HasColumnName("HabilitarImpresora2").IsRequired();
            builder.Property(r => r.NombreCashDrawer).HasColumnName("NombreCashDrawer").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.HabilitarCashDrawer).HasColumnName("HabilitarCashDrawer").IsRequired();
        
            base.Configure(builder);
        }
    }
}
