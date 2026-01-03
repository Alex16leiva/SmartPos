using Dominio.Context.Entidades.Articulos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Context.Mapping.Articulos
{
    internal class InventarioMovimientoMap : EntityMap<InventarioMovimiento>
    {
        public override void Configure(EntityTypeBuilder<InventarioMovimiento> builder)
        {
            builder.ToTable("InventarioMovimiento");
            builder.HasKey(e => e.InventarioMovimientoId);
            builder.Property(e => e.InventarioMovimientoId).IsRequired().HasComputedColumnSql();
            builder.Property(e => e.ArticuloId).HasMaxLength(50).IsRequired();
            builder.Property(e => e.CantidadAnterior).HasColumnType("decimal(18,4)");
            builder.Property(e => e.CantidadMovimiento).HasColumnType("decimal(18,4)");
            builder.Property(e => e.CantidadNueva).HasColumnType("decimal(18,4)");
            builder.Property(e => e.CostoUnitario).HasColumnType("decimal(18,4)");
            builder.Property(e => e.TipoMovimiento).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Referencia).HasMaxLength(100);
            builder.Property(e => e.Notas).HasMaxLength(500);
            // Mapeo de relaciones
            builder.HasOne(e => e.Articulo)
                .WithMany(a => a.InventarioMovimiento)
                .HasForeignKey(e => e.ArticuloId);
        }
    }
}
