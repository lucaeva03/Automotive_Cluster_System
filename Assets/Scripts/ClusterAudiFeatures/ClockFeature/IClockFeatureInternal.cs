using ClusterAudi;

namespace ClusterAudiFeatures
{
	public interface IClockFeatureInternal : IFeatureInternal
	{
		Client GetClient();
	}
}