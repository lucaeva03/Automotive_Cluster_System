using System.Threading.Tasks;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	public interface ISpeedometerFeatureInternal : IFeatureInternal
	{
		Client GetClient();

		// Configurazione corrente
		SpeedometerConfig GetCurrentConfiguration();
	}
}