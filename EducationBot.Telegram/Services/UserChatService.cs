using EducationBot.EfData;
using EducationBot.EfData.Entities;
using EducationBot.Telegram.Model.Telegram;
using Microsoft.EntityFrameworkCore;

namespace EducationBot.Telegram.Services
{
    public class UserChatService
    {
        private readonly DataBaseContext _context;

        public UserChatService(DataBaseContext context)
        {
            _context = context;
        }

        public async Task ChangeUserSubs(long chatId, bool newState, CancellationToken ct)
        {
            var user = await _context.TelegramUser
                .Where(x => x.UserIdent == chatId)
                .FirstOrDefaultAsync(ct);

            user.IsGetLessonShedulle = newState;

            _context.TelegramUser.Update(user);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<TelegramUser> GetUserByChatId(long FromId, CancellationToken ct)
        {
            var user = await _context.TelegramUser
                .Where(x => x.UserIdent == FromId)
                .FirstOrDefaultAsync(ct);
            return user;
        }

        public async Task<dynamic> GetAllUser(CancellationToken ct)
        {
            return await _context.TelegramUser
                .Select(x => new
                {
                    x.Id,
                    x.FirstName,
                    x.LastName,
                    x.UserIdent,
                    x.IsGetLessonShedulle,
                    x.Chats
                })
                .ToListAsync(ct);
        }

        public async Task<dynamic> GetAllChats(CancellationToken ct)
        {
            return await _context.TelegramChat
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.ChatType,
                    x.ChatIdent,
                    x.Users
                })
                .ToListAsync(ct);
        }

        public async Task<TelegramChatUser> GetUserDialog(Chat chat, From from, CancellationToken ct)
        {
            var dialogEnt = await _context.TelegramChatUser
                .Where(x => x.TelegramChat.ChatIdent == chat.Id && x.TelegramUser.UserIdent == from.Id)
                .FirstOrDefaultAsync(ct);
            return dialogEnt;
        }

        public async Task UpdateUserDialog(TelegramChatUser dialog, CancellationToken ct)
        {
            var dialogEnt = await _context.TelegramChatUser
                .Where(x => x.Id == dialog.Id)
                .FirstOrDefaultAsync(ct);

            dialogEnt.LastAction = dialog.LastAction;

            _context.TelegramChatUser.Update(dialogEnt);
            await _context.SaveChangesAsync(ct);
        }

        public async Task SetChatUser(Chat chat, From from, CancellationToken ct)
        {
            var chatCheck = await _context.TelegramChat
                .Include(x => x.Users)
                .FirstOrDefaultAsync(x => x.ChatIdent == chat.Id);

            if (chatCheck == null)
            {
                var userCheck = await _context.TelegramUser
                    .FirstOrDefaultAsync(x => x.UserIdent == from.Id);

                if (userCheck != null)
                {
                    var newChat = new TelegramChat
                    {
                        Title = chat.Title,
                        ChatIdent = chat.Id,
                        ChatType = chat.Type,
                        Users = new List<TelegramUser> { userCheck }
                    };

                    var insChat = await _context.TelegramChat.AddAsync(newChat, ct);
                    await _context.SaveChangesAsync(ct);
                }
                else
                {
                    var newChat = new TelegramChat
                    {
                        Title = chat.Title,
                        ChatIdent = chat.Id,
                        ChatType = chat.Type,
                        Users = new List<TelegramUser>()
                        {
                            new TelegramUser {
                                FirstName = from.FirstName,
                                LastName = from.LastName,
                                UserIdent = from.Id,
                            }
                        }
                    };
                    await _context.TelegramChat.AddAsync(newChat, ct);
                    await _context.SaveChangesAsync(ct);
                }
            }
        }

        public async Task<List<long>> GetUserChatToLessonShedule(CancellationToken ct)
        {
            var result = await _context.TelegramUser
                .Where(x => x.IsGetLessonShedulle)
                .Select(x => x.UserIdent)
                .ToListAsync(ct);
            return result;
        }

        public async Task<List<TelegramUserShedullers>> GetAllUserShedullers(long chatId, CancellationToken ct)
        {
            var user = await _context.TelegramUser
                .Include(x => x.Shedullers)
                .Where(x => x.UserIdent == chatId)
                .FirstOrDefaultAsync(ct);
            return user.Shedullers;
        }

        public async Task<int> DeleteAllUserShedullers(long chatId, CancellationToken ct)
        {
            var user = await _context.TelegramUser
                .Include(x => x.Shedullers)
                .Where(x => x.UserIdent == chatId)
                .FirstOrDefaultAsync(ct);

            var sh = user.Shedullers;

            if (sh.Any())
            {
                _context.TelegramUserShedullers.RemoveRange(sh);
                await _context.SaveChangesAsync(ct);
                return sh.Count;
            }
            else
                return 0;

        }

        public async Task AddUserSheduller(TelegramChatUser dialog, DateTime date, CancellationToken ct)
        {
            TelegramUserShedullers newItem = new()
            {
                DateTimeUtc = date,
                Title = "без опсания",
                TelegramUser = dialog.TelegramUser
            };
            await _context.TelegramUserShedullers.AddAsync(newItem);
            await _context.SaveChangesAsync(ct);
        }

        public async Task FeetUserSheduller(TelegramChatUser dialog, string description, CancellationToken ct)
        {
            var sh = await _context.TelegramUserShedullers
                .FirstOrDefaultAsync(x => x.TelegramUser.Id == dialog.TelegramUserId && x.Title == "без опсания", ct);

            sh.Title = description;

            _context.TelegramUserShedullers.Update(sh);
            await _context.SaveChangesAsync(ct);
        }
    }
}
