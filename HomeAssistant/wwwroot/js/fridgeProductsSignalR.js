var connection = new signalR.HubConnectionBuilder().withUrl("/fridgeHub").build();

connection.on("UpdateProductQuantity", function (productId, shouldIncrease) {
    var currentProduct = document.querySelector("div#p" + productId);

    if (currentProduct) {

        var productsCount = parseInt(currentProduct.querySelector(".product-count").textContent);

        if (shouldIncrease) {
            currentProduct.querySelector(".product-count").textContent = productsCount+1;
        }
        else {
            currentProduct.querySelector(".product-count").textContent = productsCount - 1;
        }
        
    }
});

connection.on("RequestLimitReached", function () {
    var checkbox = document.querySelector(`input[onchange='togleInstantProductChange(this);']`);

    checkbox.checked = false;
    checkbox.disabled = true;

    document.querySelectorAll(".action-btn").forEach(btn => {
        btn.setAttribute("onclick", `CountProducts(this)`)
    });
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

async function IncreaseProduct(productId) {
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

    currentProduct.querySelector(".product-count").textContent = productsCount + 1;

    await connection
        .invoke("IncreaseProductQuantity", productId)
        .catch(function (error) {
            currentProduct.classList += (" alert alert-danger");
            currentProduct.querySelector(".product-count").style.color = "orange";
            currentProduct.querySelector(".product-count").textContent = productsCount;
        });
}

async function DecreaseProduct(productId) {
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

    if (productsCount ===0) {

        return;
    }
    currentProduct.querySelector(".product-count").textContent = productsCount - 1;

    await connection
        .invoke("DecreaseProductQuantity", productId)
        .catch(function (error) {
            currentProduct.classList += (" alert alert-danger");
            currentProduct.querySelector(".product-count").style.color = "orange";
            currentProduct.querySelector(".product-count").textContent = productsCount;
        });
}

let prodtoChnageCount = 0;
function CountProducts(e) {
    document.querySelector(".save-message-container").style.display = "flex";
    window.onbeforeunload = function () {
        return true;
    };

    var product = e.parentElement.parentElement
    var counter = product.querySelector(".product-count");

    counter.style.color = "orange";
    counter.style.fontWeight = "700";

    let count = parseInt(counter.textContent);

    if (e.classList.contains("min")) {
        if (count===0) {
            return;
        }

        counter.textContent = --count;
    }
    else {
        counter.textContent = ++count;
    }

    var existingInput=document.getElementById("i" + e.parentElement.parentElement.id.split("p")[1]);

    if (existingInput) {
        existingInput.setAttribute("value", `${count}`);
        return;
    }

    document.querySelector(".multiple-products-update").innerHTML += `
        <input hidden name="products[${prodtoChnageCount}].Key" value="${e.parentElement.parentElement.id.split("p")[1]}" />
		<input hidden id="i${e.parentElement.parentElement.id.split("p")[1]}" name="products[${prodtoChnageCount}].Value" value="${count}" />`;   
    prodtoChnageCount++;

}

function togleInstantProductChange(checkbox) {

    if (checkbox.checked == true) {
        location.reload();
    } else {
        document.querySelectorAll(".action-btn").forEach(btn => {
            btn.setAttribute("onclick", `CountProducts(this)`)
        });
    }
}

function reload() {
    window.onbeforeunload = null;
    location.reload();
}