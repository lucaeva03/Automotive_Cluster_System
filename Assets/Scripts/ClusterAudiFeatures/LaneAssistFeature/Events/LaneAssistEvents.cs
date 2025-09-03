namespace ClusterAudiFeatures
{

	// Rilevazione Lane Assist
	public class LaneDepartureDetectedEvent
	{
		public LaneAssistData.LaneDepartureType DepartureType { get; }
		public float DepartureTime { get; }

		public LaneDepartureDetectedEvent(LaneAssistData.LaneDepartureType departureType, float departureTime)
		{
			DepartureType = departureType;
			DepartureTime = departureTime;
		}
	}

	// Reset Lane assist
	public class LaneDepartureResetEvent
	{
		public LaneDepartureResetEvent() { }
	}

	// Richiesta riproduzione audio
	public class LaneAssistAudioRequestEvent
	{
		public string AudioPath { get; }
		public float Volume { get; }
		public int Priority { get; }
		public LaneAssistData.LaneDepartureType DepartureType { get; }

		public LaneAssistAudioRequestEvent(string audioPath, LaneAssistData.LaneDepartureType departureType)
		{
			AudioPath = audioPath;
			Volume = LaneAssistData.LANE_ASSIST_AUDIO_VOLUME;
			Priority = LaneAssistData.LANE_ASSIST_AUDIO_PRIORITY;
			DepartureType = departureType;
		}
	}
}