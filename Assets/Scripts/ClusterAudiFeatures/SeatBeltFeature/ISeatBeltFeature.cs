using System.Threading.Tasks;
using ClusterAudi;

namespace ClusterAudiFeatures
{
	// Interfaccia pubblica per SeatBelt feature
	public interface ISeatBeltFeature : IFeature
	{
		// Istanzia la UI
		Task InstantiateSeatBeltFeature();

		// Stato di una cintura specifica
		void SetSeatBeltStatus(SeatBeltData.SeatBeltPosition position, SeatBeltData.SeatBeltStatus status);

		// Froza controllo debug
		void ForceWarningCheck();
	}
}