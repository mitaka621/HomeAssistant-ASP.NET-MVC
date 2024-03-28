document.querySelector("input#search").addEventListener("focusout", clear);
document.querySelector("input#search").addEventListener("focus", (e) => onInput(e.target));

function onInput(e) {
	
    fetch("/Fridge/ProductSearch?keyphrase=" + e.value)
        .then(x => x.json())
        .then(r => {
            document.querySelector(".search-results").innerHTML = "";

            if (r.length === 0 && e.value !== "") {
                document.querySelector(".search-results")
                    .innerHTML = `<a class="btn btn-primary" href="/Fridge/addProduct?prodName=${e.value}">Add Product</a>`;
			}

			let existingProductsIds = (Array.from(document.querySelectorAll("form>ul>li"))).map(x => x.id);

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
	
	document.querySelector(`form[action="/Recipe/Add"]`).innerHTML +=
        `<input type="hidden" asp-for="ProductsIds" class="form-control" value="${e.id}">`;
	document.querySelector("form>ul").innerHTML +=
	`<li id="${e.id}"><p>${e.getAttribute("name")} </p>
	<a id="${e.id}" class="btn btn-danger" onClick="removeProduct(this)">remove</a>
	</li>`;

	e.parentElement.innerHTML = "";
	document.querySelector("#search").value = "";
}

function removeProduct(e) {
	document.querySelector(`li[id="${e.id}"]`).remove();
}

function clear() {
		document.querySelector("tbody.search-results").innerHTML = "";
}