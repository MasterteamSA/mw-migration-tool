using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using MWDataMigrationApp.Data.TargetModels;
using Action = MWDataMigrationApp.Data.TargetModels.Action;
using File = MWDataMigrationApp.Data.TargetModels.File;
using Task = MWDataMigrationApp.Data.TargetModels.Task;

namespace MWDataMigrationApp.Data
{
    public partial class TargetContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public TargetContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TargetContext(DbContextOptions<TargetContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("TargetConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public virtual DbSet<Action> Actions { get; set; } = null!;
        public virtual DbSet<ActionHistory> ActionHistories { get; set; } = null!;
        public virtual DbSet<ActiveUser> ActiveUsers { get; set; } = null!;
        public virtual DbSet<Activity> Activities { get; set; } = null!;
        public virtual DbSet<AggregatedCounter> AggregatedCounters { get; set; } = null!;
        public virtual DbSet<AppAccessibility> AppAccessibilities { get; set; } = null!;
        public virtual DbSet<AppAccessibilityGroup> AppAccessibilityGroups { get; set; } = null!;
        public virtual DbSet<AppCredential> AppCredentials { get; set; } = null!;
        public virtual DbSet<AppGroup> AppGroups { get; set; } = null!;
        public virtual DbSet<AppGroupPermission> AppGroupPermissions { get; set; } = null!;
        public virtual DbSet<AppGroupUser> AppGroupUsers { get; set; } = null!;
        public virtual DbSet<AppPermission> AppPermissions { get; set; } = null!;
        public virtual DbSet<AppRole> AppRoles { get; set; } = null!;
        public virtual DbSet<AppRolePermission> AppRolePermissions { get; set; } = null!;
        public virtual DbSet<AppSetting> AppSettings { get; set; } = null!;
        public virtual DbSet<AppUser> AppUsers { get; set; } = null!;
        public virtual DbSet<AppUserClaim> AppUserClaims { get; set; } = null!;
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; } = null!;
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; } = null!;
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; } = null!;
        public virtual DbSet<BasicProperty> BasicProperties { get; set; } = null!;
        public virtual DbSet<BillOfQuantity> BillOfQuantities { get; set; } = null!;
        public virtual DbSet<Cache> Caches { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<CategoryGroup> CategoryGroups { get; set; } = null!;
        public virtual DbSet<CategoryLevel> CategoryLevels { get; set; } = null!;
        public virtual DbSet<ChangeRequest> ChangeRequests { get; set; } = null!;
        public virtual DbSet<Charter> Charters { get; set; } = null!;
        public virtual DbSet<Closure> Closures { get; set; } = null!;
        public virtual DbSet<Configuration> Configurations { get; set; } = null!;
        public virtual DbSet<Connection> Connections { get; set; } = null!;
        public virtual DbSet<ConnectionsDatum> ConnectionsData { get; set; } = null!;
        public virtual DbSet<Counter> Counters { get; set; } = null!;
        public virtual DbSet<DbFile> DbFiles { get; set; } = null!;
        public virtual DbSet<Delegation> Delegations { get; set; } = null!;
        public virtual DbSet<Deliverable> Deliverables { get; set; } = null!;
        public virtual DbSet<DeliverableAcceptance> DeliverableAcceptances { get; set; } = null!;
        public virtual DbSet<Dependency> Dependencies { get; set; } = null!;
        public virtual DbSet<DeviceToken> DeviceTokens { get; set; } = null!;
        public virtual DbSet<DiscussionLog> DiscussionLogs { get; set; } = null!;
        public virtual DbSet<Draft> Drafts { get; set; } = null!;
        public virtual DbSet<Escalation> Escalations { get; set; } = null!;
        public virtual DbSet<EscalationAction> EscalationActions { get; set; } = null!;
        public virtual DbSet<EscalationSchema> EscalationSchemas { get; set; } = null!;
        public virtual DbSet<Event> Events { get; set; } = null!;
        public virtual DbSet<ExecutedScript> ExecutedScripts { get; set; } = null!;
        public virtual DbSet<Expense> Expenses { get; set; } = null!;
        public virtual DbSet<File> Files { get; set; } = null!;
        public virtual DbSet<Folder> Folders { get; set; } = null!;
        public virtual DbSet<GeneralTask> GeneralTasks { get; set; } = null!;
        public virtual DbSet<Hash> Hashes { get; set; } = null!;
        public virtual DbSet<History> Histories { get; set; } = null!;
        public virtual DbSet<History1> Histories1 { get; set; } = null!;
        public virtual DbSet<InstanceGroupPermission> InstanceGroupPermissions { get; set; } = null!;
        public virtual DbSet<Invoice> Invoices { get; set; } = null!;
        public virtual DbSet<Issue> Issues { get; set; } = null!;
        public virtual DbSet<Job> Jobs { get; set; } = null!;
        public virtual DbSet<JobParameter> JobParameters { get; set; } = null!;
        public virtual DbSet<JobQueue> JobQueues { get; set; } = null!;
        public virtual DbSet<Level> Levels { get; set; } = null!;
        public virtual DbSet<LevelDataFinancial> LevelDataFinancials { get; set; } = null!;
        public virtual DbSet<LevelDataSnapshot> LevelDataSnapshots { get; set; } = null!;
        public virtual DbSet<LevelsDatum> LevelsData { get; set; } = null!;
        public virtual DbSet<LevelsLog> LevelsLogs { get; set; } = null!;
        public virtual DbSet<List> Lists { get; set; } = null!;
        public virtual DbSet<Log> Logs { get; set; } = null!;
        public virtual DbSet<LogsDatum> LogsData { get; set; } = null!;
        public virtual DbSet<Lookup> Lookups { get; set; } = null!;
        public virtual DbSet<LookupItem> LookupItems { get; set; } = null!;
        public virtual DbSet<MatrixValue> MatricesValues { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;
        public virtual DbSet<Milestone> Milestones { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<PaymentPlan> PaymentPlans { get; set; } = null!;
        public virtual DbSet<PaymentPlanItem> PaymentPlanItems { get; set; } = null!;
        public virtual DbSet<PaymentPlanItemBoq> PaymentPlanItemBoqs { get; set; } = null!;
        public virtual DbSet<PhaseGate> PhaseGates { get; set; } = null!;
        public virtual DbSet<PhaseGateItem> PhaseGateItems { get; set; } = null!;
        public virtual DbSet<PhaseGateItemLog> PhaseGateItemLogs { get; set; } = null!;
        public virtual DbSet<Predecessor> Predecessors { get; set; } = null!;
        public virtual DbSet<ProcurementSchema> ProcurementSchemas { get; set; } = null!;
        public virtual DbSet<ProcurementStage> ProcurementStages { get; set; } = null!;
        public virtual DbSet<ProgressStatus> ProgressStatuses { get; set; } = null!;
        public virtual DbSet<ProjectEarnedValue> ProjectEarnedValues { get; set; } = null!;
        public virtual DbSet<PropertiesStep> PropertiesSteps { get; set; } = null!;
        public virtual DbSet<PropertiesValue> PropertiesValues { get; set; } = null!;
        public virtual DbSet<Property> Properties { get; set; } = null!;
        public virtual DbSet<PropertyDisplaySetting> PropertyDisplaySettings { get; set; } = null!;
        public virtual DbSet<PropertySection> PropertySections { get; set; } = null!;
        public virtual DbSet<PropertyStatus> PropertyStatuses { get; set; } = null!;
        public virtual DbSet<Receiver> Receivers { get; set; } = null!;
        public virtual DbSet<Registery> Registeries { get; set; } = null!;
        public virtual DbSet<Request> Requests { get; set; } = null!;
        public virtual DbSet<RequestProperty> RequestProperties { get; set; } = null!;
        public virtual DbSet<RequestPropertyValue> RequestPropertyValues { get; set; } = null!;
        public virtual DbSet<RequestsSchema> RequestsSchemas { get; set; } = null!;
        public virtual DbSet<Risk> Risks { get; set; } = null!;
        public virtual DbSet<ScheduleColumn> ScheduleColumns { get; set; } = null!;
        public virtual DbSet<ScheduleView> ScheduleViews { get; set; } = null!;
        public virtual DbSet<ScheduleViewColumn> ScheduleViewColumns { get; set; } = null!;
        public virtual DbSet<Schema> Schemas { get; set; } = null!;
        public virtual DbSet<SchemaPhaseGate> SchemaPhaseGates { get; set; } = null!;
        public virtual DbSet<SchemaPhaseGateItem> SchemaPhaseGateItems { get; set; } = null!;
        public virtual DbSet<SchemaPhaseGateLog> SchemaPhaseGateLogs { get; set; } = null!;
        public virtual DbSet<Server> Servers { get; set; } = null!;
        public virtual DbSet<Set> Sets { get; set; } = null!;
        public virtual DbSet<Stakeholder> Stakeholders { get; set; } = null!;
        public virtual DbSet<State> States { get; set; } = null!;
        public virtual DbSet<Step> Steps { get; set; } = null!;
        public virtual DbSet<StepConnection> StepConnections { get; set; } = null!;
        public virtual DbSet<StepConnectionsSchema> StepConnectionsSchemas { get; set; } = null!;
        public virtual DbSet<StepsSchema> StepsSchemas { get; set; } = null!;
        public virtual DbSet<SummaryNotification> SummaryNotifications { get; set; } = null!;
        public virtual DbSet<Task> Tasks { get; set; } = null!;
        public virtual DbSet<TaskBaseline> TaskBaselines { get; set; } = null!;
        public virtual DbSet<TaskProgressUpdateRequest> TaskProgressUpdateRequests { get; set; } = null!;
        public virtual DbSet<TeamMember> TeamMembers { get; set; } = null!;
        public virtual DbSet<Template> Templates { get; set; } = null!;
        public virtual DbSet<TimeSheet> TimeSheets { get; set; } = null!;
        public virtual DbSet<TimeSheetItem> TimeSheetItems { get; set; } = null!;
        public virtual DbSet<UserConnection> UserConnections { get; set; } = null!;
        public virtual DbSet<UserNotification> UserNotifications { get; set; } = null!;
        public virtual DbSet<WorkflowAction> WorkflowActions { get; set; } = null!;
        public virtual DbSet<WorkflowActivity> WorkflowActivities { get; set; } = null!;
        public virtual DbSet<WorkflowRegistry> WorkflowRegistries { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Action>(entity =>
            {
                entity.ToTable("actions", "wf");

                entity.HasIndex(e => e.ActivityId, "IX_actions_ActivityId");

                entity.HasIndex(e => e.WorkflowActionId, "IX_actions_WorkflowActionId");

                entity.HasOne(d => d.Activity)
                    .WithMany(p => p.Actions)
                    .HasForeignKey(d => d.ActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WorkflowAction)
                    .WithMany(p => p.Actions)
                    .HasForeignKey(d => d.WorkflowActionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ActionHistory>(entity =>
            {
                entity.ToTable("ActionHistory");
            });

            modelBuilder.Entity<ActiveUser>(entity =>
            {
                entity.HasIndex(e => e.TokenSignature, "IX_ActiveUsers_TokenSignature")
                    .IsUnique()
                    .HasFilter("([TokenSignature] IS NOT NULL)");
            });

            modelBuilder.Entity<Activity>(entity =>
            {
                entity.ToTable("activities", "wf");

                entity.HasIndex(e => e.RegistryId, "IX_activities_RegistryId");

                entity.HasIndex(e => e.WorkflowActivityId, "IX_activities_WorkflowActivityId");

                entity.HasOne(d => d.Registry)
                    .WithMany(p => p.Activities)
                    .HasForeignKey(d => d.RegistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.WorkflowActivity)
                    .WithMany(p => p.Activities)
                    .HasForeignKey(d => d.WorkflowActivityId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AggregatedCounter>(entity =>
            {
                entity.HasKey(e => e.Key)
                    .HasName("PK_HangFire_CounterAggregated");

                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_AggregatedCounter_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<AppAccessibility>(entity =>
            {
                entity.ToTable("appAccessibility", "identity");

                entity.HasIndex(e => e.ModuleKey, "IX_appAccessibility_ModuleKey")
                    .IsUnique();
            });

            modelBuilder.Entity<AppAccessibilityGroup>(entity =>
            {
                entity.ToTable("appAccessibilityGroup", "identity");

                entity.HasIndex(e => e.AppAccessibilityId, "IX_appAccessibilityGroup_AppAccessibilityId");

                entity.HasIndex(e => e.AppGroupId, "IX_appAccessibilityGroup_AppGroupId");

                entity.HasOne(d => d.AppAccessibility)
                    .WithMany(p => p.AppAccessibilityGroups)
                    .HasForeignKey(d => d.AppAccessibilityId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.AppGroup)
                    .WithMany(p => p.AppAccessibilityGroups)
                    .HasForeignKey(d => d.AppGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AppCredential>(entity =>
            {
                entity.HasKey(e => e.AppId);

                entity.ToTable("appCredentials", "identity");
            });

            modelBuilder.Entity<AppGroup>(entity =>
            {
                entity.ToTable("appGroups", "identity");
            });

            modelBuilder.Entity<AppGroupPermission>(entity =>
            {
                entity.ToTable("appGroupPermission", "identity");

                entity.HasIndex(e => e.AppPermissionId, "IX_appGroupPermission_AppPermissionId");

                entity.HasIndex(e => e.GroupId, "IX_appGroupPermission_GroupId");

                entity.HasOne(d => d.AppPermission)
                    .WithMany(p => p.AppGroupPermissions)
                    .HasForeignKey(d => d.AppPermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.AppGroupPermissions)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AppGroupUser>(entity =>
            {
                entity.ToTable("appGroupUsers", "identity");

                entity.HasIndex(e => e.AppUserId, "IX_appGroupUsers_AppUserId");

                entity.HasIndex(e => e.GroupId, "IX_appGroupUsers_GroupId");

                entity.HasIndex(e => e.UserId, "IX_appGroupUsers_UserId");

                entity.HasOne(d => d.AppUser)
                    .WithMany(p => p.AppGroupUserAppUsers)
                    .HasForeignKey(d => d.AppUserId);

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.AppGroupUsers)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AppGroupUserUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<AppPermission>(entity =>
            {
                entity.ToTable("appPermissions", "identity");

                entity.HasIndex(e => e.AppRoleId, "IX_appPermissions_AppRoleId");

                entity.HasIndex(e => new { e.Command, e.ModuleId }, "IX_appPermissions_Command_ModuleId")
                    .IsUnique()
                    .HasFilter("([ModuleId] IS NOT NULL)");

                entity.HasOne(d => d.AppRole)
                    .WithMany(p => p.AppPermissions)
                    .HasForeignKey(d => d.AppRoleId);
            });

            modelBuilder.Entity<AppRole>(entity =>
            {
                entity.ToTable("appRoles", "identity");

                entity.HasIndex(e => e.NormalizedName, "RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AppRolePermission>(entity =>
            {
                entity.ToTable("appRolePermission", "identity");

                entity.HasIndex(e => e.AppPermissionId, "IX_appRolePermission_AppPermissionId");

                entity.HasIndex(e => e.RoleId, "IX_appRolePermission_RoleId");

                entity.HasOne(d => d.AppPermission)
                    .WithMany(p => p.AppRolePermissions)
                    .HasForeignKey(d => d.AppPermissionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AppRolePermissions)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AppSetting>(entity =>
            {
                entity.ToTable("appSettings");
            });

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.ToTable("appUsers", "identity");

                entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AppUserClaim>(entity =>
            {
                entity.ToTable("appUserClaims", "identity");

                entity.HasIndex(e => e.UserId, "IX_appUserClaims_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AppUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.ToTable("AspNetUserLogins", "identity");

                entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId, e.ModuleId });

                entity.ToTable("AspNetUserRoles", "identity");

                entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.ToTable("AspNetUserTokens", "identity");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<BasicProperty>(entity =>
            {
                entity.ToTable("basicProperties");

                entity.HasIndex(e => e.LogId, "IX_basicProperties_LogId");

                entity.Property(e => e.IsCalculated)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.BasicProperties)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<BillOfQuantity>(entity =>
            {
                entity.ToTable("billOfQuantities");

                entity.HasIndex(e => e.LevelId, "IX_billOfQuantities_LevelId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.BillOfQuantities)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Cache>(entity =>
            {
                entity.HasKey(e => e.Key);

                entity.ToTable("caches");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");
            });

            modelBuilder.Entity<CategoryGroup>(entity =>
            {
                entity.ToTable("categoryGroups");

                entity.HasIndex(e => e.CategoryId, "IX_categoryGroups_CategoryId");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.CategoryGroups)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<CategoryLevel>(entity =>
            {
                entity.ToTable("categoryLevels");

                entity.HasIndex(e => e.CategoryId, "IX_categoryLevels_CategoryId");

                entity.HasIndex(e => e.LevelDataId, "IX_categoryLevels_LevelDataId");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.CategoryLevels)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.LevelData)
                    .WithMany(p => p.CategoryLevels)
                    .HasForeignKey(d => d.LevelDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ChangeRequest>(entity =>
            {
                entity.ToTable("changeRequests");

                entity.HasIndex(e => e.LevelId, "IX_changeRequests_LevelId");

                entity.HasIndex(e => e.LogId, "IX_changeRequests_LogId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.ChangeRequests)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.ChangeRequests)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Charter>(entity =>
            {
                entity.ToTable("charters");

                entity.HasIndex(e => e.LevelId, "IX_charters_LevelId");

                entity.HasIndex(e => e.LogId, "IX_charters_LogId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Charters)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.Charters)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Closure>(entity =>
            {
                entity.ToTable("closures");

                entity.HasIndex(e => e.LevelId, "IX_closures_LevelId");

                entity.HasIndex(e => e.LogId, "IX_closures_LogId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Closures)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.Closures)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.ToTable("configurations");
            });

            modelBuilder.Entity<Connection>(entity =>
            {
                entity.ToTable("connections");

                entity.HasIndex(e => e.SourceLevelId, "IX_connections_SourceLevelId");

                entity.HasIndex(e => e.TargetLevelId, "IX_connections_TargetLevelId");

                entity.Property(e => e.AllowManyToMany)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.HasOne(d => d.SourceLevel)
                    .WithMany(p => p.ConnectionSourceLevels)
                    .HasForeignKey(d => d.SourceLevelId);

                entity.HasOne(d => d.TargetLevel)
                    .WithMany(p => p.ConnectionTargetLevels)
                    .HasForeignKey(d => d.TargetLevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ConnectionsDatum>(entity =>
            {
                entity.ToTable("connectionsData");

                entity.HasIndex(e => e.ConnectionId, "IX_connectionsData_ConnectionId");

                entity.HasIndex(e => e.SourceLevelDataId, "IX_connectionsData_SourceLevelDataId");

                entity.HasIndex(e => e.TargetLevelDataId, "IX_connectionsData_TargetLevelDataId");

                entity.HasOne(d => d.Connection)
                    .WithMany(p => p.ConnectionsData)
                    .HasForeignKey(d => d.ConnectionId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SourceLevelData)
                    .WithMany(p => p.ConnectionsDatumSourceLevelData)
                    .HasForeignKey(d => d.SourceLevelDataId);

                entity.HasOne(d => d.TargetLevelData)
                    .WithMany(p => p.ConnectionsDatumTargetLevelData)
                    .HasForeignKey(d => d.TargetLevelDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Counter", "HangFire");

                entity.HasIndex(e => e.Key, "CX_HangFire_Counter")
                    .IsClustered();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key).HasMaxLength(100);
            });

            modelBuilder.Entity<Delegation>(entity =>
            {
                entity.ToTable("delegations");
            });

            modelBuilder.Entity<Deliverable>(entity =>
            {
                entity.ToTable("deliverables");

                entity.HasIndex(e => e.DeliverableAcceptanceId, "IX_deliverables_DeliverableAcceptanceId");

                entity.HasIndex(e => e.LevelId, "IX_deliverables_LevelId");

                entity.HasIndex(e => e.LogId, "IX_deliverables_LogId");

                entity.HasIndex(e => e.PhaseGateId, "IX_deliverables_PhaseGateId");

                entity.HasIndex(e => e.TaskId, "IX_deliverables_TaskId")
                    .IsUnique();

                entity.HasOne(d => d.DeliverableAcceptance)
                    .WithMany(p => p.Deliverables)
                    .HasForeignKey(d => d.DeliverableAcceptanceId);

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Deliverables)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.Deliverables)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PhaseGate)
                    .WithMany(p => p.Deliverables)
                    .HasForeignKey(d => d.PhaseGateId);

                entity.HasOne(d => d.Task)
                    .WithOne(p => p.Deliverable)
                    .HasForeignKey<Deliverable>(d => d.TaskId);
            });

            modelBuilder.Entity<DeliverableAcceptance>(entity =>
            {
                entity.ToTable("deliverableAcceptances");

                entity.HasIndex(e => e.LevelId, "IX_deliverableAcceptances_LevelId");

                entity.HasIndex(e => e.LogId, "IX_deliverableAcceptances_LogId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.DeliverableAcceptances)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.DeliverableAcceptances)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Dependency>(entity =>
            {
                entity.ToTable("dependencies");

                entity.HasIndex(e => e.AffectedTaskId, "IX_dependencies_AffectedTaskId");

                entity.HasIndex(e => e.AffectingTaskId, "IX_dependencies_AffectingTaskId");

                entity.HasOne(d => d.AffectedTask)
                    .WithMany(p => p.DependencyAffectedTasks)
                    .HasForeignKey(d => d.AffectedTaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.AffectingTask)
                    .WithMany(p => p.DependencyAffectingTasks)
                    .HasForeignKey(d => d.AffectingTaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasMany(d => d.Issues)
                    .WithMany(p => p.Dependencies)
                    .UsingEntity<Dictionary<string, object>>(
                        "DependencyIssue",
                        l => l.HasOne<Issue>().WithMany().HasForeignKey("IssueId").OnDelete(DeleteBehavior.ClientSetNull),
                        r => r.HasOne<Dependency>().WithMany().HasForeignKey("DependencyId"),
                        j =>
                        {
                            j.HasKey("DependencyId", "IssueId");

                            j.ToTable("dependencyIssues");

                            j.HasIndex(new[] { "IssueId" }, "IX_dependencyIssues_IssueId");
                        });

                entity.HasMany(d => d.Risks)
                    .WithMany(p => p.Dependencies)
                    .UsingEntity<Dictionary<string, object>>(
                        "DependencyRisk",
                        l => l.HasOne<Risk>().WithMany().HasForeignKey("RiskId").OnDelete(DeleteBehavior.ClientSetNull),
                        r => r.HasOne<Dependency>().WithMany().HasForeignKey("DependencyId"),
                        j =>
                        {
                            j.HasKey("DependencyId", "RiskId");

                            j.ToTable("dependencyRisks");

                            j.HasIndex(new[] { "RiskId" }, "IX_dependencyRisks_RiskId");
                        });
            });

            modelBuilder.Entity<DeviceToken>(entity =>
            {
                entity.ToTable("DeviceTokens", "identity");

                entity.HasIndex(e => e.UserId, "IX_DeviceTokens_UserId");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.DeviceTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Draft>(entity =>
            {
                entity.ToTable("drafts", "wf");

                entity.HasIndex(e => e.WorkflowRegistryId, "IX_drafts_WorkflowRegistryId");

                entity.HasOne(d => d.WorkflowRegistry)
                    .WithMany(p => p.Drafts)
                    .HasForeignKey(d => d.WorkflowRegistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Escalation>(entity =>
            {
                entity.ToTable("escalations");

                entity.HasIndex(e => e.EscalationSchemaId, "IX_escalations_EscalationSchemaId");

                entity.HasOne(d => d.EscalationSchema)
                    .WithMany(p => p.Escalations)
                    .HasForeignKey(d => d.EscalationSchemaId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<EscalationAction>(entity =>
            {
                entity.ToTable("escalationActions");

                entity.HasIndex(e => e.EscalationId, "IX_escalationActions_EscalationId");

                entity.HasOne(d => d.Escalation)
                    .WithMany(p => p.EscalationActions)
                    .HasForeignKey(d => d.EscalationId);
            });

            modelBuilder.Entity<EscalationSchema>(entity =>
            {
                entity.ToTable("escalationSchema");

                entity.HasIndex(e => e.LevelId, "IX_escalationSchema_LevelId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.EscalationSchemas)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Event>(entity =>
            {
                entity.ToTable("events", "notifications");

                entity.Property(e => e.ModuleType).HasDefaultValueSql("(N'')");
            });

            modelBuilder.Entity<ExecutedScript>(entity =>
            {
                entity.HasKey(e => e.ScriptName);
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.ToTable("expenses");

                entity.HasIndex(e => e.LevelId, "IX_expenses_LevelId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Expenses)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasMany(d => d.Deliverables)
                    .WithMany(p => p.Expenses)
                    .UsingEntity<Dictionary<string, object>>(
                        "ExpenseDeliverable",
                        l => l.HasOne<Deliverable>().WithMany().HasForeignKey("DeliverableId").OnDelete(DeleteBehavior.ClientSetNull),
                        r => r.HasOne<Expense>().WithMany().HasForeignKey("ExpenseId").OnDelete(DeleteBehavior.ClientSetNull),
                        j =>
                        {
                            j.HasKey("ExpenseId", "DeliverableId");

                            j.ToTable("expenseDeliverables");

                            j.HasIndex(new[] { "DeliverableId" }, "IX_expenseDeliverables_DeliverableId");
                        });
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.ToTable("files");

                entity.HasIndex(e => e.FolderId, "IX_files_FolderId");

                entity.HasOne(d => d.Folder)
                    .WithMany(p => p.Files)
                    .HasForeignKey(d => d.FolderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Folder>(entity =>
            {
                entity.ToTable("folders");
            });

            modelBuilder.Entity<GeneralTask>(entity =>
            {
                entity.ToTable("generalTasks");
            });

            modelBuilder.Entity<Hash>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Field })
                    .HasName("PK_HangFire_Hash");

                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Hash_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Field).HasMaxLength(100);
            });

            modelBuilder.Entity<History>(entity =>
            {
                entity.ToTable("histories", "pb");

                entity.HasIndex(e => e.RequestId, "IX_histories_RequestId");

                entity.HasIndex(e => e.StepId, "IX_histories_StepId")
                    .IsUnique()
                    .HasFilter("([StepId] IS NOT NULL)");

                entity.Property(e => e.FileUid).HasColumnName("FileUId");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.Histories)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Step)
                    .WithOne(p => p.History)
                    .HasForeignKey<History>(d => d.StepId);
            });

            modelBuilder.Entity<History1>(entity =>
            {
                entity.ToTable("Histories", "wf");

                entity.HasIndex(e => e.ActionId, "IX_Histories_ActionId");

                entity.HasIndex(e => e.RegistryId, "IX_Histories_RegistryId");

                entity.Property(e => e.FileUid).HasColumnName("FileUId");

                entity.HasOne(d => d.Action)
                    .WithMany(p => p.History1s)
                    .HasForeignKey(d => d.ActionId);

                entity.HasOne(d => d.Registry)
                    .WithMany(p => p.History1s)
                    .HasForeignKey(d => d.RegistryId);
            });

            modelBuilder.Entity<InstanceGroupPermission>(entity =>
            {
                entity.ToTable("instanceGroupPermission", "identity");

                entity.HasIndex(e => e.GroupId, "IX_instanceGroupPermission_GroupId");

                entity.HasOne(d => d.Group)
                    .WithMany(p => p.InstanceGroupPermissions)
                    .HasForeignKey(d => d.GroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.ToTable("invoices");

                entity.HasIndex(e => e.PaymentPlanItemId, "IX_invoices_PaymentPlanItemId");

                entity.HasOne(d => d.PaymentPlanItem)
                    .WithMany(p => p.Invoices)
                    .HasForeignKey(d => d.PaymentPlanItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Issue>(entity =>
            {
                entity.ToTable("issues");

                entity.HasIndex(e => e.LevelId, "IX_issues_LevelId");

                entity.HasIndex(e => e.LogId, "IX_issues_LogId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Issues)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.Issues)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Job_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => e.StateName, "IX_HangFire_Job_StateName")
                    .HasFilter("([StateName] IS NOT NULL)");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameter>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Name })
                    .HasName("PK_HangFire_JobParameter");

                entity.ToTable("JobParameter", "HangFire");

                entity.Property(e => e.Name).HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameters)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueue>(entity =>
            {
                entity.HasKey(e => new { e.Queue, e.Id })
                    .HasName("PK_HangFire_JobQueue");

                entity.ToTable("JobQueue", "HangFire");

                entity.Property(e => e.Queue).HasMaxLength(50);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.ToTable("levels");

                entity.HasIndex(e => e.Name, "IX_levels_Name")
                    .IsUnique()
                    .HasFilter("([IsDeleted]=(0))");
            });

            modelBuilder.Entity<LevelDataFinancial>(entity =>
            {
                entity.ToTable("levelDataFinancials");

                entity.HasIndex(e => e.LevelDataId, "IX_levelDataFinancials_LevelDataId");

                entity.HasOne(d => d.LevelData)
                    .WithMany(p => p.LevelDataFinancials)
                    .HasForeignKey(d => d.LevelDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LevelDataSnapshot>(entity =>
            {
                entity.HasIndex(e => e.LevelDataId, "IX_LevelDataSnapshots_LevelDataId");

                entity.HasOne(d => d.LevelData)
                    .WithMany(p => p.LevelDataSnapshots)
                    .HasForeignKey(d => d.LevelDataId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LevelsDatum>(entity =>
            {
                entity.ToTable("levelsData");

                entity.HasIndex(e => e.LevelId, "IX_levelsData_LevelId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.LevelsData)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<LevelsLog>(entity =>
            {
                entity.ToTable("levelsLogs");

                entity.HasIndex(e => e.LevelId, "IX_levelsLogs_LevelId");

                entity.HasIndex(e => e.LogId, "IX_levelsLogs_LogId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.LevelsLogs)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.LevelsLogs)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Id })
                    .HasName("PK_HangFire_List");

                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_List_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.ToTable("logs");

                entity.Property(e => e.CanHaveProperties).HasDefaultValueSql("(CONVERT([bit],(1)))");
            });

            modelBuilder.Entity<LogsDatum>(entity =>
            {
                entity.ToTable("logsData");

                entity.HasIndex(e => e.LevelId, "IX_logsData_LevelId");

                entity.HasIndex(e => e.LogId, "IX_logsData_LogId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.LogsData)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.LogsData)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Lookup>(entity =>
            {
                entity.ToTable("lookups");
            });

            modelBuilder.Entity<LookupItem>(entity =>
            {
                entity.ToTable("lookupItems");

                entity.HasIndex(e => e.LookupId, "IX_lookupItems_LookupId");

                entity.Property(e => e.AllowDelete)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.HasOne(d => d.Lookup)
                    .WithMany(p => p.LookupItems)
                    .HasForeignKey(d => d.LookupId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<MatrixValue>(entity =>
            {
                entity.ToTable("matrixValues");
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("messages");
            });

            modelBuilder.Entity<Milestone>(entity =>
            {
                entity.ToTable("milestones");

                entity.HasIndex(e => e.LevelDataId, "IX_milestones_LevelDataId");

                entity.HasIndex(e => e.LevelId, "IX_milestones_LevelId");

                entity.HasIndex(e => e.LogId, "IX_milestones_LogId");

                entity.HasIndex(e => e.PhaseGateId, "IX_milestones_PhaseGateId");

                entity.HasIndex(e => e.TaskId, "IX_milestones_TaskId")
                    .IsUnique();

                entity.Property(e => e.PlannedStartDate).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.HasOne(d => d.LevelData)
                    .WithMany(p => p.MilestoneLevelData)
                    .HasForeignKey(d => d.LevelDataId);

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.MilestoneLevels)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.Milestones)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PhaseGate)
                    .WithMany(p => p.Milestones)
                    .HasForeignKey(d => d.PhaseGateId);

                entity.HasOne(d => d.Task)
                    .WithOne(p => p.Milestone)
                    .HasForeignKey<Milestone>(d => d.TaskId);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications", "notifications");

                entity.Property(e => e.Links).HasDefaultValueSql("(N'')");

                entity.Property(e => e.UserCausedEvent).HasDefaultValueSql("(N'')");
            });

            modelBuilder.Entity<PaymentPlan>(entity =>
            {
                entity.ToTable("paymentPlans");

                entity.HasIndex(e => e.LevelId, "IX_paymentPlans_LevelId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.PaymentPlans)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PaymentPlanItem>(entity =>
            {
                entity.ToTable("paymentPlanItems");

                entity.HasIndex(e => e.PaymentPlanId, "IX_paymentPlanItems_PaymentPlanId");

                entity.HasIndex(e => e.PaymentPlanId1, "IX_paymentPlanItems_PaymentPlanId1");

                entity.HasOne(d => d.PaymentPlan)
                    .WithMany(p => p.PaymentPlanItemPaymentPlans)
                    .HasForeignKey(d => d.PaymentPlanId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PaymentPlanId1Navigation)
                    .WithMany(p => p.PaymentPlanItemPaymentPlanId1Navigations)
                    .HasForeignKey(d => d.PaymentPlanId1);

                entity.HasMany(d => d.Deliverables)
                    .WithMany(p => p.PaymentPlanItems)
                    .UsingEntity<Dictionary<string, object>>(
                        "PaymentPlanItemDeliverable",
                        l => l.HasOne<Deliverable>().WithMany().HasForeignKey("DeliverableId").OnDelete(DeleteBehavior.ClientSetNull),
                        r => r.HasOne<PaymentPlanItem>().WithMany().HasForeignKey("PaymentPlanItemId").OnDelete(DeleteBehavior.ClientSetNull),
                        j =>
                        {
                            j.HasKey("PaymentPlanItemId", "DeliverableId");

                            j.ToTable("paymentPlanItemDeliverables");

                            j.HasIndex(new[] { "DeliverableId" }, "IX_paymentPlanItemDeliverables_DeliverableId");
                        });
            });

            modelBuilder.Entity<PaymentPlanItemBoq>(entity =>
            {
                entity.ToTable("PaymentPlanItemBOQs");

                entity.HasIndex(e => e.BoqId, "IX_PaymentPlanItemBOQs_BoqId");

                entity.HasIndex(e => e.PaymentPlanItemId, "IX_PaymentPlanItemBOQs_PaymentPlanItemId");

                entity.HasOne(d => d.Boq)
                    .WithMany(p => p.PaymentPlanItemBoqs)
                    .HasForeignKey(d => d.BoqId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PaymentPlanItem)
                    .WithMany(p => p.PaymentPlanItemBoqs)
                    .HasForeignKey(d => d.PaymentPlanItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PhaseGate>(entity =>
            {
                entity.ToTable("phaseGates");

                entity.HasIndex(e => e.LevelId, "IX_phaseGates_LevelDataId");

                entity.HasIndex(e => e.LogId, "IX_phaseGates_LogId");

                entity.HasIndex(e => e.SchemaPhaseGateId, "IX_phaseGates_SchemaPhaseGateId");

                entity.Property(e => e.LogId).HasDefaultValueSql("((50))");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.PhaseGates)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.PhaseGates)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SchemaPhaseGate)
                    .WithMany(p => p.PhaseGates)
                    .HasForeignKey(d => d.SchemaPhaseGateId);
            });

            modelBuilder.Entity<PhaseGateItem>(entity =>
            {
                entity.ToTable("phaseGateItems");

                entity.HasIndex(e => e.LevelId, "IX_phaseGateItems_LevelId");

                entity.HasIndex(e => e.PhaseGateId, "IX_phaseGateItems_PhaseGateId");

                entity.HasIndex(e => e.SchemaPhaseGateItemId, "IX_phaseGateItems_SchemaPhaseGateItemId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.PhaseGateItems)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PhaseGate)
                    .WithMany(p => p.PhaseGateItems)
                    .HasForeignKey(d => d.PhaseGateId);

                entity.HasOne(d => d.SchemaPhaseGateItem)
                    .WithMany(p => p.PhaseGateItems)
                    .HasForeignKey(d => d.SchemaPhaseGateItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PhaseGateItemLog>(entity =>
            {
                entity.ToTable("phaseGateItemLogs");

                entity.HasIndex(e => e.PhaseGateItemId, "IX_phaseGateItemLogs_PhaseGateItemId");

                entity.HasOne(d => d.PhaseGateItem)
                    .WithMany(p => p.PhaseGateItemLogs)
                    .HasForeignKey(d => d.PhaseGateItemId);
            });

            modelBuilder.Entity<Predecessor>(entity =>
            {
                entity.ToTable("predecessors");

                entity.HasIndex(e => e.PredecessorTaskId, "IX_predecessors_PredecessorTaskId");

                entity.HasIndex(e => e.TaskId, "IX_predecessors_TaskId");

                entity.HasOne(d => d.PredecessorTask)
                    .WithMany(p => p.PredecessorPredecessorTasks)
                    .HasForeignKey(d => d.PredecessorTaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.PredecessorTasks)
                    .HasForeignKey(d => d.TaskId);
            });

            modelBuilder.Entity<ProcurementSchema>(entity =>
            {
                entity.HasIndex(e => e.LevelId, "IX_ProcurementSchemas_LevelId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.ProcurementSchemas)
                    .HasForeignKey(d => d.LevelId);
            });

            modelBuilder.Entity<ProcurementStage>(entity =>
            {
                entity.HasIndex(e => e.LevelId, "IX_ProcurementStages_LevelId");

                entity.HasIndex(e => e.LogId, "IX_ProcurementStages_LogId");

                entity.HasIndex(e => e.ProcurementSchemaId, "IX_ProcurementStages_ProcurementSchemaId");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.Property(e => e.IsCurrent)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.ProcurementStages)
                    .HasForeignKey(d => d.LevelId);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.ProcurementStages)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ProcurementSchema)
                    .WithMany(p => p.ProcurementStages)
                    .HasForeignKey(d => d.ProcurementSchemaId);
            });

            modelBuilder.Entity<ProgressStatus>(entity =>
            {
                entity.ToTable("progressStatuses");

                entity.HasIndex(e => e.LevelId, "IX_progressStatuses_LevelId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.ProgressStatuses)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ProjectEarnedValue>(entity =>
            {
                entity.ToTable("ProjectEarnedValue");
            });

            modelBuilder.Entity<PropertiesStep>(entity =>
            {
                entity.ToTable("propertiesSteps", "pb");

                entity.HasIndex(e => e.PropertyId, "IX_propertiesSteps_PropertyId");

                entity.HasIndex(e => e.StepSchemaId, "IX_propertiesSteps_StepSchemaId");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.PropertiesSteps)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.StepSchema)
                    .WithMany(p => p.PropertiesSteps)
                    .HasForeignKey(d => d.StepSchemaId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<PropertiesValue>(entity =>
            {
                entity.ToTable("propertiesValues");

                entity.HasIndex(e => e.ChangeRequest, "IX_propertiesValues_ChangeRequest");

                entity.HasIndex(e => e.Charter, "IX_propertiesValues_Charter");

                entity.HasIndex(e => e.Closure, "IX_propertiesValues_Closure");

                entity.HasIndex(e => e.Deliverable, "IX_propertiesValues_Deliverable");

                entity.HasIndex(e => e.DeliverableAcceptance, "IX_propertiesValues_DeliverableAcceptance");

                entity.HasIndex(e => e.GeneralTask, "IX_propertiesValues_GeneralTask");

                entity.HasIndex(e => e.Issue, "IX_propertiesValues_Issue");

                entity.HasIndex(e => e.LevelDataId, "IX_propertiesValues_LevelDataId");

                entity.HasIndex(e => e.LogDataId, "IX_propertiesValues_LogDataId");

                entity.HasIndex(e => e.Milestone, "IX_propertiesValues_Milestone");

                entity.HasIndex(e => e.PhaseGateId, "IX_propertiesValues_PhaseGateId");

                entity.HasIndex(e => e.ProcurementStageId, "IX_propertiesValues_ProcurementStageId");

                entity.HasIndex(e => new { e.PropertyId, e.ChangeRequest }, "IX_propertiesValues_PropertyId_ChangeRequest")
                    .IsUnique()
                    .HasFilter("([ChangeRequest] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.Charter }, "IX_propertiesValues_PropertyId_Charter")
                    .IsUnique()
                    .HasFilter("([Charter] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.Closure }, "IX_propertiesValues_PropertyId_Closure")
                    .IsUnique()
                    .HasFilter("([Closure] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.Deliverable }, "IX_propertiesValues_PropertyId_Deliverable")
                    .IsUnique()
                    .HasFilter("([Deliverable] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.DeliverableAcceptance }, "IX_propertiesValues_PropertyId_DeliverableAcceptance")
                    .IsUnique()
                    .HasFilter("([DeliverableAcceptance] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.GeneralTask }, "IX_propertiesValues_PropertyId_GeneralTask")
                    .IsUnique()
                    .HasFilter("([GeneralTask] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.Issue }, "IX_propertiesValues_PropertyId_Issue")
                    .IsUnique()
                    .HasFilter("([Issue] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.LevelDataId }, "IX_propertiesValues_PropertyId_LevelDataId")
                    .IsUnique()
                    .HasFilter("([LevelDataId] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.LogDataId }, "IX_propertiesValues_PropertyId_LogDataId")
                    .IsUnique()
                    .HasFilter("([LogDataId] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.Milestone }, "IX_propertiesValues_PropertyId_Milestone")
                    .IsUnique()
                    .HasFilter("([Milestone] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.PhaseGateId }, "IX_propertiesValues_PropertyId_PhaseGateId")
                    .IsUnique()
                    .HasFilter("([PhaseGateId] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.ProcurementStage }, "IX_propertiesValues_PropertyId_ProcurementStage")
                    .IsUnique()
                    .HasFilter("([ProcurementStage] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.Risk }, "IX_propertiesValues_PropertyId_Risk")
                    .IsUnique()
                    .HasFilter("([Risk] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.Stakeholder }, "IX_propertiesValues_PropertyId_Stakeholder")
                    .IsUnique()
                    .HasFilter("([Stakeholder] IS NOT NULL)");

                entity.HasIndex(e => new { e.PropertyId, e.Task }, "IX_propertiesValues_PropertyId_Task")
                    .IsUnique()
                    .HasFilter("([Task] IS NOT NULL)");

                entity.HasIndex(e => e.Risk, "IX_propertiesValues_Risk");

                entity.HasIndex(e => e.Stakeholder, "IX_propertiesValues_Stakeholder");

                entity.HasIndex(e => e.Task, "IX_propertiesValues_Task");

                entity.HasOne(d => d.ChangeRequestNavigation)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.ChangeRequest)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.CharterNavigation)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.Charter)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ClosureNavigation)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.Closure)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.DeliverableNavigation)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.Deliverable);

                entity.HasOne(d => d.DeliverableAcceptanceNavigation)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.DeliverableAcceptance)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.GeneralTaskNavigation)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.GeneralTask)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.IssueNavigation)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.Issue)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.LevelData)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.LevelDataId);

                entity.HasOne(d => d.LogData)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.LogDataId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.MilestoneNavigation)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.Milestone);

                entity.HasOne(d => d.PhaseGate)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.PhaseGateId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.ProcurementStageNavigation)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.ProcurementStageId);

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.RiskNavigation)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.Risk)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.StakeholderNavigation)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.Stakeholder)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.TaskNavigation)
                    .WithMany(p => p.PropertiesValues)
                    .HasForeignKey(d => d.Task);
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("properties");

                entity.HasIndex(e => e.Key, "IX_properties_Key")
                    .IsUnique()
                    .HasFilter("([IsDeleted]=(0))");

                entity.HasIndex(e => e.LevelId, "IX_properties_LevelId");

                entity.HasIndex(e => e.LevelLogId, "IX_properties_LevelLogId");

                entity.HasIndex(e => e.LogId, "IX_properties_LogId");

                entity.HasIndex(e => e.RefId, "IX_properties_RefId");

                entity.HasIndex(e => e.SectionId, "IX_properties_SectionId");

                entity.Property(e => e.IsHiddenInCreation)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.IsHiddenInEdition)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.Key).HasMaxLength(400);

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.PropertyLevels)
                    .HasForeignKey(d => d.LevelId);

                entity.HasOne(d => d.LevelLog)
                    .WithMany(p => p.Properties)
                    .HasForeignKey(d => d.LevelLogId);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.Properties)
                    .HasForeignKey(d => d.LogId);

                entity.HasOne(d => d.Ref)
                    .WithMany(p => p.PropertyRefs)
                    .HasForeignKey(d => d.RefId);

                entity.HasOne(d => d.Section)
                    .WithMany(p => p.Properties)
                    .HasForeignKey(d => d.SectionId);
            });

            modelBuilder.Entity<PropertySection>(entity =>
            {
                entity.HasIndex(e => e.LevelId, "IX_PropertySections_LevelId");

                entity.HasIndex(e => e.LevelLogId, "IX_PropertySections_LevelLogId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.PropertySections)
                    .HasForeignKey(d => d.LevelId);

                entity.HasOne(d => d.LevelLog)
                    .WithMany(p => p.PropertySections)
                    .HasForeignKey(d => d.LevelLogId);
            });

            modelBuilder.Entity<PropertyStatus>(entity =>
            {
                entity.ToTable("propertyStatuses");

                entity.HasIndex(e => e.PropertyId, "IX_propertyStatuses_PropertyId");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.PropertyStatuses)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Receiver>(entity =>
            {
                entity.ToTable("receivers", "notifications");

                entity.HasIndex(e => e.TemplateId, "IX_receivers_TemplateId");

                entity.HasOne(d => d.Template)
                    .WithMany(p => p.Receivers)
                    .HasForeignKey(d => d.TemplateId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Registery>(entity =>
            {
                entity.ToTable("registeries", "wf");

                entity.HasIndex(e => e.WorkflowRegistryId, "IX_registeries_WorkflowRegistryId");

                entity.HasOne(d => d.WorkflowRegistry)
                    .WithMany(p => p.Registeries)
                    .HasForeignKey(d => d.WorkflowRegistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Request>(entity =>
            {
                entity.ToTable("requests", "pb");

                entity.HasIndex(e => e.RequestSchemaId, "IX_requests_RequestSchemaId");

                entity.Property(e => e.CreatedAt).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.HasOne(d => d.RequestSchema)
                    .WithMany(p => p.Requests)
                    .HasForeignKey(d => d.RequestSchemaId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<RequestProperty>(entity =>
            {
                entity.ToTable("requestProperties", "pb");

                entity.HasIndex(e => new { e.RefId, e.Type, e.RequestSchemaId }, "IX_requestProperties_RefId_Type_RequestSchemaId")
                    .IsUnique()
                    .HasFilter("([Type] IS NOT NULL)");

                entity.HasIndex(e => e.RequestSchemaId, "IX_requestProperties_RequestSchemaId");

                entity.HasOne(d => d.RequestSchema)
                    .WithMany(p => p.RequestProperties)
                    .HasForeignKey(d => d.RequestSchemaId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<RequestPropertyValue>(entity =>
            {
                entity.ToTable("requestPropertyValues", "pb");

                entity.HasIndex(e => e.RequestId, "IX_requestPropertyValues_RequestId");

                entity.HasIndex(e => e.RequestPropertyId, "IX_requestPropertyValues_RequestPropertyId");

                entity.HasIndex(e => e.StepId, "IX_requestPropertyValues_StepId");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.RequestPropertyValues)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.RequestProperty)
                    .WithMany(p => p.RequestPropertyValues)
                    .HasForeignKey(d => d.RequestPropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Step)
                    .WithMany(p => p.RequestPropertyValues)
                    .HasForeignKey(d => d.StepId);
            });

            modelBuilder.Entity<RequestsSchema>(entity =>
            {
                entity.ToTable("requestsSchema", "pb");
            });

            modelBuilder.Entity<Risk>(entity =>
            {
                entity.ToTable("risks");

                entity.HasIndex(e => e.LevelId, "IX_risks_LevelId");

                entity.HasIndex(e => e.LogId, "IX_risks_LogId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Risks)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.Risks)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<ScheduleViewColumn>(entity =>
            {
                entity.HasKey(e => new { e.ScheduleColumnId, e.ScheduleViewId });

                entity.HasIndex(e => e.ScheduleViewId, "IX_ScheduleViewColumns_ScheduleViewId");

                entity.HasOne(d => d.ScheduleColumn)
                    .WithMany(p => p.ScheduleViewColumns)
                    .HasForeignKey(d => d.ScheduleColumnId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ScheduleView)
                    .WithMany(p => p.ScheduleViewColumns)
                    .HasForeignKey(d => d.ScheduleViewId);
            });

            modelBuilder.Entity<Schema>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<SchemaPhaseGate>(entity =>
            {
                entity.HasIndex(e => e.LevelId, "IX_SchemaPhaseGates_LevelId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.SchemaPhaseGates)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SchemaPhaseGateItem>(entity =>
            {
                entity.ToTable("schemaPhaseGateItems");

                entity.HasIndex(e => e.SchemaPhaseGateId, "IX_schemaPhaseGateItems_SchemaPhaseGateId");

                entity.HasOne(d => d.SchemaPhaseGate)
                    .WithMany(p => p.SchemaPhaseGateItems)
                    .HasForeignKey(d => d.SchemaPhaseGateId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SchemaPhaseGateLog>(entity =>
            {
                entity.ToTable("schemaPhaseGateLogs");

                entity.HasIndex(e => e.LogId, "IX_schemaPhaseGateLogs_LogId");

                entity.HasIndex(e => e.SchemaPhaseGateId, "IX_schemaPhaseGateLogs_SchemaPhaseGateId");

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.SchemaPhaseGateLogs)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SchemaPhaseGate)
                    .WithMany(p => p.SchemaPhaseGateLogs)
                    .HasForeignKey(d => d.SchemaPhaseGateId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Server>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.HasIndex(e => e.LastHeartbeat, "IX_HangFire_Server_LastHeartbeat");

                entity.Property(e => e.Id).HasMaxLength(200);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.HasKey(e => new { e.Key, e.Value })
                    .HasName("PK_HangFire_Set");

                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => e.ExpireAt, "IX_HangFire_Set_ExpireAt")
                    .HasFilter("([ExpireAt] IS NOT NULL)");

                entity.HasIndex(e => new { e.Key, e.Score }, "IX_HangFire_Set_Score");

                entity.Property(e => e.Key).HasMaxLength(100);

                entity.Property(e => e.Value).HasMaxLength(256);

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Stakeholder>(entity =>
            {
                entity.ToTable("stakeholders");

                entity.HasIndex(e => e.LevelId, "IX_stakeholders_LevelId");

                entity.HasIndex(e => e.LogId, "IX_stakeholders_LogId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Stakeholders)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.Stakeholders)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasKey(e => new { e.JobId, e.Id })
                    .HasName("PK_HangFire_State");

                entity.ToTable("State", "HangFire");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name).HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.States)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            modelBuilder.Entity<Step>(entity =>
            {
                entity.ToTable("steps", "pb");

                entity.HasIndex(e => e.RequestId, "IX_steps_RequestId");

                entity.HasIndex(e => e.StepSchemaId, "IX_steps_StepSchemaId");

                entity.Property(e => e.ActionDate).HasDefaultValueSql("('0001-01-01T00:00:00.0000000')");

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.Steps)
                    .HasForeignKey(d => d.RequestId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.StepSchema)
                    .WithMany(p => p.Steps)
                    .HasForeignKey(d => d.StepSchemaId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StepConnection>(entity =>
            {
                entity.ToTable("stepConnections", "pb");

                entity.HasIndex(e => e.SourceStepId, "IX_stepConnections_SourceStepId");

                entity.HasIndex(e => e.StepConnectionSchemaId, "IX_stepConnections_StepConnectionSchemaId");

                entity.HasIndex(e => e.TargetStepId, "IX_stepConnections_TargetStepId");

                entity.HasOne(d => d.SourceStep)
                    .WithMany(p => p.StepConnectionSourceSteps)
                    .HasForeignKey(d => d.SourceStepId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.StepConnectionSchema)
                    .WithMany(p => p.StepConnections)
                    .HasForeignKey(d => d.StepConnectionSchemaId);

                entity.HasOne(d => d.TargetStep)
                    .WithMany(p => p.StepConnectionTargetSteps)
                    .HasForeignKey(d => d.TargetStepId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StepConnectionsSchema>(entity =>
            {
                entity.ToTable("stepConnectionsSchema", "pb");

                entity.HasIndex(e => e.RequestSchemaId, "IX_stepConnectionsSchema_RequestSchemaId");

                entity.HasIndex(e => e.SourceStepSchemaId, "IX_stepConnectionsSchema_SourceStepSchemaId");

                entity.HasIndex(e => e.TargetStepSchemaId, "IX_stepConnectionsSchema_TargetStepSchemaId");

                entity.HasOne(d => d.RequestSchema)
                    .WithMany(p => p.StepConnectionsSchemas)
                    .HasForeignKey(d => d.RequestSchemaId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.SourceStepSchema)
                    .WithMany(p => p.StepConnectionsSchemaSourceStepSchemas)
                    .HasForeignKey(d => d.SourceStepSchemaId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TargetStepSchema)
                    .WithMany(p => p.StepConnectionsSchemaTargetStepSchemas)
                    .HasForeignKey(d => d.TargetStepSchemaId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<StepsSchema>(entity =>
            {
                entity.ToTable("stepsSchema", "pb");

                entity.HasIndex(e => e.RequestSchemaId, "IX_stepsSchema_RequestSchemaId");

                entity.HasOne(d => d.RequestSchema)
                    .WithMany(p => p.StepsSchemas)
                    .HasForeignKey(d => d.RequestSchemaId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<SummaryNotification>(entity =>
            {
                entity.ToTable("summaryNotification", "notifications");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.ToTable("tasks");

                entity.HasIndex(e => e.LevelId, "IX_tasks_LevelId");

                entity.HasIndex(e => e.LogId, "IX_tasks_LogId");

                entity.HasIndex(e => e.PhaseGateId, "IX_tasks_PhaseGateId");

                entity.Property(e => e.HasEscalations)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.PhaseGate)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.PhaseGateId);
            });

            modelBuilder.Entity<TaskBaseline>(entity =>
            {
                entity.ToTable("taskBaselines");

                entity.HasIndex(e => e.LevelId, "IX_taskBaselines_LevelId");

                entity.HasIndex(e => e.TaskId, "IX_taskBaselines_TaskId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.TaskBaselines)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskBaselines)
                    .HasForeignKey(d => d.TaskId);
            });

            modelBuilder.Entity<TaskProgressUpdateRequest>(entity =>
            {
                entity.ToTable("taskProgressUpdateRequests");
            });

            modelBuilder.Entity<TeamMember>(entity =>
            {
                entity.ToTable("teamMembers");

                entity.HasIndex(e => e.LevelId, "IX_teamMembers_LevelId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.TeamMembers)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Template>(entity =>
            {
                entity.ToTable("templates", "notifications");

                entity.HasIndex(e => e.EventId, "IX_templates_EventId");

                entity.Property(e => e.Smscontent).HasColumnName("SMSContent");

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Templates)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<TimeSheet>(entity =>
            {
                entity.ToTable("timeSheets");

                entity.HasIndex(e => e.LevelId, "IX_timeSheets_LevelId");

                entity.HasIndex(e => e.LogId, "IX_timeSheets_LogId");

                entity.HasOne(d => d.Level)
                    .WithMany(p => p.TimeSheets)
                    .HasForeignKey(d => d.LevelId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Log)
                    .WithMany(p => p.TimeSheets)
                    .HasForeignKey(d => d.LogId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<TimeSheetItem>(entity =>
            {
                entity.ToTable("timeSheetItems");

                entity.HasIndex(e => e.DeliverableId, "IX_timeSheetItems_DeliverableId");

                entity.HasIndex(e => e.TimeSheetId, "IX_timeSheetItems_TimeSheetId");

                entity.HasOne(d => d.Deliverable)
                    .WithMany(p => p.TimeSheetItems)
                    .HasForeignKey(d => d.DeliverableId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.TimeSheet)
                    .WithMany(p => p.TimeSheetItems)
                    .HasForeignKey(d => d.TimeSheetId);
            });

            modelBuilder.Entity<UserNotification>(entity =>
            {
                entity.Property(e => e.UpdatedBy).HasColumnName("updatedBy");
            });

            modelBuilder.Entity<WorkflowAction>(entity =>
            {
                entity.ToTable("workflowActions", "wf");

                entity.HasIndex(e => e.WorkflowActivityId, "IX_workflowActions_WorkflowActivityId");

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.Sla).HasColumnName("SLA");

                entity.HasOne(d => d.WorkflowActivity)
                    .WithMany(p => p.WorkflowActions)
                    .HasForeignKey(d => d.WorkflowActivityId);
            });

            modelBuilder.Entity<WorkflowActivity>(entity =>
            {
                entity.ToTable("workflowActivities", "wf");

                entity.HasIndex(e => e.FalseDirectionActivityId, "IX_workflowActivities_FalseDirectionActivityId")
                    .IsUnique()
                    .HasFilter("([FalseDirectionActivityId] IS NOT NULL)");

                entity.HasIndex(e => e.PreviousWorkflowActivityId, "IX_workflowActivities_PreviousWorkflowActivityId");

                entity.HasIndex(e => e.TrueDirectionActivityId, "IX_workflowActivities_TrueDirectionActivityId")
                    .IsUnique()
                    .HasFilter("([TrueDirectionActivityId] IS NOT NULL)");

                entity.HasIndex(e => e.WorkflowRegistryId, "IX_workflowActivities_WorkflowRegistryId");

                entity.Property(e => e.IsDeleted)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.HasOne(d => d.FalseDirectionActivity)
                    .WithOne(p => p.InverseFalseDirectionActivity)
                    .HasForeignKey<WorkflowActivity>(d => d.FalseDirectionActivityId);

                entity.HasOne(d => d.PreviousWorkflowActivity)
                    .WithMany(p => p.InversePreviousWorkflowActivity)
                    .HasForeignKey(d => d.PreviousWorkflowActivityId);

                entity.HasOne(d => d.TrueDirectionActivity)
                    .WithOne(p => p.InverseTrueDirectionActivity)
                    .HasForeignKey<WorkflowActivity>(d => d.TrueDirectionActivityId);

                entity.HasOne(d => d.WorkflowRegistry)
                    .WithMany(p => p.WorkflowActivities)
                    .HasForeignKey(d => d.WorkflowRegistryId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<WorkflowRegistry>(entity =>
            {
                entity.ToTable("workflowRegistries", "wf");

                entity.Property(e => e.IsDraftActive)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");

                entity.Property(e => e.PreventMultiInstances)
                    .IsRequired()
                    .HasDefaultValueSql("(CONVERT([bit],(0)))");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
