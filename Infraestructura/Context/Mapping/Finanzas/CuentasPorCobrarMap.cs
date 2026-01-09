using Dominio.Context.Entidades.Finanzas;
using Infraestructura.Context.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Contexto.Mapping.Finanzas
{
    internal class CuentasPorCobrarMap : EntityMap<CuentasPorCobrar>
    {
        public override void Configure(EntityTypeBuilder<CuentasPorCobrar> builder)
        {

            builder.ToTable("CuentasPorCobrar", "Finanzas");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("Id").IsRequired().HasComputedColumnSql();
            builder.Property(r => r.NumeroCuenta).HasColumnName("NumeroCuenta").IsRequired().IsUnicode(false).HasMaxLength(30);
            builder.Property(r => r.Fecha).HasColumnName("Fecha");
            builder.Property(r => r.FechaDeVencimiento).HasColumnName("FechaDeVencimiento");
            builder.Property(r => r.CantidadOriginal).HasColumnName("CantidadOriginal");
            builder.Property(r => r.NumeroFactura).HasColumnName("NumeroFactura").HasMaxLength(50);
            builder.Property(r => r.Tipo).HasColumnName("Tipo");
            builder.Property(r => r.Balance).HasColumnName("Balance");

            builder.HasOne(r => r.FacturaEncabezado).WithMany(r => r.CuentaPorCobrar).HasForeignKey(r => r.NumeroFactura);
        
            base.Configure(builder);
        }
    }
}