using Dominio.Context.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infraestructura.Context.Mapping.Vendedores
{
    internal class VendedorMap : EntityMap<Vendedor>
    {
        public override void Configure(EntityTypeBuilder<Vendedor> builder)
        {
            builder.HasKey(r => r.VendedorId);
            builder.ToTable("Vendedor", "dbo");
            builder.Property(r => r.VendedorId).HasColumnName("VendedorId").IsRequired().HasComputedColumnSql();
            builder.Property(r => r.Codigo).HasColumnName("Codigo").IsRequired().HasMaxLength(25);
            builder.Property(r => r.Nombre).HasColumnName("Nombre").IsRequired().IsUnicode(false).HasMaxLength(25);
            builder.Property(r => r.Apellido).HasColumnName("Apellido").IsRequired().IsUnicode(false).HasMaxLength(25);
            builder.Property(r => r.Telefono).HasColumnName("Telefono").IsUnicode(false).HasMaxLength(30);
            base.Configure(builder);
        }
    }
}
