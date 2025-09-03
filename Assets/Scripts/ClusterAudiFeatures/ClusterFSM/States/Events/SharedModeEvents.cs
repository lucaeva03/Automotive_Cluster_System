using ClusterAudi;

namespace ClusterAudiFeatures
{
	// Cambio modalità guida
	public class DriveModeChangedEvent
	{
		public DriveMode NewMode { get; }

		public DriveModeChangedEvent(DriveMode newMode)
		{
			NewMode = newMode;
		}
	}
}