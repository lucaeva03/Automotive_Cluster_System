using System;
using UnityEngine;

namespace ClusterAudi
{
	// Simulazione dati veicolo
	public class VehicleDataService : IVehicleDataService
	{
		// Stato corrente veicolo
		private float _currentSpeed = 0f;
		private float _currentRPM = 800f;
		private int _currentGear = 0;
		private DriveMode _currentDriveMode = DriveMode.Comfort;
		private bool _isEngineRunning = true;

		// Simulazione fisica
		private float _throttlePosition = 0f;
		private float _brakeForce = 0f;
		private float _acceleration = 0f;
		private float _maxRPM = 7000f;
		private float _currentConsumption = 12f;
		private float _estimatedRange = 450f;

		// Parametri cambio automatico realistico
		private readonly float[] _shiftUpSpeeds = { 0f, 25f, 45f, 70f, 95f, 125f };
		private readonly float _redZoneRPM = 6500f;
		private readonly float _optimalShiftRPM = 3000f;

		// Esposizione read-only dei dati interni
		public float CurrentSpeed => _currentSpeed;
		public float CurrentRPM => _currentRPM;
		public int CurrentGear => _currentGear;
		public DriveMode CurrentDriveMode => _currentDriveMode;
		public bool IsEngineRunning => _isEngineRunning;

		// Eventi per notifica cambiamenti
		public event Action<float> OnSpeedChanged;
		public event Action<float> OnRPMChanged;
		public event Action<int> OnGearChanged;
		public event Action<DriveMode> OnDriveModeChanged;

		// Setup iniziale servizio
		public VehicleDataService()
		{
			Debug.Log("[VEHICLE DATA SERVICE] Servizio dati veicolo inizializzato");
		}

		// Set velocità con validazione + evento se cambiamento significativo
		public void SetSpeed(float speed)
		{
			if (Math.Abs(_currentSpeed - speed) <= 0.1f) return;

			_currentSpeed = Mathf.Clamp(speed, 0f, 300f);
			UpdateAutomaticSystems(); // Trigger sistemi dipendenti
			OnSpeedChanged?.Invoke(_currentSpeed);
		}

		// Set RPM con auto-aggiornamento marce
		public void SetRPM(float rpm)
		{
			if (Math.Abs(_currentRPM - rpm) <= 10f) return;

			_currentRPM = Mathf.Clamp(rpm, 0f, _maxRPM);
			OnRPMChanged?.Invoke(_currentRPM);
			UpdateGearBasedOnRPM(); // Cambio automatico basato su RPM
		}

		// Set Marcia con calcolo RPM automatico
		public void SetGear(int gear)
		{
			if (_currentGear == gear) return;

			_currentGear = Mathf.Clamp(gear, -1, 8);
			UpdateRPMForGear(); // Calcolo RPM realistici per marcia
			OnGearChanged?.Invoke(_currentGear);
			OnRPMChanged?.Invoke(_currentRPM);
		}

		// Set modalità guida con configurazione automatica
		public void SetDriveMode(DriveMode mode)
		{
			if (_currentDriveMode == mode) return;

			_currentDriveMode = mode;
			ConfigureForDriveMode(mode); // Adattamento parametri motore
			OnDriveModeChanged?.Invoke(_currentDriveMode);
		}

		// Controllo motore con reset completo se spento
		public void SetEngineRunning(bool isRunning)
		{
			if (_isEngineRunning == isRunning) return;

			_isEngineRunning = isRunning;
			if (!_isEngineRunning)
			{
				_currentRPM = 0f;
				_currentSpeed = 0f;
			}
			else
			{
				_currentRPM = 800f; // RPM a veicolo fermo
			}

			OnRPMChanged?.Invoke(_currentRPM);
			OnSpeedChanged?.Invoke(_currentSpeed);
		}

		// Sistema master che coordina tutti i sottosistemi automatici
		private void UpdateAutomaticSystems()
		{
			UpdateAutomaticGearShifting();
			UpdateRPMForGear();
			UpdateConsumption();
		}

		// Logica cambio automatico realistica basata su velocità
		private void UpdateAutomaticGearShifting()
		{
			if (!_isEngineRunning || _currentSpeed <= 0f)
			{
				if (_currentGear != 0) SetGear(0); // in P quando veicolo fermo
				return;
			}

			// Inserimento prima marcia
			if (_currentSpeed > 5f && _currentGear == 0)
			{
				SetGear(1);
			}
			// Cambio automatico basate su soglie velocità
			else if (ShouldShiftUp())
			{
				SetGear(Mathf.Min(_currentGear + 1, 6));
			}
			else if (ShouldShiftDown())
			{
				SetGear(Mathf.Max(_currentGear - 1, 1));
			}
		}

		// Calcolo RPM realistici basati su velocità e marcia
		private void UpdateRPMForGear()
		{
			if (!_isEngineRunning)
			{
				_currentRPM = 0f;
				return;
			}

			if (_currentSpeed <= 0f)
			{
				_currentRPM = 800f;
				return;
			}

			if (_currentGear <= 0 || _currentGear > 6)
			{
				_currentRPM = 800f + (_throttlePosition * 1000f);
				return;
			}

			// RPM = f(velocità, rapporto marcia, throttle)
			float baseRPM = CalculateRPMForGear(_currentSpeed, _currentGear);
			float throttleInfluence = _throttlePosition * 800f;
			float targetRPM = baseRPM + throttleInfluence;

			// Smooth damping basato su modalità guida
			float damping = GetDampingForMode();
			_currentRPM = Mathf.Lerp(_currentRPM, targetRPM, damping);
			_currentRPM = Mathf.Clamp(_currentRPM, 800f, _maxRPM);
		}

