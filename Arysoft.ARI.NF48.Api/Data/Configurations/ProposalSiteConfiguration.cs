using Arysoft.ARI.NF48.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Arysoft.ARI.NF48.Api.Data.Configurations
{
    public class ProposalSiteConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProposalSite>()
                .ToTable("ProposalSites")
                .HasKey(m => m.ID);

            modelBuilder.Entity<ProposalSite>()
                .Property(m => m.ID)
                .HasColumnName("ProposalSiteID");

            modelBuilder.Entity<ProposalSite>()
                .Property(m => m.ProposalID)
                .IsRequired();

            modelBuilder.Entity<ProposalSite>()
                .Property(m => m.SiteID)
                .IsRequired();

            modelBuilder.Entity<ProposalSite>()
                .Property(m => m.CertificateIssue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProposalSite>()
                .Property(m => m.TotalCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<ProposalSite>()
                .Property(m => m.AuditSteps)
                .HasMaxLength(500);

            modelBuilder.Entity<ProposalSite>()
                .Property(m => m.Status)
                .IsRequired();

            modelBuilder.Entity<ProposalSite>()
                .Property(m => m.Created)
                .IsRequired();

            modelBuilder.Entity<ProposalSite>()
                .Property(m => m.Updated)
                .IsRequired();

            modelBuilder.Entity<ProposalSite>()
                .Property(m => m.UpdatedUser)
                .HasMaxLength(50)
                .IsRequired();

            // RELATIONS


        }
    }
}