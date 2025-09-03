using ClusterAudi;


namespace ClusterAudiFeatures
{
	public interface IClusterDriveModeFeatureInternal : IFeatureInternal
	{
		Client GetClient();
	}
}