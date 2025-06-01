<h1>Default Admin User</h1>
<p>Username: <b>admin</b></p>
<p>Password: <b>admin123</b></p>
<h1>User Secrets</h1>
<pre>
"ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HomeAssistant;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
    "MongoUri": "mongodb://localhost:27017"
},
"ExternalServiceApiKeys": {
    "weatherApi": "[token]", - [OpemWeather](https://openweathermap.org/)
    "NASHostToken": "[token]",
    "ipinfo": "[token]" - [ipinfo](https://ipinfo.io/)
},
"WakeOnLanPcs": {
    "hostpc1": "MainPc-Host:[MAC-address]:192.168.0.2:[security-token]",
    "nas1": "NAS:[MAC-address]:192.168.0.8:[security-nas-token-TrueNas]"
},
"VAPID": { - browser notifications configuration
    "PublicKey": "[token]",
    "PrivateKey": "[token]",
    "Mail": "mailto:example@example.com"
},
"Configuration": {
    "PublicURL": "https://mysite.com"
}
</pre>
