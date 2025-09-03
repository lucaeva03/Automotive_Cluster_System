using ClusterAudi;

namespace ClusterAudiFeatures
{
	public interface ISeatBeltFeatureInternal : IFeatureInternal
	{
		Client GetClient();

		// Restituisce array con stati attuali di tutte le cinture
		SeatBeltData.SeatBeltStatus[] GetAllSeatBeltStates();
	}
}