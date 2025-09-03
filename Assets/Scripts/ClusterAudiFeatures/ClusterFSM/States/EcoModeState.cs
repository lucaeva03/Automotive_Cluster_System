using ClusterAudi;
using UnityEngine;
using ClusterAudiFeatures;

namespace ClusterAudiFeatures
{
	public class EcoModeState : ClusterBaseState
	{
		private IVehicleDataService _vehicleDataService;
		private IBroadcaster _broadcaster;

		// Variabili sistema anti-spam F1-F3
		private static float _lastModeChangeTime = -10f;
		private const float MODE_CHANGE_COOLDOWN = 2f;

		public EcoModeState(ClusterStateContext context) : base(context)
		{
			_vehicleDataService = context.Client.Services.Get<IVehicleDataService>();
			_broadcaster = context.Client.Services.Get<IBroadcaster>();
		}

		// Attivazione stato ECO
		public override void StateOnEnter()
		{
			Debug.Log("[ECO MODE] Attivazione modalità Eco");

			_lastModeChangeTime = Time.time; // Aggiorna timer anti-spam
			_vehicleDataService.SetDriveMode(DriveMode.Eco); // Imposta modalità veicolo

			_broadcaster.Broadcast(new DriveModeChangedEvent(DriveMode.Eco));
		}

		// Disattivazione stato ECO
		public override void StateOnExit()
		{
			Debug.Log("[ECO MODE] Disattivazione modalità Eco");
		}

		// Update continuo: metriche, feedback e controlli input
		public override void StateOnUpdate()
		{
			CheckModeTransitionsWithTimer(); // Gestisce cambi modalità con cooldown
			HandleRealisticSpeedControl(); // Controllo velocità W/Spazio
		}

		// Sistema anti-spam: blocca input F1/F2/F3 per 2 secondi
		private void CheckModeTransitionsWithTimer()
		{
			float timeSinceLastChange = Time.time - _lastModeChangeTime;
			float remainingCooldown = MODE_CHANGE_COOLDOWN - timeSinceLastChange;

			// Se in cooldown, blocca con feedback
			if (remainingCooldown > 0f)
			{
				if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.F2) || Input.GetKeyDown(KeyCode.F3))
				{
					Debug.Log($"[ECO MODE] COOLDOWN ATTIVO! Attendi ancora {remainingCooldown:F1} secondi");
				}
				CheckDebugInputs(); // Input debug sempre permessi
				return;
			}

			CheckModeInputs(); // Permetti cambi modalità
			CheckDebugInputs();
		}

		// Input modalità guida (F1/F2/F3)
		private void CheckModeInputs()
		{
			if (Input.GetKeyDown(KeyCode.F1))
			{
				Debug.Log("[ECO MODE] F1 premuto - Rimani in Eco Mode");
				_lastModeChangeTime = Time.time;
			}
			else if (Input.GetKeyDown(KeyCode.F2))
			{
				Debug.Log("[ECO MODE] F2 premuto - Transizione a Comfort Mode");
				_context.ClusterStateMachine.GoTo("ComfortModeState");
			}
			else if (Input.GetKeyDown(KeyCode.F3))
			{
				Debug.Log("[ECO MODE] F3 premuto - Transizione a Sport Mode");
				_context.ClusterStateMachine.GoTo("SportModeState");
			}
		}

		// Input debug sempre permessi (F4, ESC, F5)
		private void CheckDebugInputs()
		{
			if (Input.GetKeyDown(KeyCode.F4))
			{
				Debug.Log("[ECO MODE] F4 premuto - Transizione a Welcome State");
				_context.ClusterStateMachine.GoTo("WelcomeState");
			}
			else if (Input.GetKeyDown(KeyCode.Escape))
			{
				LogCurrentModeInfoWithTimer();
			}
			else if (Input.GetKeyDown(KeyCode.F5))
			{
				Debug.Log("[ECO MODE] DEBUG: StateOnUpdate funziona!");
			}
		}

		// Controllo velocità realistico con W/Spazio (accelerazione ridotta per ECO)
		private void HandleRealisticSpeedControl()
		{
			float currentSpeed = _vehicleDataService.CurrentSpeed;
			float deltaTime = Time.deltaTime;
			float newSpeed = currentSpeed;

			float accelerationRate = 20f; // Ridotto per ECO
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

		// Debug completo stato
		private void LogCurrentModeInfoWithTimer()
		{
			float timeSinceLastChange = Time.time - _lastModeChangeTime;
			float remainingCooldown = MODE_CHANGE_COOLDOWN - timeSinceLastChange;

			Debug.Log("=== ECO MODE INFO ===");
			Debug.Log($"Current State: EcoModeState");
			Debug.Log($"Vehicle Mode: {_vehicleDataService.CurrentDriveMode}");
			Debug.Log($"Speed: {_vehicleDataService.CurrentSpeed:F1} km/h");
			Debug.Log($"RPM: {_vehicleDataService.CurrentRPM:F0}");
			Debug.Log($"Gear: {_vehicleDataService.CurrentGear}");

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
			Debug.Log("W=Accelera | SPAZIO=Frena | R=Random RPM | ESC=Info");
		}
	}
}