
using SneakerAPI.Core.Interfaces;

namespace SneakerAPI.Api.Controllers.ProductControllers
{
    public class UProductController : BaseController
    {
        private readonly IUnitOfWork _uow;

        public UProductController(IUnitOfWork uow):base(uow)
        {
            _uow = uow;
        }
    }
}