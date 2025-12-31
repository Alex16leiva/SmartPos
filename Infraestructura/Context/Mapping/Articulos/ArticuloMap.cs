using Dominio.Context.Entidades.Articulos;
using Infraestructura.Context.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SmartPos.Data.Mapping
{
    internal class ArticuloMap : EntityMap<Articulo>
    {
        public override void Configure(EntityTypeBuilder<Articulo> builder)
        {
            builder.ToTable("Articulo");
            builder.HasKey(e => e.ArticuloId);
            builder.Property(e => e.ArticuloId)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(e => e.DescripcionExtendida);
            builder.Property(e => e.Cantidad)
                .HasColumnType("decimal(18,4)");
            builder.Property(e => e.Costo)
                .HasColumnType("decimal(18,4)");
            builder.Property(e => e.UltimoCosto)
                .HasColumnType("decimal(18,4)");
            builder.Property(e => e.Precio)
                .HasColumnType("decimal(18,4)");
            builder.Property(e => e.PrecioA)
                .HasColumnType("decimal(18,4)");
            builder.Property(e => e.PrecioB)
                .HasColumnType("decimal(18,4)");
            builder.Property(e => e.PrecioC)
                .HasColumnType("decimal(18,4)");
            builder.Property(e => e.PrecioD)
                .HasColumnType("decimal(18,4)");
            builder.Property(e => e.PrecioE)
                .HasColumnType("decimal(18,4)");
            // Mapeo de relaciones y otras propiedades se pueden agregar aquí
        }
    }
}
