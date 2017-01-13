using System.Threading.Tasks;

namespace Showaspnetcore.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}