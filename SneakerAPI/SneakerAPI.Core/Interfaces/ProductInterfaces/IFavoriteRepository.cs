using SneakerAPI.Core.DTOs;
using SneakerAPI.Core.Models.ProductEntities;

namespace SneakerAPI.Core.Interfaces.ProductInterfaces;

    public interface IFavoriteRepository : IRepository<Favorite>
    {
        Task<List<GetFavoriteDTO>> GetFavorites(int account_id);
    }
