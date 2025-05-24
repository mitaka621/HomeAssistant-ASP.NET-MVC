using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Enums;
using HomeAssistant.Core.Models.Product;
using HomeAssistant.Core.Services;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace UnitTests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private HomeAssistantDbContext _dbContext;
        private IProductService _productService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HomeAssistantDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _dbContext = new HomeAssistantDbContext(options);
            _productService = new ProductService(_dbContext);
            SeedData();
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        private async void SeedData()
        {
            _dbContext.Categories.Add(new Category()
            {
                Id = 1,
                Name = "cat1"
            });
            _dbContext.Categories.Add(new Category()
            {
                Id = 2,
                Name = "cat1"
            });

            _dbContext.Products.Add(new Product
            {
                Id = 100,
                Name = "Test Product 1",
                Count = 5,
                AddedOn = DateTime.Now,
                CategoryId = 1
            });

            _dbContext.Products.Add(new Product
            {
                Id = 101,
                Name = "Product 2",
                Count = 0,
                AddedOn = DateTime.Now,
                CategoryId = 2
            });


            await _dbContext.SaveChangesAsync();
        }

        [Test]
        public async Task GetProducts_Returns_Correct_Page_Count()
        {

            int expectedPageCount = 1;
            int productsOnPage = 20;
            bool available = true;
            int? categoryId = null;
            OrderBy orderBy = OrderBy.Recent;
            int page = 1;


            var result = await _productService.GetProducts(available, categoryId, orderBy, page, productsOnPage);


            Assert.That(expectedPageCount == result.PageCount);
        }

        [Test]
        public async Task GetAllCategories_Returns_All_Categories()
        {

            var result = await _productService.GetAllCategories();


            Assert.That(2 == result.Count());
        }

        [Test]
        public async Task IncreaseQuantityByOne_Increases_Product_Count()
        {

            int productId = 100;
            int expectedCount = 6;


            await _productService.IncreaseQuantityByOne(productId);


            var product = await _dbContext.Products.FindAsync(productId);
            Assert.That(expectedCount == product.Count);
        }

        [Test]
        public async Task DecreaseQuantityByOne_Decreases_Product_Count()
        {

            int productId = 100;
            int expectedCount = 4;


            await _productService.DecreaseQuantityByOne(productId);


            var product = await _dbContext.Products.FindAsync(productId);
            Assert.That(expectedCount == product.Count);
        }

        [Test]
        public async Task SearchProducts_Returns_Products_Containing_Keyphrase()
        {

            string keyphrase = "Test";


            var result = await _productService.SearchProducts(keyphrase);


            Assert.That(1 == result.Count());
            Assert.That(result.All(p => p.Name.Contains(keyphrase)));
        }

        [Test]
        public async Task GetProduct_Returns_Correct_Product()
        {

            int productId = 100;


            var result = await _productService.GetProduct(productId);


            Assert.That(productId == result.Id);
            Assert.That("Test Product 1" == result.Name);
        }

        [Test]
        public async Task AddProduct_Adds_New_Product()
        {

            string userId = "user123";
            var product = new ProductFormViewModel
            {
                Name = "New Product",
                SelectedCategoryId = 1,
                Count = 10,
                Weight = 5
            };


            var productId = await _productService.AddProduct(userId, product);
            var addedProduct = await _dbContext.Products.FindAsync(productId);


            Assert.That(addedProduct, Is.Not.Null);
            Assert.That("New Product" == addedProduct.Name);
            Assert.That(userId == addedProduct.UserId);
        }

        [Test]
        public async Task DeleteProduct_Removes_Product()
        {

            int productId = 101;


            await _productService.DeleteProduct(productId);
            var deletedProduct = await _dbContext.Products.FindAsync(productId);


            Assert.That(deletedProduct, Is.Null);
            Assert.That(1 == _dbContext.Products.Count());
            Assert.That(100 == _dbContext.Products.First().Id);
        }

        [Test]
        public async Task EditProduct_Updates_Product_Details()
        {

            int productId = 100;

            var updatedProduct = new ProductFormViewModel
            {
                Id = productId,
                Name = "New Name",
                SelectedCategoryId = 1,
                Count = 10,
                Weight = 3
            };


            await _productService.EditProduct("user123", updatedProduct);
            var editedProduct = await _dbContext.Products.FindAsync(productId);


            Assert.That("New Name" == editedProduct.Name);
            Assert.That(10 == editedProduct.Count);
            Assert.That(3 == editedProduct.Weight);
        }
    }
}
