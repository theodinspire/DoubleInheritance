using System;
using System.Collections.Generic;
using System.Linq;
using DoubleInheritance.Locomotion;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class VehicleTests
	{
		private LocalDatabase local;
		private DataConnection connection;

		[OneTimeSetUp]
		public void SetUp()
		{
			this.local = new LocalDatabase("VehicleTests");

			var provider = SqlServerTools.GetDataProvider(SqlServerVersion.v2016);
			this.connection = new DataConnection(provider, this.local.Connection);

			this.connection.Execute(@"
CREATE SCHEMA [DoubleInheritance]
	AUTHORIZATION [dbo];
");
			this.connection.Execute(@"
CREATE TABLE [DoubleInheritance].[Vehicle]
(
	[Id]             INT IDENTITY (1, 1) NOT NULL,
	[Type]           INT                 NOT NULL,
	[BicycleType]    INT                 NULL,
	[AutomobileType] INT                 NULL,
	[CargoCapacity]  DECIMAL(10, 5)      NULL,
	[MaxOccupancy]   INT                 NULL,
);
");
			this.connection.Execute(@"
INSERT INTO
	[DoubleInheritance].[Vehicle] (Type, BicycleType, AutomobileType, CargoCapacity, MaxOccupancy)
VALUES
	(0, 0,    NULL, NULL,  NULL),
	(0, 3,    NULL, NULL,  NULL),
	(1, NULL, 0,    33187, NULL),
	(1, NULL, 0,    2000,  NULL),
	(1, NULL, 1,    NULL,  5   ),
	(1, NULL, 1,    NULL,  8   );
");
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			this.connection.Dispose();
			this.local.Dispose();
		}

		[Test]
		public void CanReadBicycles()
		{
			var bicycle = this.connection.GetTable<Bicycle>().FirstOrDefault();

			Assert.That(bicycle, Is.Not.Null);
			// Passes
		}

		[Test]
		public void CanReadCars()
		{
			var car = this.connection.GetTable<Car>().FirstOrDefault();

			Assert.That(car, Is.Not.Null);
			// Passes
		}

		[Test]
		public void CanReadLorries()
		{
			var lorry = this.connection.GetTable<Lorry>().FirstOrDefault();

			Assert.That(lorry, Is.Not.Null);
			// Passes
		}

		[Test]
		public void CanReadCarFromAutomobiles()
		{
			var automobiles = this.connection.GetTable<Automobile>().ToList();
			// System.ArgumentException : 'CargoCapacity' is not a member of type 'DoubleInheritance.Locomotion.Automobile'

			var car = automobiles.FirstOrDefault(x => x.AutomobileType == AutomobileType.Car);

			Assert.That(car, Is.Not.Null);
			Assert.That(car, Is.TypeOf(typeof(Car)));
		}

		[Test]
		public void CanReadCarFromVehicles()
		{
			var vehicles = this.connection.GetTable<Vehicle>().ToList();
			// System.InvalidOperationException : Instances of abstract classes cannot be created.

			var automobiles = vehicles.Where(x => x.Type == VehicleType.Automobile).Cast<Automobile>().ToList();

			var car = automobiles.FirstOrDefault(x => x.AutomobileType == AutomobileType.Car);

			Assert.That(car, Is.Not.Null);
			Assert.That(car, Is.TypeOf(typeof(Car)));
		}

		[Test]
		public void CanReadBicycleFromListOfVehicles()
		{
			var vehicles = this.connection.GetTable<Vehicle>().ToList();
			// System.InvalidOperationException : Instances of abstract classes cannot be created.

			var bicycle = vehicles.FirstOrDefault(x => x.Type == VehicleType.Bicycle);

			Assert.That(bicycle, Is.Not.Null);
			Assert.That(bicycle, Is.TypeOf(typeof(Bicycle)));
		}

		[Test]
		public void CanReadBicycleFromBicyclesAsVehicles()
		{
			var vehicles = this.connection.GetTable<Vehicle>()
				.Where(x => x.Type == VehicleType.Bicycle).ToList();
			// System.InvalidOperationException : Instances of abstract classes cannot be created.

			var bicycle = vehicles.FirstOrDefault();

			Assert.That(bicycle, Is.Not.Null);
			Assert.That(bicycle, Is.TypeOf(typeof(Bicycle)));
		}

		[Test]
		public void CanInsertBicycle()
		{
			var bicycle = new Bicycle()
			{
				BicycleType = BicycleType.Cargo
			};

			bicycle.Id = this.connection.InsertWithInt32Identity(bicycle);

			Assert.That(bicycle.Id, Is.Not.EqualTo(default(int)));

			var readBicycle = this.connection.GetTable<Bicycle>().FirstOrDefault(x => x.Id == bicycle.Id);
			Assert.That(readBicycle, Is.Not.Null);
			Assert.That(readBicycle.BicycleType, Is.EqualTo(bicycle.BicycleType));
			// Passes
		}

		[Test]
		public void CanInsertBicycleAsVehicle()
		{
			Vehicle bicycle = new Bicycle()
			{
				BicycleType = BicycleType.Cargo
			};

			bicycle.Id = this.connection.InsertWithInt32Identity(bicycle);

			Assert.That(bicycle.Id, Is.Not.EqualTo(default(int)));
			// Passes
		}

		[Test]
		public void CanInsertLorry()
		{
			var lorry = new Lorry()
			{
				CargoCapacity = 10.34,
			};

			lorry.Id = this.connection.InsertWithInt32Identity(lorry);

			Assert.That(lorry.Id, Is.Not.EqualTo(default(int)));
			// Passes
		}

		[Test]
		public void CanInsertMixedVehicleCollection()
		{
			var bicycle = new Bicycle()
			{
				BicycleType = BicycleType.Cargo
			};
			var lorry = new Lorry()
			{
				CargoCapacity = 10.34,
			};

			var list = new List<Vehicle>(2)
			{
				bicycle,
				lorry,
			};

			var objectsInserted = this.connection.BulkCopy(list).RowsCopied;
			// System.InvalidCastException : Unable to cast object of type 'DoubleInheritance.Locomotion.Bicycle' to type 'DoubleInheritance.Locomotion.Automobile'.

			Assert.That(objectsInserted, Is.EqualTo(list.Count));
		}

		[Test]
		public void CanInsertMixedAutomobileCollection()
		{
			var car = new Car()
			{
				MaxOccupancy = 2,
			};
			var lorry = new Lorry()
			{
				CargoCapacity = 10.34,
			};

			var list = new List<Automobile>(2)
			{
				car,
				lorry,
			};

			var objectsInserted = this.connection.BulkCopy(list).RowsCopied;
			// System.InvalidCastException : Unable to cast object of type 'DoubleInheritance.Locomotion.Car' to type 'DoubleInheritance.Locomotion.Lorry'

			Assert.That(objectsInserted, Is.EqualTo(list.Count));
		}

		[Test]
		public void CanInsertBicycleCollection()
		{
			var foo = new Bicycle()
			{
				BicycleType = BicycleType.Recumbent,
			};
			var bar = new Bicycle()
			{
				BicycleType = BicycleType.Standard,
			};;

			var list = new List<Bicycle>(2)
			{
				foo,
				bar,
			};

			var objectsInserted = this.connection.BulkCopy(list).RowsCopied;

			Assert.That(objectsInserted, Is.EqualTo(list.Count));
			// Passes
		}

		[Test]
		public void CanInsertCarCollection()
		{
			var foo = new Car()
			{
				MaxOccupancy = 36,
			};
			var bar = new Car()
			{
				MaxOccupancy = 4,
			};;

			var list = new List<Car>(2)
			{
				foo,
				bar,
			};

			var objectsInserted = this.connection.BulkCopy(list).RowsCopied;

			Assert.That(objectsInserted, Is.EqualTo(list.Count));
			// Passes
		}

		[Test]
		public void CanInsertBicycleAsVehicleCollection()
		{
			var foo = new Bicycle()
			{
				BicycleType = BicycleType.Recumbent,
			};
			var bar = new Bicycle()
			{
				BicycleType = BicycleType.Standard,
			};;

			var list = new List<Vehicle>(2)
			{
				foo,
				bar,
			};

			var objectsInserted = this.connection.BulkCopy(list).RowsCopied;
			// System.InvalidCastException : Unable to cast object of type 'DoubleInheritance.Locomotion.Bicycle' to type 'DoubleInheritance.Locomotion.Automobile'.

			Assert.That(objectsInserted, Is.EqualTo(list.Count));
		}
	}
}
