var connection = new signalR.HubConnectionBuilder().withUrl("/fridgeHub").build();

connection.on("UpdateProductQuantity", function (productId, count) {
    var currentProduct = document.querySelector("div#p" + productId);

    if (currentProduct) {
        currentProduct.querySelector(".product-count").textContent = count;
    }
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

function IncreaseProduct(productId) {
    var currentProduct = document.querySelector("div#p" + productId);

    var productsCount = parseInt(currentProduct.querySelector(".product-count").textContent);
  
    if (productsCount === 0) {
        currentProduct.style.height = currentProduct.offsetHeight+"px";
        currentProduct.style.maxWidth = "0px";
        currentProduct.style.paddingLeft = "0px";
        currentProduct.style.paddingRight = "0px";
        currentProduct.style.opacity = "0";
        currentProduct.style.borderWidth = "0";

        setTimeout(()=>currentProduct.remove(), 600);      
    }

    connection
        .invoke("IncreaseProductQuantity", productId)
        .catch(function (error) {
            currentProduct.classList += (" alert alert-danger");
            currentProduct.querySelector(".product-count").style.color = "orange";
        });
}

function DecreaseProduct(productId) {
    var currentProduct = document.querySelector("div#p" + productId);

    var productsCount = parseInt(currentProduct.querySelector(".product-count").textContent);

    if (productsCount <=0) {
        return;
    }

    if (productsCount === 1) {
        currentProduct.style.height = currentProduct.offsetHeight + "px";
        currentProduct.style.maxWidth = "0px";
        currentProduct.style.paddingLeft = "0px";
        currentProduct.style.paddingRight = "0px";
        currentProduct.style.opacity = "0";
        currentProduct.style.borderWidth = "0";

        setTimeout(() => currentProduct.remove(), 600);
    }

    connection
        .invoke("DecreaseProductQuantity", productId)
        .catch(function (error) {
            currentProduct.classList += (" alert alert-danger");
            currentProduct.querySelector(".product-count").style.color = "orange";
        });
}