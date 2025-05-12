namespace SocialNetworkForPets.Services
{
    public interface INotificationService
    {
        Task AddNewNotificationAsync(int UserId, string Message, string Type);

        Task<int> GetNotificationsCountAsync(int UserId);
    }
}
