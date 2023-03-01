using EducationBot.EfData.Entities.Cient;
using EducationBot.EfData.Entities.Education;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.EfData.Context
{
    public class DataBaseContext : DbContext
    {
        public DbSet<TelegramUser> TelegramUser { get; set; }
        public DbSet<TelegramChatUser> TelegramChatUser { get; set; }
        public DbSet<TelegramChat> TelegramChat { get; set; }

        public DbSet<TelegramUserShedullers> TelegramUserShedullers { get; set; }

        public DbSet<LessonShedulle> LessonShedulle { get; set; }
        public DbSet<Discipline> Discipline { get; set; }
        public DbSet<Teacher> Teacher { get; set; }
        public DbSet<DisciplineType> DisciplineType { get; set; }
        public DbSet<Lesson> Lesson { get; set; }

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
                    j.HasKey(k => k.Id);
                    j.HasIndex(t => new { t.TelegramChatId, t.TelegramUserId }).IsUnique();
                });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        { }

        public static void SeedInitilData(DataBaseContext context)
        {
            #region TelegramChat

            TelegramChat vhtc = new()
            {
                Title = null,
                ChatType = "private",
                ChatIdent = 1338551358,
                Users = new List<TelegramUser>()
                {
                    new()
                    {
                        FirstName = "Виктория",
                        LastName = "Хорина",
                        UserIdent = 1338551358,
                        IsGetLessonShedulle = true
                    }
                }
            };
            context.TelegramChat.Add(vhtc);
            context.SaveChanges();

            TelegramChat kptc = new()
            {
                Title = null,
                ChatType = "private",
                ChatIdent = 1077072257,
                Users = new List<TelegramUser>()
                {
                    new()
                    {
                        FirstName = "Kirill",
                        LastName = "Portnov",
                        UserIdent = 1077072257,
                        IsGetLessonShedulle = true
                    }
                }
            };
            context.TelegramChat.Add(kptc);
            context.SaveChanges();

            TelegramChat aatc = new()
            {
                Title = null,
                ChatType = "private",
                ChatIdent = 347455790,
                Users = new List<TelegramUser>()
                {
                     new()
                     {
                         FirstName = "Alena",
                         LastName = "Alekseevna",
                         UserIdent = 347455790,
                         IsGetLessonShedulle = true
                     }
                }
            };
            context.TelegramChat.Add(aatc);
            context.SaveChanges();

            TelegramChat gltc = new()
            {
                Title = null,
                ChatType = "private",
                ChatIdent = 572877873,
                Users = new List<TelegramUser>()
                {
                    new()
                    {
                        FirstName = "Grisha",
                        LastName = "Lisianskii",
                        UserIdent = 572877873,
                        IsGetLessonShedulle = true
                    }
                }
            };
            context.TelegramChat.Add(gltc);
            context.SaveChanges();

            #endregion TelegramChat
        }
    }
}
