self.addEventListener('push', function (event) {
    var data = event.data.json();

    var options = {
        body: data.body,
        icon: data.icon,
        badge: data.badge,
        vibrate:[100, 50, 100],
        data: {
            url: data.url
        }
    };

    event.waitUntil(
        self.registration.showNotification(data.title, options)
    );
});

self.addEventListener('notificationclick', function (event) {
    event.notification.close();
    event.waitUntil(
        clients.openWindow(event.notification.data.url)
    );
});