		// Sistema cambio automatico basato su RPM (complementare a velocità)
		private void UpdateGearBasedOnRPM()
		{
			if (!_isEngineRunning) return;

			// Cambiate up se RPM troppo alti
			if (_currentRPM > 3000f && _currentGear < 6 && _currentSpeed > 20f)
			{
				SetGear(_currentGear + 1);
			}
			// Cambiate down se RPM troppo bassi
			else if (_currentRPM < 1500f && _currentGear > 1 && _currentSpeed < 80f)
			{
				SetGear(_currentGear - 1);
			}
		}

		// Simulazione consumo carburante realistica
		private void UpdateConsumption()
		{
			float baseConsumption = 8f;
			float speedFactor = (_currentSpeed / 100f) * 0.3f; // Consumo aumenta con velocità
			float rpmFactor = (_currentRPM / 3000f) * 0.2f; // Consumo aumenta con RPM

			_currentConsumption = baseConsumption + speedFactor + rpmFactor;
			_estimatedRange = 450f - (_currentConsumption * 10f); // Autonomia residua
		}

		// Logica decisionale cambio automatico up
		public bool ShouldShiftUp()
		{
			return _currentGear > 0 && _currentGear < 6 && _currentSpeed > _shiftUpSpeeds[_currentGear];
		}

		// Logica decisionale cambio automatico down
		public bool ShouldShiftDown()
		{
			if (_currentGear <= 1) return false;
			float minSpeedForGear = _shiftUpSpeeds[_currentGear - 1] - 15f; // Isteresi
			return _currentSpeed < minSpeedForGear && _currentSpeed > 0f;
		}

		// Controlli zona rossa e punti ottimali cambio
		public bool IsInRedZone() => _currentRPM > _redZoneRPM;
		public float GetRedZoneThreshold() => _redZoneRPM;
		public float GetOptimalShiftPointRPM() => _optimalShiftRPM;

		// Get per dati simulazione avanzata
		public float GetCurrentConsumption() => _currentConsumption;
		public float GetEstimatedRange() => _estimatedRange;
		public float GetThrottlePosition() => _throttlePosition;
		public float GetBrakeForce() => _brakeForce;
		public float GetAcceleration() => _acceleration;
		public float GetMaxRPM() => _maxRPM;

		// Calcoli metriche guida per modalità Eco/Comfort/Sport
		public float GetDrivingEfficiency()
		{
			float speedEfficiency = _currentSpeed < 90f ? 0.8f : 0.6f;
			float rpmEfficiency = _currentRPM < 2500f ? 0.9f : 0.7f;
			return (speedEfficiency + rpmEfficiency) / 2f;
		}

		// Metriche qualità guida per feedback utente
		public float GetAccelerationSmoothness() => Mathf.Clamp01(1f - (Mathf.Abs(_acceleration) / 5f));
		public float GetSpeedStability() => UnityEngine.Random.Range(0.6f, 0.95f);
		public float GetGearUsageOptimality()
		{
			if (_currentGear == 0) return 1f;
			float optimalRPMRange = Mathf.InverseLerp(1500f, 3000f, _currentRPM);
			return Mathf.Clamp01(1f - Mathf.Abs(optimalRPMRange - 0.5f) * 2f);
		}

		public float GetRPMEfficiency() => Mathf.InverseLerp(_maxRPM, 2000f, _currentRPM);
		public float GetAccelerationControl() => Mathf.Clamp01(1f - Mathf.Abs(_acceleration) / 3f);
		public float GetCorneringPerformance() => UnityEngine.Random.Range(0.7f, 0.9f);
		public float GetRawSpeed() => _currentSpeed + UnityEngine.Random.Range(-2f, 2f);

		// Set per controlli fisici
		public void SetThrottlePosition(float throttle) => _throttlePosition = Mathf.Clamp01(throttle);
		public void SetBrakeForce(float brake) => _brakeForce = Mathf.Clamp01(brake);

		// Configurazione parametri motore basata su modalità guida
		private void ConfigureForDriveMode(DriveMode mode)
		{
			_maxRPM = mode switch
			{
				DriveMode.Eco => 5000f,      // Limitato per efficienza
				DriveMode.Comfort => 6500f,   // Bilanciato
				DriveMode.Sport => 7000f,     // Prestazioni massime
				_ => 6500f
			};
		}

		// Formula fisica RPM basata su velocità e rapporto marcia
		private float CalculateRPMForGear(float speed, int gear)
		{
			float[] gearRatios = { 0f, 3.5f, 2.1f, 1.4f, 1.0f, 0.8f, 0.65f }; // Rapporti realistici
			if (gear >= gearRatios.Length) return 800f;

			return (speed * gearRatios[gear] * 45f) + 800f; // Formula semplificata
		}

		// Damping response basato su modalità - Sport più reattivo
		private float GetDampingForMode()
		{
			return _currentDriveMode switch
			{
				DriveMode.Eco => 0.85f,      // Dolce
				DriveMode.Comfort => 0.75f,   // Medio
				DriveMode.Sport => 0.95f,     // Reattivo
				_ => 0.8f
			};
		}
	}

	// Enum modalità guida
	public enum DriveMode { Eco, Comfort, Sport }
}