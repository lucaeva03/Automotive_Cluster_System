using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public class WelcomeFeature : BaseFeature, IWelcomeFeature, IWelcomeFeatureInternal
	{
		public WelcomeFeature(Client client) : base(client)
		{
			Debug.Log("[WELCOME FEATURE] WelcomeFeature inizializzata");

			// Sottoscrive eventi audio
			_broadcaster.Add<WelcomeAudioRequestEvent>(OnWelcomeAudioRequest);
		}

		// Istanzia Welcome Screen prefab + controllo
		public async Task InstantiateWelcomeFeature()
		{
			Debug.Log("[WELCOME FEATURE] Inizio istanziazione Welcome Feature...");

			try
			{
				// Carica prefab
				var welcomeScreenInstance = await _assetService.InstantiateAsset<WelcomeScreenBehaviour>(WelcomeData.WELCOME_SCREEN_PREFAB_PATH);

				if (welcomeScreenInstance != null)
				{
					welcomeScreenInstance.Initialize(this);
					Debug.Log("[WELCOME FEATURE] Welcome Feature istanziata!");
				}
				else
				{
					Debug.LogError("[WELCOME FEATURE] Impossibile caricare prefab");
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[WELCOME FEATURE] Errore durante istanziazione: {ex.Message}");
				Debug.LogException(ex);
			}
		}

		// Gestione richieste audio
		private void OnWelcomeAudioRequest(WelcomeAudioRequestEvent e)
		{

		}

		// Pulizia sottoscrizioni eventi
		~WelcomeFeature()
		{
			_broadcaster?.Remove<WelcomeAudioRequestEvent>(OnWelcomeAudioRequest);
		}

		// Accesso al client per behaviour
		public Client GetClient()
		{
			return _client;
		}
	}
}