1) Какое у нас апи:

**Middleware**
POST /token
примет логин и пароль и вернет токен
(если логин и пароль есть в базе)

**Калькулятор**
GET /calculator/add/4/8
открытый контроллер, используют все желающие
вернет сумму

GET /calculator/mult/3/5
закрытый контроллер, работает только если передан верный токен
возвращает произведение

**Аккаунт менеджер**

[open] GET /account/register/user@mail.ru/Qwerty1]
регистрирует пользователей и показывает ошибки регистрации

[closed] GET /account/authenticated
залогинен ли юзер?

[open] GET /account/allusers/
список пользователей

[closed] GET /account/claims/
список утверждений текущего пользователя


2) Примеры запросов для фиддлера:

Шаг 1 - Посылаем запрос с логином и паролем
(TEST - тестовый юзер, у него нет айдентити)
=====================================
POST http://localhost:5000/token HTTP/1.1
Content-Type: application/x-www-form-urlencoded
Host: localhost:5000
Content-Length: 30

username=TEST&password=TEST123
=====================================

Ответ получаем в виде:
++++++++++++++++++++++++++++++++++++++
{
  "access_token": "eyJhbGciOiJI.......",
  "expires_in": 1800
}
++++++++++++++++++++++++++++++++++++++

Шаг 2 - Можем послать запрос на умножение, подставив свой токен
=========================
GET http://localhost:5000/api/calculator/mult/2/2 HTTP/1.1
Host: localhost:5000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR.........
X-Requested-With: XMLHttpRequest

========================
