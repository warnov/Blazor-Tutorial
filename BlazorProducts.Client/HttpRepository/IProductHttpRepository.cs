using BlazorProducts.Client.Features;
using Entities.Models;
using Entities.RequestFeatures;
using System.Threading.Tasks;

namespace BlazorProducts.Client.HttpRepository
{
    public interface IProductHttpRepository
    {
        Task<PagingResponse<Product>> GetProducts(ProductParameters productParameters);
    }
}
