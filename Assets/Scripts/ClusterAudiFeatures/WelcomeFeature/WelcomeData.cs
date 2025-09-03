namespace ClusterAudiFeatures
{
	public static class WelcomeData
	{
		// Path prefab
		public const string WELCOME_SCREEN_PREFAB_PATH = "WelcomeScreen/WelcomeScreenPrefab";

		// Path audio welcome
		public const string WELCOME_SOUND_PATH = "Audio/SFX/Welcome/WelcomeSound";
		public const float WELCOME_AUDIO_VOLUME = 0.7f;
		public const int WELCOME_AUDIO_PRIORITY = 3;

		// Durata Welcome prima di transizione automatica
		public const float WELCOME_SCREEN_DURATION = 5f;

		// Nomi stati FSM
		public const string ECO_MODE_STATE = "EcoModeState";
		public const string COMFORT_MODE_STATE = "ComfortModeState";
		public const string SPORT_MODE_STATE = "SportModeState";
		public const string WELCOME_STATE = "WelcomeState";

		// Tasti per transizioni
		public static readonly UnityEngine.KeyCode DEBUG_ECO_KEY = UnityEngine.KeyCode.F1;
		public static readonly UnityEngine.KeyCode DEBUG_COMFORT_KEY = UnityEngine.KeyCode.F2;
		public static readonly UnityEngine.KeyCode DEBUG_SPORT_KEY = UnityEngine.KeyCode.F3;
		public static readonly UnityEngine.KeyCode DEBUG_WELCOME_KEY = UnityEngine.KeyCode.F4;

		// Verifica nome stato FSM
		public static bool IsValidState(string stateName)
		{
			return stateName == ECO_MODE_STATE ||
				   stateName == COMFORT_MODE_STATE ||
				   stateName == SPORT_MODE_STATE ||
				   stateName == WELCOME_STATE;
		}
	}
}