using System.Threading.Tasks;
using ClusterAudi;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public class SpeedometerFeature : BaseFeature, ISpeedometerFeature, ISpeedometerFeatureInternal
	{
		// Configurazione corrente
		private SpeedometerConfig _currentConfiguration;
		private SpeedometerBehaviour _speedometerBehaviour;

		public SpeedometerFeature(Client client) : base(client)
		{
			Debug.Log("[SPEEDOMETER FEATURE] SpeedometerFeature inizializzata");

			// Situazione inziale
			var vehicleService = client.Services.Get<IVehicleDataService>();
			_currentConfiguration = SpeedometerData.GetConfigForDriveMode(vehicleService.CurrentDriveMode);

			// Sottoscrizione eventi
			_broadcaster.Add<DriveModeChangedEvent>(OnDriveModeChanged);
		}

		// Istanzia UI
		public async Task InstantiateSpeedometerFeature()
		{
			Debug.Log("[SPEEDOMETER FEATURE] Istanziazione Speedometer UI...");

			try
			{
				var speedometerInstance = await _assetService.InstantiateAsset<SpeedometerBehaviour>(
					SpeedometerData.SPEEDOMETER_PREFAB_PATH);

				if (speedometerInstance != null)
				{
					speedometerInstance.Initialize(this);
					_speedometerBehaviour = speedometerInstance;
					_speedometerBehaviour.ApplyConfiguration(_currentConfiguration);

					Debug.Log("[SPEEDOMETER FEATURE] Speedometer UI istanziata");
				}
				else
				{
					Debug.LogWarning("[SPEEDOMETER FEATURE] Prefab non trovato");
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[SPEEDOMETER FEATURE] Errore istanziazione: {ex.Message}");
			}
		}

		// Aggiorna configurazione per nuova modalità
		public void UpdateConfigurationForDriveMode(DriveMode mode)
		{
			_currentConfiguration = SpeedometerData.GetConfigForDriveMode(mode);
			_speedometerBehaviour?.ApplyConfiguration(_currentConfiguration);
			Debug.Log($"[SPEEDOMETER FEATURE] Configurazione aggiornata: {mode}");
		}

		// Accesso al client per behaviour
		public Client GetClient() => _client;

		// Configurazione corrente
		public SpeedometerConfig GetCurrentConfiguration() => _currentConfiguration;

		// Gestione eventi cambio modalità guida
		private void OnDriveModeChanged(DriveModeChangedEvent e)
		{
			Debug.Log($"[SPEEDOMETER FEATURE] Modalità cambiata: {e.NewMode}");
			UpdateConfigurationForDriveMode(e.NewMode);
		}

		// Cleanup sottoscrizioni eventi
		~SpeedometerFeature()
		{
			_broadcaster?.Remove<DriveModeChangedEvent>(OnDriveModeChanged);
		}
	}
}