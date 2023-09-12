* Dependencies/Package
1) Microsoft.AspNet.Mvc
2) Microsoft.AspNetCore.Authentication.JwtBearer
3) Microsoft.EntityFrameworkCore.Design
4) Microsoft.EntityFrameworkCore.SqlServer
5) Microsoft.EntityFrameworkCore.Tools

* Changes for database connection
{
  "ConnectionStrings": {
    "DefaultConnection": "..."
  }, 
in appsettings.json for database connection

* API
1) Todos 
     * PUT -> /api/Todo/{todoId} for updating todo status as (active,pending,completed)
    Note: All API need Authorization Token
2) User 
     1) api/user create/register user
     2) api/user/login for login which create token for 30sec and also
	create refresh token for 10hr 10min that store in the database
     3) api/user/refreshtokencheck/{id} check for refreshtoken in the database
	by userid;
     4) api/user/logout for logout (required Authorization Token)


