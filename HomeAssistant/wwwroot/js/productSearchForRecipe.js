document.querySelector("#search").addEventListener("focusout", clear);
document.querySelector("#search").addEventListener("focus", (e) => onInput(e.target));

function onInput(e) {
	
    fetch("/Fridge/ProductSearch?keyphrase=" + e.value)
        .then(x => x.json())
        .then(r => {
            document.querySelector(".search-results").innerHTML = "";

			if (r.length === 0 && e.value !== "") {
				document.getElementById("product-name").value = e.value;

				CheckIfEmpty(e);

                document.querySelector(".search-results")
					.innerHTML = `<button type="button" class="btn btn-primary result-item" data-bs-toggle="modal" data-bs-target="#staticBackdrop">
									Add New Product to Fridge and Recipe
								</button>`;
				document.querySelector("#search").removeEventListener("focusout", clear);

				return;
			}

			document.querySelector("#search").addEventListener("focusout", clear);

			let existingProductsIds = (Array.from(document.querySelectorAll(`a[onClick="removeProduct(this)"]`))).map(x => x.id);

            r.forEach(product => {
                if (!existingProductsIds.includes(product.id.toString())) {


                    document.querySelector(".search-results").innerHTML +=
						`<tr name="${product.name}" id="${product.id}" onmousedown="ViewProductDetails(this)" class="result-item">
					<td>
						<p>${product.name}</p>
					</td>
					<td>
						<p>Weight: ${product.weight ? product.weight : "--"} g</p>
					</td>
					<td>
						<p>Category: ${product.productCategory.name}</p>
					</td>
					<td>
						<p>Quantity: ${product.count}</p>
					</td>
				</tr>`;
                }
                else {
                    document.querySelector(".search-results").innerHTML +=
                        `<tr name="${product.name}" id="${product.id}" class="table-dark">
					<td>
						<p>${product.name}</p>
					</td>
					<td>
						<p>Weight: ${product.weight ? product.weight : "--"} g</p>
					</td>
					<td>
						<p>Category: ${product.productCategory.name}</p>
					</td>
					<td>
						<p>Quantity: ${product.count}</p>
					</td>
				</tr>`;
                }
            });
        });
}
var products = Array.from(document.querySelectorAll("div.add-products-container")).map(x => parseInt( x.id.split("p")[1]));
let count = 0;
if (products.length!=0) {
	products.sort((a, b) => b - a);
	count = products[0]+1;
}
function ViewProductDetails(e) {
	const productsContainer = document.querySelector('div.products');

	const productDiv = document.createElement('div');
	productDiv.className = 'add-products-container';

	const nameInput = document.createElement('input');
	nameInput.className = 'form-control';
	nameInput.value = e.getAttribute('name');
	nameInput.disabled = true;

	const hiddenInput = document.createElement('input');
	hiddenInput.type = 'hidden';
	hiddenInput.name = `SelectedProducts[${count}].Id`;
	hiddenInput.className = 'form-control';
	hiddenInput.id = e.id;
	hiddenInput.value = e.id;

	const quantityInput = document.createElement('input');
	quantityInput.type = 'number';
	quantityInput.value = 1;
	quantityInput.min = '1';
	quantityInput.step = '1';
	quantityInput.name = `SelectedProducts[${count++}].Quantity`;
	quantityInput.className = 'form-control';
	quantityInput.placeholder = 'Enter product amount';
	quantityInput.required = true;

	const removeButton = document.createElement('a');
	removeButton.id = e.id;
	removeButton.className = 'btn btn-danger';
	removeButton.textContent = 'remove';
	removeButton.setAttribute('onClick', 'removeProduct(this)');

	productDiv.appendChild(nameInput);
	productDiv.appendChild(hiddenInput);
	productDiv.appendChild(quantityInput);
	productDiv.appendChild(removeButton);

	productsContainer.insertBefore(productDiv, productsContainer.firstChild);

	document.querySelector("#search").addEventListener("focusout", clear);
	document.querySelector("#search").addEventListener("focus", (e) => onInput(e.target));
	clear();
	document.querySelector("#search").value = "";
}

function removeProduct(e) {
	count--;
	e.parentElement.remove();
}

function clear() {
		document.querySelector("tbody.search-results").innerHTML = "";
}

function redirect() {
	window.location.href = `/Fridge/addProduct`;
}

function CheckIfEmpty(e) {
	if (e.value) {
		document.getElementById("submit-new-prod").removeAttribute("disabled");
	}
	else {
		document.getElementById("submit-new-prod").setAttribute("disabled","true");
	}
}

function CreateNewProduct() {
	const productName = document.getElementById("product-name").value;
	const categoryId = document.getElementById("selected-category").value;


	fetch('/api/Fridge/AddProductApi', {
		method: 'POST',
		headers: {
			"Content-Type": "application/json",
			"Accept": "application/json"
		},
		body: JSON.stringify({
			Name: productName,
			SelectedCategoryId: categoryId
		})
	})
		.then(response => response.json())
		.then(j => {
			prodId = j.prodId;

			const productsContainer = document.querySelector('div.products');

			const productDiv = document.createElement('div');
			productDiv.className = 'add-products-container';

			const nameInput = document.createElement('input');
			nameInput.className = 'form-control';
			nameInput.value = productName;
			nameInput.disabled = true;

			const hiddenInput = document.createElement('input');
			hiddenInput.type = 'hidden';
			hiddenInput.name = `SelectedProducts[${count}].Id`;
			hiddenInput.className = 'form-control';
			hiddenInput.id = prodId;
			hiddenInput.value = prodId;

			const quantityInput = document.createElement('input');
			quantityInput.type = 'number';
			quantityInput.value = 1;
			quantityInput.min = '1';
			quantityInput.step = '1';
			quantityInput.name = `SelectedProducts[${count++}].Quantity`;
			quantityInput.className = 'form-control';
			quantityInput.placeholder = 'Enter product amount';
			quantityInput.required = true;

			const removeButton = document.createElement('a');
			removeButton.id = prodId;
			removeButton.className = 'btn btn-danger';
			removeButton.textContent = 'remove';
			removeButton.setAttribute('onClick', 'removeProduct(this)');

			productDiv.appendChild(nameInput);
			productDiv.appendChild(hiddenInput);
			productDiv.appendChild(quantityInput);
			productDiv.appendChild(removeButton);

			productsContainer.insertBefore(productDiv, productsContainer.firstChild);

			document.querySelector("#search").addEventListener("focusout", clear);
			clear();
			document.querySelector("tbody.search-results").innerHTML = "";
			document.querySelector("#search").value = "";

			document.querySelector("button.btn-close").click();
		})
		.catch(error => {
			console.error('Error:', error);
		});
}

document.addEventListener('keydown', function (event) {
	if (event.code === 'Enter' || event.keyCode === 13) {
		event.preventDefault();

		document.getElementById("search").value = "";

		const button = document.querySelector('.result-item');
		simulateMouseDown(button);
		button.click();
	}
});

function simulateMouseDown(element) {
	const event = new MouseEvent('mousedown', {
		view: window,
		bubbles: true,
		cancelable: true,
		buttons: 1
	});
	element.dispatchEvent(event);
}