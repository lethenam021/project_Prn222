-- Users (id sẽ là 1..5 theo thứ tự)
INSERT INTO [User] (username, email, password, role, avatarURL)
VALUES
(N'Nguyễn Minh Tuấn', N'minhtuan@gmail.com', N'hashed_pw_1', N'buyer', N'https://i.pravatar.cc/150?img=1'),
(N'Lê Thị Hồng Nhung', N'hongnhung@gmail.com', N'hashed_pw_2', N'seller', N'https://i.pravatar.cc/150?img=2'),
(N'Trần Quang Huy', N'quanghuy@gmail.com', N'hashed_pw_3', N'seller', N'https://i.pravatar.cc/150?img=3'),
(N'Phạm Thị Lan Anh', N'lananh@gmail.com', N'hashed_pw_4', N'buyer', N'https://i.pravatar.cc/150?img=4'),
(N'Đỗ Trung Kiên', N'trungkien@gmail.com', N'hashed_pw_5', N'buyer', N'https://i.pravatar.cc/150?img=5');

-- Addresses
INSERT INTO [Address] (userId, fullName, phone, street, city, state, country, isDefault)
VALUES
(1, N'Nguyễn Minh Tuấn', N'0905123456', N'12 Nguyễn Huệ', N'Huế', N'Thừa Thiên Huế', N'Việt Nam', 1),
(2, N'Lê Thị Hồng Nhung', N'0916234789', N'45 Lê Lợi', N'Hà Nội', N'Hà Nội', N'Việt Nam', 1),
(3, N'Trần Quang Huy', N'0934785123', N'78 Nguyễn Văn Cừ', N'TP.HCM', N'Hồ Chí Minh', N'Việt Nam', 1),
(4, N'Phạm Thị Lan Anh', N'0978123456', N'21 Võ Văn Ngân', N'TP.Thủ Đức', N'Hồ Chí Minh', N'Việt Nam', 1),
(5, N'Đỗ Trung Kiên', N'0987654321', N'35 Điện Biên Phủ', N'Đà Nẵng', N'Đà Nẵng', N'Việt Nam', 1);

-- Categories (id 1..5)
INSERT INTO [Category] (name)
VALUES
(N'Áo nam'),
(N'Quần nam'),
(N'Áo nữ'),
(N'Váy & đầm'),
(N'Phụ kiện thời trang');

-- Products (sellerId 2 và 3 là người bán)
INSERT INTO [Product] (title, description, price, images, categoryId, sellerId, isAuction, auctionEndTime)
VALUES
(N'Áo sơ mi nam Routine trắng trơn', 
 N'Áo sơ mi cotton trắng cao cấp, dáng slimfit, thương hiệu Routine. Phù hợp công sở hoặc sự kiện.', 
 459000, N'https://routine.vn/media/catalog/product/cache/8/image/600x/040ec09b1e35df139433887a97daa66f/a/o/ao-so-mi-nam-10s22shl007.jpg', 
 1, 2, 0, NULL),

(N'Quần tây nam Owen màu đen', 
 N'Quần tây nam chất liệu poly-rayon cao cấp, co giãn nhẹ, thích hợp văn phòng. Thương hiệu Owen.', 
 699000, N'https://cdn.owen.vn/media/catalog/product/cache/8/image/600x/040ec09b1e35df139433887a97daa66f/q/u/quan-tay-nam-owen-den.jpg', 
 2, 2, 0, NULL),

(N'Áo thun nữ Yody cổ tròn màu hồng pastel', 
 N'Áo thun nữ cotton mềm mại, thấm hút tốt, thiết kế trẻ trung của Yody.', 
 259000, N'https://bizweb.dktcdn.net/100/438/408/products/ao-thun-nu-co-tron-yody.jpg?v=1697073386', 
 3, 3, 0, NULL),

