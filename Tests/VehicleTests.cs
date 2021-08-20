using System.Linq;
using DoubleInheritance.Locomotion;
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
		}

		[Test]
		public void CanReadCars()
		{
			var car = this.connection.GetTable<Car>().FirstOrDefault();

			Assert.That(car, Is.Not.Null);
		}

		[Test]
		public void CanReadLorries()
		{
			var lorry = this.connection.GetTable<Lorry>().FirstOrDefault();

			Assert.That(lorry, Is.Not.Null);
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
		public void CanReadBicycleFromVehicles()
		{
			var vehicles = this.connection.GetTable<Vehicle>().ToList();
			// System.InvalidOperationException : Instances of abstract classes cannot be created.

			var bicycle = vehicles.FirstOrDefault(x => x.Type == VehicleType.Bicycle);

			Assert.That(bicycle, Is.Not.Null);
			Assert.That(bicycle, Is.TypeOf(typeof(Bicycle)));
		}
	}
}
