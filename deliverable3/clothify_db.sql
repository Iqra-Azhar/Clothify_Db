----------------------------------------------------------SCHEMA----------------------------------------------------------------------------------------

-- Create Database
CREATE DATABASE clothify;
USE clothify;


-- Customer Table with UserRole (Customer/Admin)
CREATE TABLE Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(50) NOT NULL,
    Role NVARCHAR(20) NOT NULL
);

-- Insert Admin record
INSERT INTO Users (Username, Password, Role)
VALUES ('admin', 'admin123', 'Admin');

-- Insert User record
INSERT INTO Users (Username, Password, Role)
VALUES ('user', 'user123', 'User');


CREATE TABLE Customer
(
    Customer_ID INT PRIMARY KEY,
    First_name VARCHAR(50) NOT NULL,
    Last_name VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL,
    Address VARCHAR(200) NOT NULL,
    PhoneNumber VARCHAR(11) NOT NULL,
    DateOfBirth DATE NOT NULL,
    UserRole VARCHAR(10) NOT NULL DEFAULT 'Customer',
    CONSTRAINT chk_UserRole CHECK (UserRole IN ('Admin', 'Customer'))
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

CREATE TABLE Cart (
    CartID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL DEFAULT 1,
    FOREIGN KEY (UserID) REFERENCES Users(UserId),
    FOREIGN KEY (ProductID) REFERENCES Product(ProductID)
);




/* Discounts Table for sales functionality
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
);*/

-- Trigger to update stock after an order is placed  ...actually there is no error , the red line apppears because we have to run trigger before the insertion commands
CREATE TRIGGER update_stock_after_order
ON OrderItem
AFTER INSERT
AS
BEGIN
    UPDATE p
    SET p.StockCount = p.StockCount - i.Quantity
    FROM Product p
    JOIN inserted i ON p.ProductID = i.ProductID;
END;

----------------------------------------------------------DATA INSERTION----------------------------------------------------------------------------------------



-- Inserting into the Customers table
INSERT INTO Customer (Customer_ID, First_name, Last_name, Email, Address, PhoneNumber, DateOfBirth)
VALUES 
(1, 'Ahmad', 'Iyaz', 'AMD@gmail.com', 'Hunza Block near jamia Masjid, Lahore', '03001234567', '2003-02-17'),
(2, 'Suleiman', 'Asif', 'SuleimanA228@gamil.com', 'Baharia Town Sector B, Lahore', '03212312744', '2001-03-23'),
(3, 'Daem', 'Azeem', 'DaemBuilder11@outlook.com', 'Khayaban e Amin, Block G, Defense Road, Lahore', '03334556780', '2002-04-22'),
(4, 'Saleh', 'Ahmad', 'SalehXYZ@outlook.com', 'Ravi Block Allama Iqbal Town, Lahore', '03219787756', '2003-09-03'),
(5, 'Sohaib', 'Fiaz', 'SohaibF222@gmail.com', 'Tariq Block Allama Iqbal Town, Lahore', '03337779125', '2003-09-09'),
(6, 'Sift', 'Ullah', 'SiftullahPro939@outlook.com', 'Baharia Town Sector D, Lahore', '03210099954', '2003-02-06'),
(7, 'Emaan', 'Butt', 'EmaanButt332@gmail.com', 'Askari11, Lahore', '03316667912', '1999-09-03');



-- Inserting into Payment_methods table
INSERT INTO Payment_methods VALUES (1, 'Debit Card');
INSERT INTO Payment_methods VALUES (2, 'Credit card');
INSERT INTO Payment_methods VALUES (3, 'Cash on Delivery');


-- Inserting into Payment table
INSERT INTO Payment VALUES (1, 1, 2, 'Ahmad Iyaz Butt', '1111666789999111', '02-07-2028', '4641');
INSERT INTO Payment VALUES (2, 6, 1, 'Siftullah', '1233904487771239', '03-07-2026', '5454');
INSERT INTO Payment VALUES (3, 2, 2, 'Rana Suleiman Asif', '1344974728283930', '08-09-2028', '5520');
INSERT INTO Payment VALUES (4, 1, 1, 'Ahmad Iyaz Butt', '1111666789999111', '10-04-2029', '8541');
INSERT INTO Payment VALUES (5, 4, 2, 'Saleh Ahmad', '1775900937228888', '10-10-2030', '7432');
INSERT INTO Payment VALUES (6, 7, 1, 'Emaan Butt', '1998181830098881', '10-09-2028', '5066');
INSERT INTO Payment VALUES (7, 5, 2, 'Sohaib Fiaz', '1449225598760169', '01-09-2028', '7964');
INSERT INTO Payment VALUES (8, 3, 1, 'Deam Azeem', '1232999923456006', '10-26-2026', '3258');
INSERT INTO Payment VALUES (9, 7, 1, 'Emaan Butt', '1771282765631919', '10-09-2028', '5066');
INSERT INTO Payment VALUES (10, 1, 3, NULL, NULL, NULL, NULL);



-- Inserting into Category table
INSERT INTO Category VALUES (1, 'Men wear');
INSERT INTO Category VALUES (2, 'Women wear');



-- Inserting into Product table
INSERT INTO Product (ProductID, ProductName, Description, ImageURL, CategoryID, Price, StockCount) 
VALUES 
(1, 'Slim Fit Denim Jacket', 'A stylish slim-fit denim jacket perfect for casual wear.', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcR0-Jh4Pas3HCNh9pjwQ2-q8_g8dY-pyH33_A&s', 1, 1499.99, 120),
(2, 'Leather Bomber Jacket', 'High-quality leather bomber jacket for a sleek look.', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSb1hseaAl7ClhKgNege9ZTSqt_ppgBwjzqow&s', 1, 1999.99, 80),
(3, 'Casual Chinos', 'Comfortable chinos perfect for a day out or office wear.', 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSWLi4Pk_pkZWHCzCiE1_PDT3W1In-20_ATxWNdRJ96g4CKHpvsORIddSEUNe1Fbb8Z2ss&usqp=CAU', 1, 899.99, 150),
(4, 'White Dress Shirt', 'Classic white dress shirt for formal occasions.', 'https://e7.pngegg.com/pngimages/328/492/png-clipart-white-dress-shirt-blouse-white-dress-shirt-formal-wear-a-white-shirt-tshirt-black-white.png', 1, 1199.99, 100),
(5, 'Black Turtleneck Sweater', 'Warm and stylish turtleneck sweater for the winter season.', 'https://via.placeholder.com/300x400.png?text=Black+Turtleneck+Sweater', 1, 899.99, 110),
(6, 'Track Pants', 'Perfect track pants for workout or casual wear.', 'https://via.placeholder.com/300x400.png?text=Track+Pants', 1, 699.99, 200),
(7, 'Stylish Hoodie', 'A comfortable hoodie with a modern design.', 'https://via.placeholder.com/300x400.png?text=Stylish+Hoodie', 1, 1099.99, 130),
(8, 'Business Suit', 'A professional business suit for important meetings.', 'https://via.placeholder.com/300x400.png?text=Business+Suit', 1, 2499.99, 60),
(9, 'Flannel Shirt', 'Warm flannel shirt with a checkered pattern for casual days.', 'https://via.placeholder.com/300x400.png?text=Flannel+Shirt', 1, 799.99, 140),
(10, 'Cargo Shorts', 'Comfortable and stylish cargo shorts for the summer.', 'https://via.placeholder.com/300x400.png?text=Cargo+Shorts', 1, 599.99, 180);

INSERT INTO Product (ProductID, ProductName, Description, ImageURL, CategoryID, Price, StockCount) 
VALUES 
(11, 'Floral Summer Dress', 'A beautiful floral dress perfect for warm weather.', 'https://via.placeholder.com/300x400.png?text=Floral+Summer+Dress', 2, 1299.99, 150),
(12, 'Black Leather Handbag', 'Elegant black leather handbag to complete your outfit.', 'https://via.placeholder.com/300x400.png?text=Black+Leather+Handbag', 2, 1899.99, 70),
(13, 'Summer Maxi Dress', 'Comfortable maxi dress ideal for a sunny day out.', 'https://via.placeholder.com/300x400.png?text=Summer+Maxi+Dress', 2, 1599.99, 120),
(14, 'Cotton Blouse', 'A soft cotton blouse perfect for any occasion.', 'https://via.placeholder.com/300x400.png?text=Cotton+Blouse', 2, 799.99, 140),
(15, 'High Waist Skinny Jeans', 'Stylish high waist jeans with a perfect fit.', 'https://via.placeholder.com/300x400.png?text=High+Waist+Skinny+Jeans', 2, 999.99, 160),
(16, 'V-Neck Sweater', 'Cozy V-neck sweater for the chilly days.', 'https://via.placeholder.com/300x400.png?text=V-Neck+Sweater', 2, 1199.99, 130),
(17, 'Red Party Dress', 'Perfect red dress for a night out or party.', 'https://via.placeholder.com/300x400.png?text=Red+Party+Dress', 2, 1699.99, 110),
(18, 'Bootcut Jeans', 'Comfortable bootcut jeans for everyday wear.', 'https://via.placeholder.com/300x400.png?text=Bootcut+Jeans', 2, 849.99, 180),
(19, 'Striped Summer Top', 'Light and breezy striped top for the summer season.', 'https://via.placeholder.com/300x400.png?text=Striped+Summer+Top', 2, 699.99, 160),
(20, 'Chiffon Blouse', 'Elegant chiffon blouse for a professional look.', 'https://via.placeholder.com/300x400.png?text=Chiffon+Blouse', 2, 1099.99, 140);



-- Inserting into Ratings table
INSERT INTO Ratings VALUES (1, 'Worst');
INSERT INTO Ratings VALUES (2, 'Bad');
INSERT INTO Ratings VALUES (3, 'OK');
INSERT INTO Ratings VALUES (4, 'Good');
INSERT INTO Ratings VALUES (5, 'Best');



-- Inserting Customer Reviews for Men’s Products
INSERT INTO Review (ReviewID, CustomerID, ProductID, Rating, Comment, ReviewDate) 
VALUES
(1, 1, 1, 5, 'Great quality denim jacket. Fits well and looks stylish!', '2024-11-27'),
(2, 2, 2, 4, 'Nice leather bomber jacket, but the fit is a bit tight around the shoulders.', '2024-11-27'),
(3, 3, 3, 3, 'Chinos are good, but the fabric is not as soft as expected.', '2024-11-27'),
(4, 4, 4, 5, 'Love the white dress shirt. Very comfortable for formal events.', '2024-11-27'),
(5, 5, 5, 4, 'The turtleneck sweater is warm, but I wish it was slightly more stretchy.', '2024-11-27'),
(6, 6, 6, 5, 'Perfect for workouts and casual wear. Great fit!', '2024-11-27'),
(7, 7, 7, 4, 'The hoodie is stylish but a little thick for the warmer months.', '2024-11-27');

-- Inserting Customer Reviews for Women’s Products
INSERT INTO Review (ReviewID, CustomerID, ProductID, Rating, Comment, ReviewDate) 
VALUES
(8, 1, 11, 5, 'The floral summer dress is absolutely beautiful and perfect for sunny days!', '2024-11-27'),
(9, 2, 12, 4, 'Great handbag, but a bit heavy for daily use.', '2024-11-27'),
(10, 3, 13, 4, 'Maxi dress is comfortable and fits perfectly. Would recommend it for casual outings.', '2024-11-27'),
(11, 4, 14, 3, 'The cotton blouse is okay, but it wrinkles easily.', '2024-11-27'),
(12, 5, 15, 5, 'Love the high waist skinny jeans. Very flattering and comfortable!', '2024-11-27'),
(13, 6, 16, 4, 'The V-neck sweater is cozy, but it shrank a bit after washing.', '2024-11-27'),
(14, 7, 17, 5, 'The red party dress is stunning. Perfect for a night out!', '2024-11-27');



