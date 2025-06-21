# Expense Tracker App

A simple and intuitive application to track your daily expenses, manage budgets, and visualize spending patterns.

## Features

- Add, edit, and delete expenses
- Categorize expenses
- View spending summaries and charts
- Set monthly budgets
- User authentication and authorization
- Responsive design for mobile and desktop
- Data persistence with SQL Server
- RESTful API for integration with other services
- Docker support for easy deployment

## API Documentation

The Expense Tracker App provides a RESTful API for managing expenses, categories, and budgets. The API is documented using Swagger/OpenAPI.

- **Swagger UI:**  
    After running the application, access the interactive API documentation at:  
    `http://localhost:5000/swagger` (or the port specified in your configuration)

- **OpenAPI Spec:**  
    The OpenAPI (Swagger) specification is available in `doc.json`.  
    You can use this file for client code generation or API exploration.

### Example Endpoints

- `GET /api/expenses` — List all expenses
- `POST /api/expenses` — Add a new expense
- `GET /api/categories` — List all categories
- `POST /api/categories` — Add a new category
- `GET /api/budgets` — Get budget details

Refer to the Swagger UI or `doc.json` for detailed request/response schemas and additional endpoints.

## Technologies Used

- .NET / C#
- ASP.NET Core
- Entity Framework Core
- SQL Server
- Docker
- Swagger/OpenAPI
- cqrs
- MediatR
- AutoMapper

## Getting Started

1. **Clone the repository:**
        ```bash
        git clone https://github.com/yourusername/PFE.ExpenseTracker.APP.git
        ```
2. **Navigate to the project directory:**
        ```bash
        cd PFE.ExpenseTracker.APP
        ```
3. **Set up the database:**  
        Update the connection string in `appsettings.json` and run migrations.

4. **Run the application:**
        ```bash
        dotnet run
        ```

5. **Alternatively, run with Docker Compose:**  
        Make sure you have [Docker](https://www.docker.com/) and [Docker Compose](https://docs.docker.com/compose/) installed, then run:
        ```bash
        docker compose up
        ```

## Installation Steps

1. Clone the repository
2. Build the solution
3. Set up Docker and SQL Server (see docker-compose.yml)
4. Install local tools:
   ```bash
   dotnet new tool-manifest --output src/PFE.ExpenseTracker.API
   cd src/PFE.ExpenseTracker.API && dotnet tool install dotnet-ef
   ```
5. Create and apply migrations (if not already present):
   ```bash
   dotnet tool run dotnet-ef migrations add InitialCreate --project ../PFE.ExpenseTracker.Infrastructure --startup-project .
   dotnet tool run dotnet-ef database update --project ../PFE.ExpenseTracker.Infrastructure --startup-project .
   ```
6. Run the application:
   ```bash
   docker-compose up --build
   ```

