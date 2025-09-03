using ClusterAudi;

namespace ClusterAudiFeatures
{
	// Transizione stato dal Drive Mode UI alla State Machine
	public class ClusterDriveModeStateTransitionRequest
	{
		public string TargetState { get; }

		public ClusterDriveModeStateTransitionRequest(string targetState)
		{
			TargetState = targetState;
		}
	}

	// Aggiornamento tema Drive Mode (colori e stili)
	public class ClusterDriveModeThemeUpdateEvent
	{
		public DriveMode CurrentMode { get; }
		public UnityEngine.Color PrimaryColor { get; }
		public UnityEngine.Color SecondaryColor { get; }

		public ClusterDriveModeThemeUpdateEvent(DriveMode mode, UnityEngine.Color primary, UnityEngine.Color secondary)
		{
			CurrentMode = mode;
			PrimaryColor = primary;
			SecondaryColor = secondary;
		}
	}
}