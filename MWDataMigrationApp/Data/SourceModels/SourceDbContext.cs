using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace MWDataMigrationApp.Data.SourceModels
{
    public partial class SourceDbContext : DbContext
    {

        private readonly IConfiguration _configuration;

        public SourceDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public SourceDbContext(DbContextOptions<SourceDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("SourceConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public DbSet<AdJiraUser> AdJiraUsers { get; set; }
        public virtual DbSet<DomainMt> DomainMts { get; set; }
        public virtual DbSet<IssuesMt> IssuesMts { get; set; }
        public virtual DbSet<ProjectDomainMt> ProjectDomainMts { get; set; }
        public virtual DbSet<ProjectRoleMembersMt> ProjectRoleMembersMts { get; set; }
        public virtual DbSet<ProjectsMt> ProjectsMts { get; set; }
        public virtual DbSet<TempoExpensesMt> TempoExpensesMts { get; set; }
        public virtual DbSet<TimesheetMt> TimesheetMts { get; set; }
        public virtual DbSet<UsersMt> UsersMts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AdJiraUser>().ToTable("AD_JIRA_USERS_MT");



            modelBuilder.Entity<AdJiraUser>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("AD_JIRA_USERS_MT", "jir");
            });

            modelBuilder.Entity<DomainMt>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Domain_MT", "jir");

                entity.Property(e => e.DomainName).HasColumnName("Domain_Name");

                entity.Property(e => e.ProgramDirector).HasColumnName("Program_Director");

                entity.Property(e => e.SeqId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("seq_id");

                entity.Property(e => e.UserAccountId).HasColumnName("USER_ACCOUNT_ID");
            });



            modelBuilder.Entity<IssuesMt>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Issues_MT", "jir");

                entity.Property(e => e.ActualEndDate).HasColumnName("Actual_End_Date");

                entity.Property(e => e.ActualStartDate).HasColumnName("Actual_Start_Date");

                entity.Property(e => e.BaselineEndDate).HasColumnName("Baseline_end_date");

                entity.Property(e => e.BaselineStartDate).HasColumnName("Baseline_start_date");

                entity.Property(e => e.Budget10052).HasColumnName("Budget_10052");

                entity.Property(e => e.Created).HasColumnName("CREATED");

                entity.Property(e => e.CreatorAccountId).HasColumnName("CREATOR_ACCOUNT_ID");

                entity.Property(e => e.CreatorName).HasColumnName("CREATOR_NAME");

                entity.Property(e => e.CurrentAssigneeAccountId).HasColumnName("CURRENT_ASSIGNEE_ACCOUNT_ID");

                entity.Property(e => e.CurrentAssigneeName).HasColumnName("CURRENT_ASSIGNEE_NAME");

                entity.Property(e => e.Description).HasColumnName("DESCRIPTION");

                entity.Property(e => e.DueDate).HasColumnName("DUE_DATE");

                entity.Property(e => e.EarnedValue).HasColumnName("Earned_Value");

                entity.Property(e => e.Email10050).HasColumnName("Email_10050");

                entity.Property(e => e.EndDate10097).HasColumnName("End_date_10097");

                entity.Property(e => e.EpicName10011).HasColumnName("Epic_Name_10011");

                entity.Property(e => e.EpicStatus10012).HasColumnName("Epic_Status_10012");

                entity.Property(e => e.Impact10004).HasColumnName("Impact_10004");

                entity.Property(e => e.InherentImpact10082).HasColumnName("Inherent_impact_10082");

                entity.Property(e => e.InherentProbability10084).HasColumnName("Inherent_probability_10084");

                entity.Property(e => e.InherentRisk10086).HasColumnName("Inherent_risk_10086");

                entity.Property(e => e.InvoiceNumber).HasColumnName("Invoice_Number");

                entity.Property(e => e.IssueId).HasColumnName("ISSUE_ID");

                entity.Property(e => e.IssueKey).HasColumnName("ISSUE_KEY");

                entity.Property(e => e.IssueStatusId).HasColumnName("ISSUE_STATUS_ID");

                entity.Property(e => e.IssueStatusName).HasColumnName("ISSUE_STATUS_NAME");

                entity.Property(e => e.IssueTypeId).HasColumnName("ISSUE_TYPE_ID");

                entity.Property(e => e.IssueTypeName).HasColumnName("ISSUE_TYPE_NAME");

                entity.Property(e => e.NewExpectedDate).HasColumnName("New_Expected_Date");

                entity.Property(e => e.Owner10054).HasColumnName("Owner_10054");

                entity.Property(e => e.ParentIssueId).HasColumnName("PARENT_ISSUE_ID");

                entity.Property(e => e.ParentIssueKey).HasColumnName("PARENT_ISSUE_KEY");

                entity.Property(e => e.ParentIssueStatusId).HasColumnName("PARENT_ISSUE_STATUS_ID");

                entity.Property(e => e.ParentIssueStatusName).HasColumnName("PARENT_ISSUE_STATUS_NAME");

                entity.Property(e => e.ParentIssueSummary).HasColumnName("PARENT_ISSUE_SUMMARY");

                entity.Property(e => e.ParentIssueTypeId).HasColumnName("PARENT_ISSUE_TYPE_ID");

                entity.Property(e => e.ParentIssueTypeName).HasColumnName("PARENT_ISSUE_TYPE_NAME");

                entity.Property(e => e.ParentPriority).HasColumnName("PARENT_PRIORITY");

                entity.Property(e => e.PaymentPlan).HasColumnName("Payment_Plan");

                entity.Property(e => e.PlannedEndDate).HasColumnName("Planned_End_Date");

                entity.Property(e => e.PlannedStartDate).HasColumnName("Planned_Start_Date");

                entity.Property(e => e.PlannedValue).HasColumnName("Planned_Value");

                entity.Property(e => e.Priority).HasColumnName("PRIORITY");

                entity.Property(e => e.ProjectId).HasColumnName("PROJECT_ID");

                entity.Property(e => e.ProjectKey).HasColumnName("PROJECT_KEY");

                entity.Property(e => e.ReporterAccountId).HasColumnName("REPORTER_ACCOUNT_ID");

                entity.Property(e => e.ReporterName).HasColumnName("REPORTER_NAME");

                entity.Property(e => e.Resolution).HasColumnName("RESOLUTION");

                entity.Property(e => e.ResolutionDate).HasColumnName("RESOLUTION_DATE");

                entity.Property(e => e.StartDate10015).HasColumnName("Start_date_10015");

                entity.Property(e => e.Summary).HasColumnName("SUMMARY");

                entity.Property(e => e.TempoTeamId).HasColumnName("TEMPO_TEAM_ID");

                entity.Property(e => e.Type10074).HasColumnName("Type_10074");

                entity.Property(e => e.Updated).HasColumnName("UPDATED");

                entity.Property(e => e.Value10071).HasColumnName("Value_10071");

                entity.Property(e => e.Watchers).HasColumnName("WATCHERS");
            });

            modelBuilder.Entity<ProjectDomainMt>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("PROJECT_DOMAIN_MT", "jir");

                entity.Property(e => e.DomainName).HasColumnName("Domain_Name");

                entity.Property(e => e.Name).HasColumnName("NAME");

                entity.Property(e => e.ProjectId).HasColumnName("PROJECT_ID");

                entity.Property(e => e.UserAccountId).HasColumnName("USER_ACCOUNT_ID");

                entity.Property(e => e.UserName).HasColumnName("USER_NAME");
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

                entity.Property(e => e.AccountManager).HasColumnName("ACCOUNT_MANAGER");

                entity.Property(e => e.BusninessLine).HasColumnName("BUSNINESS_LINE");

                entity.Property(e => e.Category).HasColumnName("CATEGORY");

                entity.Property(e => e.Company).HasColumnName("COMPANY");

                entity.Property(e => e.Description).HasColumnName("DESCRIPTION");

                entity.Property(e => e.EndDate)
                    .HasColumnType("date")
                    .HasColumnName("END_DATE");

                entity.Property(e => e.LeadName).HasColumnName("LEAD_NAME");

                entity.Property(e => e.Name).HasColumnName("NAME");

                entity.Property(e => e.Organization).HasColumnName("ORGANIZATION");

                entity.Property(e => e.PipeDriveLink)
                    .HasMaxLength(45)
                    .IsUnicode(false)
                    .HasColumnName("PipeDrive_Link");

                entity.Property(e => e.ProjectContractValue).HasColumnName("PROJECT_CONTRACT_VALUE");

                entity.Property(e => e.ProjectId).HasColumnName("PROJECT_ID");

                entity.Property(e => e.ProjectKey).HasColumnName("PROJECT_KEY");

                entity.Property(e => e.StartDate)
                    .HasColumnType("date")
                    .HasColumnName("START_DATE");
            });

            modelBuilder.Entity<TempoExpensesMt>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TEMPO_EXPENSES_MT", "jir");

                entity.Property(e => e.AmountValue).HasColumnName("AMOUNT_VALUE");

                entity.Property(e => e.CategoryName).HasColumnName("CATEGORY_NAME");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("DATE");

                entity.Property(e => e.Description).HasColumnName("DESCRIPTION");

                entity.Property(e => e.ExpenseId).HasColumnName("EXPENSE_ID");

                entity.Property(e => e.ProjectId).HasColumnName("PROJECT_ID");
            });

            modelBuilder.Entity<TimesheetMt>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TIMESHEET_MT", "jir");

                entity.Property(e => e.AccountId).HasColumnName("accountId");

                entity.Property(e => e.BillableSeconds).HasColumnName("billableSeconds");

                entity.Property(e => e.Count).HasColumnName("count");

                entity.Property(e => e.CreatedAt).HasColumnName("createdAt");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.IssueId).HasColumnName("issue_id");

                entity.Property(e => e.SeqId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("seq_id");

                entity.Property(e => e.StartDate).HasColumnName("startDate");

                entity.Property(e => e.StartTime).HasColumnName("startTime");

                entity.Property(e => e.TempoWorklogId).HasColumnName("tempoWorklogId");

                entity.Property(e => e.TimeSpentSeconds).HasColumnName("timeSpentSeconds");

                entity.Property(e => e.UpdatedAt).HasColumnName("updatedAt");
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
