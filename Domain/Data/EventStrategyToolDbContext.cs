using System;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.Data;

public partial class EventStrategyToolDbContext : DbContext
{
    public EventStrategyToolDbContext(DbContextOptions<EventStrategyToolDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AnnualBudget> AnnualBudgets { get; set; }

    public virtual DbSet<Approval> Approvals { get; set; }

    public virtual DbSet<Assessment> Assessments { get; set; }

    public virtual DbSet<AssessmentFactorResult> AssessmentFactorResults { get; set; }

    public virtual DbSet<AssessmentInputAnswer> AssessmentInputAnswers { get; set; }

    public virtual DbSet<AssessmentKpiValue> AssessmentKpiValues { get; set; }

    public virtual DbSet<BandingThreshold> BandingThresholds { get; set; }

    public virtual DbSet<ComparisonMetric> ComparisonMetrics { get; set; }

    public virtual DbSet<DecisionOutcome> DecisionOutcomes { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DepartmentAlignment> DepartmentAlignments { get; set; }

    public virtual DbSet<EntertainmentType> EntertainmentTypes { get; set; }

    public virtual DbSet<EvaluationFactor> EvaluationFactors { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventComment> EventComments { get; set; }

    public virtual DbSet<EventComparison> EventComparisons { get; set; }

    public virtual DbSet<EventDepartment> EventDepartments { get; set; }

    public virtual DbSet<EventResponsibility> EventResponsibilities { get; set; }

    public virtual DbSet<EventRiskUpside> EventRiskUpsides { get; set; }

    public virtual DbSet<EventStatus> EventStatuses { get; set; }

    public virtual DbSet<EventStatusHistory> EventStatusHistories { get; set; }

    public virtual DbSet<EventTicketTier> EventTicketTiers { get; set; }

    public virtual DbSet<EventType> EventTypes { get; set; }

    public virtual DbSet<FundingSource> FundingSources { get; set; }

    public virtual DbSet<GoverningEntity> GoverningEntities { get; set; }

    public virtual DbSet<Icp> Icps { get; set; }

    public virtual DbSet<InputVariable> InputVariables { get; set; }

    public virtual DbSet<InputVariableOption> InputVariableOptions { get; set; }

    public virtual DbSet<InviteMethod> InviteMethods { get; set; }

    public virtual DbSet<Kpi> Kpis { get; set; }

    public virtual DbSet<Objective> Objectives { get; set; }

    public virtual DbSet<ObjectiveBenchMark> ObjectiveBenchMarks { get; set; }

    public virtual DbSet<PnlCategory> PnlCategories { get; set; }

    public virtual DbSet<PnlLine> PnlLines { get; set; }

    public virtual DbSet<PnlLineType> PnlLineTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TicketTierKind> TicketTierKinds { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VwEventComparison> VwEventComparisons { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnnualBudget>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Annual_Budget");

            entity.ToTable("Annual Budget");

            entity.HasIndex(e => e.FiscalYear, "UQ_Annual_Budget_Year").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.BudgetAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Currency)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasDefaultValue("EGP", "DF_Annual_Budget_Currency");
        });

        modelBuilder.Entity<Approval>(entity =>
        {
            entity.ToTable("Approval");

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.ApproverRoleId).HasMaxLength(21);
            entity.Property(e => e.ApproverUserId).HasMaxLength(21);
            entity.Property(e => e.AssessmentId).HasMaxLength(21);
            entity.Property(e => e.DecisionOutcomeId).HasMaxLength(21);
            entity.Property(e => e.DecisionSummary).HasMaxLength(2000);
            entity.Property(e => e.EventId).HasMaxLength(21);
            entity.Property(e => e.OverrideReason).HasMaxLength(500);
            entity.Property(e => e.SignedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Approval_SignedAt");

            entity.HasOne(d => d.ApproverRole).WithMany(p => p.Approvals)
                .HasForeignKey(d => d.ApproverRoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Approval_Role");

            entity.HasOne(d => d.ApproverUser).WithMany(p => p.Approvals)
                .HasForeignKey(d => d.ApproverUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Approval_User");

            entity.HasOne(d => d.Assessment).WithMany(p => p.Approvals)
                .HasForeignKey(d => d.AssessmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Approval_Assessment");

            entity.HasOne(d => d.DecisionOutcome).WithMany(p => p.Approvals)
                .HasForeignKey(d => d.DecisionOutcomeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Approval_Outcome");

            entity.HasOne(d => d.Event).WithMany(p => p.Approvals)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Approval_Event");
        });

        modelBuilder.Entity<Assessment>(entity =>
        {
            entity.ToTable("Assessment");

            entity.HasIndex(e => new { e.EventId, e.Phase }, "UQ_Assessment_Event_Phase").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.EventId).HasMaxLength(21);
            entity.Property(e => e.Phase)
                .HasMaxLength(10)
                .HasDefaultValue("PRE", "DF_Assessment_Phase");
            entity.Property(e => e.RecommendedOutcomeId).HasMaxLength(21);
            entity.Property(e => e.RoiInclMarketing).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.RoiPure).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.ScalabilityScore).HasColumnType("decimal(9, 4)");
            entity.Property(e => e.SubmittedByUserId).HasMaxLength(21);
            entity.Property(e => e.TotalScore).HasColumnType("decimal(9, 4)");

            entity.HasOne(d => d.Event).WithMany(p => p.Assessments)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assessment_Event");

            entity.HasOne(d => d.RecommendedOutcome).WithMany(p => p.Assessments)
                .HasForeignKey(d => d.RecommendedOutcomeId)
                .HasConstraintName("FK_Assessment_Outcome");

            entity.HasOne(d => d.SubmittedByUser).WithMany(p => p.Assessments)
                .HasForeignKey(d => d.SubmittedByUserId)
                .HasConstraintName("FK_Assessment_SubmittedBy");
        });

        modelBuilder.Entity<AssessmentFactorResult>(entity =>
        {
            entity.ToTable("AssessmentFactorResult");

            entity.HasIndex(e => new { e.AssessmentId, e.EvaluationFactorId }, "UQ_AssessmentFactorResult").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.AssessmentId).HasMaxLength(21);
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.EvaluationFactorId).HasMaxLength(21);
            entity.Property(e => e.Score).HasColumnType("decimal(9, 4)");
            entity.Property(e => e.Weight).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.WeightedScore).HasColumnType("decimal(9, 4)");

            entity.HasOne(d => d.Assessment).WithMany(p => p.AssessmentFactorResults)
                .HasForeignKey(d => d.AssessmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentFactorResult_Assessment");

            entity.HasOne(d => d.EvaluationFactor).WithMany(p => p.AssessmentFactorResults)
                .HasForeignKey(d => d.EvaluationFactorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentFactorResult_Factor");
        });

        modelBuilder.Entity<AssessmentInputAnswer>(entity =>
        {
            entity.ToTable("AssessmentInputAnswer");

            entity.HasIndex(e => new { e.AssessmentId, e.InputVariableId, e.InputVariableOptionId }, "UX_AssessmentInputAnswer").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.AssessmentId).HasMaxLength(21);
            entity.Property(e => e.InputVariableId).HasMaxLength(21);
            entity.Property(e => e.InputVariableOptionId).HasMaxLength(21);
            entity.Property(e => e.NumberValue).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.OtherText).HasMaxLength(400);

            entity.HasOne(d => d.Assessment).WithMany(p => p.AssessmentInputAnswers)
                .HasForeignKey(d => d.AssessmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentInputAnswer_Assessment");

            entity.HasOne(d => d.InputVariable).WithMany(p => p.AssessmentInputAnswers)
                .HasForeignKey(d => d.InputVariableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentInputAnswer_Variable");

            entity.HasOne(d => d.InputVariableOption).WithMany(p => p.AssessmentInputAnswers)
                .HasPrincipalKey(p => new { p.Id, p.InputVariableId })
                .HasForeignKey(d => new { d.InputVariableOptionId, d.InputVariableId })
                .HasConstraintName("FK_AssessmentInputAnswer_Option");
        });

        modelBuilder.Entity<AssessmentKpiValue>(entity =>
        {
            entity.ToTable("AssessmentKpiValue");

            entity.HasIndex(e => new { e.AssessmentId, e.KpiId }, "UQ_AssessmentKpiValue_Assessment_Kpi").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.AssessmentId).HasMaxLength(21);
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.InputValue).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.KpiId).HasMaxLength(21);
            entity.Property(e => e.Score).HasColumnType("decimal(9, 4)");
            entity.Property(e => e.WeightedScore).HasColumnType("decimal(9, 4)");

            entity.HasOne(d => d.Assessment).WithMany(p => p.AssessmentKpiValues)
                .HasForeignKey(d => d.AssessmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentKpiValue_Assessment");

            entity.HasOne(d => d.Kpi).WithMany(p => p.AssessmentKpiValues)
                .HasForeignKey(d => d.KpiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentKpiValue_KPI");
        });

        modelBuilder.Entity<BandingThreshold>(entity =>
        {
            entity.ToTable("Banding_Thresholds");

            entity.HasIndex(e => new { e.KpiId, e.Score }, "UQ_Banding_Thresholds_Kpi_Score").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.KpiId).HasMaxLength(21);
            entity.Property(e => e.MaxValue).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.MinValue).HasColumnType("decimal(18, 6)");

            entity.HasOne(d => d.Kpi).WithMany(p => p.BandingThresholds)
                .HasForeignKey(d => d.KpiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Banding_Thresholds_KPI");
        });

