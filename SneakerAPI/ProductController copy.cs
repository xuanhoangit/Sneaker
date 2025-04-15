using SneakerAPI.Core.Interfaces;

namespace SneakerAPI.AdminApi.Controllers.CartControllers
{
    public class UCartController : BaseController
    {
        private readonly IUnitOfWork _uow;

        public UCartController(IUnitOfWork uow):base(uow)
        {
            _uow = uow;
        }
    }
}