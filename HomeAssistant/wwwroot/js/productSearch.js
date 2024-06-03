function onInput(e) {
    fetch("Fridge/ProductSearch?keyphrase=" + e.value)
        .then(x => x.json())
        .then(r =>
		{
			document.querySelector(".search-results").innerHTML = "";

			if (r.length === 0 && e.value!=="") {
				document.querySelector(".search-results")
					.innerHTML = `<a class="btn btn-primary result-item" href="/Fridge/addProduct?prodName=${e.value}">Add Product</a>`;
			}

            r.forEach(product => {
				document.querySelector(".search-results").innerHTML +=
					`<tr id="${product.id}" class="result-item" onClick="ViewProductDetails(this)">
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

document.addEventListener('keydown', function (event) {
	if (event.code === 'Enter' || event.keyCode === 13) {
		event.preventDefault();

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