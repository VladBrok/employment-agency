using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Proxies;

namespace EmploymentAgency.Models;

public partial class EmploymentAgencyContext : DbContext
{
    private readonly Settings _settings;

    public EmploymentAgencyContext(Settings settings)
    {
        _settings = settings;
    }

    public EmploymentAgencyContext(
        DbContextOptions<EmploymentAgencyContext> options,
        Settings settings)
        : base(options)
    {
        _settings = settings;
    }

    public virtual DbSet<Address> Addresses { get; set; } = null!;
    public virtual DbSet<Application> Applications { get; set; } = null!;
    public virtual DbSet<AverageSeekerAgesByPosition> AverageSeekerAgesByPositions { get; set; } = null!;
    public virtual DbSet<ChangeLog> ChangeLogs { get; set; } = null!;
    public virtual DbSet<District> Districts { get; set; } = null!;
    public virtual DbSet<Employer> Employers { get; set; } = null!;
    public virtual DbSet<EmployerAddress> EmployerAddresses { get; set; } = null!;
    public virtual DbSet<EmployersAndVacancy> EmployersAndVacancies { get; set; } = null!;
    public virtual DbSet<EmploymentType> EmploymentTypes { get; set; } = null!;
    public virtual DbSet<EmploymentTypesAndSalary> EmploymentTypesAndSalaries { get; set; } = null!;
    public virtual DbSet<NumVacanciesFromEachEmployer> NumVacanciesFromEachEmployers { get; set; } = null!;
    public virtual DbSet<Position> Positions { get; set; } = null!;
    public virtual DbSet<Property> Properties { get; set; } = null!;
    public virtual DbSet<Seeker> Seekers { get; set; } = null!;
    public virtual DbSet<SeekersAndApplication> SeekersAndApplications { get; set; } = null!;
    public virtual DbSet<Status> Statuses { get; set; } = null!;
    public virtual DbSet<Street> Streets { get; set; } = null!;
    public virtual DbSet<VacanciesAndSalary> VacanciesAndSalaries { get; set; } = null!;
    public virtual DbSet<Vacancy> Vacancies { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseLazyLoadingProxies().UseNpgsql(
                _settings.ConnectionString,
                options => options.EnableRetryOnFailure(_settings.MaxRetryCount));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("addresses");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.BuildingNumber).HasColumnName("building_number");

            entity.Property(e => e.DistrictId).HasColumnName("district_id");

            entity.HasOne(d => d.District)
                .WithMany()
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("addresses_district_id_fkey");
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.ToTable("applications");

            entity.HasIndex(e => e.Experience, "applications_experience");

            entity.HasIndex(e => e.Salary, "applications_salary");

