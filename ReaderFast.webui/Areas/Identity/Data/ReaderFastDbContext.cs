using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReaderFast.webui.Areas.Identity.Data;
using ReaderFast.webui.Models;
using System.Reflection.Emit;

namespace ReaderFast.webui.Data;

public class ReaderFastDbContext : IdentityDbContext<ApplicationUser>
{
    // ReaderFastDbContext.cs
    public DbSet<Sentence> Sentences { get; set; }

    public DbSet<Exercise> Exercises { get; set; }
    public DbSet<ExerciseDay> ExerciseDays { get; set; }
    public DbSet<ExerciseExerciseDay> ExerciseExerciseDays { get; set; } // ExerciseExerciseDay'ı DbContext'e ekleyin
    public DbSet<Story> Stories { get; set; }
    public DbSet<OrderModel> Orders { get; set; }
    public DbSet<UserExercise> UserExercises { get; set; }
    public DbSet<Product> Products { get; set; }
    public ReaderFastDbContext(DbContextOptions<ReaderFastDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ExerciseExerciseDay>()
             .HasKey(eed => eed.Id); // Sadece 'Id' alanını primary key olarak tanımlıyoruz

        builder.Entity<ExerciseExerciseDay>()
            .HasOne(eed => eed.Exercise)
            .WithMany(e => e.ExerciseExerciseDays)
            .HasForeignKey(eed => eed.ExerciseId);

        builder.Entity<ExerciseExerciseDay>()
            .HasOne(eed => eed.ExerciseDay)
            .WithMany(ed => ed.ExerciseExerciseDays)
            .HasForeignKey(eed => eed.ExerciseDayId);
    }
}
