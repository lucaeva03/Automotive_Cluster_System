using ClusterAudi;

namespace ClusterAudiFeatures
{
	/// <summary>
	/// Interfaccia interna per Lane Assist Feature
	/// IDENTICA al pattern IClusterDriveModeFeatureInternal del tuo progetto
	/// </summary>
	public interface ILaneAssistFeatureInternal : IFeatureInternal
	{
		/// <summary>
		/// Ottiene il Client per accesso ai servizi
		/// IDENTICO al pattern GetClient() del tuo progetto
		/// </summary>
		Client GetClient();
	}
}