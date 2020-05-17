# Forum
При первом запуске генерируется база данных, миграции и БД заполняется кастомнными данными.

## Пользователи
  - Созданна кастомная модель с дополнительными полями.
  - При регистрации - отправка письма для подтвержденния на электронную почту.
  - Есть три типа ролей - Админ, Модератор и Пользователь. При регистрации по дефолту дает роль Пользователь.
  - Админ может создать пользователя с ролью Админ и Модератор из Админ Панеле.
  - Пользователь может редактировать личные данные типа Имя, Фамилия, Номер телефона, Аватар.
  - Зарегистрированный пользователь может создавать вопросы и оставлять комментарии.

### Админ
  - При первой загрузке содается один админ.
```sh
login - delme@gmail.com
pass  - Admin123*
```
  - В футере для админа создана админ панель, где Админ может просматривать / создавать / удалять / обновлять данные о Вопросах, Категории и Пользователе.
  - В разделе Категорий используется CRUD. 
  - В разделе Вопросы используется CRUD. Для отображения используется DataTable , где по дефолту встроенная пагинация, сортировка, поиск.
  - В разделе Пользователи админ может блокировать / разблокировать пользователей, и создавать новых пользователей с ролями Админа / Модератора. Для отображения используется DataTable , где по дефолту встроенная пагинация, сортировка, поиск.

## Главная страница
  - ***Пагинация*** - по 6 вопросов на страницу. Переход на другую страницу происходит как с помощью бутстрап пагинатора внизу страницы, так и на напрямую через url.
```ssh
https://localhost:44314/?pageIndex=4
```
  - где pageIndex - номер страницы.
  - если выбранная страница не найдена, больше чем есть на самом деле, то перейдет на последнюю страницу.
==========================================================================================
  - **Фильтр по категориям** - вопросы можно фильтровать на 5 категориям. 
  - Админ может создавать новые категории.
  - Поддержка фильтра и пагинации.
```ssh
https://localhost:44314/?categoryId=1&pageIndex=2
```
  - где categoryId=1 - id категории, pageIndex=2 - номер страницы.
===========================================================================================
  - **Поиск вопросов по названию**.
  - Поддержка поисковика и пагинации.
```ssh
https://localhost:44314/?search=what&pageIndex=2
```
  - где search=what - критерии поиска, pageIndex=2 - номер страницы.

## Детализация Вопроса
  - Можно оставлять комментарии к вопросам - отвечать на данные коменты.


### Аутентификация
  - Только зарегистрированные пользователи могут создавать вопросы, комментарии.

### Права доступа
  - Редактировать вопросы и комменты может только Автор, Админ и модератор.
  - Проверка на соответствии прав происходит как со стороны HTML Кода, так и со стороны контроллера. 
  - Была создана проверочная функция, что зарегистрированный пользователь относиться либо к Админам, либо к Модератором, либо является Автором Вопроса / Комментария.
```sh
// access to edit have admin, moderator and post author
bool result = AccessRights.AuthorAdminAccessRight(HttpContext, postVM.Post.ApplicationUserId, _db);
if (result)
    return View(postVM);
return new RedirectResult("~/Identity/Account/AccessDenied");
```
  - В случае несоответствия - переход на страницу О запрете данного действия.


## Файл менеджмент.
При обновление картинки в вопросах или аватарки - старая фотка удаляется. При удалении вопроса также происходит удаленные картинок.
