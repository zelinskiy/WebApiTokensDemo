1) Какое у нас апи:
POST /token
примет логин и пароль и вернет токен
(если логин и пароль есть в базе)

GET /calculator/add/4/8
открытый контроллер, используют все желающие
вернет сумму

GET /calculator/mult/3/5
закрытый контроллер, работает только если передан верный токен
возвращает произведение

2) Примеры запросов для фиддлера:

Шаг 1 - Посылаем запрос с логином и паролем
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
