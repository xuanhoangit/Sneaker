using Microsoft.AspNetCore.Mvc;
using SneakerAPI.Core.Interfaces;
using SneakerAPI.Core.Libraries;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.AdminApi.Controllers.ProductControllers
{       
    [ApiController]
    [Route("api/product-images")]
    public class ProductColorFileController : BaseController
    {
        private readonly IUnitOfWork _uow;
        private static readonly string localStorage="uploads/product-images";
        private readonly string uploadFilePath=Path.Combine(Directory.GetCurrentDirectory(),$"wwwroot/{localStorage}");

        public ProductColorFileController(IUnitOfWork uow):base(uow)
        {
            _uow = uow;
        }
        [HttpGet("{product-color-id:int?}")]
        public IActionResult GetAll(int pc_id){
            var pcf=_uow.ProductColorFile.GetAll(x=>x.ProductColorFile__ProductColorId==pc_id);
            foreach (var item in pcf)
            {
                item.ProductColorFile__Name=$"{Request.Scheme}://{Request.Host}/{localStorage}/{item.ProductColorFile__Name}";
            }
            return Ok();
        }
        [HttpDelete("delete")]
        public IActionResult Remove(List<int> file_ids){
            var files=_uow.ProductColorFile.Find(x=>file_ids.Contains(x.ProductColorFile__Id)).ToList(); 
            var result=_uow.ProductColorFile.RemoveRange(files);
            if(result){
                 // Kiểm tra tệp có tồn tại không
                 foreach(var file in files){
                    if (!System.IO.File.Exists(uploadFilePath+"/"+file.ProductColorFile__Name))
                    {
                        return NotFound(new { message = "File not found" });
                    }
                    
                    System.IO.File.Delete(uploadFilePath+"/"+file.ProductColorFile__Name);
                 }
                
            }
            return Ok(result);
        }
        [HttpDelete("{pc_id:int?}")]
        public IActionResult RemoveAll(int pc_id){
            var files=_uow.ProductColorFile.Find(x=>x.ProductColorFile__ProductColorId==pc_id).ToList();
            var result=_uow.ProductColorFile.RemoveRange(files);
            if(result){
                 // Kiểm tra tệp có tồn tại không
                 foreach(var file in files){
                    if (!System.IO.File.Exists(uploadFilePath+"/"+file.ProductColorFile__Name))
                    {
                        return NotFound(new { message = "File not found" });
                    }
                    
                    System.IO.File.Delete(uploadFilePath+"/"+file.ProductColorFile__Name);
                 }
                
            }
            return Ok(result);
        }
        // public async Task<IActionResult> Remove(int){

        // }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] List<ProductColorFile> productColorFiles)
        {  
            System.Console.WriteLine(productColorFiles);
            if (productColorFiles == null || !productColorFiles.Any())
                return BadRequest("No files were uploaded.");

            foreach (var file in productColorFiles)
            {   
                var result=_uow.ProductColorFile.Add(file);
                    if(result){
                    var filePath = Path.Combine(uploadFilePath, file.ProductColorFile__Name!);
                    // Lưu file vào thư mục server
                    var isSuccess=await HandleFile.Upload(file.ProductColorFile__File,filePath);
                    if(!isSuccess){
                        //Xóa nếu không thành công
                        _uow.ProductColorFile.Remove(_uow.ProductColorFile.FirstOrDefault(x=>x.ProductColorFile__Name==file.ProductColorFile__Name));
                    }
                }
                
            }
            var fileUrls=_uow.ProductColorFile.Find(x=>x.ProductColorFile__ProductColorId==productColorFiles[0].ProductColorFile__ProductColorId);
                        foreach (var file in fileUrls)
                    {
                        file.Url= $"{Request.Scheme}://{Request.Host}/{localStorage}/{file.ProductColorFile__Name}";
                    }
            return Ok(new { isSuccess = true, fileUrls });
        }

    }
}