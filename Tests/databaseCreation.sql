CREATE SCHEMA [DoubleInheritance]
	AUTHORIZATION [dbo];
GO;

CREATE TABLE [DoubleInheritance].[Vehicle]
(
	[Id]             INT IDENTITY (1, 1) NOT NULL,
	[Type]           INT                 NOT NULL,
	[BicycleType]    INT                 NULL,
	[AutomobileType] INT                 NULL,
	[CargoCapacity]  DECIMAL(10, 5)      NULL,
	[MaxOccupancy]   INT                 NULL,
);
GO;

INSERT INTO
	[DoubleInheritance].[Vehicle] (Type, BicycleType, AutomobileType, CargoCapacity, MaxOccupancy)
VALUES
	(0, 0,    NULL, NULL,  NULL),
	(0, 3,    NULL, NULL,  NULL),
	(1, NULL, 0,    33187, NULL),
	(1, NULL, 0,    2000,  NULL),
	(1, NULL, 1,    NULL,  5   ),
	(1, NULL, 1,    NULL,  8   );
GO;
