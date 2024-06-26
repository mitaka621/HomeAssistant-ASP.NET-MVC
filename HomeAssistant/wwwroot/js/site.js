function loadToolTips() {

    document.querySelectorAll(`div[role="tooltip"]`).forEach(x => x.remove());

    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })

}

loadToolTips();

Notification.requestPermission();

if ('serviceWorker' in navigator && 'PushManager' in window ) {
    fetch("/api/Notification/CheckSubscription?deviceType=" + getDeviceType())
        .then(r => {
            if (r.ok && Notification.permission != "granted") {
                fetch('/api/Notification/RemoveSubscription?deviceType=' + getDeviceType(), {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });
            }
            if (r.status === 404 && Notification.permission === "granted") {
                navigator.serviceWorker.register('/js/service-worker.js').then(function (registration) {
                    registration.pushManager.getSubscription().then(async function (subscription) {
                        if (subscription) {
                            await subscription.unsubscribe();
                        }
                        registration.pushManager.subscribe({
                            userVisibleOnly: true,
                            applicationServerKey: urlBase64ToUint8Array('BPD2hgSH5oyXW_fzPmB9nZGuDviCqg1VuNU_PyONX-VUY-Vsf0bLJr8pVT7eFo18fw4cCoNYHZIELnoYZiFTv6I')
                        }).then(function (newSubscription) {

                            let subscribtionObj = JSON.parse(JSON.stringify(newSubscription));
                            subscribtionObj["deviceType"] = getDeviceType();

                            fetch('/api/Notification/Subscribe', {
                                method: 'POST',
                                body: JSON.stringify(subscribtionObj),
                                headers: {
                                    'Content-Type': 'application/json'
                                }
                            });
                        });

                    });
                });
            }
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

function getDeviceType() {
    const userAgent = navigator.userAgent || navigator.vendor || window.opera;

    // Detect iOS devices
    if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
        return 'iOS';
    }

    // Detect Android devices
    if (/android/i.test(userAgent)) {
        return 'Android';
    }

    // Detect Windows Phone devices
    if (/windows phone/i.test(userAgent)) {
        return 'Windows Phone';
    }

    // Detect Windows devices
    if (/windows/i.test(userAgent)) {
        return 'Windows';
    }

    // Detect MacOS devices
    if (/macintosh|mac os x/i.test(userAgent)) {
        return 'MacOS';
    }

    // Detect ChromeOS devices
    if (/CrOS/.test(userAgent)) {
        return 'ChromeOS';
    }

    // Detect Linux devices
    if (/linux/i.test(userAgent)) {
        return 'Linux';
    }

    // Detect Kindle devices
    if (/kindle|silk/i.test(userAgent)) {
        return 'Kindle';
    }

    // Detect FireFox OS devices
    if (/firefox/i.test(userAgent) && /mobile/i.test(userAgent)) {
        return 'Firefox OS';
    }

    // Detect BlackBerry devices
    if (/blackberry|bb/i.test(userAgent)) {
        return 'BlackBerry';
    }

    // Detect PlayStation devices
    if (/playstation/i.test(userAgent)) {
        return 'PlayStation';
    }

    // Detect Nintendo devices
    if (/nintendo/i.test(userAgent)) {
        return 'Nintendo';
    }

    // Detect Tizen devices
    if (/tizen/i.test(userAgent)) {
        return 'Tizen';
    }

    // Detect Sailfish OS devices
    if (/sailfish/i.test(userAgent)) {
        return 'Sailfish OS';
    }

    // Detect other unknown platforms
    return 'Unknown';
}