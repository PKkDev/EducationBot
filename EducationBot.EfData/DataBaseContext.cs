using EducationBot.EfData.Entities;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.EfData
{
    public class DataBaseContext : DbContext
    {
        public DbSet<TelegramUser> TelegramUser { get; set; }
        public DbSet<TelegramChatUser> TelegramChatUser { get; set; }
        public DbSet<TelegramChat> TelegramChat { get; set; }

        public DbSet<TelegramUserShedullers> TelegramUserShedullers { get; set; }

        public DbSet<Lesson> Lesson { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<TypeLesson> TypeLesson { get; set; }
        public DbSet<WeekDay> WeekDay { get; set; }
        public DbSet<LessonTime> LessonTime { get; set; }

        public DataBaseContext(DbContextOptions<DataBaseContext> options)
            : base(options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<TelegramChat>()
                .HasMany(c => c.Users)
                .WithMany(s => s.Chats)
                .UsingEntity<TelegramChatUser>(
                j => j
                .HasOne(pt => pt.TelegramUser)
                .WithMany(t => t.ChatUsers)
                .HasForeignKey(pt => pt.TelegramUserId),
                j => j
                .HasOne(pt => pt.TelegramChat)
                .WithMany(p => p.ChatUsers)
                .HasForeignKey(pt => pt.TelegramChatId),
                j =>
                {
                    j.HasKey(t => new { t.TelegramChatId, t.TelegramUserId });
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        { }
    }
}
