//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace yazilimYapimi.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class yazilimyapimiEntities : DbContext
    {
        public yazilimyapimiEntities()
            : base("name=yazilimyapimiEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<kullanici> kullanici { get; set; }
        public virtual DbSet<ourunler> ourunler { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<urunler> urunler { get; set; }
        public virtual DbSet<urunSiparis> urunSiparis { get; set; }
        public virtual DbSet<logSatis> logSatis { get; set; }
        public virtual DbSet<obakiye> obakiye { get; set; }
    }
}
