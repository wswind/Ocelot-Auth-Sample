# OcelotAuthenticationDemo

this is a demo to learn about ocelot auth feature.

api1 service  has no authentication restrictions

if you go ocelot gateway ,the response will be 401
```
curl -X GET  http://localhost:5010/api1/values
```

but http://localhost:5001/api/values will return the response directly
