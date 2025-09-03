using ClusterAudiFeatures;
using UnityEngine;

namespace ClusterAudi
{
	// Implementazione specifica per il cluster
	public class ClusterClient : Client
	{
		#pragma warning disable CS0414  // trovato su internet per disabilitare avviso 
		[SerializeField] private bool _autoStartOnAwake = true;
		#pragma warning disable CS0414

		[SerializeField] private bool _enableDebugMode = true;

		private ClusterFSM _clusterStateMachine;

		// Registrazione Feature - ognuna riceve this (Client) per accedere ai servizi
		public override void InitFeatures()
		{
			Debug.Log("[CLUSTER CLIENT] Inizializzazione Features...");

			// Registrazione tutte le features
			Features.Add<IAudioFeature>(new AudioFeature(this));
			Features.Add<IWelcomeFeature>(new WelcomeFeature(this));
			Features.Add<IClusterDriveModeFeature>(new ClusterDriveModeFeature(this));
			Features.Add<ISpeedometerFeature>(new SpeedometerFeature(this));
			Features.Add<IAutomaticGearboxFeature>(new AutomaticGearboxFeature(this));
			Features.Add<ISeatBeltFeature>(new SeatBeltFeature(this));
			Features.Add<IClockFeature>(new ClockFeature(this));
			Features.Add<IDoorLockFeature>(new DoorLockFeature(this));
			Features.Add<ILaneAssistFeature>(new LaneAssistFeature(this));

			Debug.Log("[CLUSTER CLIENT] Features inizializzate");
		}

		// Avvio Cluster Client
		public override void StartClient()
		{
			Debug.Log("[CLUSTER CLIENT] Avvio Cluster Client...");

			ValidateServices();

			// Classe apposita per l'avvio
			ClusterStartUpFlow startupFlow = new ClusterStartUpFlow();
			startupFlow.BeginStartUp(this);
		}

		private void Update()
		{
			// Aggiornamento State Machine ogni frame
			_clusterStateMachine?.UpdateState();

			// Debug input se abilitato
			if (_enableDebugMode)
			{
				HandleDebugInput();
			}
		}

		// Controllo servizi disponibili
		private void ValidateServices()
		{
			Debug.Log("[CLUSTER CLIENT] Validazione servizi...");

			try
			{
				var broadcaster = Services.Get<IBroadcaster>();
				var assetService = Services.Get<IAssetService>();
				var vehicleDataService = Services.Get<IVehicleDataService>();

				Debug.Log("[CLUSTER CLIENT] Tutti i servizi sono disponibili");

				if (_enableDebugMode)
				{
					Debug.Log($"[CLUSTER CLIENT] Modalità: {vehicleDataService.CurrentDriveMode}, " +
						$"Motore: {(vehicleDataService.IsEngineRunning ? "Acceso" : "Spento")}");
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[CLUSTER CLIENT] Errore validazione servizi: {ex.Message}");
			}
		}

		// Debug - Info sistema
		private void HandleDebugInput()
		{
			if (Input.GetKeyDown(KeyCode.F5)) LogSystemStatus();
		}

		private void LogSystemStatus()
		{
			Debug.Log("=== CLUSTER CLIENT STATUS ===");
			Debug.Log($"Client Instance: {(Instance != null ? "Active" : "Null")}");
			Debug.Log($"Services Count: {Services?.GetAll()?.Count ?? 0}");
			Debug.Log($"Features Count: {Features?.GetAll()?.Count ?? 0}");

			var vehicleService = Services?.Get<IVehicleDataService>();
			if (vehicleService != null)
			{
				Debug.Log($"Speed: {vehicleService.CurrentSpeed:F1} km/h, " +
					$"RPM: {vehicleService.CurrentRPM:F0}, " +
					$"Gear: {vehicleService.CurrentGear}, " +
					$"Mode: {vehicleService.CurrentDriveMode}");
			}
		}

		// Collega la State Machine (non-MonoBehaviour) all'Update loop di Unity
		public void SetStateMachine(ClusterFSM stateMachine)
		{
			_clusterStateMachine = stateMachine;
			Debug.Log("[CLUSTER CLIENT] State Machine collegata per Update");
		}
	}
}