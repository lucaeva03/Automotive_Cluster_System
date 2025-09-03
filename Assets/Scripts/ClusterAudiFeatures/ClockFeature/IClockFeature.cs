using ClusterAudi;
using System.Threading.Tasks;

namespace ClusterAudiFeatures
{
	public interface IClockFeature : IFeature
	{
		Task InstantiateClockFeature();
	}
}