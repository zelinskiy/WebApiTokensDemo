1) Our API:

**Token**
[open] POST /api/token
returns token for given username/password pair
{
  UserName:"user@mail.ru",
  Password:"Qwerty1]"
}
**Calculator**
[open] GET /api/calculator/add/4/8

[closed] GET /api/calculator/mult/3/5

**Account Manager**

[open] GET /api/account/register/user@mail.ru/Qwerty1]
register new user

[closed] GET /api/account/authenticated
is user authenticated?

[open] GET /api/account/allusers/
list of all users

[closed] GET /api/account/claims/
current user's claims


2) Fiddler http requests examples:
=====================================
Step 0 - Register user
=====================================
GET http://localhost:5000/api/account/register/user@mail.ru/Qwerty1]
Host: localhost:5000
Content-Length: 51
=====================================
Step 1 - Request token for a registered user
=====================================
POST http://localhost:5000/token HTTP/1.1
Content-Type: application/json
Host: localhost:5000
Content-Length: 51

{
  UserName:"user@mail.ru",
  Password:"Qwerty1]"
}
=====================================

If succeed, we will get our token:
++++++++++++++++++++++++++++++++++++++
{
  "access_token": "eyJhbGciOiJI.......",
  "expires_in": 1800,
  "valid_to":"30.07.2016 22:53:07"
}
++++++++++++++++++++++++++++++++++++++

Шаг 2 - Now we can multiply numbers by accessing the closed controller
=========================
GET http://localhost:5000/api/calculator/mult/2/2 HTTP/1.1
Host: localhost:5000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR.........
X-Requested-With: XMLHttpRequest

========================
