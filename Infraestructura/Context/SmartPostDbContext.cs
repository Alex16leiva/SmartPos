using Dominio.Context.Entidades;
using Dominio.Context.Entidades.Articulos;
using Dominio.Context.Entidades.FacturaAgg;
using Dominio.Context.Entidades.Finanzas;
using Dominio.Context.Entidades.Seguridad;
using Dominio.Core;
using Infraestructura.Context.Mapping;
using Infraestructura.Context.Mapping.Articulos;
using Infraestructura.Context.Mapping.Factura;
using Infraestructura.Context.Mapping.Seguridad;
using Infraestructura.Context.Mapping.Vendedores;
using Infraestructura.Contexto.Mapping.Factura;
using Infraestructura.Contexto.Mapping.Finanzas;
using Infraestructura.Core;
using Microsoft.EntityFrameworkCore;
using SmartPos.Data.Mapping;

namespace Infraestructura.Context
{
    public class SmartPostDbContext : BCUnitOfWork, IDataContext
    {
        public SmartPostDbContext(DbContextOptions<SmartPostDbContext> context)
            : base(context)
        {
            Database.SetCommandTimeout((int)TimeSpan.FromSeconds(1).TotalSeconds);
        }

        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Rol> Rol {  get; set; }
        public virtual DbSet<Pantalla> Pantalla { get; set; }
        public virtual DbSet<Permisos> Permisos { get; set; }
        public virtual DbSet<Articulo> Artuculo { get; set; }
        public virtual DbSet<InventarioMovimiento> InventarioMovimiento { get; set; }
        public virtual DbSet<FacturaEncabezado> FacturaEncabezado { get; set; }
        public virtual DbSet<FacturaDetalle> FacturaDetalle { get; set; }
        public virtual DbSet<Vendedor> Vendedor { get; set; }
        public virtual Batch Batch { get; set; }
        public virtual Caja Caja { get; set; }
        public virtual Diario Diario { get; set; }
        public virtual CuentasPorCobrar CuentasPorCobrar { get; set; }
        public virtual FormaPagoDetalle FormaPagoDetalle { get; set; }
        public virtual CuentasPorCobrarHistorial CuentasPorCobrarHistorial { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual TipoCuenta TipoCuenta { get; set; }
        public virtual FormasPago FormasPago { get; set; }
        public virtual RegimenFiscal RegimenFiscal { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuarioMap());
            modelBuilder.ApplyConfiguration(new RolMap());
            modelBuilder.ApplyConfiguration(new PantallaMap());
            modelBuilder.ApplyConfiguration(new PermisosMap());
            modelBuilder.ApplyConfiguration(new ArticuloMap());
            modelBuilder.ApplyConfiguration(new InventarioMovimientoMap());
            modelBuilder.ApplyConfiguration(new FacturaEncabezadoMap());
            modelBuilder.ApplyConfiguration(new FacturaDetalleMap());
            modelBuilder.ApplyConfiguration(new VendedorMap());
            modelBuilder.ApplyConfiguration(new BatchMap());
            modelBuilder.ApplyConfiguration(new CajaMap());
            modelBuilder.ApplyConfiguration(new DiarioMap());
            modelBuilder.ApplyConfiguration(new CuentasPorCobrarMap());
            modelBuilder.ApplyConfiguration(new FormaPagoDetalleMap());
            modelBuilder.ApplyConfiguration(new CuentasPorCobrarHistorialMap());
            modelBuilder.ApplyConfiguration(new ClienteMap());
            modelBuilder.ApplyConfiguration(new TipoCuentaMap());
            modelBuilder.ApplyConfiguration(new FormasPagoMap());
            modelBuilder.ApplyConfiguration(new RegimenFiscalMap());

            base.OnModelCreating(modelBuilder);
        }


        public override void Commit(TransactionInfo transactionInfo)
        {
            base.Commit(transactionInfo);
        }
    }
}
