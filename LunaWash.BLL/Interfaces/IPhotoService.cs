using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LunaWash.BLL.Interfaces;

public interface IPhotoService
{
    Task<string> UploadPhotoAsync(IFormFile file);
}
