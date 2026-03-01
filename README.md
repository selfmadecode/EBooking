## SETUP INSTRUCTIONS

## This project has two parts:
Backend API → .NET 8 + SQL Server
Postman collection can be found [here](https://github.com/selfmadecode/EBooking/blob/master/EBooking.postman_collection.json)

Frontend → React

## install the following please
  - .NET 8 SDK          
  - SQL Server (MSSQL)  
  - Node.js v18+        
  - Visual Studio

## Backend API Setup
In Visual Studio, open the Package Manager Console:
go to tools > NuGet Package Manager > Package Manager Console
In the console, set the Default Project dropdown to:
    EBooking.Infrastructure

Then run this command:

    Update-Database

This will creates the database and all tables automatically.

## Run the API
Set EBooking.API as the startup project:
  Right-click EBooking.API in Solution Explorer
  > Set as Startup Project
  then press f5 to run the API


# FRONTEND
    Open a terminal in the frontend folder
Run this command

    npm install
Then Run:

    npm start

The browser will open at http://localhost:3000 automatically.
Go to http://localhost:3000

### Test Login Credentials
You can log in using the accounts below:

Normal User

Email: appuser@ebookingapi.com  
Password: appUser@12345

### Admin User

admin@ebookingapi.com
Password: appAdmin@12345

## What Users Can Do
### Normal Users can:
Top up wallet
View events
Buy tickets

### Admin users can:
Create events
Manage events
Manage tickets
Only admins can create and manage events.


