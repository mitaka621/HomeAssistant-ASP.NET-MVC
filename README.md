<h1>Default Admin User</h1>
<p>Username: <b>admin</b></p>
<p>Password: <b>admin123</b></p>
<h1>User Secrets</h1>
<p>- (Required) Sql server connection string</p>
<p>- (Required) MongoDb access uri</p>
<p>- (Optional) Wheather api key - More info https://openweathermap.org/</p>
<p>- (Optional) HomeTelemetryServerIp - More info https://github.com/mitaka621/HomeTelemetryStation</p>
<h2>Example: </h2>
<pre>
{
  "HomeTelemetryServerIp": "192.168.0.5",
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=HomeAssistant;Encrypt=True;Integrated Security=True;TrustServerCertificate=True",
    "MongoUri": "mongodb+srv://d418201:oU9CZJ5ZmkPNTICh@cluster0.nfrcbmt.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0",
  },
  "ExternalServiceApiKeys": {
    "weatherApi": "1a9789d617380a5921737ac607d3edbd"
  }
}
</pre>
