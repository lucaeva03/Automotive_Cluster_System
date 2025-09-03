using ClusterAudi;

namespace ClusterAudiFeatures
{
	public interface IAutomaticGearboxFeatureInternal : IFeatureInternal
	{
		Client GetClient();
		AutomaticGearboxConfig GetCurrentConfiguration();
	}
}