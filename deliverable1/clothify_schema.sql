-- Create Database
CREATE DATABASE Clothify;
USE Clothify;

-- Customer Table with UserRole (Customer/Admin)
CREATE TABLE Customer
(
    Customer_ID INT PRIMARY KEY,
    First_name VARCHAR(50) NOT NULL,
    Last_name VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Address VARCHAR(200) NOT NULL,
    PhoneNumber VARCHAR(11) NOT NULL,
    DateOfBirth DATE NOT NULL,
    UserRole ENUM('Admin', 'Customer') DEFAULT 'Customer' -- Added to distinguish Customer/Admin
);

-- Payment_methods Table
CREATE TABLE Payment_methods
(
     Payment_type_ID INT PRIMARY KEY,
     payment_method_name VARCHAR(50) NOT NULL
);

-- Payment Table
CREATE TABLE Payment
(
    PaymentID INT PRIMARY KEY,
    CustomerID INT NOT NULL,
    PaymentType INT NOT NULL, 
    CardholderName VARCHAR(100),
    CardNumber VARCHAR(16),
    ExpiryDate DATE,
    CVV VARCHAR(4), -- card verification value (unique no. at the back of credit card)
    FOREIGN KEY (CustomerID) REFERENCES Customer(Customer_ID),
    FOREIGN KEY (PaymentType) REFERENCES Payment_methods(Payment_type_ID)
);

-- Category Table
CREATE TABLE Category 
(
  CategoryID INT PRIMARY KEY,
  CategoryName VARCHAR(50) NOT NULL
);

-- Product Table with AverageRating
CREATE TABLE Product 
(
  ProductID INT PRIMARY KEY,
  ProductName VARCHAR(100) NOT NULL,
  Description VARCHAR(500),
  ImageURL VARCHAR(200) NOT NULL,
  CategoryID INT NOT NULL,
  Price DECIMAL(12, 2) NOT NULL,
  StockCount INT NOT NULL,
  AverageRating DECIMAL(3, 2) DEFAULT 0, -- Added to store product average rating
  FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);

-- Orders Table
CREATE TABLE Orders
(
    OrderID INT PRIMARY KEY,
    CustomerID INT NOT NULL,
    OrderDate DATE NOT NULL,
    ShippingAddress VARCHAR(200) NOT NULL,
    TotalPrice DECIMAL(12, 2) NOT NULL,
    payment_id INT NOT NULL,
    FOREIGN KEY (CustomerID) REFERENCES Customer(Customer_ID),
    FOREIGN KEY (payment_id) REFERENCES Payment(PaymentID)
);

-- OrderItem Table
CREATE TABLE OrderItem 
(
  OrderItemID INT PRIMARY KEY,
  OrderID INT NOT NULL,
  ProductID INT NOT NULL,
  Quantity INT NOT NULL,
  Price DECIMAL(10, 2) NOT NULL,
  FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
  FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);

-- Ratings Table
CREATE TABLE Ratings
(
     Rating_ID INT PRIMARY KEY,
     Remarks VARCHAR(40)
);

-- Review Table
CREATE TABLE Review 
(
  ReviewID INT PRIMARY KEY,
  CustomerID INT NOT NULL,
  ProductID INT NOT NULL,
  Rating INT NOT NULL,
  Comment VARCHAR(500),
  ReviewDate DATE NOT NULL,
  FOREIGN KEY (CustomerID) REFERENCES Customer(Customer_ID),
  FOREIGN KEY (ProductID) REFERENCES Product(ProductID),
  FOREIGN KEY (Rating) REFERENCES Ratings(Rating_ID)
);

-- Discounts Table for sales functionality
CREATE TABLE Discounts 
(
    DiscountID INT PRIMARY KEY,
    ProductID INT,
    CategoryID INT,
    DiscountPercentage DECIMAL(5, 2),
    StartDate DATE,
    EndDate DATE,
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID),
    FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
);

-- trigger to update stock after an order is placed
DELIMITER $$
CREATE TRIGGER update_stock_after_order
AFTER INSERT ON OrderItem
FOR EACH ROW
BEGIN
    UPDATE Product 
    SET StockCount = StockCount - NEW.Quantity 
    WHERE ProductID = NEW.ProductID;
END$$
DELIMITER ;