        modelBuilder.Entity<ComparisonMetric>(entity =>
        {
            entity.ToTable("ComparisonMetric");

            entity.HasIndex(e => e.Name, "UQ_ComparisonMetric_Name").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_ComparisonMetric_IsActive");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.TolerancePct)
                .HasDefaultValue(0.10m, "DF_ComparisonMetric_Tolerance")
                .HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Unit).HasMaxLength(20);
        });

        modelBuilder.Entity<DecisionOutcome>(entity =>
        {
            entity.ToTable("DecisionOutcome");

            entity.HasIndex(e => e.Name, "UQ_DecisionOutcome_Name").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.ToTable("Department");

            entity.HasIndex(e => e.Name, "UQ_Department_Name").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Department_IsActive");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<DepartmentAlignment>(entity =>
        {
            entity.HasIndex(e => new { e.AssessmentId, e.KpiId, e.DepartmentId }, "UQ_DepartmentAlignments_Assessment_Kpi_Dept").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.AlignedByUserId).HasMaxLength(21);
            entity.Property(e => e.AssessmentId).HasMaxLength(21);
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.DepartmentId).HasMaxLength(21);
            entity.Property(e => e.KpiId).HasMaxLength(21);

            entity.HasOne(d => d.AlignedByUser).WithMany(p => p.DepartmentAlignments)
                .HasForeignKey(d => d.AlignedByUserId)
                .HasConstraintName("FK_DepartmentAlignments_User");

            entity.HasOne(d => d.Assessment).WithMany(p => p.DepartmentAlignments)
                .HasForeignKey(d => d.AssessmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DepartmentAlignments_Assessment");

            entity.HasOne(d => d.Department).WithMany(p => p.DepartmentAlignments)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DepartmentAlignments_Department");

            entity.HasOne(d => d.Kpi).WithMany(p => p.DepartmentAlignments)
                .HasForeignKey(d => d.KpiId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DepartmentAlignments_KPI");
        });

        modelBuilder.Entity<EntertainmentType>(entity =>
        {
            entity.ToTable("Entertainment_Type");

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.EntertainmentTaxRate).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<EvaluationFactor>(entity =>
        {
            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Weight).HasColumnType("decimal(9, 6)");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.ToTable("Event");

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.AccessLevels).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_Event_CreatedAt");
            entity.Property(e => e.CreatedByUserId).HasMaxLength(21);
            entity.Property(e => e.EntertainmentTypeId).HasMaxLength(21);
            entity.Property(e => e.EventOwnerUserId).HasMaxLength(21);
            entity.Property(e => e.EventTypeId).HasMaxLength(21);
            entity.Property(e => e.FundingSourceId).HasMaxLength(21);
            entity.Property(e => e.GoverningEntityId).HasMaxLength(21);
            entity.Property(e => e.InviteMethodId).HasMaxLength(21);
            entity.Property(e => e.Location).HasMaxLength(100);
            entity.Property(e => e.Name).HasMaxLength(150);
            entity.Property(e => e.PrimaryIcpId).HasMaxLength(21);
            entity.Property(e => e.PrimaryObjectiveId).HasMaxLength(21);
            entity.Property(e => e.SecondaryIcpId).HasMaxLength(21);
            entity.Property(e => e.SecondaryObjectiveId).HasMaxLength(21);
            entity.Property(e => e.SpecialExperiences).HasMaxLength(500);
            entity.Property(e => e.StatusId).HasMaxLength(21);
            entity.Property(e => e.TargetAudience).HasMaxLength(200);
            entity.Property(e => e.TicketTypes).HasMaxLength(500);
            entity.Property(e => e.Venue).HasMaxLength(200);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.EventCreatedByUsers)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_CreatedBy");

            entity.HasOne(d => d.EntertainmentType).WithMany(p => p.Events)
                .HasForeignKey(d => d.EntertainmentTypeId)
                .HasConstraintName("FK_Event_EntertainmentType");

            entity.HasOne(d => d.EventOwnerUser).WithMany(p => p.EventEventOwnerUsers)
                .HasForeignKey(d => d.EventOwnerUserId)
                .HasConstraintName("FK_Event_Owner");

            entity.HasOne(d => d.EventType).WithMany(p => p.Events)
                .HasForeignKey(d => d.EventTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_EventType");

            entity.HasOne(d => d.FundingSource).WithMany(p => p.Events)
                .HasForeignKey(d => d.FundingSourceId)
                .HasConstraintName("FK_Event_FundingSource");

            entity.HasOne(d => d.GoverningEntity).WithMany(p => p.Events)
                .HasForeignKey(d => d.GoverningEntityId)
                .HasConstraintName("FK_Event_GoverningEntity");

            entity.HasOne(d => d.InviteMethod).WithMany(p => p.Events)
                .HasForeignKey(d => d.InviteMethodId)
                .HasConstraintName("FK_Event_InviteMethod");

            entity.HasOne(d => d.PrimaryIcp).WithMany(p => p.EventPrimaryIcps)
                .HasForeignKey(d => d.PrimaryIcpId)
                .HasConstraintName("FK_Event_PrimaryIcp");

            entity.HasOne(d => d.PrimaryObjective).WithMany(p => p.EventPrimaryObjectives)
                .HasForeignKey(d => d.PrimaryObjectiveId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_PrimaryObjective");

            entity.HasOne(d => d.SecondaryIcp).WithMany(p => p.EventSecondaryIcps)
                .HasForeignKey(d => d.SecondaryIcpId)
                .HasConstraintName("FK_Event_SecondaryIcp");

            entity.HasOne(d => d.SecondaryObjective).WithMany(p => p.EventSecondaryObjectives)
                .HasForeignKey(d => d.SecondaryObjectiveId)
                .HasConstraintName("FK_Event_SecondaryObjective");

            entity.HasOne(d => d.Status).WithMany(p => p.Events)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Status");
        });

        modelBuilder.Entity<EventComment>(entity =>
        {
            entity.ToTable("EventComment");

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.Comment).HasMaxLength(2000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_EventComment_CreatedAt");
            entity.Property(e => e.CreatedByUserId).HasMaxLength(21);
            entity.Property(e => e.DepartmentId).HasMaxLength(21);
            entity.Property(e => e.EventId).HasMaxLength(21);

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.EventComments)
                .HasForeignKey(d => d.CreatedByUserId)
                .HasConstraintName("FK_EventComment_User");

            entity.HasOne(d => d.Department).WithMany(p => p.EventComments)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventComment_Department");

            entity.HasOne(d => d.Event).WithMany(p => p.EventComments)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventComment_Event");
        });

        modelBuilder.Entity<EventComparison>(entity =>
        {
            entity.ToTable("EventComparison");

            entity.HasIndex(e => new { e.EventId, e.ComparisonMetricId }, "UQ_EventComparison_Event_Metric").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.ActualEnteredByUserId).HasMaxLength(21);
            entity.Property(e => e.ActualValue).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.ComparisonMetricId).HasMaxLength(21);
            entity.Property(e => e.Difference)
                .HasComputedColumnSql("([ActualValue]-[ExpectedValue])", true)
                .HasColumnType("decimal(19, 4)");
            entity.Property(e => e.DifferencePct)
                .HasComputedColumnSql("(case when [ExpectedValue] IS NULL OR [ExpectedValue]=(0) OR [ActualValue] IS NULL then NULL else CONVERT([decimal](18,6),([ActualValue]-[ExpectedValue])/abs([ExpectedValue])) end)", true)
                .HasColumnType("decimal(18, 6)");
            entity.Property(e => e.EventId).HasMaxLength(21);
            entity.Property(e => e.ExpectedEnteredByUserId).HasMaxLength(21);
            entity.Property(e => e.ExpectedValue).HasColumnType("decimal(18, 4)");

            entity.HasOne(d => d.ActualEnteredByUser).WithMany(p => p.EventComparisonActualEnteredByUsers)
                .HasForeignKey(d => d.ActualEnteredByUserId)
                .HasConstraintName("FK_EventComparison_ActualUser");

            entity.HasOne(d => d.ComparisonMetric).WithMany(p => p.EventComparisons)
                .HasForeignKey(d => d.ComparisonMetricId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventComparison_Metric");

            entity.HasOne(d => d.Event).WithMany(p => p.EventComparisons)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventComparison_Event");

            entity.HasOne(d => d.ExpectedEnteredByUser).WithMany(p => p.EventComparisonExpectedEnteredByUsers)
                .HasForeignKey(d => d.ExpectedEnteredByUserId)
                .HasConstraintName("FK_EventComparison_ExpectedUser");
        });

        modelBuilder.Entity<EventDepartment>(entity =>
        {
            entity.HasKey(e => new { e.EventId, e.DepartmentId });

            entity.ToTable("EventDepartment");

            entity.Property(e => e.EventId).HasMaxLength(21);
            entity.Property(e => e.DepartmentId).HasMaxLength(21);
            entity.Property(e => e.DepartmentRole).HasMaxLength(20);

            entity.HasOne(d => d.Department).WithMany(p => p.EventDepartments)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventDepartment_Department");

            entity.HasOne(d => d.Event).WithMany(p => p.EventDepartments)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventDepartment_Event");
        });

        modelBuilder.Entity<EventResponsibility>(entity =>
        {
            entity.ToTable("EventResponsibility");

            entity.HasIndex(e => new { e.EventId, e.ResponsibilityType }, "UQ_EventResponsibility").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.EventId).HasMaxLength(21);
            entity.Property(e => e.PersonName).HasMaxLength(100);
            entity.Property(e => e.ResponsibilityType).HasMaxLength(30);
            entity.Property(e => e.UserId).HasMaxLength(21);

            entity.HasOne(d => d.Event).WithMany(p => p.EventResponsibilities)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventResponsibility_Event");

            entity.HasOne(d => d.User).WithMany(p => p.EventResponsibilities)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_EventResponsibility_User");
        });

        modelBuilder.Entity<EventRiskUpside>(entity =>
        {
            entity.ToTable("EventRiskUpside");

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.EventId).HasMaxLength(21);
            entity.Property(e => e.ItemType).HasMaxLength(10);

            entity.HasOne(d => d.Event).WithMany(p => p.EventRiskUpsides)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventRiskUpside_Event");
        });

        modelBuilder.Entity<EventStatus>(entity =>
        {
            entity.ToTable("EventStatus");

            entity.HasIndex(e => e.Name, "UQ_EventStatus_Name").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<EventStatusHistory>(entity =>
        {
            entity.ToTable("EventStatusHistory");

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.ChangedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_EventStatusHistory_ChangedAt");
            entity.Property(e => e.ChangedByUserId).HasMaxLength(21);
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.EventId).HasMaxLength(21);
            entity.Property(e => e.FromStatusId).HasMaxLength(21);
            entity.Property(e => e.ToStatusId).HasMaxLength(21);

            entity.HasOne(d => d.ChangedByUser).WithMany(p => p.EventStatusHistories)
                .HasForeignKey(d => d.ChangedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventStatusHistory_User");

            entity.HasOne(d => d.Event).WithMany(p => p.EventStatusHistories)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventStatusHistory_Event");

            entity.HasOne(d => d.FromStatus).WithMany(p => p.EventStatusHistoryFromStatuses)
                .HasForeignKey(d => d.FromStatusId)
                .HasConstraintName("FK_EventStatusHistory_FromStatus");

            entity.HasOne(d => d.ToStatus).WithMany(p => p.EventStatusHistoryToStatuses)
                .HasForeignKey(d => d.ToStatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventStatusHistory_ToStatus");
        });

        modelBuilder.Entity<EventTicketTier>(entity =>
        {
            entity.ToTable("EventTicketTier");

            entity.HasIndex(e => new { e.EventId, e.TicketTierKindId, e.TierNumber }, "UQ_EventTicketTier").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.ActualTotal)
                .HasComputedColumnSql("(CONVERT([decimal](18,2),[ActualQuantity]*[UnitPrice]))", true)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EventId).HasMaxLength(21);
            entity.Property(e => e.ExpectedTotal)
                .HasComputedColumnSql("(CONVERT([decimal](18,2),[ExpectedQuantity]*[UnitPrice]))", true)
                .HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TicketTierKindId).HasMaxLength(21);
            entity.Property(e => e.TierName).HasMaxLength(100);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Event).WithMany(p => p.EventTicketTiers)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventTicketTier_Event");

            entity.HasOne(d => d.TicketTierKind).WithMany(p => p.EventTicketTiers)
                .HasForeignKey(d => d.TicketTierKindId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EventTicketTier_Kind");
        });

        modelBuilder.Entity<EventType>(entity =>
        {
            entity.ToTable("EventType");

            entity.HasIndex(e => e.Name, "UQ_EventType_Name").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_EventType_IsActive");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<FundingSource>(entity =>
        {
            entity.ToTable("FundingSource");

            entity.HasIndex(e => e.Name, "UQ_FundingSource_Name").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_FundingSource_IsActive");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<GoverningEntity>(entity =>
        {
            entity.ToTable("GoverningEntity");

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.MaxScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MinScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Icp>(entity =>
        {
            entity.ToTable("ICP");

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_ICP_IsActive");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<InputVariable>(entity =>
        {
            entity.ToTable("InputVariable");

            entity.HasIndex(e => e.Name, "UQ_InputVariable_Name").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.GovernanceWeight).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.InputType).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_InputVariable_IsActive");
            entity.Property(e => e.KpiId).HasMaxLength(21);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Unit).HasMaxLength(20);

            entity.HasOne(d => d.Kpi).WithMany(p => p.InputVariables)
                .HasForeignKey(d => d.KpiId)
                .HasConstraintName("FK_InputVariable_KPI");
        });

        modelBuilder.Entity<InputVariableOption>(entity =>
        {
            entity.ToTable("InputVariableOption");

            entity.HasIndex(e => new { e.Id, e.InputVariableId }, "UQ_InputVariableOption_Id_Variable").IsUnique();

            entity.HasIndex(e => new { e.InputVariableId, e.Score }, "UX_InputVariableOption_Variable_Score")
                .IsUnique()
                .HasFilter("([Score] IS NOT NULL)");

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.Description).HasMaxLength(250);
            entity.Property(e => e.InputVariableId).HasMaxLength(21);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_InputVariableOption_IsActive");

            entity.HasOne(d => d.InputVariable).WithMany(p => p.InputVariableOptions)
                .HasForeignKey(d => d.InputVariableId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InputVariableOption_Variable");
        });

        modelBuilder.Entity<InviteMethod>(entity =>
        {
            entity.ToTable("InviteMethod");

            entity.HasIndex(e => e.Name, "UQ_InviteMethod_Name").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_InviteMethod_IsActive");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Kpi>(entity =>
        {
            entity.ToTable("KPI");

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.AlignedDepartmentId).HasMaxLength(21);
            entity.Property(e => e.EvaluationFactorId).HasMaxLength(21);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.ScoringMethod).HasMaxLength(30);
            entity.Property(e => e.Unit).HasMaxLength(20);
            entity.Property(e => e.Weight).HasColumnType("decimal(9, 6)");

            entity.HasOne(d => d.AlignedDepartment).WithMany(p => p.Kpis)
                .HasForeignKey(d => d.AlignedDepartmentId)
                .HasConstraintName("FK_KPI_Department");

            entity.HasOne(d => d.EvaluationFactor).WithMany(p => p.Kpis)
                .HasForeignKey(d => d.EvaluationFactorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KPI_Factor");
        });

        modelBuilder.Entity<Objective>(entity =>
        {
            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.DirectInvitesPct).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.EmployeeAttendancePct).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.Explanation).HasMaxLength(2000);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Rationale).HasMaxLength(2000);
        });

        modelBuilder.Entity<ObjectiveBenchMark>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Objective_BenchMark");

            entity.ToTable("Objective BenchMark");

            entity.HasIndex(e => new { e.ObjectiveId, e.MetricName }, "UQ_Objective_BenchMark").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.DisplayText).HasMaxLength(100);
            entity.Property(e => e.InputVariableId).HasMaxLength(21);
            entity.Property(e => e.KpiId).HasMaxLength(21);
            entity.Property(e => e.MaxValue).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.MetricName).HasMaxLength(100);
            entity.Property(e => e.MinValue).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.ObjectiveId).HasMaxLength(21);

            entity.HasOne(d => d.InputVariable).WithMany(p => p.ObjectiveBenchMarks)
                .HasForeignKey(d => d.InputVariableId)
                .HasConstraintName("FK_Objective_BenchMark_Input");

            entity.HasOne(d => d.Kpi).WithMany(p => p.ObjectiveBenchMarks)
                .HasForeignKey(d => d.KpiId)
                .HasConstraintName("FK_Objective_BenchMark_KPI");

            entity.HasOne(d => d.Objective).WithMany(p => p.ObjectiveBenchMarks)
                .HasForeignKey(d => d.ObjectiveId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Objective_BenchMark_Objective");
        });

        modelBuilder.Entity<PnlCategory>(entity =>
        {
            entity.ToTable("PnlCategory");

            entity.HasIndex(e => e.Name, "UQ_PnlCategory_Name").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<PnlLine>(entity =>
        {
            entity.ToTable("PnlLine");

            entity.HasIndex(e => new { e.EventId, e.PnlLineTypeId }, "UQ_PnlLine_Event_Type").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.ActualAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Difference)
                .HasComputedColumnSql("([ActualAmount]-[ExpectedAmount])", true)
                .HasColumnType("decimal(19, 2)");
            entity.Property(e => e.EventId).HasMaxLength(21);
            entity.Property(e => e.ExpectedAmount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Note).HasMaxLength(500);
            entity.Property(e => e.PnlLineTypeId).HasMaxLength(21);
            entity.Property(e => e.RateUsed).HasColumnType("decimal(9, 6)");

            entity.HasOne(d => d.Event).WithMany(p => p.PnlLines)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PnlLine_Event");

            entity.HasOne(d => d.PnlLineType).WithMany(p => p.PnlLines)
                .HasForeignKey(d => d.PnlLineTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PnlLine_LineType");
        });

        modelBuilder.Entity<PnlLineType>(entity =>
        {
            entity.ToTable("PnlLineType");

            entity.HasIndex(e => e.Name, "UQ_PnlLineType_Name").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.AmountPerTicket).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CalculationMethod).HasMaxLength(30);
            entity.Property(e => e.DefaultRate).HasColumnType("decimal(9, 6)");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_PnlLineType_IsActive");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PnlCategoryId).HasMaxLength(21);
            entity.Property(e => e.Sign).HasDefaultValue((short)1, "DF_PnlLineType_Sign");

            entity.HasOne(d => d.PnlCategory).WithMany(p => p.PnlLineTypes)
                .HasForeignKey(d => d.PnlCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PnlLineType_Category");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TicketTierKind>(entity =>
        {
            entity.ToTable("TicketTierKind");

            entity.HasIndex(e => e.Name, "UQ_TicketTierKind_Name").IsUnique();

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UQ_User_Email").IsUnique();

            entity.HasIndex(e => e.LoginName, "UX_User_LoginName")
                .IsUnique()
                .HasFilter("([LoginName] IS NOT NULL)");

            entity.Property(e => e.Id).HasMaxLength(21);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())", "DF_User_CreatedAt");
            entity.Property(e => e.DepartmentId).HasMaxLength(21);
            entity.Property(e => e.Email).HasMaxLength(254);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_User_IsActive");
            entity.Property(e => e.LoginName).HasMaxLength(256);
            entity.Property(e => e.RoleId).HasMaxLength(21);

            entity.HasOne(d => d.Department).WithMany(p => p.Users)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_User_Department");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_Role");
        });

        modelBuilder.Entity<VwEventComparison>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_EventComparison");

            entity.Property(e => e.ActualValue).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Comment).HasMaxLength(500);
            entity.Property(e => e.Difference).HasColumnType("decimal(19, 4)");
            entity.Property(e => e.DifferencePct).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.EventId).HasMaxLength(21);
            entity.Property(e => e.EventName).HasMaxLength(150);
            entity.Property(e => e.ExpectedValue).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Metric).HasMaxLength(100);
            entity.Property(e => e.Result).HasMaxLength(21);
            entity.Property(e => e.Unit).HasMaxLength(20);
        });
        modelBuilder.HasSequence<int>("seq_AnnualBudget")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_Approval")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_Assessment")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_AssessmentFactorResult")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_AssessmentInputAnswer")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_AssessmentKpiValue")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_Banding_Thresholds")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_ComparisonMetric")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_DecisionOutcome")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_Department")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_DepartmentAlignments")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_Entertainment_Type")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_EvaluationFactors")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_Event")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_EventComment")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_EventComparison")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_EventResponsibility")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_EventRiskUpside")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_EventStatus")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_EventStatusHistory")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_EventTicketTier")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_EventType")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_FundingSource")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_GoverningEntity")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_ICP")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_InputVariable")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_InputVariableOption")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_InviteMethod")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_KPI")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_ObjectiveBenchMark")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_Objectives")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_PnlCategory")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_PnlLine")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_PnlLineType")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_Role")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_TicketTierKind")
            .HasMin(1L)
            .HasMax(9999999L);
        modelBuilder.HasSequence<int>("seq_User")
            .HasMin(1L)
            .HasMax(9999999L);

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
