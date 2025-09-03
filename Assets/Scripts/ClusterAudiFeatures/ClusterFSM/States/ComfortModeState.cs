using ClusterAudi;
using UnityEngine;
using ClusterAudiFeatures;

namespace ClusterAudiFeatures
{
	public class ComfortModeState : ClusterBaseState
	{
		private IVehicleDataService _vehicleDataService;
		private IBroadcaster _broadcaster;

		// Sistema anti-spam: timer condiviso con altri stati
		private static float _lastModeChangeTime = -10f;
		private const float MODE_CHANGE_COOLDOWN = 2f;

		// Inizializzazione servizi
		public ComfortModeState(ClusterStateContext context) : base(context)
		{
			_vehicleDataService = context.Client.Services.Get<IVehicleDataService>();
			_broadcaster = context.Client.Services.Get<IBroadcaster>();
		}

		// Attivazione COMFORT: equilibrio tra prestazioni ed efficienza
		public override void StateOnEnter()
		{
			Debug.Log("[COMFORT MODE] Attivazione modalità Comfort");

			_lastModeChangeTime = Time.time; // Aggiorna timer
			_vehicleDataService.SetDriveMode(DriveMode.Comfort); // Imposta modalità

			_broadcaster.Broadcast(new DriveModeChangedEvent(DriveMode.Comfort));
		}

		// Disattivazione COMFORT
		public override void StateOnExit()
		{
			Debug.Log("[COMFORT MODE] Disattivazione modalità Comfort");
		}

		// Update continuo: metriche comfort + controlli
		public override void StateOnUpdate()
		{
			CheckModeTransitionsWithTimer(); // Input modalità con cooldown
			HandleRealisticSpeedControl(); // Controllo velocità bilanciato
		}

		// Sistema anti-spam per cambi modalità
		private void CheckModeTransitionsWithTimer()
		{
			float timeSinceLastChange = Time.time - _lastModeChangeTime;
			float remainingCooldown = MODE_CHANGE_COOLDOWN - timeSinceLastChange;

			if (remainingCooldown > 0f)
			{
				if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.F2) || Input.GetKeyDown(KeyCode.F3))
				{
					Debug.Log($"[COMFORT MODE] COOLDOWN ATTIVO! Attendi ancora {remainingCooldown:F1} secondi");
				}
				CheckDebugInputs();
				return;
			}

			CheckModeInputs();
			CheckDebugInputs();
		}

		// Input cambi modalità F1/F2/F3
		private void CheckModeInputs()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				Debug.Log("[COMFORT MODE] F1 premuto - Transizione a Eco Mode");
				_context.ClusterStateMachine.GoTo("EcoModeState");
			}
			else if (Input.GetKeyDown(KeyCode.F2))
			{
				Debug.Log("[COMFORT MODE] F2 premuto - Rimani in Comfort Mode");
				_lastModeChangeTime = Time.time;
			}
			else if (Input.GetKeyDown(KeyCode.F3))
			{
				Debug.Log("[COMFORT MODE] F3 premuto - Transizione a Sport Mode");
				_context.ClusterStateMachine.GoTo("SportModeState");
			}
		}

		// Input debug sempre attivi
		private void CheckDebugInputs()
		{
			if (Input.GetKeyDown(KeyCode.F4))
			{
				Debug.Log("[COMFORT MODE] F4 premuto - Transizione a Welcome State");
				_context.ClusterStateMachine.GoTo("WelcomeState");
			}
			else if (Input.GetKeyDown(KeyCode.Escape))
			{
				LogCurrentModeInfoWithTimer();
			}
			else if (Input.GetKeyDown(KeyCode.F5))
			{
				Debug.Log("[COMFORT MODE] DEBUG: StateOnUpdate funziona!");
			}
		}

		// Controllo velocità bilanciato (tra ECO e SPORT)
		private void HandleRealisticSpeedControl()
		{
			float currentSpeed = _vehicleDataService.CurrentSpeed;
			float deltaTime = Time.deltaTime;
			float newSpeed = currentSpeed;

			float accelerationRate = 25f; // Bilanciato per COMFORT
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

		// Info debug con timer
		private void LogCurrentModeInfoWithTimer()
		{
			float timeSinceLastChange = Time.time - _lastModeChangeTime;
			float remainingCooldown = MODE_CHANGE_COOLDOWN - timeSinceLastChange;

			Debug.Log("=== COMFORT MODE INFO ===");
			Debug.Log($"Current State: ComfortModeState");
			Debug.Log($"Vehicle Mode: {_vehicleDataService.CurrentDriveMode}");
			Debug.Log($"Speed: {_vehicleDataService.CurrentSpeed:F1} km/h");
			Debug.Log($"RPM: {_vehicleDataService.CurrentRPM:F0}");
			Debug.Log($"Gear: {_vehicleDataService.CurrentGear}");

			Debug.Log($"=== TIMER INFO ===");
			Debug.Log($"Tempo dall'ultimo cambio: {timeSinceLastChange:F1}s");
			if (remainingCooldown > 0)
			{
				Debug.Log($" COOLDOWN ATTIVO: {remainingCooldown:F1}s rimanenti");
			}
			else
			{
				Debug.Log($" COOLDOWN TERMINATO: Cambio modalità permesso");
			}

			Debug.Log("=== CONTROLLI ===");
			Debug.Log("F1=Eco | F2=Comfort | F3=Sport (5s cooldown tra cambi)");
			Debug.Log("W=Accelera | SPAZIO=Frena | ESC=Info");
		}
	}
}