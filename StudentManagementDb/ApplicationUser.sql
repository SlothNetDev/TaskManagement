CREATE TABLE [dbo].[ApplicationUser]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Firstname] NVARCHAR(90) NOT NULL, 
    [Lastname] NCHAR(90) NOT NULL, 
    [Gendersid] int NOT NULL
    FOREIGN KEY (Gendersid) REFERENCES Gender(GenderID)
)
