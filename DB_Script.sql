CREATE DATABASE StaffRegistryDB
GO

USE StaffRegistryDB
GO

CREATE TABLE Departments (
    Id INT PRIMARY KEY IDENTITY,
    DepartmentName NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Positions (
    Id INT PRIMARY KEY IDENTITY,
    PositionName NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Employees (
    Id INT PRIMARY KEY IDENTITY,
    FullName NVARCHAR(55) NOT NULL,
    Residence NVARCHAR(75),
    PhoneNumber NVARCHAR(12),
    DateOfBirth Date,
    HireDate DATE,
    Salary DECIMAL(10, 2),
	DepartmentId INT,
    PositionId INT,
    FOREIGN KEY (DepartmentId) REFERENCES Departments(Id),
    FOREIGN KEY (PositionId) REFERENCES Positions(Id),

	 CONSTRAINT UC_PhoneNumber UNIQUE (PhoneNumber),
	 CONSTRAINT UC_FullName_DateOfBirth UNIQUE (DateOfBirth, FullName),
);

CREATE TABLE CompanyInformation (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CompanyName NVARCHAR(55) NOT NULL,
    PhysicalAddress NVARCHAR(255) NOT NULL,
    ContactPhone NVARCHAR(12) NOT NULL,
    Email NVARCHAR(55) NOT NULL
);

--Insert test data
INSERT INTO Departments (DepartmentName) VALUES
('HR'),
('IT'),
('Finance'),
('Marketing'),
('Sales');
GO

INSERT INTO Positions (PositionName) VALUES
('Manager'),
('Developer'),
('Analyst'),
('Designer'),
('Sales Representative');
GO

INSERT INTO Employees (FullName, Residence, PhoneNumber, DateOfBirth, HireDate, Salary, DepartmentId, PositionId) VALUES
('John Doe', '123 Main St, Anytown', '380234567890', '1985-05-15', '2020-06-01', 60000.00, 1, 1),
('Jane Smith', '456 Oak St, Anytown', '380345678901', '1990-08-22', '2019-03-15', 75000.00, 2, 2),
('Emily Johnson', '789 Pine St, Anytown', '380456789012', '1982-11-30', '2018-07-23', 80000.00, 3, 3),
('Michael Brown', '101 Maple St, Anytown', '380567890123', '1979-02-14', '2021-02-01', 50000.00, 4, 4),
('Jessica Davis', '202 Birch St, Anytown', '380678901234', '1995-07-07', '2017-09-10', 45000.00, 5, 5),
('David Wilson', '303 Cedar St, Anytown', '380789012345', '1988-03-25', '2016-11-12', 62000.00, 1, 2),
('Sophia Martinez', '404 Elm St, Anytown', '380890123456', '1992-05-05', '2015-10-01', 70000.00, 2, 3),
('James Anderson', '505 Spruce St, Anytown', '380901234567', '1984-09-18', '2022-01-20', 55000.00, 3, 4),
('Olivia Thomas', '606 Aspen St, Anytown', '380012345678', '1998-12-12', '2014-08-08', 48000.00, 4, 5),
('Lucas White', '707 Willow St, Anytown', '380123456789', '1991-10-10', '2023-03-05', 52000.00, 5, 1);
GO

INSERT INTO CompanyInformation (CompanyName, PhysicalAddress, ContactPhone, Email) VALUES
('Tech Solutions Inc.', '789 Innovation Drive, Tech City', '380998357732', 'info@techsolutions.com');
GO

--Get Company info
CREATE PROCEDURE GetCompanyInfo
AS
BEGIN
	SELECT *FROM CompanyInformation
END
GO

--Update Company info
CREATE PROCEDURE UpdateCompanyInfo
    @Id INT,
    @CompanyName NVARCHAR(55),
    @PhysicalAddress NVARCHAR(255),
    @ContactPhone NVARCHAR(12),
    @Email NVARCHAR(55)
AS
BEGIN
    UPDATE CompanyInformation
    SET CompanyName = @CompanyName,
        PhysicalAddress = @PhysicalAddress,
        ContactPhone = @ContactPhone,
        Email = @Email
    WHERE Id = @Id;
END;
GO

--Get Employees
CREATE PROCEDURE GetEmployees
AS
BEGIN
	SELECT *FROM dbo.Employees
END
GO

--Get Employees
CREATE PROCEDURE GetDepartments
AS
BEGIN
	SELECT *FROM dbo.Departments
END
GO

CREATE PROCEDURE GetPositions
AS
BEGIN
	SELECT *FROM dbo.Positions
END
GO

--Get Employees (join position, departmant)
CREATE PROCEDURE GetAllEmployees
AS
BEGIN
	SELECT 
    Employees.Id,
    Employees.FullName,
    Employees.Residence,
    Employees.PhoneNumber,
    Employees.DateOfBirth,
    Employees.HireDate,
	Employees.Salary, 
	Employees.DepartmentId,
    Departments.DepartmentName,
	Employees.PositionId,
    Positions.PositionName
	FROM 
		Employees
	INNER  JOIN
		Departments ON Employees.DepartmentId = Departments.Id
	INNER JOIN
		Positions ON Employees.PositionId = Positions.Id;
END
GO

--Get Employee by id
CREATE PROCEDURE GetEmployeeById
(
	@Id INT
)
AS
BEGIN
	SELECT  Employees.Id,
    Employees.FullName,
    Employees.Residence,
    Employees.PhoneNumber,
    Employees.DateOfBirth,
    Employees.HireDate,
	Employees.Salary, 
	Employees.DepartmentId,
    Departments.DepartmentName,
	Employees.PositionId,
    Positions.PositionName
	FROM dbo.Employees		
	INNER  JOIN
		Departments ON Employees.DepartmentId = Departments.Id
	INNER JOIN
		Positions ON Employees.PositionId = Positions.Id
		WHERE Employees.Id = @Id
END
GO

--Insert Employee
CREATE PROCEDURE InsertEmployee
(
    @FullName NVARCHAR(55),
    @Residence NVARCHAR(75),
    @PhoneNumber NVARCHAR(12),
    @DateOfBirth DATE,
    @HireDate DATE,
    @Salary DECIMAL,
	@DepartmentId INT,
    @PositionId INT
)
AS
BEGIN
	INSERT INTO dbo.Employees ( FullName, Residence, PhoneNumber, DateOfBirth, HireDate, Salary, DepartmentId, PositionId)
	VALUES
	(
		@FullName
		,@Residence
		,@PhoneNumber
		,@DateOfBirth
		,@HireDate
		,@Salary
		,@DepartmentId
		,@PositionId
	)
END
GO

--Update Employee
CREATE PROCEDURE UpdateEmployee
(
	@Id INT,
    @FullName NVARCHAR(55),
    @Residence NVARCHAR(75),
    @PhoneNumber NVARCHAR(12),
    @DateOfBirth DATE,
    @HireDate DATE,
    @Salary DECIMAL
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
			UPDATE dbo.Employees 
				SET
					FullName = @FullName,
					Residence = @Residence,
					PhoneNumber = @PhoneNumber,
					DateOfBirth = @DateOfBirth,
					HireDate = @HireDate,
					Salary = @Salary
				WHERE Id = @Id
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH
END
GO

--Delete Employee
CREATE PROCEDURE DeleteEmployee
(
	@Id INT
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
			DELETE FROM Employees 
				WHERE Id = @Id
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
	END CATCH
END
GO

--Get Sorted Employee list (sorted, filtred)
CREATE PROCEDURE GetAllEmployeesSorted
(
    @sortedOrder VARCHAR(4) = NULL,
	@sortedColumn VARCHAR(11) = NULL,
	@positions VARCHAR(MAX) = NULL
)
AS
BEGIN
    SELECT 
        Employees.Id,
        Employees.FullName,
        Employees.Residence,
        Employees.PhoneNumber,
        Employees.DateOfBirth,
        Employees.HireDate,
        Employees.Salary, 
        Employees.DepartmentId,
        Departments.DepartmentName,
        Employees.PositionId,
        Positions.PositionName
    FROM 
        Employees
    INNER JOIN
        Departments ON Employees.DepartmentId = Departments.Id
    INNER JOIN
        Positions ON Employees.PositionId = Positions.Id
    WHERE
        (LEN(@positions) = 0 OR Positions.PositionName IN (SELECT value FROM STRING_SPLIT(@positions, ',')))
    ORDER BY
        CASE 
            WHEN @sortedOrder = 'desc' AND @sortedColumn = 'DateOfBirth' THEN Employees.DateOfBirth END DESC,
        CASE 
            WHEN @sortedOrder = 'asc' AND @sortedColumn = 'DateOfBirth' THEN Employees.DateOfBirth END ASC,
        CASE 
            WHEN @sortedOrder = 'desc' AND @sortedColumn = 'HireDate' THEN Employees.HireDate END DESC,
        CASE 
            WHEN @sortedOrder = 'asc' AND @sortedColumn = 'HireDate' THEN Employees.HireDate END ASC,
        CASE 
            WHEN @sortedOrder = 'desc' AND @sortedColumn = 'Salary' THEN Employees.Salary END DESC,
        CASE 
            WHEN @sortedOrder = 'asc' AND @sortedColumn = 'Salary' THEN Employees.Salary END ASC
END
GO

--Find Employee by name
CREATE PROCEDURE SearchEmployeeByName
    @searchString NVARCHAR(55)
AS
BEGIN
    SELECT 
        Employees.Id,
        Employees.FullName,
        Employees.Residence,
        Employees.PhoneNumber,
        Employees.DateOfBirth,
        Employees.HireDate,
        Employees.Salary, 
        Employees.DepartmentId,
        Departments.DepartmentName,
        Employees.PositionId,
        Positions.PositionName
     FROM 
        Employees
    INNER JOIN
        Departments ON Employees.DepartmentId = Departments.Id
    INNER JOIN
        Positions ON Employees.PositionId = Positions.Id
    WHERE 
        FullName LIKE '%' + @searchString + '%'
END
GO

--Salary reports by departmant
CREATE PROCEDURE GetByDepartmentEmployeesSalary
(
    @departmentIdList VARCHAR(MAX) = NULL
)
AS
BEGIN
    SELECT 
        FullName,
        PhoneNumber,
		Departments.DepartmentName,
        Salary
    FROM 
		Employees
	INNER JOIN
        Departments ON Employees.DepartmentId = Departments.Id
    WHERE (LEN(@departmentIdList) = 0 OR Departments.Id IN (SELECT value FROM STRING_SPLIT(@departmentIdList, ',')))
END
GO

--Salary reports by position
CREATE PROCEDURE GetByPositionEmployeesSalary
(
    @positionsIdList VARCHAR(MAX) = NULL
)
AS
BEGIN
    SELECT 
        FullName,
        PhoneNumber,
		Positions.PositionName,
        Salary
    FROM 
		Employees
	INNER JOIN
        Positions ON Employees.PositionId = Positions.Id
    WHERE (LEN(@positionsIdList) = 0 OR Positions.Id IN (SELECT value FROM STRING_SPLIT(@positionsIdList, ',')))
END
GO

--Salary reports by hireDate
CREATE PROCEDURE GetByHireDateEmployeesSalary
(
    @startDate DATE = NULL,
    @endDate DATE = NULL
)
AS
BEGIN
    SELECT 
        FullName,
        PhoneNumber,
        HireDate,
        Salary
    FROM 
	Employees
    WHERE (@startDate IS NULL OR HireDate >= @startDate) AND 
		(@endDate IS NULL OR HireDate <= @endDate);
END
GO