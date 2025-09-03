using System.Threading.Tasks;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	public interface ISpeedometerFeature : IFeature
	{
		// Istanzia la UI
		Task InstantiateSpeedometerFeature();

		// Aggiorna configurazione per modalità
		void UpdateConfigurationForDriveMode(DriveMode mode);
	}
}