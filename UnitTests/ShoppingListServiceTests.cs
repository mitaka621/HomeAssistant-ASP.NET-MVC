using HomeAssistant.Core.Contracts;
using HomeAssistant.Core.Models.ShoppingList;
using HomeAssistant.Core.Services;
using HomeAssistant.Infrastructure.Data;
using HomeAssistant.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace UnitTests
{
    [TestFixture]
    public class ShoppingListServiceTests
    {
        private HomeAssistantDbContext _dbContext;
        private IProductService _productService;
        private Mock<IImageService> _imageServiceMock;
        private IShoppingListService _service;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<HomeAssistantDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;
            _dbContext = new HomeAssistantDbContext(options);

            _dbContext.Users.Add(new HomeAssistantUser() { Id = "user1", FirstName = "pisho" });
            _dbContext.SaveChanges();

            _productService = new ProductService(_dbContext);

            _imageServiceMock = new Mock<IImageService>();
            _service = new ShoppingListService(_dbContext, _productService, _imageServiceMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public async Task IsStarted_Returns_True_When_ShoppingList_Is_Started()
        {
            var userId = "user1";
            _dbContext.ShoppingLists.Add(new ShoppingList { UserId = userId, IsStarted = true });
            await _dbContext.SaveChangesAsync();

            var result = await _service.IsStarted(userId);

            Assert.That(result);
        }

        [Test]
        public async Task IsFinished_Returns_True_When_ShoppingList_Is_Finished()
        {

            var userId = "user1";
            _dbContext.ShoppingLists.Add(new ShoppingList { UserId = userId, IsFinished = true });
            await _dbContext.SaveChangesAsync();


            var result = await _service.IsFinished(userId);


            Assert.That(result);
        }

        [Test]
        public async Task GetShoppingList_Returns_New_ShoppingList_When_Doesnt_Exist()
        {

            var userId = "user1";

            var result = await _service.GetShoppingList(userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.OutOfStockProducts, Is.Not.Null);
            Assert.That(result.AllCategories, Is.Not.Null);
            Assert.That(result.Products, Is.Empty);
        }

        [Test]
        public async Task DeleteShoppingList_ShouldRemoveShoppingList_WhenShoppingListExists()
        {

            var userId = "user1";
            await _dbContext.ShoppingLists.AddAsync(new ShoppingList { UserId = userId });
            await _dbContext.SaveChangesAsync();


            await _service.DeleteShoppingList(userId);


            var deletedShoppingList = await _dbContext.ShoppingLists.FirstOrDefaultAsync(x => x.UserId == userId);
            Assert.That(deletedShoppingList, Is.Null);
        }

        [Test]
        public void DeleteShoppingList_ShouldThrowArgumentNullException_WhenShoppingListDoesNotExist()
        {
            var userId = "user1";

            Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.DeleteShoppingList(userId));
        }

        [Test]
        public async Task AddProductToShoppingList_ShouldAddProduct_WhenProductDoesNotExistInShoppingList()
        {

            var userId = "user1";
            var productViewModel = new ShoppingListProductViewModel
            {
                Id = 1,
                Price = 10,
                QuantityToBuy = 2
            };

            _dbContext.Products.Add(new Product()
            {
                Id = 1,
                Name = "domat",
            });


            _dbContext.ShoppingLists.Add(new ShoppingList()
            {
                UserId = userId,
                IsFinished = false,
                IsStarted = true,
            });

            await _dbContext.SaveChangesAsync();


            await _service.AddProductToShoppingList(userId, productViewModel);


            var shoppingListProduct = await _dbContext.ShoppingListsProducts.FirstOrDefaultAsync(x => x.ProductId == productViewModel.Id && x.ShoppingListId == userId);
            Assert.That(shoppingListProduct, Is.Not.Null);
            Assert.That(productViewModel.Price == shoppingListProduct.StorePrice);
            Assert.That(productViewModel.QuantityToBuy == shoppingListProduct.QuantityToBuy);
        }

        [Test]
        public async Task AddProductToShoppingList_ShouldThrowArgumentException_WhenProductAlreadyExistsInShoppingList()
        {

            var userId = "user1";
            var productId = 1;
            await _dbContext.Products.AddAsync(new Product { Id = productId });
            await _dbContext.ShoppingListsProducts.AddAsync(new ShoppingListProduct { ProductId = productId, ShoppingListId = userId });
            await _dbContext.SaveChangesAsync();


            var productViewModel = new ShoppingListProductViewModel
            {
                Id = productId,
                Price = 10,
                QuantityToBuy = 2
            };
            Assert.ThrowsAsync<ArgumentException>(() => _service.AddProductToShoppingList(userId, productViewModel));
        }

        [Test]
        public async Task AddProductToShoppingList_ShouldThrowArgumentNullException_WhenProductDoesNotExist()
        {

            var userId = "user1";
            var productViewModel = new ShoppingListProductViewModel { Id = 1 };


            Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddProductToShoppingList(userId, productViewModel));
        }

        [Test]
        public async Task DeleteProductFromShoppingList_ShouldDeleteProduct_WhenProductExistsInShoppingList()
        {

            var userId = "user1";
            var productId = 1;
            await _dbContext.ShoppingListsProducts.AddAsync(new ShoppingListProduct { ShoppingListId = userId, ProductId = productId });
            await _dbContext.SaveChangesAsync();


            await _service.DeleteProductFromShoppingList(userId, productId);


            var product = await _dbContext.ShoppingListsProducts.FirstOrDefaultAsync(x => x.ProductId == productId && x.ShoppingListId == userId);
            Assert.That(product, Is.Null);
        }

        [Test]
        public void DeleteProductFromShoppingList_ShouldThrowArgumentNullException_WhenProductDoesNotExistInShoppingList()
        {

            var userId = "user1";
            var productId = 1;


            Assert.ThrowsAsync<ArgumentNullException>(() => _service.DeleteProductFromShoppingList(userId, productId));
        }

        [Test]
        public async Task AddNewProductToFridgeAndShoppingList_ShouldAddProductToDatabaseAndShoppingList()
        {

            var userId = "user1";
            var product = new ShoppingListProductViewModel
            {
                Name = "Test Product",
                CategoryName = "meat",
                QuantityToBuy = 1,
                Price = 10
            };

            _dbContext.Categories.Add(new Category() { Name = "meat" });

            await _dbContext.SaveChangesAsync();

            var categoryId = 1;


            await _service.AddNewProductToFridgeAndShoppingList(userId, product);


            var productInDb = await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == 1);
            Assert.That(productInDb, Is.Not.Null);
            Assert.That(product.Name == productInDb.Name);

            var productInShoppingList = await _dbContext.ShoppingListsProducts.FirstOrDefaultAsync(x => x.ProductId == 1 && x.ShoppingListId == userId);
            Assert.That(productInShoppingList, Is.Not.Null);
            Assert.That(product.QuantityToBuy == productInShoppingList.QuantityToBuy);
        }

        [Test]
        public async Task StartShopping_ShouldStartShoppingListIfNotEmpty()
        {

            var userId = "user1";
            var product = new ShoppingListProduct { ProductId = 1, ShoppingListId = userId };

            var shoppingList = new ShoppingList
            {
                UserId = userId,
                ShoppingListProducts = new List<ShoppingListProduct> { product }
            };

            _dbContext.ShoppingLists.Add(shoppingList);
            await _dbContext.SaveChangesAsync();


            await _service.StartShopping(userId);


            var updatedShoppingList = await _dbContext.ShoppingLists.FindAsync(userId);

            Assert.That(updatedShoppingList, Is.Not.Null);
            Assert.That(updatedShoppingList.IsStarted);
            Assert.That(updatedShoppingList.StartedOn, Is.Not.Null);
        }

        [Test]
        public async Task StartShopping_ShouldNotStartEmptyShoppingList()
        {

            var userId = "user1";

            var shoppingList = new ShoppingList
            {
                UserId = userId,
            };

            _dbContext.ShoppingLists.Add(shoppingList);
            await _dbContext.SaveChangesAsync();


            await _service.StartShopping(userId);


            var updatedShoppingList = await _dbContext.ShoppingLists.FindAsync(userId);

            Assert.That(updatedShoppingList, Is.Not.Null);
            Assert.That(!updatedShoppingList.IsStarted);
            Assert.That(updatedShoppingList.StartedOn, Is.Null);
        }

        [Test]
        public async Task CancelShopping_ShouldCancelShoppingList()
        {

            var userId = "user1";
            var product1 = new ShoppingListProduct { ProductId = 1, ShoppingListId = userId, IsBought = true };
            var product2 = new ShoppingListProduct { ProductId = 2, ShoppingListId = userId, IsBought = true };

            var shoppingList = new ShoppingList
            {
                UserId = userId,
                IsStarted = true,
                ShoppingListProducts = new List<ShoppingListProduct> { product1, product2 }
            };

            _dbContext.ShoppingLists.Add(shoppingList);
            await _dbContext.SaveChangesAsync();


            await _service.CancelShopping(userId);


            var updatedShoppingList = await _dbContext.ShoppingLists.FindAsync(userId);
            Assert.That(updatedShoppingList, Is.Not.Null);
            Assert.That(!updatedShoppingList.IsStarted);

            foreach (var item in updatedShoppingList.ShoppingListProducts)
            {
                Assert.That(!item.IsBought);
            }
        }

        [Test]
        public async Task GetProductsByCategory_ShouldReturnCorrectProducts()
        {

            var userId = "user1";

            var products = new List<ShoppingListProduct>
            {
                new ShoppingListProduct
                {
                    ProductId = 1,
                    ShoppingListId = userId,
                    IsBought = true,
                    StorePrice = 5,
                    QuantityToBuy = 2,
                    Product = new Product { Name = "Product1", Category = new Category { Name = "Category1" } }
                },
                new ShoppingListProduct
                {
                    ProductId = 2,
                    ShoppingListId = userId,
                    IsBought = false,
                    StorePrice = 7,
                    QuantityToBuy = 3,
                    Product = new Product { Name = "Product2", Category = new Category { Name = "Category2" } }
                },
                new ShoppingListProduct
                {
                    ProductId = 3,
                    ShoppingListId = userId,
                    IsBought = false,
                    StorePrice = 9,
                    QuantityToBuy = 1,
                    Product = new Product { Name = "Product3", Category = new Category { Name = "Category1" } }
                }
            };

            _dbContext.ShoppingListsProducts.AddRange(products);
            await _dbContext.SaveChangesAsync();


            var result = await _service.GetProductsByCategory(userId, 1, 1);


            Assert.That(1 == result.PageNumber);
            Assert.That(2 == result.TotalPages);
            Assert.That(33 == result.Progress);

            Assert.That(1 == result.BoughtProducts.Count);
            Assert.That(1 == result.UnboughtProductsByCategory.Count);


            var result2 = await _service.GetProductsByCategory(userId, 2, 1);


            Assert.That(2 == result2.PageNumber);
            Assert.That(2 == result2.TotalPages);
            Assert.That(33 == result2.Progress);

            Assert.That(1 == result2.BoughtProducts.Count);
            Assert.That(1 == result2.UnboughtProductsByCategory.Count);


        }

        [Test]
        public async Task MarkAsBought_ShouldMarkProductAsBought()
        {

            var userId = "user1";
            var prodId = 1;

            var product = new ShoppingListProduct { ShoppingListId = userId, ProductId = prodId, IsBought = false };
            _dbContext.ShoppingListsProducts.Add(product);
            await _dbContext.SaveChangesAsync();


            await _service.MarkAsBought(userId, prodId);


            var updatedProduct = await _dbContext.ShoppingListsProducts.FirstOrDefaultAsync(x => x.ShoppingListId == userId && x.ProductId == prodId);
            Assert.That(updatedProduct, Is.Not.Null);
            Assert.That(updatedProduct.IsBought);
        }

        [Test]
        public async Task MarkAsUnbought_ShouldMarkProductAsUnbought()
        {

            var userId = "user1";
            var productId = 1;

            var product = new ShoppingListProduct { ShoppingListId = userId, ProductId = productId, IsBought = true };
            _dbContext.ShoppingListsProducts.Add(product);
            await _dbContext.SaveChangesAsync();


            await _service.MarkAsUnbought(userId, productId);


            var updatedProduct = await _dbContext.ShoppingListsProducts.FirstOrDefaultAsync(x => x.ShoppingListId == userId && x.ProductId == productId);
            Assert.That(updatedProduct, Is.Not.Null);
            Assert.That(!updatedProduct.IsBought);
        }

        [Test]
        public async Task SaveBoughtProducts_ShouldUpdateProductQuantitiesAndMarkAsFinished()
        {

            var userId = "user1";

            _dbContext.Products.AddRange(new Product() { Id = 1, Name = "salam" }, new Product() { Id = 2, Name = "not salam" });

            var productList = new List<ShoppingListProduct>
            {
                new ShoppingListProduct { ShoppingListId = userId, ProductId = 1, QuantityToBuy = 2 },
                new ShoppingListProduct { ShoppingListId = userId, ProductId = 2, QuantityToBuy = 3 }
            };
            var shoppingList = new ShoppingList { UserId = userId, ShoppingListProducts = productList };
            _dbContext.ShoppingLists.Add(shoppingList);
            await _dbContext.SaveChangesAsync();


            await _service.SaveBoughtProducts(userId);


            var updatedProducts = await _dbContext.Products.Where(x => productList.Select(y => y.ProductId).Contains(x.Id)).ToListAsync();
            Assert.That(2 == updatedProducts.Count);

            foreach (var product in updatedProducts)
            {
                var quantityBought = productList.First(p => p.ProductId == product.Id).QuantityToBuy;
                Assert.That(quantityBought == product.Count);
                Assert.That(product.AddedOn, Is.Not.Default);
            }

            var updatedShoppingList = await _dbContext.ShoppingLists.FirstAsync(x => x.UserId == userId);
            Assert.That(updatedShoppingList.IsFinished);
        }

        [Test]
        public async Task GetTop20StartedShoppingListsExceptCurrentUser_ShouldReturnEmptyList_WhenNoShoppingListsFound()
        {

            var userId = "user1";

            var result = await _service.GetTop20StartedShoppingListsExceptCurrentUser(userId);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetShoppingListProgress_ShouldCalculateProgressCorrectly()
        {

            var userId = "user1";

            var shoppingList = new ShoppingList { UserId = userId };
            var productList = new List<ShoppingListProduct>
            {
                new ShoppingListProduct { ShoppingListId = userId, ProductId = 1, IsBought = true },
                new ShoppingListProduct { ShoppingListId = userId, ProductId = 2 },
                new ShoppingListProduct { ShoppingListId = userId, ProductId = 3, IsBought = true }
            };
            _dbContext.ShoppingLists.Add(shoppingList);
            _dbContext.ShoppingListsProducts.AddRange(productList);
            await _dbContext.SaveChangesAsync();


            var result = await _service.GetShoppingListProgress(userId);


            Assert.That(66 == result);
        }

    }
}
