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

        public async Task<TelegramUser> GetUserByChatId(long chatId, CancellationToken ct)
        {
            var user = await _context.TelegramUser
                .Where(x => x.UserIdent == chatId)
                .FirstOrDefaultAsync(ct);
            return user;
        }

        public async Task<dynamic> GetAllUser(CancellationToken ct)
        {
            var usesrs = await _context.TelegramUser
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
            return usesrs;
        }

        public async Task<dynamic> GetAllChats(CancellationToken ct)
        {
            var chats = await _context.TelegramChat
                .Select(x => new
                {
                    x.Id,
                    x.Title,
                    x.ChatType,
                    x.ChatIdent,
                    x.Users
                })
                .ToListAsync(ct);
            return chats;
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
                        Title = chat.Type == "group" ? chat.Title : null,
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
                        Title = chat.Type == "group" ? chat.Title : null,
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
    }
}
