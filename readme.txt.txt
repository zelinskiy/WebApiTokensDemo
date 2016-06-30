1)  акое у нас апи:
POST /token
примет логин и пароль и вернет токен
(если логин и пароль есть в базе)

GET /calculator/add/4/8
открытый контроллер, используют все желающие
вернет сумму

GET /calculator/mult/3/5
закрытый контроллер, работает только если передан верный токен
возвращает произведение

2) ѕримеры запросов дл€ фиддлера:

Ўаг 1 - ѕосылаем запрос с логином и паролем
=====================================
POST http://localhost:5000/token HTTP/1.1
Content-Type: application/x-www-form-urlencoded
Host: localhost:5000
Content-Length: 30

username=TEST&password=TEST123
=====================================

ќтвет получаем в виде:
++++++++++++++++++++++++++++++++++++++
{
  "access_token": "eyJhbGciOiJI.......",
  "expires_in": 1800
}
++++++++++++++++++++++++++++++++++++++

Ўаг 2 - ћожем послать запрос на умножение, подставив свой токен
=========================
GET http://localhost:5000/api/calculator/mult/2/2 HTTP/1.1
Host: localhost:5000
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR.........
X-Requested-With: XMLHttpRequest

========================