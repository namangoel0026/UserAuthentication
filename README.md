# User Management API

## Overview

User Management API is a RESTful service that allows for the management of user accounts and roles. The API supports features such as user creation, updating user information, assigning roles, and soft deletion. It is built using ASP.NET Core and Entity Framework with support for localization in Hindi and English.

## Features

- **User Management**: Create, update, and delete users.
- **Role Management**: Assign multiple roles to users.
- **Soft Deletion**: Users can be marked as inactive instead of being deleted permanently.
- **Localization**: Supports both English and Hindi languages for messages.
- **JWT Authentication**: Secure access using JSON Web Tokens.

## Technologies Used

- ASP.NET Core
- Entity Framework Core
- Dapper for data access
- Microsoft SQL Server
- JWT for authentication
- Swagger for API documentation

## Installation

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version X.X)
- SQL Server (LocalDB or any version)
- Visual Studio or your preferred code editor

### Getting Started

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/yourusername/your-repo-name.git
   cd your-repo-name
2. **Configure the Database:**

Update the connection string in appsettings.json to point to your SQL Server instance.
3. **Run Migrations:**
Open a terminal and run the following command to apply database migrations:

>>dotnet ef database update

4.**Start the API:**
Run the application
5. **Test the API:**

You can use tools like Postman(collection present in the repo only) or Swagger UI to test the endpoints.


API Endpoints
Authentication:-
POST /api/Authentication/login
{
  "email": "admin@example.com",
  "password": "Admin"
}
Copy Token and use it in Authorization header with Prefix bearer.


User Management
Create User
POST /api/users
Request Body:
json
{
    "username": "test101",
    "email": "Test101@abc.com",
    "userroles": "2,3",
    "password": "Test1234",
    "isActive": true
}

Update User
PUT /api/users/

Get User
GET /api/users/{id}

Delete User (Soft Delete)
DELETE /api/users/{id}


Role Management
Create Role
POST /api/Role
Request Body:
json
{
  "name": "Test Role 3",
  "description": "Test Role 3",
  "isActive": true
}

Update Role
PUT /api/Role

Get Role
GET /api/Role/{id}

Delete Role (Soft Delete)
DELETE /api/Role/{id}
Localization
API responses are localized based on the Accept-Language header. You can specify hi-IN for Hindi or en-US for English.
