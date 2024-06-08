function loadToolTips() {

    document.querySelectorAll(`div[role="tooltip"]`).forEach(x => x.remove());

    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })

}

loadToolTips();

if ('serviceWorker' in navigator && 'PushManager' in window) {
    navigator.serviceWorker.register('/js/service-worker.js').then(function (registration) {
        registration.pushManager.getSubscription().then(function (subscription) {
            if (!subscription) {
                registration.pushManager.subscribe({
                    userVisibleOnly: true,
                    applicationServerKey: urlBase64ToUint8Array('BPD2hgSH5oyXW_fzPmB9nZGuDviCqg1VuNU_PyONX-VUY-Vsf0bLJr8pVT7eFo18fw4cCoNYHZIELnoYZiFTv6I')
                }).then(function (newSubscription) {
                    fetch('/api/Notification/Subscribe', {
                        method: 'POST',
                        body: JSON.stringify(newSubscription),
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    });
                });
            }
        });
    });
}

function urlBase64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);

    for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
}