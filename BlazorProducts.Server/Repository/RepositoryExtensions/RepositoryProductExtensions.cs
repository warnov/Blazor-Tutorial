using Entities.Models;
using System.Linq;

namespace BlazorProducts.Server.Repository.RepositoryExtensions
{
    public static class RepositoryProductExtensions
    {
        public static IQueryable<Product> Search(this IQueryable<Product> products, string searchTearm)
        {
            if (string.IsNullOrWhiteSpace(searchTearm))
                return products;
            var lowerCaseSearchTerm = searchTearm.Trim().ToLower();
            return products.Where(p => p.Name.ToLower().Contains(lowerCaseSearchTerm));
        }
    }
}
