using System;

namespace ClusterAudi
{
	// Interfaccia per dati veicolo - centralizza stato auto (velocità, RPM, marcia, modalità)
	public interface IVehicleDataService : IService
	{
		// Dati correnti veicolo - read-only da esterno
		float CurrentSpeed { get; }
		float CurrentRPM { get; }
		int CurrentGear { get; }
		DriveMode CurrentDriveMode { get; }
		bool IsEngineRunning { get; }

		// Eventi per notificare cambiamenti
		event Action<float> OnSpeedChanged;
		event Action<float> OnRPMChanged;
		event Action<int> OnGearChanged;
		event Action<DriveMode> OnDriveModeChanged;

		// Metodi per modificare stato veicolo
		void SetSpeed(float speed);
		void SetRPM(float rpm);
		void SetGear(int gear);
		void SetDriveMode(DriveMode mode);
		void SetEngineRunning(bool isRunning);

		// Calcoli simulazione eventi realistici - POSSIBILI MIGLIORIE
		float GetCurrentConsumption();
		float GetEstimatedRange();
		float GetThrottlePosition();
		float GetBrakeForce();
		float GetAcceleration();
		float GetMaxRPM();
		float GetDrivingEfficiency();
		float GetAccelerationSmoothness();
		float GetSpeedStability();
		float GetGearUsageOptimality();
		float GetRPMEfficiency();
		float GetAccelerationControl();
		float GetCorneringPerformance();
		float GetRawSpeed();
		void SetThrottlePosition(float throttle);
		void SetBrakeForce(float brake);

		// Logica cambio automatico - POSSIBILI MIGLIORIE
		bool ShouldShiftUp();
		bool ShouldShiftDown();
		bool IsInRedZone();
		float GetRedZoneThreshold();
		float GetOptimalShiftPointRPM();
	}
}