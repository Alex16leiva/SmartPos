using Dominio.Context.Entidades;
using Infraestructura.Context.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infraestructura.Contexto.Mapping
{
    internal class ConfiguracionTiendaMap : EntityMap<ConfiguracionTienda>
    {
        public override void Configure(EntityTypeBuilder<ConfiguracionTienda> builder)
        {

            builder.ToTable("ConfiguracionTienda", "Comunes");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id).HasColumnName("Id").IsRequired().HasComputedColumnSql();
            builder.Property(r => r.Nombre).HasColumnName("Nombre").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Pais).HasColumnName("Pais").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Estado).HasColumnName("Estado").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Ciudad).HasColumnName("Ciudad").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.RTN).HasColumnName("RTN").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Direccion1).HasColumnName("Direccion1").IsRequired().IsUnicode(false).HasMaxLength(100);
            builder.Property(r => r.Direccion2).HasColumnName("Direccion2").IsUnicode(false).HasMaxLength(100);
            builder.Property(r => r.CodigoZip).HasColumnName("CodigoZip").IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Telefono1).HasColumnName("Telefono1").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Telefono2).HasColumnName("Telefono2").IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Telefono3).HasColumnName("Telefono3").IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.CorreoElectronico).HasColumnName("CorreoElectronico").IsRequired().IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Personalizado1).HasColumnName("Personalizado1").IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Personalizado2).HasColumnName("Personalizado2").IsUnicode(false).HasMaxLength(50);
            builder.Property(r => r.Personalizado3).HasColumnName("Personalizado3").IsUnicode(false).HasMaxLength(50);
            Property(r => r.Personalizado4).HasColumnName("Personalizado4").IsUnicode(false).HasMaxLength(50);
        
            base.Configure(builder);
        }
    }
}