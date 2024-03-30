document.querySelector("#search").addEventListener("focusout", clear);
document.querySelector("#search").addEventListener("focus", (e) => onInput(e.target));

function onInput(e) {
	
    fetch("/Fridge/ProductSearch?keyphrase=" + e.value)
        .then(x => x.json())
        .then(r => {
            document.querySelector(".search-results").innerHTML = "";

            if (r.length === 0 && e.value !== "") {
                document.querySelector(".search-results")
					.innerHTML = `<a class="btn btn-primary" onmousedown="redirect()">Add Product</a>`;
			}
			
			let existingProductsIds = (Array.from(document.querySelectorAll("div.products>input"))).map(x => x.id);
            r.forEach(product => {
                if (!existingProductsIds.includes(product.id.toString())) {


                    document.querySelector(".search-results").innerHTML +=
						`<tr name="${product.name}" id="${product.id}" onmousedown="ViewProductDetails(this)">
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

function ViewProductDetails(e) {
	document.querySelector(`div.products`).innerHTML +=
		`<input disabled class="form-control" value="${e.getAttribute("name")}">
		<input type="hidden" name="ProductsIds" class="form-control" id="${e.id}" value="${e.id}">
		<a id="${e.id}" class="btn btn-danger" onClick="removeProduct(this)">remove</a>
		`;

	document.querySelector("#search").addEventListener("focusout", clear);
	document.querySelector("#search").addEventListener("focus", (e) => onInput(e.target));
	clear();
	e.parentElement.innerHTML = "";
	document.querySelector("#search").value = "";
}

function removeProduct(e) {
	document.querySelector(`input[id="${e.id}"]`).remove();
	e.remove();
}

function clear() {
		document.querySelector("tbody.search-results").innerHTML = "";
}

function redirect() {
	window.location.href = `/Fridge/addProduct`;
}