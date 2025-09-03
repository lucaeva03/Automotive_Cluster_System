using UnityEngine;

namespace ClusterAudiFeatures
{

	// Configurazione aggiornata per modalit�
	public class SpeedometerConfigUpdatedEvent
	{
		public SpeedometerConfig NewConfiguration { get; }
		public ClusterAudi.DriveMode DriveMode { get; }

		public SpeedometerConfigUpdatedEvent(SpeedometerConfig config, ClusterAudi.DriveMode mode)
		{
			NewConfiguration = config;
			DriveMode = mode;
		}
	}

	// Velocit� rossa
	public class DangerousSpeedReachedEvent
	{
		public float CurrentSpeed { get; }
		public float DangerThreshold { get; }

		public DangerousSpeedReachedEvent(float speed, float threshold)
		{
			CurrentSpeed = speed;
			DangerThreshold = threshold;
		}
	}

	// Dati aggiornati
	public class SpeedometerMetricsUpdateEvent
	{
		public float CurrentSpeed { get; set; }
		public float DisplayedSpeed { get; set; }  // Velocit� con smooth damping
		public float MaxSpeedReached { get; set; }
		public Color CurrentSpeedColor { get; set; }
		public float ResponseLatency { get; set; }  // Tempo di risposta smoothing
	}

	// Richiesta cambio configurazione
	public class SpeedometerConfigChangeRequest
	{
		public string ConfigurationType { get; }
		public object ConfigurationValue { get; }

		public SpeedometerConfigChangeRequest(string type, object value)
		{
			ConfigurationType = type;
			ConfigurationValue = value;
		}
	}
}