(N'Đầm công sở IVY Moda dáng suông', 
 N'Đầm suông công sở nữ IVY Moda, màu be thanh lịch, tôn dáng, chất liệu thoáng mát.', 
 990000, N'https://pubcdn.ivymoda.com/files/product/thump/480/2022/03/11/bb2d6b8459f9a5b69f0e43a2a2c7e623.jpg', 
 4, 3, 0, NULL),

(N'Mũ bucket Canifa unisex màu kem', 
 N'Mũ bucket Canifa, chất vải canvas, phù hợp phong cách trẻ trung.', 
 199000, N'https://canifa.com/media/catalog/product/cache/8/image/600x/040ec09b1e35df139433887a97daa66f/m/u/mu_bucket_canifa.jpg', 
 5, 2, 0, NULL);

-- Stores
INSERT INTO [Store] (sellerId, storeName, description, bannerImageURL)
VALUES
(2, N'Hồng Nhung Fashion', N'Cửa hàng chuyên đồ nam Owen, Routine chính hãng.', N'https://example.com/store/nhungfashion.jpg'),
(3, N'Quang Huy Boutique', N'Cung cấp đồ nữ thanh lịch IVY Moda, Yody, Canifa.', N'https://example.com/store/huyboutique.jpg');

-- Inventory
INSERT INTO [Inventory] (productId, quantity, lastUpdated)
VALUES
(1, 35, GETDATE()),
(2, 25, GETDATE()),
(3, 60, GETDATE()),
(4, 20, GETDATE()),
(5, 40, GETDATE());

-- Orders
INSERT INTO [OrderTable] (buyerId, addressId, orderDate, totalPrice, status)
VALUES
(1, 1, DATEADD(DAY, -2, GETDATE()), 699000, N'Completed'),
(4, 4, GETDATE(), 990000, N'Processing');

-- Order Items
INSERT INTO [OrderItem] (orderId, productId, quantity, unitPrice)
VALUES
(1, 2, 1, 699000),
(2, 4, 1, 990000);

-- Payments
INSERT INTO [Payment] (orderId, userId, amount, method, status, paidAt)
VALUES
(1, 1, 699000, N'Momo', N'Paid', DATEADD(DAY, -2, GETDATE())),
(2, 4, 990000, N'VNPay', N'Pending', GETDATE());

-- Shipping
INSERT INTO [ShippingInfo] (orderId, carrier, trackingNumber, status, estimatedArrival)
VALUES
(1, N'GHN Express', N'GHN987654VN', N'Delivered', DATEADD(DAY, -1, GETDATE())),
(2, N'J&T Express', N'JT112233VN', N'Standard Shipping', DATEADD(DAY, 3, GETDATE()));

-- Reviews
INSERT INTO [Review] (productId, reviewerId, rating, comment, createdAt)
VALUES
(2, 1, 5, N'Quần chất lượng, đúng size, đóng gói đẹp, rất hài lòng.', GETDATE()),
(4, 4, 4, N'Đầm đẹp, giao hàng nhanh, nhưng hơi mỏng.', GETDATE());

-- Messages
INSERT INTO [Message] (senderId, receiverId, content, timestamp)
VALUES
(1, 2, N'Shop ơi, quần tây còn size 30 không?', GETDATE()),
(2, 1, N'Chào anh Tuấn, size 30 còn hàng ạ!', DATEADD(MINUTE, 5, GETDATE()));

-- Coupons
INSERT INTO [Coupon] (code, discountPercent, startDate, endDate, maxUsage, productId)
VALUES
(N'OWEN10', 10.00, GETDATE(), DATEADD(MONTH, 1, GETDATE()), 100, 2),
(N'IVY15', 15.00, GETDATE(), DATEADD(MONTH, 1, GETDATE()), 50, 4);

-- (Tùy chọn) Feedback mẫu cho seller
INSERT INTO [Feedback] (sellerId, averageRating, totalReviews, positiveRate)
VALUES
(2, 4.80, 125, 96.00),
(3, 4.65, 98, 93.50);