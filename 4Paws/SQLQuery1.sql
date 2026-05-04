SELECT * FROM Owners WHERE UserId = 3
INSERT INTO Owners (UserId, UserName, OwnerRating, CreatedAt, UpdatedAt) 
VALUES (3, 'Daisy', 0, GETDATE(), GETDATE())