            entity.HasIndex(e => e.SeekerDay, "applications_seeker_day");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.EmploymentTypeId).HasColumnName("employment_type_id");

            entity.Property(e => e.Experience)
                .HasPrecision(2)
                .HasColumnName("experience");

            entity.Property(e => e.Information).HasColumnName("information");

            entity.Property(e => e.Photo)
                .HasMaxLength(100)
                .HasColumnName("photo");

            entity.Property(e => e.PositionId).HasColumnName("position_id");

            entity.Property(e => e.Salary)
                .HasPrecision(8, 2)
                .HasColumnName("salary");

            entity.Property(e => e.SeekerDay)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("seeker_day")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.SeekerId).HasColumnName("seeker_id");

            entity.HasOne(d => d.EmploymentType)
                .WithMany()
                .HasForeignKey(d => d.EmploymentTypeId)
                .HasConstraintName("applications_employment_type_id_fkey");

            entity.HasOne(d => d.Position)
                .WithMany()
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("applications_position_id_fkey");

            entity.HasOne(d => d.Seeker)
                .WithMany()
                .HasForeignKey(d => d.SeekerId)
                .HasConstraintName("applications_seeker_id_fkey");
        });

        modelBuilder.Entity<AverageSeekerAgesByPosition>(entity =>
        {
            entity.HasNoKey();

            entity.ToView("average_seeker_ages_by_positions");

            entity.Property(e => e.Position).HasColumnName("Должность").HasMaxLength(40);

            entity.Property(e => e.AverageAge).HasColumnName("Средний возраст");
        });

        modelBuilder.Entity<ChangeLog>(entity =>
        {
            entity.ToTable("change_log");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Operation)
                .HasMaxLength(10)
                .HasColumnName("operation");

            entity.Property(e => e.RecordId).HasColumnName("record_id");

            entity.Property(e => e.TableName)
                .HasMaxLength(50)
                .HasColumnName("table_name");

            entity.Property(e => e.TimeModified)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("time_modified")
                .HasDefaultValueSql("now()");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.ToTable("districts");

            entity.HasIndex(e => e.DistrictName, "districts_district_key")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.DistrictName)
                .HasMaxLength(30)
                .HasColumnName("district");
        });

        modelBuilder.Entity<Employer>(entity =>
        {
            entity.ToTable("employers");

            entity.HasIndex(e => e.EmployerName, "employers_employer_key")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.AddressId).HasColumnName("address_id");

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");

            entity.Property(e => e.EmployerName)
                .HasMaxLength(40)
                .HasColumnName("employer");

            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .HasColumnName("phone");

            entity.Property(e => e.PropertyId).HasColumnName("property_id");

            entity.HasOne(d => d.Address)
                .WithMany()
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employers_address_id_fkey");

            entity.HasOne(d => d.Property)
                .WithMany()
                .HasForeignKey(d => d.PropertyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employers_property_id_fkey");
        });

        modelBuilder.Entity<EmployerAddress>(entity =>
        {
            entity.HasNoKey();

            entity.ToView("employer_addresses");

            entity.Property(e => e.Company).HasColumnName("Компания").HasMaxLength(40);

            entity.Property(e => e.BuildingNumber).HasColumnName("Номер дома");

            entity.Property(e => e.District).HasColumnName("Район").HasMaxLength(30);

            entity.Property(e => e.Street).HasColumnName("Улица").HasMaxLength(30);
        });

        modelBuilder.Entity<EmployersAndVacancy>(entity =>
        {
            entity.HasNoKey();

            entity.ToView("employers_and_vacancies");

            entity.Property(e => e.PublicationDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Дата размещения вакансии");

            entity.Property(e => e.Position).HasColumnName("Должность").HasMaxLength(40);

            entity.Property(e => e.Salary).HasColumnName("Зарплата").HasPrecision(8, 2);

            entity.Property(e => e.Company).HasColumnName("Компания").HasMaxLength(40);
        });

        modelBuilder.Entity<EmploymentType>(entity =>
        {
            entity.ToTable("employment_types");

            entity.HasIndex(e => e.Type, "employment_types_type_key")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .HasColumnName("type");
        });

        modelBuilder.Entity<EmploymentTypesAndSalary>(entity =>
        {
            entity.HasNoKey();

            entity.ToView("employment_types_and_salaries");

            entity.Property(e => e.ExpectedSalary)
                .HasPrecision(8, 2)
                .HasColumnName("Ожидаемая зарплата");

            entity.Property(e => e.EmploymentType)
                .HasMaxLength(20)
                .HasColumnName("Тип занятости");
        });

        modelBuilder.Entity<NumVacanciesFromEachEmployer>(entity =>
        {
            entity.HasNoKey();

            entity.ToView("num_vacancies_from_each_employer");

            entity.Property(e => e.NumVacancies).HasColumnName("Количество вакансий");

            entity.Property(e => e.Company).HasColumnName("Компания").HasMaxLength(40);
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.ToTable("positions");

            entity.HasIndex(e => e.PositionName, "positions_position_key")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.PositionName)
                .HasMaxLength(40)
                .HasColumnName("position");
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.ToTable("properties");

            entity.HasIndex(e => e.PropertyName, "properties_property_key")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.PropertyName)
                .HasMaxLength(30)
                .HasColumnName("property");
        });

        modelBuilder.Entity<Seeker>(entity =>
        {
            entity.ToTable("seekers");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.AddressId).HasColumnName("address_id");

            entity.Property(e => e.Birthday).HasColumnName("birthday");

            entity.Property(e => e.Education)
                .HasMaxLength(50)
                .HasColumnName("education");

            entity.Property(e => e.FirstName)
                .HasMaxLength(20)
                .HasColumnName("first_name");

            entity.Property(e => e.LastName)
                .HasMaxLength(20)
                .HasColumnName("last_name");

            entity.Property(e => e.Patronymic)
                .HasMaxLength(20)
                .HasColumnName("patronymic");

            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .HasColumnName("phone");

            entity.Property(e => e.Pol).HasColumnName("pol");

            entity.Property(e => e.Recommended).HasColumnName("recommended");

            entity.Property(e => e.RegistrationCity)
                .HasMaxLength(20)
                .HasColumnName("registration_city");

            entity.Property(e => e.SpecialityId).HasColumnName("speciality_id");

            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Address)
                .WithMany()
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("seekers_address_id_fkey");

            entity.HasOne(d => d.Speciality)
                .WithMany()
                .HasForeignKey(d => d.SpecialityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("seekers_speciality_id_fkey");

            entity.HasOne(d => d.Status)
                .WithMany()
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("seekers_status_id_fkey");
        });

        modelBuilder.Entity<SeekersAndApplication>(entity =>
        {
            entity.HasNoKey();

            entity.ToView("seekers_and_applications");

            entity.Property(e => e.RegistrationCity)
                .HasMaxLength(20)
                .HasColumnName("Город регистрации");

            entity.Property(e => e.PublicationDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("Дата размещения заявки");

            entity.Property(e => e.FirstName).HasColumnName("Имя").HasMaxLength(20);

            entity.Property(e => e.ExpectedSalary)
                .HasPrecision(8, 2)
                .HasColumnName("Ожидаемая зарплата");

            entity.Property(e => e.Patronymic).HasColumnName("Отчество").HasMaxLength(20);

            entity.Property(e => e.LastName).HasColumnName("Фамилия").HasMaxLength(20);
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.ToTable("statuses");

            entity.HasIndex(e => e.StatusName, "statuses_status_key")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.StatusName)
                .HasMaxLength(20)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Street>(entity =>
        {
            entity.ToTable("streets");

            entity.HasIndex(e => e.StreetName, "streets_street_key")
                .IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.DistrictId).HasColumnName("district_id");

            entity.Property(e => e.PostalCode).HasColumnName("postal_code");

            entity.Property(e => e.StreetName)
                .HasMaxLength(30)
                .HasColumnName("street");

            entity.HasOne(d => d.District)
                .WithMany()
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("streets_district_id_fkey");
        });

        modelBuilder.Entity<VacanciesAndSalary>(entity =>
        {
            entity.HasNoKey();

            entity.ToView("vacancies_and_salaries");

            entity.Property(e => e.Chart).HasColumnName("График").HasMaxLength(50);

            entity.Property(e => e.Position).HasColumnName("Должность").HasMaxLength(40);

            entity.Property(e => e.Salary).HasColumnName("Зарплата").HasPrecision(8, 2);
        });

        modelBuilder.Entity<Vacancy>(entity =>
        {
            entity.ToTable("vacancies");

            entity.HasIndex(e => e.EmployerDay, "vacancies_employer_day");

            entity.HasIndex(e => e.SalaryNew, "vacancies_salary_new");

            entity.Property(e => e.Id).HasColumnName("id");

            entity.Property(e => e.ChartNew)
                .HasMaxLength(50)
                .HasColumnName("chart_new");

            entity.Property(e => e.EmployerDay)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("employer_day")
                .HasDefaultValueSql("now()");

            entity.Property(e => e.EmployerId).HasColumnName("employer_id");

            entity.Property(e => e.PositionId).HasColumnName("position_id");

            entity.Property(e => e.SalaryNew)
                .HasPrecision(8, 2)
                .HasColumnName("salary_new");

            entity.Property(e => e.VacancyEnd).HasColumnName("vacancy_end");

            entity.HasOne(d => d.Employer)
                .WithMany()
                .HasForeignKey(d => d.EmployerId)
                .HasConstraintName("vacancies_employer_id_fkey");

            entity.HasOne(d => d.Position)
                .WithMany()
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("vacancies_position_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
