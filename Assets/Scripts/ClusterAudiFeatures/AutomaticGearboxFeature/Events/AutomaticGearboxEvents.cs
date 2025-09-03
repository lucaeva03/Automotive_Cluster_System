using ClusterAudi;

namespace ClusterAudiFeatures
{
	// Aggiornamento configurazione AutomaticGearbox
	public class AutomaticGearboxConfigUpdateEvent
	{
		public AutomaticGearboxConfig NewConfiguration { get; }
		public DriveMode CurrentMode { get; }

		public AutomaticGearboxConfigUpdateEvent(AutomaticGearboxConfig config, DriveMode mode)
		{
			NewConfiguration = config;
			CurrentMode = mode;
		}
	}

	// Attraversamento soglie RPM
	public class RPMThresholdCrossedEvent
	{
		public float CurrentRPM { get; }
		public float Threshold { get; }
		public RPMZone Zone { get; }

		public RPMThresholdCrossedEvent(float rpm, float threshold, RPMZone zone)
		{
			CurrentRPM = rpm;
			Threshold = threshold;
			Zone = zone;
		}
	}

	// Enum zone RPM - (UI e audio)
	public enum RPMZone
	{
		Idle,    // 0-1000 RPM
		Normal,  // 1000-3000 RPM
		High,    // 3000-5000 RPM
		RedZone  // 5000+ RPM
	}
}