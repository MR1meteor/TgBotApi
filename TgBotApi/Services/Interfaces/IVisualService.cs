namespace TgBotApi.Services.Interfaces
{
    public interface IVisualService
    {
        Task<string> GetByLink(string link);
    }
}
