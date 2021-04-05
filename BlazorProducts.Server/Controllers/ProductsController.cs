using BlazorProducts.Server.Repository;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace BlazorProducts.Server.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _repo;
        public ProductsController(IProductRepository repo)
        {
            _repo = repo;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductParameters productParameters)
        {
            var products = await _repo.GetProducts(productParameters);

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(products.MetaData));

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (product == null)
                return BadRequest();

            //model validation…

            await _repo.CreateProduct(product);

            return Created("", product);
        }
    }
}
