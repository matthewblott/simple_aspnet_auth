# simple_aspnet_auth

Simple ASP.NET Authorisation boilerplate project. No EF, no database, no IdentityServer4, no session storage, just a basic logging in system for both cookies and JWT with a couple of authorisation policies and a controller with a set of examples.

### Getting Started

To get up and running just ```cd``` to the root where the ```.csproj``` file resides and run the following commands.

```
dotnet restore
dotnet run

```

### Examples

The app simulates a basic group based admin system. There are three groups users can be members of: ```users```, ```superusers``` and ```admins```. All users are members of the ```users``` group but not all are members of ```superusers``` and or ```admins``` and there are parts of the app that use the ```Authorize``` attribute with policy based access to filter access.

There are a series of logins to test out. The home page has a table of the available users and passwords and indicates if a user is a member of ```superusers``` and / or ```admins```. You can then run the series of tests below and see which sections you are able to view dependent upon login. All the tests make calls to the ```ExampleController``` class.


#### Cookies

Browse to ```http://localhost/cookie/auth``` and if you are logged in under any user you should see the following. 

```
Only authenticated cookie based requests from superusers receive this message.
```

Browse to ```http://localhost/cookie/superuser``` and if you are logged in under a user in either ```superusers``` or ```admins``` you should see the following.

```
Only authenticated cookie based requests from superusers receive this message.
```

Browse to ```http://localhost/cookie/admin``` and if you are logged in under a user in ```admins``` you should see the following.

```
Only authenticated cookie based requests from admins receive this message.
```

#### JWT

The first command gets a token which will be needed for subsequent requests. In the example below the user ```admin``` with the password ```password``` is used but there are other logins to test which will give different results.

```
curl -X POST http://localhost:5000/api/login -H "Content-Type: application/x-www-form-urlencoded" -d "Name=admin&Password=password"
```

The above command should result in something similar to the following (obviously the token value will be different).

```
 {"token":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbkBkb21haW4iLCJqdGkiOiJkNGM5MDE0Zi0zOGYxLTQ3NTItODU3YS03ZTc0YzU0MjY3ZDciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJhZG1pbnMiOiIxIiwic3VwZXJ1c2VycyI6IjIiLCJ1c2VycyI6IjMiLCJleHAiOjE1MDQzODk0NTEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC9hcGkvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwL2FwaS8ifQ.gFnf39Vj16vEmcI1HdwPajH6sRHusxtjZ2eh0Xc1cJs"}
 ```

The token value is then used with subsequent requests. For the examples below replace ```TOKEN_VALUE``` with whatever value is obtained from the step above.

```
curl http://localhost:5000/api/auth -H "Authorization: Bearer TOKEN_VALUE"
```

No matter what user your token is for you should see the following.

```
Only authenticated token based requests receive this message.
```

The following test will work for users in either ```superusers``` or ```admins```.

```
curl http://localhost:5000/api/superuser -H "Authorization: Bearer TOKEN_VALUE"
```

The above command should produce the following.

```
Only authenticated token based requests from superusers receive this message.
```

The following test will work for users in ```admins```.

```
curl http://localhost:5000/api/admin -H "Authorization: Bearer TOKEN_VALUE"
```

The above command should produce the following.

```
Only authenticated token based requests from admins receive this message.
```

The following example illustrates manipulating the JWT, the code is in the ```TokenInfo``` method of the ```ExampleController``` class.

```
curl http://localhost:5000/api/tokeninfo -H "Authorization: Bearer TOKEN_VALUE"
```

Any authenticated user should be presented with something similar to the following.

