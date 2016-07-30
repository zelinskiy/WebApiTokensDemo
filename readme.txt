1) Какое у нас апи:

**Middleware**
POST /api/token
примет логин и пароль и вернет токен
(если логин и пароль есть в базе)

**Калькулятор**
GET /api/calculator/add/4/8
открытый контроллер, используют все желающие
вернет сумму

GET /api/calculator/mult/3/5
закрытый контроллер, работает только если передан верный токен
возвращает произведение

**Аккаунт менеджер**

[open] GET /api/account/register/user@mail.ru/Qwerty1]
регистрирует пользователей и показывает ошибки регистрации

[closed] GET /api/account/authenticated
залогинен ли юзер?

[open] GET /api/account/allusers/
список пользователей

[closed] GET /api/account/claims/
список утверждений текущего пользователя


2) Примеры запросов для фиддлера:

Шаг 1 - Посылаем запрос с логином и паролем
(TEST - тестовый юзер, у него нет айдентити)
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

Ответ получаем в виде:
++++++++++++++++++++++++++++++++++++++
{
  "access_token": "eyJhbGciOiJI.......",
  "expires_in": 1800,
  "valid_to":"30.07.2016 22:53:07"
}
++++++++++++++++++++++++++++++++++++++

Шаг 2 - Можем послать запрос на умножение, подставив свой токен
=========================
GET http://localhost:5000/api/calculator/mult/2/2 HTTP/1.1
Host: localhost:5000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR.........
X-Requested-With: XMLHttpRequest

========================
