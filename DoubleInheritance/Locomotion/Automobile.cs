using LinqToDB.Mapping;

namespace DoubleInheritance.Locomotion
{
	[InheritanceMapping(Code = AutomobileType.Lorry, Type = typeof(Lorry))]
	[InheritanceMapping(Code = AutomobileType.Car, Type = typeof(Car))]
	public abstract class Automobile : Vehicle
	{
		[Column]
		public override VehicleType Type => VehicleType.Automobile;

		[Column(IsDiscriminator = true)]
		public virtual AutomobileType AutomobileType { get; set; }
	}
}