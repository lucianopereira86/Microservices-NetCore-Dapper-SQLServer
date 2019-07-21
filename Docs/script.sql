-- TABLE --

IF OBJECT_ID('dbo.users', 'U') IS NOT NULL
DROP TABLE dbo.users;
GO
CREATE TABLE dbo.users
(
	idUser		INT PRIMARY KEY IDENTITY,
	name		VARCHAR(100) NOT NULL,
	email		VARCHAR(50) NOT NULL,
	password	VARCHAR(20) NOT NULL
)
GO

-- PROCEDURE --

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND OBJECT_ID = OBJECT_ID('dbo.sp_Login'))
   DROP PROCEDURE dbo.sp_Login
GO
CREATE PROCEDURE dbo.sp_Login
(
	@pEmail		VARCHAR(50),
	@pPassword	VARCHAR(20)
)
AS
BEGIN
	SELECT	u.idUser,
			u.name,
			u.email,
			u.password
	FROM	dbo.users u
	WHERE	UPPER(u.email) = UPPER(@pEmail)
	AND		UPPER(u.password) = UPPER(@pPassword)
END

GO