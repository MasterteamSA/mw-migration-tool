using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using MWDataMigrationApp.Data.SourceModels;

namespace MWDataMigrationApp.Data
{
    public partial class SourceContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public SourceContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SourceContext(DbContextOptions<SourceContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<IssuesMt> IssuesMts { get; set; } = null!;
        public virtual DbSet<ProjectRoleMembersMt> ProjectRoleMembersMts { get; set; } = null!;
        public virtual DbSet<ProjectsMt> ProjectsMts { get; set; } = null!;
        public virtual DbSet<UsersMt> UsersMts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("SourceConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IssuesMt>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Issues_MT", "jir");

                entity.Property(e => e.ActualEndDate).HasColumnName("Actual_End_Date");

                entity.Property(e => e.ActualStartDate).HasColumnName("Actual_Start_Date");

                entity.Property(e => e.BaselineEndDate).HasColumnName("Baseline_end_date");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.CreatorAccountId).HasColumnName("CREATOR_ACCOUNT_ID");

                entity.Property(e => e.CreatorName).HasColumnName("CREATOR_NAME");

                entity.Property(e => e.CurrentAssigneeAccountId).HasColumnName("CURRENT_ASSIGNEE_ACCOUNT_ID");

                entity.Property(e => e.CurrentAssigneeName).HasColumnName("CURRENT_ASSIGNEE_NAME");

                entity.Property(e => e.Description).HasColumnName("DESCRIPTION");

                entity.Property(e => e.DueDate).HasColumnName("DUE_DATE");

                entity.Property(e => e.EarnedValue).HasColumnName("Earned_Value");

                entity.Property(e => e.EndDate10097).HasColumnName("End_date_10097");

                entity.Property(e => e.EpicName10011).HasColumnName("Epic_Name_10011");

                entity.Property(e => e.Impact10004).HasColumnName("Impact_10004");

                entity.Property(e => e.IssueId).HasColumnName("ISSUE_ID");

                entity.Property(e => e.IssueKey).HasColumnName("ISSUE_KEY");

                entity.Property(e => e.IssueStatusId).HasColumnName("ISSUE_STATUS_ID");

                entity.Property(e => e.IssueStatusName).HasColumnName("ISSUE_STATUS_NAME");

                entity.Property(e => e.IssueTypeId).HasColumnName("ISSUE_TYPE_ID");

                entity.Property(e => e.IssueTypeName).HasColumnName("ISSUE_TYPE_NAME");

                entity.Property(e => e.Owner10054).HasColumnName("Owner_10054");

                entity.Property(e => e.ParentIssueId).HasColumnName("PARENT_ISSUE_ID");

                entity.Property(e => e.ParentIssueKey).HasColumnName("PARENT_ISSUE_KEY");

                entity.Property(e => e.PlannedDeliveryDate10061).HasColumnName("Planned_Delivery_Date_10061");

                entity.Property(e => e.PlannedEndDate).HasColumnName("Planned_End_Date");

                entity.Property(e => e.PlannedStartDate).HasColumnName("Planned_Start_Date");

                entity.Property(e => e.PlannedValue).HasColumnName("Planned_Value");

                entity.Property(e => e.Priority).HasColumnName("PRIORITY");

                entity.Property(e => e.ProjectId).HasColumnName("PROJECT_ID");

                entity.Property(e => e.ProjectKey).HasColumnName("PROJECT_KEY");

                entity.Property(e => e.ReporterAccountId).HasColumnName("REPORTER_ACCOUNT_ID");

                entity.Property(e => e.ReporterName).HasColumnName("REPORTER_NAME");

                entity.Property(e => e.Resolution).HasColumnName("RESOLUTION");

                entity.Property(e => e.StartDate10015).HasColumnName("Start_date_10015");

                entity.Property(e => e.StoryPoints10028).HasColumnName("Story_Points_10028");

                entity.Property(e => e.Summary).HasColumnName("SUMMARY");

                entity.Property(e => e.TargetEnd10023).HasColumnName("Target_end_10023");

                entity.Property(e => e.TargetStart10022).HasColumnName("Target_start_10022");

                entity.Property(e => e.TotalForms10068).HasColumnName("Total_forms_10068");

                entity.Property(e => e.Type10073).HasColumnName("Type_10073");

                entity.Property(e => e.Type10074).HasColumnName("Type_10074");

                entity.Property(e => e.Updated).HasColumnName("UPDATED");

                entity.Property(e => e.Value10071).HasColumnName("Value_10071");
            });

            modelBuilder.Entity<ProjectRoleMembersMt>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ProjectRoleMembers_MT", "jir");

                entity.Property(e => e.GroupName).HasColumnName("GROUP_NAME");

                entity.Property(e => e.ProjectId).HasColumnName("PROJECT_ID");

                entity.Property(e => e.ProjectName).HasColumnName("PROJECT_NAME");

                entity.Property(e => e.ProjectRoleId).HasColumnName("PROJECT_ROLE_ID");

                entity.Property(e => e.ProjectRoleName).HasColumnName("PROJECT_ROLE_NAME");

                entity.Property(e => e.UserAccountId).HasColumnName("USER_ACCOUNT_ID");

                entity.Property(e => e.UserName).HasColumnName("USER_NAME");
            });

            modelBuilder.Entity<ProjectsMt>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Projects_MT", "jir");

                entity.Property(e => e.Category).HasColumnName("CATEGORY");

                entity.Property(e => e.Description).HasColumnName("DESCRIPTION");

                entity.Property(e => e.IsPrivate).HasColumnName("IS_PRIVATE");

                entity.Property(e => e.LeadAccountId).HasColumnName("LEAD_ACCOUNT_ID");

                entity.Property(e => e.LeadName).HasColumnName("LEAD_NAME");

                entity.Property(e => e.Name).HasColumnName("NAME");

                entity.Property(e => e.ProjectId).HasColumnName("PROJECT_ID");

                entity.Property(e => e.ProjectKey).HasColumnName("PROJECT_KEY");

                entity.Property(e => e.Url).HasColumnName("URL");
            });

            modelBuilder.Entity<UsersMt>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Users_MT", "jir");

                entity.Property(e => e.AccountType).HasColumnName("ACCOUNT_TYPE");

                entity.Property(e => e.Email).HasColumnName("EMAIL");

                entity.Property(e => e.IsActive).HasColumnName("IS_ACTIVE");

                entity.Property(e => e.TempoWorkloadId).HasColumnName("TEMPO_WORKLOAD_ID");

                entity.Property(e => e.UserAccountId).HasColumnName("USER_ACCOUNT_ID");

                entity.Property(e => e.UserName).HasColumnName("USER_NAME");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
