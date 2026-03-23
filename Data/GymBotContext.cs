using GymBot.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GymBot.Data.Data
{
    public class GymBotContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Workout> Workouts => Set<Workout>();
        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<SetEntry> Sets => Set<SetEntry>();
        public DbSet<UserBodyMetric> UserBodyMetrics => Set<UserBodyMetric>();
        public DbSet<WorkoutTemplate> WorkoutTemplates => Set<WorkoutTemplate>();
        public DbSet<WorkoutTemplateExercise> WorkoutTemplateExercises => Set<WorkoutTemplateExercise>();
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql("Host=localhost;Port=5432;Username=gymbotuser;Password=gymbotpass;Database=gymbot_db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.TgId).HasColumnName("tg_id");
                e.Property(x => x.Username).HasColumnName("username").HasMaxLength(255);
                e.HasIndex(x => x.Id).IsUnique();
                e.HasIndex(x => x.TgId).IsUnique();
            });
            modelBuilder.Entity<Workout>(e =>
            {
                e.ToTable("Workouts");
                e.HasKey(x => x.Id);
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(255);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.UserId).HasColumnName("user_id");
                e.Property(x => x.Date).HasColumnName("date");
                e.HasIndex(x => x.Date).HasDatabaseName("idworkout_date");
                e.HasOne(x => x.User)
                    .WithMany(u => u.Workouts)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_workouts_user");
            });
            modelBuilder.Entity<Exercise>(e =>
            {
                e.ToTable("Exercises");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.WorkoutId).HasColumnName("workout_id");
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(255).IsRequired();
                e.Property(x => x.Date).HasColumnName("date").HasDefaultValueSql("now()");
                e.HasIndex(x => x.WorkoutId).HasDatabaseName("id_exercises_workout_id");
                e.HasIndex(x => x.Name).HasDatabaseName("id_exercises_name");
                e.HasOne(x => x.Workout)
                    .WithMany(w => w.Exercises)
                    .HasForeignKey(x => x.WorkoutId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_exercises_workout");
            });
            modelBuilder.Entity<SetEntry>(e =>
            {
                e.ToTable("Sets", t =>
                {
                    t.HasCheckConstraint("check_sets_reps", "reps >= 0");
                    t.HasCheckConstraint("check_sets_weight", "weight >= 0");
                });
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.ExerciseId).HasColumnName("exercise_id");
                e.Property(x => x.Reps).HasColumnName("reps");
                e.Property(x => x.Weight).HasColumnName("weight").HasPrecision(8, 2);
                e.Property(x => x.Date).HasColumnName("date").HasDefaultValueSql("now()");
                e.HasIndex(x => x.ExerciseId).HasDatabaseName("id_sets_exercise_id");
                e.HasOne(x => x.Exercise)
                    .WithMany(ex => ex.Sets)
                    .HasForeignKey(x => x.ExerciseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_sets_exercise");
            });
            modelBuilder.Entity<UserBodyMetric>(e =>
            {
                e.ToTable("UserBodyMetrics", t =>
                {
                    t.HasCheckConstraint("check_body_weight", "weight > 0");
                });
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.UserId).HasColumnName("user_id");
                e.Property(x => x.Weight).HasColumnName("weight").HasPrecision(6, 2).IsRequired();
                e.Property(x => x.Date).HasColumnName("date").HasDefaultValueSql("now()");
                e.HasIndex(x => x.UserId).HasDatabaseName("id_bodymetrics_userid");
                e.HasIndex(x => x.Date).HasDatabaseName("id_bodymetrics_date");
                e.HasOne(x => x.User)
                    .WithMany(u => u.BodyMetrics)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_bodymetrics_user");
            });
            modelBuilder.Entity<WorkoutTemplate>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
                entity.HasMany(x => x.Exercises)
                    .WithOne(x => x.WorkoutTemplate!)
                    .HasForeignKey(x => x.WorkoutTemplateId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<WorkoutTemplateExercise>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.ExerciseName).IsRequired().HasMaxLength(100);
            });
        }
    }
}
