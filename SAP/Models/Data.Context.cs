﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SAP.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SAPEntities : DbContext
    {
        public SAPEntities()
            : base("name=SAPEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AddToSurvey> AddToSurvey { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Links> Links { get; set; }
        public virtual DbSet<OfflineAnswer> OfflineAnswer { get; set; }
        public virtual DbSet<OfflineQuestion> OfflineQuestion { get; set; }
        public virtual DbSet<OfflineSurvey> OfflineSurvey { get; set; }
        public virtual DbSet<OfflineValues> OfflineValues { get; set; }
        public virtual DbSet<OnlineAnswer> OnlineAnswer { get; set; }
        public virtual DbSet<OnlineQuestion> OnlineQuestion { get; set; }
        public virtual DbSet<OnlineSurvey> OnlineSurvey { get; set; }
        public virtual DbSet<OnlineValues> OnlineValues { get; set; }
        public virtual DbSet<PaperSurvey> PaperSurvey { get; set; }
        public virtual DbSet<SendsSurvey> SendsSurvey { get; set; }
        public virtual DbSet<Survey> Survey { get; set; }
        public virtual DbSet<SurveyType> SurveyType { get; set; }
        public virtual DbSet<UserTokens> UserTokens { get; set; }
    }
}
