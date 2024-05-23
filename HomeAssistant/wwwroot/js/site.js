function loadToolTips() {

    document.querySelectorAll(`div[role="tooltip"]`).forEach(x => x.remove());

    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })

}

loadToolTips();