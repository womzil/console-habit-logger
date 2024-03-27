# Console Habit Logger
This is a very simple habit logger made in C# using Visual Studio 2022. It uses ADO.NET to execute querries in an SQLite database.

## Features
* Basic features
  - Application includes a basic interface which can be navigated using arrows and other keys for specific operations
  - It's possible to create multiple habits
  - Every habit has its own unit chosen by the user
  - Every record gets its date of creation
  - It's possible to add, remove and edit existing habits and their records
* Database
  - An SQLite database is created on start-up if one isn't present
  - Every habit is stored in its own table in the database

## Requirements
- [x] The application should store and retrieve data from a real database
- [x] When the application starts, it should create a sqlite database, if one isn’t present
- [x] It should also create a table in the database, where the habit will be logged
- [x] The app should show the user a menu of options
- [x] The users should be able to insert, delete, update and view their logged habit
- [ ] The application handles all possible errors so that it never crashes
- [x] The application interacts with the database using raw SQL. It doesn't use mappers such as Entity Framework
- [x] The users should be able to create their own habits to track. That will require them to choose the unit of measurement of each habit
- [x] Add an ability to generate some data automatically for debugging
- [ ] There should be a report functionality where the users can view specific information (i.e. how many times the user ran in a year? how many kms?)
- [x] Separate classes should be in different files
