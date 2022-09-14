using OakSoft.Client.Models;

namespace OakSoft.Services
{
    public static class BroadcastService
    {
        public delegate void NavigationEvent(EventArguments args);
        public delegate void NotificationEvent(string notificationJSON);
        public delegate void GenericEvent();
        public delegate void CriticalErrorEvent(string message);


        public static NotificationEvent NotifyUser;
        public static NavigationEvent OpenLogin;
        public static NavigationEvent OpenForgotPasswordControl;
        public static NavigationEvent OpenResetPasswordControl;
        public static NavigationEvent ClearContent;
        public static GenericEvent LoginChanged;
        public static CriticalErrorEvent CriticalError;
    }
}
