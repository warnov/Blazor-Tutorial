using BlazorProducts.Client.Features;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorProducts.Client.HttpRepository
{
    public class ProductHttpRepository : IProductHttpRepository
    {
        private readonly HttpClient _client;

        public ProductHttpRepository(HttpClient client)
        {
            _client = client;
        }

        public async Task<PagingResponse<Product>> GetProducts(ProductParameters productParameters)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = productParameters.PageNumber.ToString(),
                ["searchTerm"] = productParameters.SearchTerm ?? "",
                ["orderBy"] = productParameters.OrderBy
            };

            var response = await _client.GetAsync(QueryHelpers.AddQueryString("https://localhost:5011/api/products", queryStringParam));
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var pagingResponse = new PagingResponse<Product>
            {
                Items = JsonSerializer.Deserialize<List<Product>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }),
                MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            };

            return pagingResponse;
        }

        public async Task CreateProduct(Product product)
        {
            var content = JsonSerializer.Serialize(product);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var postResult = await _client.PostAsync("https://localhost:5011/api/products", bodyContent);
            var postContent = await postResult.Content.ReadAsStringAsync();

            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(postContent);
            }
        }

        public async Task<string> UploadProductImage(MultipartFormDataContent content)
        {
            var postResult = await _client.PostAsync("https://localhost:5011/api/upload", content);
            var postContent = await postResult.Content.ReadAsStringAsync();

            if (!postResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(postContent);
            }
            else
            {
                var imgUrl = Path.Combine("https://localhost:5011/", postContent);
                return imgUrl;
            }
        }

        public async Task<Product> GetProduct(string id)
        {
            var url = Path.Combine("https://localhost:5011/api/products/", id);

            var response = await _client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var product = JsonSerializer.Deserialize<Product>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return product;
        }


        public async Task UpdateProduct(Product product)
        {
            var content = JsonSerializer.Serialize(product);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var url = Path.Combine("https://localhost:5011/api/products/", product.Id.ToString());

            var putResult = await _client.PutAsync(url, bodyContent);
            var putContent = await putResult.Content.ReadAsStringAsync();

            if (!putResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(putContent);
            }
        }

        public async Task DeleteProduct(Guid id)
        {
            var url = Path.Combine("https://localhost:5011/api/products", id.ToString());

            var deleteResult = await _client.DeleteAsync(url);
            var deleteContent = await deleteResult.Content.ReadAsStringAsync();

            if (!deleteResult.IsSuccessStatusCode)
            {
                throw new ApplicationException(deleteContent);
            }
        }
    }
}
