using ClusterAudi;
using UnityEngine;
using ClusterAudiFeatures;

namespace ClusterAudiFeatures
{
	public class SportModeState : ClusterBaseState
	{
		private IVehicleDataService _vehicleDataService;
		private IBroadcaster _broadcaster;

		// Sistema anti-spam condiviso
		private static float _lastModeChangeTime = -10f;
		private const float MODE_CHANGE_COOLDOWN = 2f;

		// Tracking performance sessione
		private float _sessionMaxSpeed = 0f;
		private float _sessionMaxRPM = 0f;

		// Inizializzazione servizi
		public SportModeState(ClusterStateContext context) : base(context)
		{
			_vehicleDataService = context.Client.Services.Get<IVehicleDataService>();
			_broadcaster = context.Client.Services.Get<IBroadcaster>();
		}

		// Attivazione SPORT: prestazioni massime e tracking performance
		public override void StateOnEnter()
		{
			Debug.Log("[SPORT MODE] Attivazione modalità Sport - Performance Mode ON!");

			_lastModeChangeTime = Time.time;
			_vehicleDataService.SetDriveMode(DriveMode.Sport); // Motore in modalità prestazioni

			_broadcaster.Broadcast(new DriveModeChangedEvent(DriveMode.Sport));
		}

		// Disattivazione SPORT
		public override void StateOnExit()
		{
			Debug.Log("[SPORT MODE] Disattivazione modalità Sport");
		}

		// Update ad alta frequenza per modalità performance
		public override void StateOnUpdate()
		{
			CheckModeTransitionsWithTimer(); // Input modalità con cooldown
			HandleRealisticSpeedControl(); // Controllo velocità aggressivo
		}

		// Sistema anti-spam cambi modalità
		private void CheckModeTransitionsWithTimer()
		{
			float timeSinceLastChange = Time.time - _lastModeChangeTime;
			float remainingCooldown = MODE_CHANGE_COOLDOWN - timeSinceLastChange;

			if (remainingCooldown > 0f)
			{
				if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.F2) || Input.GetKeyDown(KeyCode.F3))
				{
					Debug.Log($"[SPORT MODE] COOLDOWN ATTIVO! Attendi ancora {remainingCooldown:F1} secondi");
				}
				CheckDebugInputs();
				return;
			}

			CheckModeInputs();
			CheckDebugInputs();
		}

		// Input modalità F1/F2/F3
		private void CheckModeInputs()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				Debug.Log("[SPORT MODE] F1 premuto - Transizione a Eco Mode");
				_context.ClusterStateMachine.GoTo("EcoModeState");
			}
			else if (Input.GetKeyDown(KeyCode.F2))
			{
				Debug.Log("[SPORT MODE] F2 premuto - Transizione a Comfort Mode");
				_context.ClusterStateMachine.GoTo("ComfortModeState");
			}
			else if (Input.GetKeyDown(KeyCode.F3))
			{
				Debug.Log("[SPORT MODE] F3 premuto - Rimani in Sport Mode");
				_lastModeChangeTime = Time.time;
			}
		}

		// Input debug sempre permessi
		private void CheckDebugInputs()
		{
			if (Input.GetKeyDown(KeyCode.F4))
			{
				Debug.Log("[SPORT MODE] F4 premuto - Transizione a Welcome State");
				_context.ClusterStateMachine.GoTo("WelcomeState");
			}
			else if (Input.GetKeyDown(KeyCode.Escape))
			{
				LogCurrentModeInfoWithTimer();
			}
			else if (Input.GetKeyDown(KeyCode.F5))
			{
				Debug.Log("[SPORT MODE] DEBUG: StateOnUpdate funziona!");
			}
		}

		// Controllo velocità aggressivo (accelerazione massima)
		private void HandleRealisticSpeedControl()
		{
			float currentSpeed = _vehicleDataService.CurrentSpeed;
			float deltaTime = Time.deltaTime;
			float newSpeed = currentSpeed;

			float accelerationRate = 30f; // Aumentato per SPORT
			float brakingRate = 40f;
			float naturalDecelerationRate = 8f;

			if (Input.GetKey(KeyCode.W))
			{
				newSpeed += accelerationRate * deltaTime;
				newSpeed = Mathf.Clamp(newSpeed, 0f, 200f);
			}
			else if (Input.GetKey(KeyCode.Space))
			{
				newSpeed -= brakingRate * deltaTime;
				newSpeed = Mathf.Max(newSpeed, 0f);
			}
			else if (currentSpeed > 0f)
			{
				newSpeed -= naturalDecelerationRate * deltaTime;
				newSpeed = Mathf.Max(newSpeed, 0f);
			}

			if (Mathf.Abs(newSpeed - currentSpeed) > 0.1f)
			{
				_vehicleDataService.SetSpeed(newSpeed);
			}
		}

		// Debug con record sessione e timer
		private void LogCurrentModeInfoWithTimer()
		{
			float timeSinceLastChange = Time.time - _lastModeChangeTime;
			float remainingCooldown = MODE_CHANGE_COOLDOWN - timeSinceLastChange;

			Debug.Log("=== SPORT MODE INFO ===");
			Debug.Log($"Current State: SportModeState");
			Debug.Log($"Vehicle Mode: {_vehicleDataService.CurrentDriveMode}");
			Debug.Log($"Speed: {_vehicleDataService.CurrentSpeed:F1} km/h");
			Debug.Log($"RPM: {_vehicleDataService.CurrentRPM:F0}");
			Debug.Log($"Gear: {_vehicleDataService.CurrentGear}");
			Debug.Log($"Session Max Speed: {_sessionMaxSpeed:F1} km/h");
			Debug.Log($"Session Max RPM: {_sessionMaxRPM:F0}");

			Debug.Log($"=== TIMER INFO ===");
			Debug.Log($"Tempo dall'ultimo cambio: {timeSinceLastChange:F1}s");
			if (remainingCooldown > 0)
			{
				Debug.Log($"COOLDOWN ATTIVO: {remainingCooldown:F1}s rimanenti");
			}
			else
			{
				Debug.Log($"COOLDOWN TERMINATO: Cambio modalità permesso");
			}

			Debug.Log("=== CONTROLLI ===");
			Debug.Log("F1=Eco | F2=Comfort | F3=Sport (5s cooldown tra cambi)");
			Debug.Log("W=Accelera | SPAZIO=Frena | ESC=Info");
		}
	}
}