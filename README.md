### 📌 **ElectroMart Backend**
A powerful and secure backend for the ElectroMart e-commerce platform, built with **ASP.NET Core**, **JWT Authentication**, and **Entity Framework Core**.

---

## 🚀 Features  
✅ **User Authentication & Authorization** (JWT, ASP.NET Core Identity)  
✅ **Product & Category Management**  
✅ **Shopping Cart & Order Processing**  
✅ **Secure API Endpoints** (Token-based authentication)  
✅ **Admin Panel Features** (User management, product control)  

---

## 🏗️ Tech Stack  
- **Backend:** ASP.NET Core 8.0  
- **Database:** Microsoft SQL Server + Entity Framework Core  
- **Authentication:** ASP.NET Core Identity + JWT  
- **Security:** Token-based authentication, role-based authorization  

---

## 📦 Installation  

### 🔹 Prerequisites  
- .NET SDK (8.0 or later)  
- Microsoft SQL Server  
- Visual Studio / VS Code  

### 🔹 Clone the Repository  
```bash
git clone https://github.com/yanyazh/ElectroMart.git
cd electromart
```

### 🔹 Configure Database  
1. Update **appsettings.json** with your database connection string:  
   ```json
   "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER;Database=ElectroMartDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
   }
   ```
2. Apply migrations and seed the database:  
   ```bash
   dotnet ef database update
   ```

### 🔹 Run the Application  
```bash
dotnet run
```

---

## 🔐 Authentication & Security  
- **JWT-based Authentication**: Secure login, token generation, and protected endpoints  
- **Role-based Authorization**: Admins can manage users, products, and orders  
- **Data Encryption**: Passwords securely stored using ASP.NET Core Identity  

---

## 🔗 API Endpoints  

### 🔹 Account API  
| Method | Endpoint                               | Description                  |
|--------|----------------------------------------|------------------------------|
| POST   | `/api/AccountApi/register`            | Register new user            |
| POST   | `/api/AccountApi/login`               | Login & get JWT token        |
| POST   | `/api/AccountApi/logout`              | Logout user                  |
| GET    | `/api/AccountApi/userdata`            | Get logged-in user data      |
| DELETE | `/api/AccountApi/deleteAccount`       | Delete own account           |
| DELETE | `/api/AccountApi/deleteAccount/{userId}` | Admin: Delete user by ID |

### 🔹 Cart API  
| Method | Endpoint                                  | Description                  |
|--------|-------------------------------------------|------------------------------|
| POST   | `/api/CartApi/AddToCart`                 | Add an item to the cart      |
| GET    | `/api/CartApi/GetCart`                   | Retrieve user’s cart         |
| DELETE | `/api/CartApi/RemoveFromCart`            | Remove item from cart        |
| DELETE | `/api/CartApi/RemoveFromCartByProductId` | Remove item by product ID    |

### 🔹 Categories API  
| Method | Endpoint                        | Description                  |
|--------|---------------------------------|------------------------------|
| GET    | `/api/CategoriesApi`           | Get all categories           |
| POST   | `/api/CategoriesApi`           | Create a new category        |
| GET    | `/api/CategoriesApi/{id}`      | Get category by ID           |
| PUT    | `/api/CategoriesApi/{id}`      | Update category by ID        |
| DELETE | `/api/CategoriesApi/{id}`      | Delete category by ID        |

### 🔹 News API  
| Method | Endpoint                  | Description                  |
|--------|---------------------------|------------------------------|
| GET    | `/api/NewsApi`            | Get all news articles        |
| POST   | `/api/NewsApi`            | Create a news article        |
| GET    | `/api/NewsApi/{id}`       | Get news article by ID       |
| PUT    | `/api/NewsApi/{id}`       | Update news article by ID    |
| DELETE | `/api/NewsApi/{id}`       | Delete news article by ID    |

### 🔹 Order API  
| Method | Endpoint                          | Description                  |
|--------|-----------------------------------|------------------------------|
| POST   | `/api/OrderApi/CreateOrder`      | Create a new order           |
| GET    | `/api/OrderApi/GetOrders`        | Get orders for user          |

### 🔹 Products API  
| Method | Endpoint                     | Description                  |
|--------|------------------------------|------------------------------|
| GET    | `/api/ProductsApi`          | Get all products             |
| POST   | `/api/ProductsApi`          | Add a new product            |
| GET    | `/api/ProductsApi/{id}`     | Get product by ID            |
| PUT    | `/api/ProductsApi/{id}`     | Update product by ID         |
| DELETE | `/api/ProductsApi/{id}`     | Delete product by ID         |

---

## ⚙️ Environment Variables  
Set these variables in your **.env** file or system environment:  
```plaintext
JWT_SECRET=your_secret_key
DB_CONNECTION=your_database_connection_string
```
