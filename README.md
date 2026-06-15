# FoodHub Food Ordering System

## Project Structure

### FoodHub/

Main ASP.NET Core Web API project containing the application source code.

### Controllers/

Contains API controllers that handle HTTP requests and responses.

Files:

* AuthController.cs – User registration and login.
* RestaurantController.cs – Restaurant management endpoints.
* CategoryController.cs – Category management endpoints.
* MenuItemController.cs – Menu item operations.
* CartController.cs – Shopping cart operations.
* OrderController.cs – Order management functionality.

### Services/

Contains business logic for the application.

Files:

* AuthService.cs – Authentication and JWT generation.
* RestaurantService.cs – Restaurant business operations.
* CategoryService.cs – Category business operations.
* MenuItemService.cs – Menu item business operations.
* CartService.cs – Cart processing logic.
* OrderService.cs – Order creation and processing.

### Repositories/

Contains data access logic using Entity Framework Core.

Files:

* RestaurantRepository.cs
* CategoryRepository.cs
* MenuItemRepository.cs
* CartRepository.cs
* OrderRepository.cs

Repositories communicate directly with the database through ApplicationDbContext.

### Models/

Contains entity classes representing database tables.

Examples:

* ApplicationUser.cs
* Restaurant.cs
* Category.cs
* MenuItem.cs
* Cart.cs
* CartItem.cs
* Order.cs
* OrderItem.cs

### DTOs/

Contains Data Transfer Objects used for requests and responses between the API and clients.

### Data/

Contains database-related configuration.

Files:

* ApplicationDbContext.cs – Main Entity Framework database context.

### Profiles/

Contains AutoMapper configuration classes used for mapping entities and DTOs.

### Migrations/

Contains Entity Framework Core migration files used to create and update the database schema.

### FoodHub.Test/

Unit test project.

Contains:

* Controller tests
* Service tests
* Repository tests

### Program.cs

Application entry point and dependency injection configuration.

### appsettings.json

Application configuration file containing connection strings and JWT settings.

### Dockerfile

Docker configuration used for containerizing the application.

---

# Compile Instructions

Restore dependencies:

```bash
dotnet restore
```

Build the project:

```bash
dotnet build
```

---

# Database Setup

Apply Entity Framework migrations:

```bash
dotnet ef database update
```

Make sure the PostgreSQL connection string in appsettings.json is configured correctly before running migrations.

---

# Run Instructions

Start the application:

```bash
dotnet run
```

Swagger documentation will be available at:

```text
https://localhost:<port>/swagger
```

---

# Run Unit Tests

Execute all tests:

```bash
dotnet test
```
