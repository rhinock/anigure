# anigure

## Stack
- ASP.NET Core
- Bootstrap
- C#
- Elasticsearch
- Entity Framework Core
- MVC
- SQL Server
- React

## Description

A Web-app for personal collections managements (it's called items in the text below).

Unauthenticated users have read-only access (they can use search, however cannot create collections and items, create comments or likes).

Authenticated users have access to everything except "admin area".

Admin area give administators abilities to manage users (view, block, remove, add to/remove from admin role). Admin see every user page and collection as its creator (e.g. admin can edit collection or add an item from users page). 

Collection can be managed (edit/add/remove) only by the owner (creator) or admin.

App should support registration and authentication.

Every page provides access to full-text search over whole site (results are represented as item list, e.g., if some text is found in comment, the results page diplays link to the corresponding item page).

Every user has a personal page, which allow to manage list of own collections (add/remove/edit) and allow to open page dedicated to given collection (that page contains table with filters and sorting as well as actions to create/remove/edit item).

Each collection consists of: name, short description with markdown formatting, "topic" (from fixed set, e.g., Alcohol|Books|Cola Cans|...), optional image (stored in the cloud, upload with the help of drag-n-drop). Also collection allows to define custom fields, which will be filled for each item in this collection. There are 3 fixed fields (id, name, tags) but it's also possible to add dynamically something from the following - 3 number fields, 3 string fields, 3 multiline text fields, 3 dates, 3 boolean checkboxes. For example, the user may define that each item in his/her collection contains (in addition to id, name and tags) string field "Author", text field "Comment" and date field "Publication year". Multiline text fields should support markdown. Each custom filed should have name (which will be displayed in the item form). Custom fields are shown in the item list on the collection page (with the sorting and filtering support).

Each item has tags (user enters several tags with autocompletion, when user starts to enter tag, the dropdown is shown with the words entered on the site before by all users).

On the main page: last added items, biggest collections, clickable tags (when users click on the tag, he/she gets search result page - it can be the same view).

When item is opened for reading by author or opened by other user, there are comments at the bottom. Comments are linear, users don't put comments to other comments, only to the end of the comment list. Automatic fetching of new comments - if page is opened with comments and somebody wrote a new comment, it should appear on the page (it's possible to have 2-5 seconds delay).

Item has likes (no more than 1 from user per item).

App supports 2 languages: English and Russian - only UI is translated, not content. App supports two visual themes (skins) - dark and light. User may change language or theme and the choice is stored for the user.

Different resolution support (including phones).
