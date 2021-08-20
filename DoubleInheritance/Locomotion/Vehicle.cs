using LinqToDB.Mapping;

namespace DoubleInheritance.Locomotion
{
	[Table("Vehicle", Schema = "DoubleInheritance")]
	[InheritanceMapping(Code = VehicleType.Bicycle, Type = typeof(Bicycle))]
	[InheritanceMapping(Code = VehicleType.Automobile, Type = typeof(Automobile))]
	public abstract class Vehicle
	{
		[Identity]
		public int Id { get; set; }

		[Column(IsDiscriminator = true)]
		public virtual VehicleType Type { get; set; }
	}
}
