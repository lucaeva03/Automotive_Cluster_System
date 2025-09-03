using ClusterAudi;
using System.Threading.Tasks;

namespace ClusterAudiFeatures
{
	public interface IDoorLockFeature : IFeature
	{
		// UI istanziata
		Task InstantiateDoorLockFeature();

		// Stato corrente del lucchetto
		bool IsLocked { get; }

		// Cambio stato del lucchetto
		void ToggleLock();
	}
}