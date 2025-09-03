using ClusterAudi;
using System.Threading.Tasks;

namespace ClusterAudiFeatures
{
	public interface IClusterDriveModeFeature : IFeature
	{
		Task InstantiateClusterDriveModeFeature();
	}
}