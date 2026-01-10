using Dominio.Context.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructura.Context.Mapping
{
    internal class ClienteMap : EntityMap<Cliente>
    {
        public override void Configure(EntityTypeBuilder<Cliente> builder)
        {
        
            builder.ToTable("Cliente", "dbo");
            builder.HasKey(r => r.NumeroCuenta);
            builder.Property(r => r.Id).HasColumnName("Id").IsRequired().HasComputedColumnSql();
            builder.Property(r => r.NumeroCuenta).HasColumnName("NumeroCuenta").IsRequired().HasMaxLength(30);
            builder.Property(r => r.Nombre).HasColumnName("Nombre").IsRequired().HasMaxLength(40);
            builder.Property(r => r.Apellido).HasColumnName("Apellido").HasMaxLength(50);
            builder.Property(r => r.Compania).HasColumnName("Compania").HasMaxLength(50);
            builder.Property(r => r.Direccion).HasColumnName("Direccion").HasMaxLength(50);
            builder.Property(r => r.Direccion2).HasColumnName("Direccion2").HasMaxLength(50);
            builder.Property(r => r.Pais).HasColumnName("Pais").HasMaxLength(20);
            builder.Property(r => r.Departamento).HasColumnName("Departamento").HasMaxLength(30);
            builder.Property(r => r.Ciudad).HasColumnName("Ciudad").HasMaxLength(50);
            builder.Property(r => r.Zip).HasColumnName("Zip").HasMaxLength(15);
            builder.Property(r => r.NumeroTelefono1).HasColumnName("NumeroTelefono1").HasMaxLength(30);
            builder.Property(r => r.NumeroTelefono2).HasColumnName("NumeroTelefono2").HasMaxLength(30);
            builder.Property(r => r.NumeroFax).HasColumnName("NumeroFax").HasMaxLength(30);
            builder.Property(r => r.CorreoElectronico).HasColumnName("CorreoElectronico").HasMaxLength(255);
            builder.Property(r => r.TipoCuentaID).HasColumnName("TipoCuentaID");
            builder.Property(r => r.AperturaCuenta).HasColumnName("AperturaCuenta");
            builder.Property(r => r.LimiteCredito).HasColumnName("LimiteCredito").HasPrecision(18, 2);
            builder.Property(r => r.SaldoCuenta).HasColumnName("SaldoCuenta").HasPrecision(18, 2);
            builder.Property(r => r.Empleado).HasColumnName("Empleado");
            builder.Property(r => r.NivelPrecio).HasColumnName("NivelPrecio");
            builder.Property(r => r.DescuentoActual).HasColumnName("DescuentoActual").HasPrecision(18, 2);
            builder.Property(r => r.FotoRuta).HasColumnName("FotoRuta").HasMaxLength(120);
            builder.Property(r => r.UltimaVisita).HasColumnName("UltimaVisita");
            builder.Property(r => r.TotalVisitas).HasColumnName("TotalVisitas");
            builder.Property(r => r.VentasTotales).HasColumnName("VentasTotales").HasPrecision(18, 2);
            builder.Property(r => r.AhorrosTotales).HasColumnName("AhorrosTotales").HasPrecision(18, 2);
            builder.Property(r => r.Notas).HasColumnName("Notas").HasMaxLength(255);
            builder.Property(r => r.EnvioPrimarioId).HasColumnName("EnvioPrimarioId");
            builder.Property(r => r.FechaPersonalizada1).HasColumnName("FechaPersonalizada1");
            builder.Property(r => r.FechaPersonalizada2).HasColumnName("FechaPersonalizada2");
            builder.Property(r => r.FechaPersonalizada3).HasColumnName("FechaPersonalizada3");
            builder.Property(r => r.FechaPersonalizada4).HasColumnName("FechaPersonalizada4");
            builder.Property(r => r.FechaPersonalizada5).HasColumnName("FechaPersonalizada5");
            builder.Property(r => r.NumeroPersonalizado1).HasColumnName("NumeroPersonalizado1");
            builder.Property(r => r.NumeroPersonalizado2).HasColumnName("NumeroPersonalizado2");
            builder.Property(r => r.NumeroPersonalizado3).HasColumnName("NumeroPersonalizado3");
            builder.Property(r => r.NumeroPersonalizado4).HasColumnName("NumeroPersonalizado4");
            builder.Property(r => r.NumeroPersonalizado5).HasColumnName("NumeroPersonalizado5");
            builder.Property(r => r.TextoPersonalizado1).HasColumnName("TextoPersonalizado1").HasMaxLength(30);
            builder.Property(r => r.TextoPersonalizado2).HasColumnName("TextoPersonalizado2").HasMaxLength(30);
            builder.Property(r => r.TextoPersonalizado3).HasColumnName("TextoPersonalizado3").HasMaxLength(30);
            builder.Property(r => r.TextoPersonalizado4).HasColumnName("TextoPersonalizado4").HasMaxLength(30);
            builder.Property(r => r.TextoPersonalizado5).HasColumnName("TextoPersonalizado5").HasMaxLength(30);

            builder.HasOne(r => r.TipoCuenta).WithMany(r => r.Cliente).HasForeignKey(r => r.TipoCuentaID);
            
            base.Configure(builder);
        }
    }
}
