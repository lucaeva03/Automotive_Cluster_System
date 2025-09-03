using ClusterAudi;

namespace ClusterAudiFeatures
{
	// Cambio modalit� guida
	public class DriveModeChangedEvent
	{
		public DriveMode NewMode { get; }

		public DriveModeChangedEvent(DriveMode newMode)
		{
			NewMode = newMode;
		}
	}
}