```

{"token":{"actor":null,"audiences":["http://localhost:5000/api/"],"claims":[{"issuer":"http://localhost:5000/api/","originalIssuer":"http://localhost:5000/api/","properties":{},"subject":null,"type":"sub","value":"admin@domain","valueType":"http://www.w3.org/2001/XMLSchema#string"},{"issuer":"http://localhost:5000/api/","originalIssuer":"http://localhost:5000/api/","properties":{},"subject":null,"type":"jti","value":"f85815ae-69c4-4fec-8553-bc1199e3cdce","valueType":"http://www.w3.org/2001/XMLSchema#string"},{"issuer":"http://localhost:5000/api/","originalIssuer":"http://localhost:5000/api/","properties":{},"subject":null,"type":"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name","value":"admin","valueType":"http://www.w3.org/2001/XMLSchema#string"},{"issuer":"http://localhost:5000/api/","originalIssuer":"http://localhost:5000/api/","properties":{},"subject":null,"type":"admins","value":"1","valueType":"http://www.w3.org/2001/XMLSchema#string"},{"issuer":"http://localhost:5000/api/","originalIssuer":"http://localhost:5000/api/","properties":{},"subject":null,"type":"superusers","value":"2","valueType":"http://www.w3.org/2001/XMLSchema#string"},{"issuer":"http://localhost:5000/api/","originalIssuer":"http://localhost:5000/api/","properties":{},"subject":null,"type":"users","value":"3","valueType":"http://www.w3.org/2001/XMLSchema#string"},{"issuer":"http://localhost:5000/api/","originalIssuer":"http://localhost:5000/api/","properties":{},"subject":null,"type":"exp","value":"1504392381","valueType":"http://www.w3.org/2001/XMLSchema#integer"},{"issuer":"http://localhost:5000/api/","originalIssuer":"http://localhost:5000/api/","properties":{},"subject":null,"type":"iss","value":"http://localhost:5000/api/","valueType":"http://www.w3.org/2001/XMLSchema#string"},{"issuer":"http://localhost:5000/api/","originalIssuer":"http://localhost:5000/api/","properties":{},"subject":null,"type":"aud","value":"http://localhost:5000/api/","valueType":"http://www.w3.org/2001/XMLSchema#string"}],"encodedHeader":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9","encodedPayload":"eyJzdWIiOiJhZG1pbkBkb21haW4iLCJqdGkiOiJmODU4MTVhZS02OWM0LTRmZWMtODU1My1iYzExOTllM2NkY2UiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJhZG1pbnMiOiIxIiwic3VwZXJ1c2VycyI6IjIiLCJ1c2VycyI6IjMiLCJleHAiOjE1MDQzOTIzODEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC9hcGkvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwL2FwaS8ifQ","header":{"alg":"HS256","typ":"JWT"},"id":"f85815ae-69c4-4fec-8553-bc1199e3cdce","issuer":"http://localhost:5000/api/","payload":{"sub":"admin@domain","jti":"f85815ae-69c4-4fec-8553-bc1199e3cdce","http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name":"admin","admins":"1","superusers":"2","users":"3","exp":1504392381,"iss":"http://localhost:5000/api/","aud":"http://localhost:5000/api/"},"innerToken":null,"rawAuthenticationTag":null,"rawCiphertext":null,"rawData":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJhZG1pbkBkb21haW4iLCJqdGkiOiJmODU4MTVhZS02OWM0LTRmZWMtODU1My1iYzExOTllM2NkY2UiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJhZG1pbnMiOiIxIiwic3VwZXJ1c2VycyI6IjIiLCJ1c2VycyI6IjMiLCJleHAiOjE1MDQzOTIzODEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC9hcGkvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwL2FwaS8ifQ.AXpvgAE3ZFN8EnRVSkLUt0iCaFTySFnMTfSx_kWYFDk","rawEncryptedKey":null,"rawInitializationVector":null,"rawHeader":"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9","rawPayload":"eyJzdWIiOiJhZG1pbkBkb21haW4iLCJqdGkiOiJmODU4MTVhZS02OWM0LTRmZWMtODU1My1iYzExOTllM2NkY2UiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJhZG1pbnMiOiIxIiwic3VwZXJ1c2VycyI6IjIiLCJ1c2VycyI6IjMiLCJleHAiOjE1MDQzOTIzODEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMC9hcGkvIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1MDAwL2FwaS8ifQ","rawSignature":"AXpvgAE3ZFN8EnRVSkLUt0iCaFTySFnMTfSx_kWYFDk","securityKey":null,"signatureAlgorithm":"HS256","signingCredentials":null,"encryptingCredentials":null,"signingKey":null,"subject":"admin@domain","validFrom":"0001-01-01T00:00:00","validTo":"2017-09-02T22:46:21Z"}}
```

#### Cookies and JWT

The address ```http://localhost/auth``` is available for both cookie and JWT based logins and the user should see the following message after execution.

```
Only authenticated requests receive this message.
```