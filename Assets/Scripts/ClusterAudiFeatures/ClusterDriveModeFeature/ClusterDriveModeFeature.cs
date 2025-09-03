using System.Threading.Tasks;
using ClusterAudi;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public class ClusterDriveModeFeature : BaseFeature, IClusterDriveModeFeature, IClusterDriveModeFeatureInternal
	{
		public ClusterDriveModeFeature(Client client) : base(client)
		{
			Debug.Log("[CLUSTER DRIVE MODE FEATURE] ClusterDriveModeFeature inizializzata");
		}

		// Istanzia la UI da prefab e controlla
		public async Task InstantiateClusterDriveModeFeature()
		{
			Debug.Log("[CLUSTER DRIVE MODE FEATURE]  Istanziazione Cluster Drive Mode UI...");

			try
			{
				// Carica prefab tramite AssetService
				var clusterDriveModeInstance = await _assetService.InstantiateAsset<ClusterDriveModeBehaviour>(
					ClusterDriveModeData.CLUSTER_DRIVE_MODE_PREFAB_PATH);

				if (clusterDriveModeInstance != null)
				{
					// Inizializza behaviour
					clusterDriveModeInstance.Initialize(this);
					Debug.Log("[CLUSTER DRIVE MODE FEATURE] Drive Mode UI istanziata da prefab");
				}
				else
				{
					Debug.LogWarning("[CLUSTER DRIVE MODE FEATURE] Prefab non trovato: " +
						ClusterDriveModeData.CLUSTER_DRIVE_MODE_PREFAB_PATH);
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[CLUSTER DRIVE MODE FEATURE] Errore istanziazione: {ex.Message}");
			}
		}

		// Client per accesso ai servizi
		public Client GetClient()
		{
			return _client;
		}
	}
}