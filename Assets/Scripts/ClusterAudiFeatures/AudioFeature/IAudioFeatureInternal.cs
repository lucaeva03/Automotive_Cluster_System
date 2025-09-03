using ClusterAudi;

namespace ClusterAudiFeatures
{
	// Interfaccia interna per comunicazione AudioFeature - MonoBehaviour
	public interface IAudioFeatureInternal : IFeatureInternal
	{
		// Accesso al Client per ottenere servizi
		Client GetClient();
	}
}