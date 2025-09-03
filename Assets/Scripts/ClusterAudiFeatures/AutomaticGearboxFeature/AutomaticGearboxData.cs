using UnityEngine;

namespace ClusterAudiFeatures
{
	public static class AutomaticGearboxData
	{
		// Path prefab
		public const string AutomaticGearbox_PREFAB_PATH = "AutomaticGearbox/AutomaticGearboxPrefab";

		// Configurazioni RPM per modalità
		public const float MAX_DISPLAY_RPM = 7000f;
		public const float ECO_MAX_RPM = 5000f;        // Limitato
		public const float COMFORT_MAX_RPM = 6500f;    // Bilanciato
		public const float SPORT_MAX_RPM = 7000f;      // Prestazioni massime

		// RPM di riferimento
		public const float IDLE_RPM = 800f;
		public const float REDZONE_RPM = 6500f;
		public const float OPTIMAL_SHIFT_RPM = 3000f;

		// Unità
		public const string RPM_UNIT_LABEL = "RPM";

		// Colori zone RPM
		public static readonly Color DEFAULT_RPM_COLOR = Color.white;
		public static readonly Color IDLE_RPM_COLOR = new Color(0.8f, 0.8f, 0.8f, 1f);
		public static readonly Color NORMAL_RPM_COLOR = new Color(0.2f, 0.8f, 0.2f, 1f);  // Verde
		public static readonly Color HIGH_RPM_COLOR = new Color(1f, 0.8f, 0f, 1f);        // Giallo
		public static readonly Color REDZONE_RPM_COLOR = new Color(1f, 0.2f, 0.2f, 1f);   // Rosso

		// Colori marce
		public static readonly Color GEAR_ACTIVE_COLOR = Color.white;
		public static readonly Color GEAR_INACTIVE_COLOR = Color.gray;

		// Fattori modalità guida
		public const float ECO_RESPONSE_DAMPING = 0.3f;      // Dolce
		public const float COMFORT_RESPONSE_DAMPING = 0.2f;   // Medio
		public const float SPORT_RESPONSE_DAMPING = 0.05f;    // Reattivo

		// Colore RPM basato su valore corrente
		public static Color GetRPMColor(float rpm)
		{
			if (rpm >= REDZONE_RPM) return REDZONE_RPM_COLOR;
			if (rpm >= OPTIMAL_SHIFT_RPM + 1000f) return HIGH_RPM_COLOR;
			if (rpm >= IDLE_RPM + 500f) return NORMAL_RPM_COLOR;
			return IDLE_RPM_COLOR;
		}

		// Configurazione per modalità guida specifica
		public static AutomaticGearboxConfig GetConfigForDriveMode(ClusterAudi.DriveMode mode)
		{
			return mode switch
			{
				ClusterAudi.DriveMode.Eco => new AutomaticGearboxConfig
				{
					MaxDisplayRPM = ECO_MAX_RPM,
					ResponseDamping = ECO_RESPONSE_DAMPING,
					ShowRedZoneWarning = false
				},
				ClusterAudi.DriveMode.Sport => new AutomaticGearboxConfig
				{
					MaxDisplayRPM = SPORT_MAX_RPM,
					ResponseDamping = SPORT_RESPONSE_DAMPING,
					ShowRedZoneWarning = true
				},
				_ => new AutomaticGearboxConfig // Comfort default
				{
					MaxDisplayRPM = COMFORT_MAX_RPM,
					ResponseDamping = COMFORT_RESPONSE_DAMPING,
					ShowRedZoneWarning = false
				}
			};
		}

		// Eliminazione decimali RPM su display
		public static string FormatRPM(float rpm)
		{
			return rpm.ToString("F0");
		}

		// Formatta marcia per display (P-1-2-3-4-5-6)
		public static string FormatGear(int gear)
		{
			return gear switch
			{
				0 => "P",   // Park
				_ => gear.ToString()
			};
		}
	}

	// Configurazione AutomaticGearbox
	[System.Serializable]
	public class AutomaticGearboxConfig
	{
		public float MaxDisplayRPM;
		public float ResponseDamping;
		public bool ShowRedZoneWarning;
	}
}