using LinqToDB.Mapping;

namespace DoubleInheritance.Locomotion
{
	public class Bicycle : Vehicle
	{
		[Column]
		public override VehicleType Type => VehicleType.Bicycle;

		[Column]
		public BicycleType BicycleType { get; set; }
	}
}