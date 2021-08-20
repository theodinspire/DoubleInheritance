using LinqToDB.Mapping;

namespace DoubleInheritance.Locomotion
{
	public class Car : Automobile
	{
		[Column]
		public override AutomobileType AutomobileType => AutomobileType.Car;

		[Column]
		public int MaxOccupancy { get; set; }
	}
}