//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SMGJ.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SMGJEntities : DbContext
    {
        public SMGJEntities()
            : base("name=SMGJEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AUTORIZIMET> AUTORIZIMETs { get; set; }
        public virtual DbSet<FERMA> FERMAs { get; set; }
        public virtual DbSet<GJEDHAT_PARAMETRAT> GJEDHAT_PARAMETRAT { get; set; }
        public virtual DbSet<GJEDHI> GJEDHIs { get; set; }
        public virtual DbSet<KOMUNA> KOMUNAs { get; set; }
        public virtual DbSet<KONTAMINIMI> KONTAMINIMIs { get; set; }
        public virtual DbSet<LAGESHTIA_AJRIT> LAGESHTIA_AJRIT { get; set; }
        public virtual DbSet<Medium> MEDIA { get; set; }
        public virtual DbSet<MENU> MENUs { get; set; }
        public virtual DbSet<MENU_ROLI> MENU_ROLI { get; set; }
        public virtual DbSet<NJOFTIMET> NJOFTIMETs { get; set; }
        public virtual DbSet<NJOFTIMET_USER> NJOFTIMET_USER { get; set; }
        public virtual DbSet<QUMESHTI> QUMESHTIs { get; set; }
        public virtual DbSet<QUMESHTI_DETAJET> QUMESHTI_DETAJET { get; set; }
        public virtual DbSet<RACA> RACAs { get; set; }
        public virtual DbSet<RRAHJET_ZEMRES> RRAHJET_ZEMRES { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TEMPERATURA> TEMPERATURAs { get; set; }
        public virtual DbSet<TIPI> TIPIs { get; set; }
        public virtual DbSet<USER> USERs { get; set; }
        public virtual DbSet<YNDYRA> YNDYRAs { get; set; }
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
    }
}
