using SocialNetworkForPets.Helper.Enums;

namespace SocialNetworkForPets.Services
{
    public interface IFileService
    {
        Task<string> UploadImageAsync(IFormFile file, ImageFileType imageFileType);
    }
}
