namespace ClusterAudiFeatures
{
	// Cambio stato bloccato/sbloccato
	public class DoorLockStateChangedEvent
	{
		public bool IsLocked { get; }
		public bool PreviousState { get; }

		public DoorLockStateChangedEvent(bool isLocked, bool previousState)
		{
			IsLocked = isLocked;
			PreviousState = previousState;
		}
	}

	// Richiesta riproduzione audio
	public class DoorLockAudioRequestEvent
	{
		public string AudioPath { get; }
		public float Volume { get; }
		public int Priority { get; }
		public bool IsLocking { get; }

		public DoorLockAudioRequestEvent(string audioPath, float volume, int priority, bool isLocking)
		{
			AudioPath = audioPath;
			Volume = volume;
			Priority = priority;
			IsLocking = isLocking;
		}
	}
}