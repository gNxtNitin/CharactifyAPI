using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Charactify.API.DataModels
{
    public partial class CContext : DbContext
    {
        public CContext()
        {
        }

        public CContext(DbContextOptions<CContext> options)
            : base(options)
        {
        }
        public virtual DbSet<ApiLogs> ApiLogs { get; set; }
        public virtual DbSet<ApprovalsMaster> ApprovalsMaster { get; set; }
        public virtual DbSet<Configurations> Configurations { get; set; }
        public virtual DbSet<ConnectionMaster> ConnectionMaster { get; set; }
        public virtual DbSet<ConnectionRequest> ConnectionRequest { get; set; }
        public virtual DbSet<CountryMaster> CountryMaster { get; set; }
        public virtual DbSet<FeedMaster> FeedMaster { get; set; }
        public virtual DbSet<FeedReactions> FeedReactions { get; set; }
        public virtual DbSet<InvitesMaster> InvitesMaster { get; set; }
        public virtual DbSet<ScoreMaster> ScoreMaster { get; set; }
        public virtual DbSet<StateMaster> StateMaster { get; set; }
        public virtual DbSet<TraitsMaster> TraitsMaster { get; set; }
        public virtual DbSet<UserMasters> UserMaster { get; set; }

        // Unable to generate entity type for table 'dbo.UserEducationDetails'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.UserWorkDetails'. Please see the warning messages.
        // Unable to generate entity type for table 'dbo.UserAuthentication'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(CResources.GetConnectionString());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApprovalsMaster>(entity =>
            {
                entity.HasKey(e => e.ApprovalsId);

                entity.Property(e => e.ApprovalsId).HasColumnName("ApprovalsID");

                entity.Property(e => e.ApprovalDate).HasColumnType("datetime");

                entity.Property(e => e.ApprovalStatus).HasMaxLength(100);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.TraitsId).HasColumnName("TraitsID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<Configurations>(entity =>
            {
                entity.Property(e => e.ConfigurationsId).HasColumnName("ConfigurationsID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<ConnectionMaster>(entity =>
            {
                entity.HasKey(e => e.ConnectionId);

                entity.Property(e => e.ConnectionId).HasColumnName("ConnectionID");

                entity.Property(e => e.ConnectedDate).HasColumnType("datetime");

                entity.Property(e => e.ConnectionType).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FromUserId).HasColumnName("FromUserID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ToUserId).HasColumnName("ToUserID");
            });

            modelBuilder.Entity<ConnectionRequest>(entity =>
            {
                entity.HasKey(e => e.Crid);

                entity.Property(e => e.Crid).HasColumnName("CRID");

                entity.Property(e => e.ConnectionType).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FromUserId).HasColumnName("FromUserID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.RequestDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.ToUserId).HasColumnName("ToUserID");
            });

            modelBuilder.Entity<CountryMaster>(entity =>
            {
                entity.HasKey(e => e.CountryId);

                entity.Property(e => e.CountryId).HasColumnName("CountryID");

                entity.Property(e => e.CountryName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.IsoCode)
                    .IsRequired()
                    .HasColumnName("ISO_Code")
                    .HasMaxLength(3);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<FeedMaster>(entity =>
            {
                entity.HasKey(e => e.FeedId);

                entity.Property(e => e.FeedId).HasColumnName("FeedID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1500);

                entity.Property(e => e.FeedType).HasMaxLength(100);

                entity.Property(e => e.FilePath).HasMaxLength(1500);

                entity.Property(e => e.FileType).HasMaxLength(100);

                entity.Property(e => e.FromUserId).HasColumnName("FromUserID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ToUserId).HasColumnName("ToUserID");
            });

            modelBuilder.Entity<FeedReactions>(entity =>
            {
                entity.HasKey(e => e.ReactionId);

                entity.Property(e => e.ReactionId).HasColumnName("ReactionID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1500);

                entity.Property(e => e.FeedId).HasColumnName("FeedID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ReactionType).HasMaxLength(100);
            });

            modelBuilder.Entity<InvitesMaster>(entity =>
            {
                entity.HasKey(e => e.InvitesId);

                entity.Property(e => e.InvitesId).HasColumnName("InvitesID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.InviteReSentDate).HasColumnType("datetime");

                entity.Property(e => e.InviteSentDate).HasColumnType("datetime");

                entity.Property(e => e.InviteVia).HasMaxLength(100);

                entity.Property(e => e.InviteViaId)
                    .HasColumnName("InviteViaID")
                    .HasMaxLength(100);

                entity.Property(e => e.InvitedEmailId)
                    .HasColumnName("InvitedEmailID")
                    .HasMaxLength(150);

                entity.Property(e => e.InvitedName).HasMaxLength(300);

                entity.Property(e => e.InviteeId).HasColumnName("InviteeID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<ScoreMaster>(entity =>
            {
                entity.HasKey(e => e.ScoreId);

                entity.Property(e => e.ScoreId).HasColumnName("ScoreID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Score).HasColumnType("numeric(10, 2)");

                entity.Property(e => e.ScoredDate).HasColumnType("datetime");

                entity.Property(e => e.TraitsId).HasColumnName("TraitsID");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<StateMaster>(entity =>
            {
                entity.HasKey(e => e.StateId);

                entity.Property(e => e.StateId).HasColumnName("StateID");

                entity.Property(e => e.CountryId).HasColumnName("CountryID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.StateCode)
                    .IsRequired()
                    .HasMaxLength(5);

                entity.Property(e => e.StateName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.StateMaster)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StateMaster_CountryMaster");
            });

            modelBuilder.Entity<TraitsMaster>(entity =>
            {
                entity.HasKey(e => e.TraitsId);

                entity.Property(e => e.TraitsId).HasColumnName("TraitsID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MaximumScore).HasColumnType("numeric(10, 2)");

                entity.Property(e => e.MinimumScore).HasColumnType("numeric(10, 2)");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.TraitName)
                    .IsRequired()
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<UserMasters>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Address1)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Address2).HasMaxLength(150);

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.CountryId).HasColumnName("CountryID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedVia).HasMaxLength(100);

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.EmailId)
                    .IsRequired()
                    .HasColumnName("EmailID")
                    .HasMaxLength(150);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.LastLoggedIn).HasColumnType("datetime");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.StateId).HasColumnName("StateID");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.UserProfilePic).HasMaxLength(1000);

                entity.Property(e => e.ViaId).HasMaxLength(250);

                entity.Property(e => e.Zip)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(d => d.Country)
                    .WithMany(p => p.UserMaster)
                    .HasForeignKey(d => d.CountryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserMaster_CountryMaster");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.UserMaster)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserMaster_StateMaster");
            });
        }
    }
}
