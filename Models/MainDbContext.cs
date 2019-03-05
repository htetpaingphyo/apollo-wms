using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApolloWMS.Models.ViewModels;

namespace ApolloWMS.Models
{
    public partial class MainDbContext : DbContext
    {
        public MainDbContext()
        {
        }

        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Balance> Balance { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeRole> EmployeeRole { get; set; }
        public virtual DbSet<EmployeeType> EmployeeType { get; set; }
        public virtual DbSet<Holidays> Holidays { get; set; }
        public virtual DbSet<LeaveRequest> LeaveRequest { get; set; }
        public virtual DbSet<LeaveType> LeaveType { get; set; }
        public virtual DbSet<Report> Report { get; set; }
        public virtual DbSet<Role> Role { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.1-servicing-10028");

            modelBuilder.Entity<Balance>(BalanceConfiguration);
            modelBuilder.Entity<Department>(DepartmentConfiguration);
            modelBuilder.Entity<Employee>(EmployeeConfiguration);
            modelBuilder.Entity<EmployeeRole>(EmployeeRoleConfiguration);
            modelBuilder.Entity<EmployeeType>(EmployeeTypeConfiguration);
            modelBuilder.Entity<Holidays>(HolidaysConfiguration);
            modelBuilder.Entity<LeaveRequest>(LeaveRequestConfiguration);
            modelBuilder.Entity<LeaveType>(LeaveTypeConfiguration);
            modelBuilder.Entity<Report>(ReportConfiguration);
            modelBuilder.Entity<Role>(RoleConfiguration);
        }

        private void BalanceConfiguration(EntityTypeBuilder<Balance> builder)
        {
            builder.Property(e => e.BalanceId).ValueGeneratedNever();

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
        }

        private void DepartmentConfiguration(EntityTypeBuilder<Department> builder)
        {
            builder.Property(e => e.DepartmentId).ValueGeneratedNever();

            builder.Property(e => e.DepartmentName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);
        }

        private void EmployeeConfiguration(EntityTypeBuilder<Employee> builder)
        {
            builder.Property(e => e.EmployeeId).ValueGeneratedNever();

            builder.Property(e => e.ContactNumber)
                .HasMaxLength(20)
                .IsUnicode(false);

            builder.Property(e => e.CreatedBy)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.Designation)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(400)
                .IsUnicode(false);

            builder.Property(e => e.Region)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
        }

        private void EmployeeRoleConfiguration(EntityTypeBuilder<EmployeeRole> builder)
        {
            builder.Property(e => e.EmployeeRoleId).ValueGeneratedNever();
        }

        private void EmployeeTypeConfiguration(EntityTypeBuilder<EmployeeType> builder)
        {
            builder.Property(e => e.EmployeeTypeId).ValueGeneratedNever();

            builder.Property(e => e.EmployeeTypeName)
                .IsRequired()
                .HasMaxLength(10)
                .IsUnicode(false);
        }

        private void HolidaysConfiguration(EntityTypeBuilder<Holidays> builder)
        {
            builder.HasKey(e => e.HolidayId)
                    .HasName("PK_NationalHolidays");

            builder.Property(e => e.HolidayId).ValueGeneratedNever();

            builder.Property(e => e.DefinedDate).HasColumnType("date");

            builder.Property(e => e.HolidayName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(50)
                .IsUnicode(false);
        }

        private void LeaveRequestConfiguration(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.Property(e => e.LeaveRequestId).ValueGeneratedNever();

            builder.Property(e => e.EmergencyContact).HasMaxLength(20);

            builder.Property(e => e.ReasonForAbsence)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(e => e.Remark).HasMaxLength(300);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(10);
        }

        private void LeaveTypeConfiguration(EntityTypeBuilder<LeaveType> builder)
        {
            builder.Property(e => e.LeaveTypeId).ValueGeneratedNever();

            builder.Property(e => e.LeaveTypeName)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false);
        }

        private void ReportConfiguration(EntityTypeBuilder<Report> builder)
        {
            builder.Property(e => e.ReportId).ValueGeneratedNever();
        }

        private void RoleConfiguration(EntityTypeBuilder<Role> builder)
        {
            builder.Property(e => e.RoleId).ValueGeneratedNever();

            builder.Property(e => e.RoleName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);
        }

        public DbSet<ApolloWMS.Models.ViewModels.LeaveRequestViewModel> LeaveRequestViewModel { get; set; }

        public DbSet<ApolloWMS.Models.ViewModels.ReportViewModel> ReportViewModel { get; set; }

        public DbSet<ApolloWMS.Models.ViewModels.LeaveRequestEditModel> LeaveRequestEditModel { get; set; }
    }
}
