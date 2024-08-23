using DATABASE_library.Models.Data;
using DATABASE_library.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// using MongoDB.Driver;
// using MongoDB.EntityFrameworkCore.Extensions;

namespace DATABASE_library;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    // private static readonly string connectionString = "mongodb://localhost:27017";
    private static readonly string connectionString; //"jdbc:postgresql://localhost:5432/postgres";
    private static readonly string databaseName = "ENSURE";
    
    public DbSet<UserModel> Users => Set<UserModel>();
    public DbSet<DataModel> Data => Set<DataModel>();
    public DbSet<RawDataModel> RawData => Set<RawDataModel>();
    public DbSet<MeasurementModel> Measurements => Set<MeasurementModel>();
    
    // private MongoClient MongoClient= new MongoClient(connectionString);
    
    public AppDbContext(DbContextOptions options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // optionsBuilder.UseMongoDB(MongoClient, databaseName);
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        var envConnectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
        if (!string.IsNullOrEmpty(envConnectionString))
        {
            connectionString = envConnectionString;
        }
        Console.WriteLine(connectionString);
        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserModel>(u =>
        {
            u.ToTable("Users");
            u.HasKey(u => u._id);
            u.Property(u => u._id).ValueGeneratedOnAdd();
            u.Property(u => u.Username).IsRequired();
            u.Property(u => u.Password).IsRequired();
            // u.Property(u => u.Role).IsRequired();
            // u.Property(u => u.Token).IsRequired();
            // u.Property(u => u.TokenExpiration).IsRequired();
            // u.Property(u => u.EnergyPrice).IsRequired();
        });
        
        modelBuilder.Entity<DataModel>(d =>
        {
            d.ToTable("Data");
            d.HasKey(d => d._id);
            d.Property(d => d.deviceID).IsRequired();
            d.Property(d => d.Timestamp).IsRequired();
            d.Property(d => d.TotalStartTime).IsRequired();
            d.Property(d => d.Total).IsRequired();
            d.Property(d => d.Today).IsRequired();
            d.Property(d => d.Power).IsRequired();
            d.Property(d => d.ApparentPower).IsRequired();
            d.Property(d => d.ReactivePower).IsRequired();
            d.Property(d => d.Factor).IsRequired();
            d.Property(d => d.Voltage).IsRequired();
            d.Property(d => d.Current).IsRequired();
            d.Property(d => d.MeasurementId).IsRequired(false);
        });
        
        modelBuilder.Entity<RawDataModel>(r =>
        {
            r.ToTable("RawData");
            r.HasKey(r => r._id);
            r.OwnsOne(r => r.Payload, p => 
                { p.OwnsOne(p => p.ENERGY); });
            r.Property(r => r.DeviceID).IsRequired();
        });
        
        modelBuilder.Entity<MeasurementModel>(m =>
        {
            m.ToTable("Measurements");
            m.HasKey(m => m._id);
            m.Property(m => m.Name).IsRequired(false);
            m.Property(m => m.Category).IsRequired(false);
            m.Property(e => e.StartTime).IsRequired();
            m.Property(e => e.EndTime).IsRequired(false);
            m.Property(e => e.DeviceId).IsRequired();
            m.Property(e => e.isFinished).IsRequired().HasDefaultValue(false);
            m.HasMany(e => e.Data)
                .WithOne(e => e.Measurement)
                .HasForeignKey(e => e.MeasurementId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });
    }

    // //Map the models to the database
    // public IMongoCollection<UserModel> UsersCollection => MongoClient.GetDatabase(databaseName).GetCollection<UserModel>("USERS");
    // public IMongoCollection<DataModel> DataCollection => MongoClient.GetDatabase(databaseName).GetCollection<DataModel>("DATA");
    // public IMongoCollection<RawDataModel> RawDataCollection => MongoClient.GetDatabase(databaseName).GetCollection<RawDataModel>("RAW");
    
}