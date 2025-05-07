using SocialNetworkForPets.Data;
using SocialNetworkForPets.Helper.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace SocialNetworkForPets.Services
{
    public class FileService: IFileService
    {
        private readonly AppDbContext _context;

        public FileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> UploadImageAsync(IFormFile image, ImageFileType imageFileType)
        {
            string filePathUpload = imageFileType switch
            {
                ImageFileType.PostImage => "post",
                ImageFileType.ProfilePicture => "profile",
                ImageFileType.CoverImage => "cover",
                _ => throw new ArgumentException("İnvalid file type")
            };
            if (image != null && image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                if (image.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, $"images/uploaded/{filePathUpload}");
                    Directory.CreateDirectory(rootFolderPathImages);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                        await image.CopyToAsync(stream);

                    return $"/images/uploaded/{filePathUpload}/{fileName}";
                }

            }
            return "";
        }
    }
}
