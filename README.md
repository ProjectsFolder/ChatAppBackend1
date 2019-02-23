## Регистрация пользователя
**URL:** `POST /api/users/register`
#### Данные запроса
- `FormData { login|minlength:3, password|minlength:3, image|file:5mb }`
#### Успешный ответ
- Code: `200 OK`
#### Ответ с ошибками
- Code: `400 BAD REQUEST { "field": ["error message"], ... }`
## Авторизация пользователя
**URL:** `POST /api/users/login`
#### Данные запроса
- `FormData { login|minlength:3, password|minlength:3 }`
#### Успешный ответ
- Code: `200 OK { token, userid, username }`
#### Ответ с ошибками
- Code: `403 FORBIDDEN`
- Code: `400 BAD REQUEST { "field": ["error message"], ... }`
## Проверка авторизации пользователя
**URL:** `GET /api/users/authorized`
#### Данные запроса
- `Headers { "Authorization": "token" }`
#### Успешный ответ
- Code: `200 OK`
#### Ответ с ошибками
- Code: `403 FORBIDDEN`
## Выход пользователя
**URL:** `POST /api/users/logout`
#### Данные запроса
- `Headers { "Authorization": "token" }`
#### Успешный ответ
- Code: `200 OK`
#### Ответ с ошибками
- Code: `400 BAD REQUEST`
## Выход пользователя со всех устройств
**URL:** `POST /api/users/logout_all`
#### Данные запроса
- `Headers { "Authorization": "token" }`
#### Успешный ответ
- Code: `200 OK`
#### Ответ с ошибками
- Code: `400 BAD REQUEST`
## Получение изображения пользователя
**URL:** `GET /api/users/image/{id}`
#### Данные запроса
- `Headers { "Authorization": "token" }`
#### Успешный ответ
- Code: `200 OK`
- Code: `204 NO CONTENT`
#### Ответ с ошибками
- Code: `403 FORBIDDEN`
## Изменение изображения пользователя
**URL:** `PUT /api/users/image`
#### Данные запроса
- `Headers { "Authorization": "token" }`
- `FormData { image|file:5mb }`
#### Успешный ответ
- Code: `200 OK`
#### Ответ с ошибками
- Code: `400 BAD REQUEST`
- Code: `403 FORBIDDEN`
- Code: `404 NOT FOUND`
## Получение списка сообщений
**URL:** `GET /api/messages`
#### Данные запроса
- `Headers { "Authorization": "token" }`
#### Успешный ответ
- Code: `200 OK [ { id, text, userid, username, timecreated }, ... ]`
#### Ответ с ошибками
- Code: `403 FORBIDDEN`
## Получение одного сообщения
**URL:** `GET /api/messages/{id}`
#### Данные запроса
- `Headers { "Authorization": "token" }`
#### Успешный ответ
- Code: `200 OK { id, text, userid, username, timecreated }`
#### Ответ с ошибками
- Code: `403 FORBIDDEN`
- Code: `404 NOT FOUND`
## Отправка сообщения
**URL:** `POST /api/messages`
#### Данные запроса
- `Headers { "Authorization": "token" }`
- `FormData { text|maxlength:1000 }`
#### Успешный ответ
- Code: `200 OK`
#### Ответ с ошибками
- Code: `400 BAD REQUEST { "field": ["error message"], ... }`
- Code: `403 FORBIDDEN`
## Обновление сообщения
**URL:** `PUT /api/messages/{id}`
#### Данные запроса
- `Headers { "Authorization": "token" }`
- `FormData { text|maxlength:1000 }`
#### Успешный ответ
- Code: `200 OK`
#### Ответ с ошибками
- Code: `400 BAD REQUEST { "field": ["error message"], ... }`
- Code: `403 FORBIDDEN`
- Code: `404 NOT FOUND`
## Удаление сообщения
**URL:** `DELETE /api/messages/{id}`
#### Данные запроса
- `Headers { "Authorization": "token" }`
#### Успешный ответ
- Code: `200 OK`
#### Ответ с ошибками
- Code: `403 FORBIDDEN`
- Code: `404 NOT FOUND`
