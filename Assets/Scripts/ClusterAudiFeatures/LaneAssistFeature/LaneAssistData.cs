using UnityEngine;

namespace ClusterAudiFeatures
{
	// Configurazioni e costanti per la feature LaneAssist
	public static class LaneAssistData
	{
		// Path prefab
		public const string LANE_ASSIST_PREFAB_PATH = "LaneAssist/LaneAssistPrefab";

		// Velocità minima attivazione
		public const float MIN_ACTIVATION_SPEED = 30f;

		// Tempo tasti A/D rilevamento
		public const float LANE_DEPARTURE_TIME_THRESHOLD = 2.0f;

		// Tempo auto-reset dopo rilascio tasti
		public const float AUTO_RESET_TIME = 1.0f;

		// Path audio
		public const string LANE_DEPARTURE_LEFT_AUDIO_PATH = "Audio/SFX/LaneAssist/LaneDepartureLeft";
		public const string LANE_DEPARTURE_RIGHT_AUDIO_PATH = "Audio/SFX/LaneAssist/LaneDepartureRight";
		public const float LANE_ASSIST_AUDIO_VOLUME = 0.8f;
		public const int LANE_ASSIST_AUDIO_PRIORITY = 4;

		// Colori stati linee
		public static readonly Color NORMAL_LANE_COLOR = new Color(1f, 1f, 1f, 0.8f);
		public static readonly Color DEPARTURE_LANE_COLOR = new Color(1f, 0.8f, 0f, 1f);
		public const float CAR_ICON_SHIFT_DISTANCE = 20f;

		// Tasti simulazione
		public static readonly KeyCode LEFT_DEPARTURE_KEY = KeyCode.A;
		public static readonly KeyCode RIGHT_DEPARTURE_KEY = KeyCode.D;

		// Verifica attivazione sistema (velocità)
		public static bool CanActivateLaneAssist(float currentSpeed)
		{
			return currentSpeed >= MIN_ACTIVATION_SPEED;
		}

		// Verifica attivazione sistema (tasto)
		public static bool IsLaneDeparture(float holdTime)
		{
			return holdTime >= LANE_DEPARTURE_TIME_THRESHOLD;
		}

		// Enum linea rilevata
		public enum LaneDepartureType
		{
			None,
			Left,
			Right
		}

		// Enum stato generale sistema
		public enum LaneAssistState
		{
			Disabled,
			Active,
			Warning
		}
	}
}