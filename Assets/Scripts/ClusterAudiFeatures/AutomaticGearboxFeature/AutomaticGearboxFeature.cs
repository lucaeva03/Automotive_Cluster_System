using System.Threading.Tasks;
using ClusterAudi;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public class AutomaticGearboxFeature : BaseFeature, IAutomaticGearboxFeature, IAutomaticGearboxFeatureInternal
	{
		private AutomaticGearboxConfig _currentConfiguration;
		private AutomaticGearboxBehaviour _automaticgearboxBehaviour;

		public AutomaticGearboxFeature(Client client) : base(client)
		{
			Debug.Log("[AUTOMATIC GEARBOX FEATURE] AutomaticGearboxFeature inizializzata");

			// Ottiene modalità corrente
			var vehicleService = client.Services.Get<IVehicleDataService>();
			_currentConfiguration = AutomaticGearboxData.GetConfigForDriveMode(vehicleService.CurrentDriveMode);

			// Sottoscrizione eventi cambio modalità
			var broadcaster = client.Services.Get<IBroadcaster>();
			broadcaster.Add<DriveModeChangedEvent>(OnDriveModeChanged);
		}

		// Istanzia prefab e controlla
		public async Task InstantiateAutomaticGearboxFeature()
		{
			Debug.Log("[AUTOMATIC GEARBOX FEATURE] Istanziazione AutomaticGearbox UI...");

			try
			{
				// Carica prefab tramite AssetService
				var automaticgearboxInstance = await _assetService.InstantiateAsset<AutomaticGearboxBehaviour>(
					AutomaticGearboxData.AutomaticGearbox_PREFAB_PATH);

				if (automaticgearboxInstance != null)
				{
					// Impostazione behaviour con configurazione corrente
					automaticgearboxInstance.Initialize(this);
					_automaticgearboxBehaviour = automaticgearboxInstance;
					_automaticgearboxBehaviour.ApplyConfiguration(_currentConfiguration);

					Debug.Log("[AUTOMATIC GEARBOX FEATURE] AutomaticGearbox UI istanziata da prefab");
				}
				else
				{
					Debug.LogError("[AUTOMATIC GEARBOX FEATURE] ERRORE CRITICO: Prefab non trovato: " + AutomaticGearboxData.AutomaticGearbox_PREFAB_PATH);
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[AUTOMATIC GEARBOX FEATURE] Errore critico istanziazione: {ex.Message}");
			}
		}

		// Aggiorna configurazione al cambio modalità
		public void UpdateConfigurationForDriveMode(DriveMode mode)
		{
			_currentConfiguration = AutomaticGearboxData.GetConfigForDriveMode(mode);

			// Applica nuova configurazione al behaviour
			if (_automaticgearboxBehaviour != null)
			{
				_automaticgearboxBehaviour.ApplyConfiguration(_currentConfiguration);
			}

			Debug.Log($"[AUTOMATIC GEARBOX FEATURE] Configurazione aggiornata per modalità: {mode}");
		}

		// Client per accesso servizi
		public Client GetClient()
		{
			return _client;
		}

		// Passaggio configurazione corrente
		public AutomaticGearboxConfig GetCurrentConfiguration()
		{
			return _currentConfiguration;
		}

		// Evento cambio modalità passato a UpdateConfigurationForDriveMode
		private void OnDriveModeChanged(DriveModeChangedEvent e)
		{
			Debug.Log($"[AUTOMATIC GEARBOX FEATURE] Modalità cambiata: {e.NewMode}");
			UpdateConfigurationForDriveMode(e.NewMode);
		}

		// Pulizia eventi
		~AutomaticGearboxFeature()
		{
			if (_broadcaster != null)
			{
				_broadcaster.Remove<DriveModeChangedEvent>(OnDriveModeChanged);
			}
		}
	}
}