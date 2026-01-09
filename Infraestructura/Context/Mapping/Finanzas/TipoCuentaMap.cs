using Dominio.Context.Entidades.Finanzas;
using Infraestructura.Context.Mapping;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Contexto.Mapping.Finanzas
{
    internal class TipoCuentaMap : EntityMap<TipoCuenta>
    {
        public override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<TipoCuenta> builder)
        {
            builder.ToTable("TipoCuenta", "Finanzas");
            builder.HasKey(r => r.TipoCuentaId);
            builder.Property(r => r.TipoCuentaId).HasColumnName("TipoCuentaId").IsRequired().HasComputedColumnSql();
            builder.Property(r => r.Codigo).HasColumnName("Codigo").HasMaxLength(17);
            builder.Property(r => r.Descripcion).HasColumnName("Descripcion").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.FechaDeVencimiento).HasColumnName("FechaDeVencimiento");
            builder.Property(r => r.PeriodoDeGracia).HasColumnName("PeriodoDeGracia");
            builder.Property(r => r.CargoFinancieroMinimo).HasColumnName("CargoFinancieroMinimo");
            builder.Property(r => r.TasaDeInteresAnual).HasColumnName("TasaDeInteresAnual");
            builder.Property(r => r.AplicarCargosFinancierosEnCargosFinancieros).HasColumnName("AplicarCargosFinancierosEnCargosFinancieros");
            builder.Property(r => r.PagoMinimo).HasColumnName("PagoMinimo");
            builder.Property(r => r.EsCredito).HasColumnName("EsCredito");
        
            base.Configure(builder);
        }
    }
}