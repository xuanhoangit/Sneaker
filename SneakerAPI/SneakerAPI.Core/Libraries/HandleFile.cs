using Microsoft.AspNetCore.Http;

namespace SneakerAPI.Core.Libraries;
public static class HandleFile
{
    public static async Task<bool> Upload(IFormFile file,string filePath){
        try
        {
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return true;
        }
        catch (System.Exception)
        {
            
            return false;
        }
    }
}