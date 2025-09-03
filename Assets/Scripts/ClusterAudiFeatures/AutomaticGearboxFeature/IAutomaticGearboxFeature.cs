using System.Threading.Tasks;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	public interface IAutomaticGearboxFeature : IFeature
	{
		Task InstantiateAutomaticGearboxFeature();
		void UpdateConfigurationForDriveMode(DriveMode mode);
	}
}