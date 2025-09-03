using ClusterAudi;

namespace ClusterAudiFeatures
{
	public interface IWelcomeFeatureInternal : IFeatureInternal
	{
		Client GetClient();
	}
}