# simple_aspnet_auth

Simple ASP.NET Authorisation boilerplate project. No EF, no database, no IdentityServer4 just a basic logging in system for both cookies and JWT with a couple of authorisation policies and a controller with a set of examples.

### Examples

curl -X POST http://localhost:5000/api/login -H "Content-Type: application/x-www-form-urlencoded" -d "Name=admin&Password=password"

token=XXXXXXXXXXXXXXXXXXXX

curl http://localhost:5000/api/auth -H "Authorization: Bearer $token"
curl http://localhost:5000/api/superuser -H "Authorization: Bearer $token"
curl http://localhost:5000/api/admin -H "Authorization: Bearer $token"
curl http://localhost:5000/api/tokeninfo -H "Authorization: Bearer $token"