using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public class ClockFeature : BaseFeature, IClockFeature, IClockFeatureInternal
	{
		public ClockFeature(Client client) : base(client)
		{
			Debug.Log("[CLOCK FEATURE] ClockFeature inizializzata");
		}

		// Istanzia prefab e controlla
		public async Task InstantiateClockFeature()
		{
			var clockDisplayInstance = await _assetService.InstantiateAsset<ClockDisplayBehaviour>(
				ClockData.CLOCK_DISPLAY_PREFAB_PATH);

			if (clockDisplayInstance != null)
			{
				// inzializza behaviour
				clockDisplayInstance.Initialize(this);
			}
			else
			{
				Debug.LogWarning("[CLOCK FEATURE] Prefab non trovato: " + ClockData.CLOCK_DISPLAY_PREFAB_PATH);
			}
		}

		// Client per accesso ai servizi
		public Client GetClient()
		{
			return _client;
		}
	}
}