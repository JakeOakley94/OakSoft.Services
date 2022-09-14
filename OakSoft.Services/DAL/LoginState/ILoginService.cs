using OakSoft.Model;

namespace OakSoft.Services
{
    public interface ILoginService
    {
        void Logon(User user);
        void Logoff();
    }
}
