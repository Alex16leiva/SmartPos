using Dominio.Context.Entidades.FacturaAgg;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Context.Mapping.Factura
{
    internal class FacturaDetalleMap : EntityMap<FacturaDetalle>
    {
        public override void Configure(EntityTypeBuilder<FacturaDetalle> builder)
        {
            builder.ToTable("FacturaDetalle", "dbo");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id).HasColumnName("Id").IsRequired().HasComputedColumnSql();
            builder.Property(r => r.FacturaId).HasColumnName("FacturaId").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.ArticuloId).HasColumnName("ArticuloId").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Precio).HasColumnName("Precio");
            builder.Property(r => r.PrecioFinal).HasColumnName("PrecioFinal");
            builder.Property(r => r.Costo).HasColumnName("Costo");
            builder.Property(r => r.Cantidad).HasColumnName("Cantidad");
            builder.Property(r => r.Impuesto).HasColumnName("Impuesto");
            builder.Property(r => r.VendedorId).HasColumnName("VendedorId");
            builder.Property(r => r.Comision).HasColumnName("Comision");
            builder.Property(r => r.Comentario).HasColumnName("Comentario");
            builder.Property(r => r.Descuento).HasColumnName("Descuento");

            builder.HasOne(r => r.FacturaEncabezado).WithMany(r => r.FacturaDetalle).HasForeignKey(r => r.FacturaId);
            builder.HasOne(r => r.Articulo).WithMany(r => r.FacturaDetalle).HasForeignKey(r => r.ArticuloId);
            base.Configure(builder);
        }
    }
}
