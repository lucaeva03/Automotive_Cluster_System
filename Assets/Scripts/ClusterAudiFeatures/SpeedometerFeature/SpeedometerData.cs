using UnityEngine;

namespace ClusterAudiFeatures
{
	// Configurazioni e costanti per la feature Speedometer
	public static class SpeedometerData
	{
		// Path prefab
		public const string SPEEDOMETER_PREFAB_PATH = "Speedometer/SpeedometerPrefab";

		// Soglie velocità cambio colori
		public const float LOW_SPEED_THRESHOLD = 30f;      // km/h
		public const float MEDIUM_SPEED_THRESHOLD = 90f;   // km/h  
		public const float HIGH_SPEED_THRESHOLD = 130f;    // km/h
		public const float DANGER_SPEED_THRESHOLD = 180f;  // km/h
		public const float MAX_DISPLAY_SPEED = 200f;       // km/h

		// Colori fasce di velocità
		public static readonly Color LOW_SPEED_COLOR = new Color(0.2f, 0.8f, 0.2f, 1f);    // Verde
		public static readonly Color MEDIUM_SPEED_COLOR = new Color(0.9f, 0.9f, 0.2f, 1f); // Giallo
		public static readonly Color HIGH_SPEED_COLOR = new Color(0.9f, 0.6f, 0.1f, 1f);   // Arancione
		public static readonly Color DANGER_SPEED_COLOR = new Color(0.9f, 0.1f, 0.1f, 1f); // Rosso
		public static readonly Color DEFAULT_SPEED_COLOR = Color.white;

		// Fattori diverse modalità di guida
		public const float ECO_RESPONSE_DAMPING = 0.3f;
		public const float COMFORT_RESPONSE_DAMPING = 0.2f;
		public const float SPORT_RESPONSE_DAMPING = 0.05f;

		// Unità velocità
		public const string SPEED_UNIT_LABEL = "km/h";

		// Configurazioni predefinite per modalità guida
		public static readonly SpeedometerConfig ECO_CONFIG = new SpeedometerConfig
		{
			MaxDisplaySpeed = 120f,
			ResponseDamping = ECO_RESPONSE_DAMPING
		};

		public static readonly SpeedometerConfig COMFORT_CONFIG = new SpeedometerConfig
		{
			MaxDisplaySpeed = MAX_DISPLAY_SPEED,
			ResponseDamping = COMFORT_RESPONSE_DAMPING
		};

		public static readonly SpeedometerConfig SPORT_CONFIG = new SpeedometerConfig
		{
			MaxDisplaySpeed = MAX_DISPLAY_SPEED,
			ResponseDamping = SPORT_RESPONSE_DAMPING
		};

		// Colore adatto in base a velocità
		public static Color GetSpeedColor(float speed)
		{
			if (speed >= DANGER_SPEED_THRESHOLD) return DANGER_SPEED_COLOR;
			if (speed >= HIGH_SPEED_THRESHOLD) return HIGH_SPEED_COLOR;
			if (speed >= MEDIUM_SPEED_THRESHOLD) return MEDIUM_SPEED_COLOR;
			if (speed >= LOW_SPEED_THRESHOLD) return LOW_SPEED_COLOR;
			return DEFAULT_SPEED_COLOR;
		}

		// Configurazione per modalità attiva
		public static SpeedometerConfig GetConfigForDriveMode(ClusterAudi.DriveMode mode)
		{
			return mode switch
			{
				ClusterAudi.DriveMode.Eco => ECO_CONFIG,
				ClusterAudi.DriveMode.Sport => SPORT_CONFIG,
				_ => COMFORT_CONFIG
			};
		}

		// Formatta velocità come stringa
		public static string FormatSpeed(float speed)
		{
			return speed.ToString("F0");
		}
	}

	// Configurazione speedometer
	[System.Serializable]
	public class SpeedometerConfig
	{
		public float MaxDisplaySpeed;
		public float ResponseDamping;
	}
}