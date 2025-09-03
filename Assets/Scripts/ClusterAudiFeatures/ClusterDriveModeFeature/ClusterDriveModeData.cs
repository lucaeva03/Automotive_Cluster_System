using UnityEngine;

namespace ClusterAudiFeatures
{
	public static class ClusterDriveModeData
	{
		// Path prefab
		public const string CLUSTER_DRIVE_MODE_PREFAB_PATH = "ClusterDriveMode/ClusterDriveModePrefab";

		// Nomi stati FSM
		public const string ECO_MODE_STATE = "EcoModeState";
		public const string COMFORT_MODE_STATE = "ComfortModeState";
		public const string SPORT_MODE_STATE = "SportModeState";
		public const string WELCOME_STATE = "WelcomeState";

		// Tasti transizioni modalità
		public static readonly KeyCode DEBUG_ECO_KEY = KeyCode.F1;
		public static readonly KeyCode DEBUG_COMFORT_KEY = KeyCode.F2;
		public static readonly KeyCode DEBUG_SPORT_KEY = KeyCode.F3;
		public static readonly KeyCode DEBUG_WELCOME_KEY = KeyCode.F4;

		// Animazioni UI
		public const float ANIMATION_DURATION = 0.5f; // Aumentato per transizioni più smooth
		public const float BACKGROUND_TRANSITION_DURATION = 0.4f;

		// Colori per modalità guida
		public static readonly Color ECO_COLOR = new Color(0.2f, 0.8f, 0.2f, 1f);        // Verde Eco
		public static readonly Color COMFORT_COLOR = new Color(0.2f, 0.5f, 0.8f, 1f);    // Blu Comfort  
		public static readonly Color SPORT_COLOR = new Color(0.9f, 0.1f, 0.1f, 1f);      // Rosso Sport
		public static readonly Color INACTIVE_COLOR = new Color(0.3f, 0.3f, 0.3f, 0.5f); // Grigio Inactive
		public static readonly Color TEXT_COLOR = new Color(0.9f, 0.9f, 0.9f, 1f);       // Bianco Testi

		// Testo modalità guida
		public const string ECO_MODE_TEXT = "ECO";
		public const string COMFORT_MODE_TEXT = "COMFORT";
		public const string SPORT_MODE_TEXT = "SPORT";

		// Path cluster modalità
		public const string ECO_BACKGROUND_PATH = "ClusterDriveMode/img/ClusterECO";
		public const string COMFORT_BACKGROUND_PATH = "ClusterDriveMode/img/ClusterCOMFORT";
		public const string SPORT_BACKGROUND_PATH = "ClusterDriveMode/img/ClusterSPORT";

		// Verifica stato
		public static bool IsValidState(string stateName)
		{
			return stateName == ECO_MODE_STATE ||
				   stateName == COMFORT_MODE_STATE ||
				   stateName == SPORT_MODE_STATE ||
				   stateName == WELCOME_STATE;
		}

		// Ottiene path background
		public static string GetBackgroundPath(ClusterAudi.DriveMode mode)
		{
			return mode switch
			{
				ClusterAudi.DriveMode.Eco => ECO_BACKGROUND_PATH,
				ClusterAudi.DriveMode.Comfort => COMFORT_BACKGROUND_PATH,
				ClusterAudi.DriveMode.Sport => SPORT_BACKGROUND_PATH,
				_ => COMFORT_BACKGROUND_PATH // Default
			};
		}

		// Ottiene colore per modalità
		public static Color GetModeColor(ClusterAudi.DriveMode mode)
		{
			return mode switch
			{
				ClusterAudi.DriveMode.Eco => ECO_COLOR,
				ClusterAudi.DriveMode.Comfort => COMFORT_COLOR,
				ClusterAudi.DriveMode.Sport => SPORT_COLOR,
				_ => COMFORT_COLOR
			};
		}
	}
}