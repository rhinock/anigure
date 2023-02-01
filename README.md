# anigure

## Stack
- ASP.NET Core
- C#
- Entity Framework Core
- MVC
- SQL Server
- React

## Description

- A Web-app for personal collections managements
- Nonauthenticated users 
  - have read-only access
    - can use search
    - cannot create
      - collections
      - items
      - comments
      - likes
- Authenticated users have access to everythng except "admin area"
- Admin area give administators abilities to manage users
  - view
  - block
  - remove
  - add to admin role
  - remove from admin role
- Admin see every user page and collection as its creator 
  - admin can 
    - edit collection
    - add an item from users page
- Collection can be managed (edit/add/remove) only by
  - the owner (creator) 
  - admin
- App supports registration and authentication
- Every page provides access to full-text search over whole site 
  - results are represented as item list
    - if some text is found in comment, the results page diplays link to the corresponding item page
- Every user has a personal page, which allow  
  - to manage list of own collections (add/remove/edit)
  - to open page dedicated to given collection 
    - that page contains table with filters and sorting as well as actions to cretae/remove/edit item
