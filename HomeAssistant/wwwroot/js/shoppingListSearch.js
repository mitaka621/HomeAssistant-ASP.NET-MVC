function onInput(e) {
    fetch("/Fridge/ProductSearch?keyphrase=" + e.value)
        .then(x => x.json())
        .then(r => {
            document.querySelector(".search-results").innerHTML = "";

            if (r.length === 0 && e.value !== "") {

                document.querySelector("#productNametoAdd").value = e.value;

                document.querySelector(".search-results")
                    .innerHTML =
                    `<button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#addNonExistentProductModal">
	                    Add New Product to Fridge and Shopping List
                    </button>`;

                return;
            }

            let existingProductsIds = (Array.from(document.querySelectorAll("tbody.added-products>tr"))).map(x => x.id);



            r.forEach(product => {
                if (!existingProductsIds.includes(product.id.toString())) {


                    document.querySelector(".search-results").innerHTML +=
                        `<tr id="${product.id}" onClick="OpenModal(this)">
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
				</tr>`
                }
            });
        });
}

function OpenModal(e) {
    var myModal = new bootstrap.Modal(document.getElementById('addExistingProductModal'), {
        keyboard: false
    })

    if (e.classList.contains("product")) {
        console.log();
        document.querySelector("#productId").value = e.id;
        document.querySelector("#productName").value = e.childNodes[1].childNodes[0].data;
        document.querySelector("#price").value = 0;
        document.querySelector("#quantity").value = 1;
    }
    else {
        document.querySelector("#productId").value = e.id;
        document.querySelector("#productName").value = e.childNodes[1].childNodes[1].innerHTML;
        document.querySelector("#price").value = 0;
        document.querySelector("#quantity").value = 1;
    }

    let m = document.querySelector("#addExistingProductModal");
    myModal.show(m);
}