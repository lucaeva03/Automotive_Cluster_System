namespace ClusterAudiFeatures
{

	// Cambio stato cintura
	public class SeatBeltStatusChangedEvent
	{
		public SeatBeltData.SeatBeltPosition Position { get; }
		public SeatBeltData.SeatBeltStatus OldStatus { get; }
		public SeatBeltData.SeatBeltStatus NewStatus { get; }

		public SeatBeltStatusChangedEvent(
			SeatBeltData.SeatBeltPosition position,
			SeatBeltData.SeatBeltStatus oldStatus,
			SeatBeltData.SeatBeltStatus newStatus)
		{
			Position = position;
			OldStatus = oldStatus;
			NewStatus = newStatus;
		}
	}

	// Avvio Warning
	public class SeatBeltWarningStartedEvent
	{
		public float CurrentSpeed { get; }
		public float SpeedThreshold { get; }
		public SeatBeltData.SeatBeltPosition[] UnfastenedBelts { get; }

		public SeatBeltWarningStartedEvent(
			float currentSpeed,
			float speedThreshold,
			SeatBeltData.SeatBeltPosition[] unfastenedBelts)
		{
			CurrentSpeed = currentSpeed;
			SpeedThreshold = speedThreshold;
			UnfastenedBelts = unfastenedBelts;
		}
	}

	// Termine warning
	public class SeatBeltWarningStoppedEvent
	{
		public float TotalWarningDuration { get; }

		public SeatBeltWarningStoppedEvent(float duration)
		{
			TotalWarningDuration = duration;
		}
	}

	// Richiesta riproduzione audio
	public class PlaySeatBeltAudioEvent
	{
		public string AudioClipPath { get; }
		public float Volume { get; }
		public int Priority { get; }

		public PlaySeatBeltAudioEvent(string audioClipPath, float volume = 1f, int priority = 5)
		{
			AudioClipPath = audioClipPath;
			Volume = volume;
			Priority = priority;
		}
	}

	// Richiesta stop audio
	public class StopSeatBeltAudioEvent
	{
		public StopSeatBeltAudioEvent() { }
	}

	// Lampeggio cinture
	public class SeatBeltFlashIconsEvent
	{
		public SeatBeltData.SeatBeltPosition[] PositionsToFlash { get; }
		public bool StartFlashing { get; }
		public float FlashInterval { get; }

		public SeatBeltFlashIconsEvent(
			SeatBeltData.SeatBeltPosition[] positions,
			bool startFlashing,
			float flashInterval = 1f) // 1 secondo
		{
			PositionsToFlash = positions ?? new SeatBeltData.SeatBeltPosition[0];
			StartFlashing = startFlashing;
			FlashInterval = flashInterval;
		}
	}
}