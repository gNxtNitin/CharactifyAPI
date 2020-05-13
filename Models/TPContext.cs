using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Charactify.API.Models
{
    public partial class TPContext : DbContext
    {
        public TPContext()
        {
        }

        public TPContext(DbContextOptions<TPContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApiLogs> ApiLogs { get; set; }
        public virtual DbSet<ApprovalsMaster> ApprovalsMaster { get; set; }
        public virtual DbSet<CategoryScoreHistory> CategoryScoreHistory { get; set; }
        public virtual DbSet<Configurations> Configurations { get; set; }
        public virtual DbSet<ConnectionMaster> ConnectionMaster { get; set; }
        public virtual DbSet<ConnectionRequest> ConnectionRequest { get; set; }
        public virtual DbSet<ConRequest> ConRequest { get; set; }
        public virtual DbSet<CountryMaster> CountryMaster { get; set; }
        public virtual DbSet<FeedImagePath> FeedImagePath { get; set; }
        public virtual DbSet<FeedMaster> FeedMaster { get; set; }
        public virtual DbSet<FeedReactions> FeedReactions { get; set; }
        public virtual DbSet<InvitesMaster> InvitesMaster { get; set; }
        public virtual DbSet<RatingApprove> RatingApprove { get; set; }
        public virtual DbSet<ScoreHistory> ScoreHistory { get; set; }
        public virtual DbSet<ScoreMaster> ScoreMaster { get; set; }
        public virtual DbSet<ShareMaster> ShareMaster { get; set; }
        public virtual DbSet<StateMaster> StateMaster { get; set; }
        public virtual DbSet<StoryMaster> StoryMaster { get; set; }
        public virtual DbSet<Tagging> Tagging { get; set; }
        public virtual DbSet<TraitsMaster> TraitsMaster { get; set; }
        public virtual DbSet<TraitsScoreHistory> TraitsScoreHistory { get; set; }
        public virtual DbSet<UserAuthentication> UserAuthentication { get; set; }
        public virtual DbSet<UserEducationDetails> UserEducationDetails { get; set; }
        public virtual DbSet<UserMaster> UserMaster { get; set; }
        public virtual DbSet<UserPrivacyDetails> UserPrivacyDetails { get; set; }
        public virtual DbSet<UserPrivacyDetails1> UserPrivacyDetails1 { get; set; }
        public virtual DbSet<UserWorkDetails> UserWorkDetails { get; set; }

        // Unable to generate entity type for table 'dbo.Notifications'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=217728;Database=DB_A3F3A5_Charactify;User Id=sa; Password=gNxt@123;");  // live
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApiLogs>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.MethodName)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

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

            modelBuilder.Entity<CategoryScoreHistory>(entity =>
            {
                entity.Property(e => e.AcquaintancesAvgScore).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CatId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CoWorkerAvgScore)
                    .HasColumnName("CO_WorkerAvgScore")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FamilyAvgScore).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.FriendAvgScore).HasColumnName("friendAvgScore");

                entity.Property(e => e.Modifieddate)
                    .HasColumnName("modifieddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.SelfAvgScore).HasColumnType("numeric(18, 2)");
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

                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ToUserId).HasColumnName("ToUserID");
            });

            modelBuilder.Entity<ConnectionRequest>(entity =>
            {
                entity.HasKey(e => e.Crid);

                entity.Property(e => e.Crid).HasColumnName("CRID");

                entity.Property(e => e.ConnectionId).HasColumnName("ConnectionID");

                entity.Property(e => e.ConnectionType).HasMaxLength(200);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FromUserId).HasColumnName("FromUserID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.RequestDate).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(100);

                entity.Property(e => e.ToUserId).HasColumnName("ToUserID");
            });

            modelBuilder.Entity<ConRequest>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedBy).HasColumnType("datetime");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FromUserId).HasColumnName("FromUserID");

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

            modelBuilder.Entity<FeedImagePath>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.Filter)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ImagePath).IsUnicode(false);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<FeedMaster>(entity =>
            {
                entity.HasKey(e => e.FeedId);

                entity.HasIndex(e => e.FeedId)
                    .HasName("FeedMaster_IDX");

                entity.Property(e => e.FeedId).HasColumnName("FeedID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.FeedType).HasMaxLength(100);

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

                entity.Property(e => e.FeedId).HasColumnName("FeedID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.ReactionTypeId).HasColumnName("ReactionTypeID");
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

                entity.Property(e => e.InvitedPhone).HasMaxLength(50);

                entity.Property(e => e.InviteeId).HasColumnName("InviteeID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<RatingApprove>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Crid).HasColumnName("CRID");

                entity.Property(e => e.FromUserId).HasColumnName("FromUserID");

                entity.Property(e => e.Modifydate).HasColumnType("datetime");

                entity.Property(e => e.Status)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.ToUserId).HasColumnName("ToUserID");
            });

            modelBuilder.Entity<ScoreHistory>(entity =>
            {
                entity.Property(e => e.AvgScore).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Createddate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Usercount).HasColumnName("usercount");
            });

            modelBuilder.Entity<ScoreMaster>(entity =>
            {
                entity.HasKey(e => e.ScoreId);

                entity.Property(e => e.ScoreId).HasColumnName("ScoreID");

                entity.Property(e => e.ConnectionReqId).HasColumnName("ConnectionReqID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Score).HasColumnType("numeric(10, 2)");

                entity.Property(e => e.ScoredDate).HasColumnType("datetime");

                entity.Property(e => e.TraitsId).HasColumnName("TraitsID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Weightedavg)
                    .HasColumnName("weightedavg")
                    .HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<ShareMaster>(entity =>
            {
                entity.HasKey(e => e.ShareId);

                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ShareMaster)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_ShareMaster_UserMaster");
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

            modelBuilder.Entity<StoryMaster>(entity =>
            {
                entity.HasKey(e => e.StoryId);

                entity.Property(e => e.StoryId).HasColumnName("StoryID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1500);

                entity.Property(e => e.FilePath).HasMaxLength(1500);

                entity.Property(e => e.FileType).HasMaxLength(100);

                entity.Property(e => e.FromUserId).HasColumnName("FromUserID");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.StoryType).HasMaxLength(100);

                entity.Property(e => e.ToUserId).HasColumnName("ToUserID");
            });

            modelBuilder.Entity<Tagging>(entity =>
            {
                entity.Property(e => e.CreatedBy)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");
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

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<TraitsScoreHistory>(entity =>
            {
                entity.Property(e => e.CourageousAvgScore).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Createddate)
                    .HasColumnName("createddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.FairAvgScore).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.ForgivingAvgScore).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.GenerousAvgScore).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.HonestAvgScore).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.HonorableAvgScore).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.LovingAvgScore).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.Modifieddate)
                    .HasColumnName("modifieddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.PoliteAvgScore).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.RespectfulAvgScore).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.TrustworthyAvgScore).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<UserAuthentication>(entity =>
            {
                entity.HasKey(e => e.UserAuthenticationId)
                    .ForSqlServerIsClustered(false);

                entity.Property(e => e.UserAuthenticationId).HasColumnName("UserAuthenticationID");

                entity.Property(e => e.Answer1).HasMaxLength(150);

                entity.Property(e => e.Answer2).HasMaxLength(150);

                entity.Property(e => e.Answer3).HasMaxLength(150);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.LastLogin).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ResetPasswordDate).HasColumnType("datetime");

                entity.Property(e => e.SecurityQuestion1).HasMaxLength(150);

                entity.Property(e => e.SecurityQuestion2).HasMaxLength(150);

                entity.Property(e => e.SecurityQuestion3).HasMaxLength(150);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserEducationDetails>(entity =>
            {
                entity.HasKey(e => e.UserSchoolId);

                entity.Property(e => e.UserSchoolId).HasColumnName("UserSchoolID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.FromDate).HasColumnType("date");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.SchoolName).HasMaxLength(250);

                entity.Property(e => e.ToDate).HasColumnType("date");

                entity.Property(e => e.TypeOfSchool).HasMaxLength(150);

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });

            modelBuilder.Entity<UserMaster>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Address1).HasMaxLength(150);

                entity.Property(e => e.Address2).HasMaxLength(150);

                entity.Property(e => e.AppUserName).HasMaxLength(500);

                entity.Property(e => e.City).HasMaxLength(100);

                entity.Property(e => e.CountryId).HasColumnName("CountryID");

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.CreatedVia).HasMaxLength(100);

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.Device)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.EmailId)
                    .HasColumnName("EmailID")
                    .HasMaxLength(150);

                entity.Property(e => e.FbuserName)
                    .HasColumnName("FBUserName")
                    .HasMaxLength(200);

                entity.Property(e => e.FirstName).HasMaxLength(150);

                entity.Property(e => e.Gender).HasMaxLength(10);

                entity.Property(e => e.GmailUserName)
                    .HasColumnName("GMailUserName")
                    .HasMaxLength(200);

                entity.Property(e => e.LastLoggedIn).HasColumnType("datetime");

                entity.Property(e => e.LastName).HasMaxLength(150);

                entity.Property(e => e.LoginKey).HasMaxLength(200);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Password).HasMaxLength(200);

                entity.Property(e => e.Phone).HasMaxLength(15);

                entity.Property(e => e.StateId).HasColumnName("StateID");

                entity.Property(e => e.Status).HasMaxLength(10);

                entity.Property(e => e.Type)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.UniqueId)
                    .HasColumnName("UniqueID")
                    .IsUnicode(false);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.UserToken).IsUnicode(false);

                entity.Property(e => e.VerificationCode)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ViaId).HasMaxLength(250);

                entity.Property(e => e.Zip).HasMaxLength(50);
            });

            modelBuilder.Entity<UserPrivacyDetails>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.UserPrivacyDetails)
                    .HasForeignKey<UserPrivacyDetails>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserPrivacyDetails_UserMaster");
            });

            modelBuilder.Entity<UserPrivacyDetails1>(entity =>
            {
                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");
            });

            modelBuilder.Entity<UserWorkDetails>(entity =>
            {
                entity.HasKey(e => e.UserEducationId);

                entity.Property(e => e.UserEducationId).HasColumnName("UserEducationID");

                entity.Property(e => e.Address).HasMaxLength(1000);

                entity.Property(e => e.CompanyName).HasMaxLength(250);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.Property(e => e.FromDate).HasColumnType("date");

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.Position).HasMaxLength(250);

                entity.Property(e => e.ToDate).HasColumnType("date");

                entity.Property(e => e.UserId).HasColumnName("UserID");
            });
        }
    }
}
