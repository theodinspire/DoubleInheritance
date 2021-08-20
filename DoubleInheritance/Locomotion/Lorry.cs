using LinqToDB.Mapping;

namespace DoubleInheritance.Locomotion
{
	public class Lorry : Automobile
	{
		[Column]
		public override AutomobileType AutomobileType => AutomobileType.Lorry;

		[Column]
		public double CargoCapacity { get; set; }
	}
}