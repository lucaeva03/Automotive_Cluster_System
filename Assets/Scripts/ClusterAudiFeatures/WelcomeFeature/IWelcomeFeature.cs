using System.Threading.Tasks;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	public interface IWelcomeFeature : IFeature
	{
		// Istanzia UI
		Task InstantiateWelcomeFeature();
	}
}