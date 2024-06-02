function onInput(e) {
    fetch("Fridge/ProductSearch?keyphrase=" + e.value)
        .then(x => x.json())
        .then(r =>
		{
			document.querySelector(".search-results").innerHTML = "";

			if (r.length === 0 && e.value!=="") {
				document.querySelector(".search-results")
					.innerHTML = `<a class="btn btn-primary" href="/Fridge/addProduct?prodName=${e.value}">Add Product</a>`;
			}

            r.forEach(product => {
				document.querySelector(".search-results").innerHTML +=
				`<tr id="${product.id}" onClick="ViewProductDetails(this)">
					<td>
						<p>${product.name}</p>
					</td>
					<td>
						<p>Weight: ${product.weight ? product.weight:"--"} g</p>
					</td>
					<td>
						<p>Category: ${product.productCategory.name}</p>
					</td>
					<td>
						<p>Quantity: ${product.count}</p>
					</td>
				</tr>`
            });
        });
}

function ViewProductDetails(e) {
	window.location.href = '/Fridge/EditProduct/'+e.id;
}