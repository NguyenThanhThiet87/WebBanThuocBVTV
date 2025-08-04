using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Threading.Tasks;
namespace WebBanThuocBVTV.Helper
{
    public class Cloudinary_Net
    {
        readonly IConfiguration _config;
        readonly Cloudinary _cloudinary;
        public Cloudinary_Net(IConfiguration config)
        {
            _config = config;
            Account account = new Account(
                              _config["Cloudinary:cloud_name"],
                             _config["Cloudinary:api_key"],
                             _config["Cloudinary:api_secret"]);

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }
        public string Upload(IFormFile file, string category)
        {
            try
            {
                string publicId = "WebBanThuocBVTV/";

                switch (category)
                {
                    case "P&H":
                        publicId += "phan bon va hoa chat/";
                        break;
                    case "TTS":
                        publicId += "thuoc tru sau/";
                        break;
                    case "TTB":
                        publicId += "thuoc tru benh/";
                        break;
                    case "TTC":
                        publicId += "thuoc tru co/";
                        break;
                    case "ND":
                        publicId += "User/";
                        break;
                    default:

                        break;
                }
                publicId += file.FileName;

                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        PublicId = publicId
                    };
                    var uploadResult = _cloudinary.UploadAsync(uploadParams);

                    string url = uploadResult.Result.SecureUrl.ToString();
                    return url;
                }
            }catch(Exception ex)
            {
                throw ex;
            }
        }
        public async Task<bool> Remove(String filePath)
        {
            string publicId = filePath.Substring(filePath.IndexOf("WebBanThuocBVTV/"));
            String[] split = publicId.Split(".");
            publicId = split[0];
            if (split.Length > 2)
            {
                publicId += "." + split[1];
            }    
            DeletionResult delectionResult = await _cloudinary.DestroyAsync( new DeletionParams(publicId));
            return true;
        }
    }
}
