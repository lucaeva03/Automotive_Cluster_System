namespace ClusterAudiFeatures
{
	// Transizione da Welcome a stato FSM
	public class WelcomeTransitionEvent
	{
		public string TargetState { get; }

		public WelcomeTransitionEvent(string targetState)
		{
			TargetState = targetState;
		}
	}

	// Richiesta riproduzione audio
	public class WelcomeAudioRequestEvent
	{
		public string AudioPath { get; }
		public float Volume { get; }
		public int Priority { get; }

		public WelcomeAudioRequestEvent(string audioPath, float volume, int priority)
		{
			AudioPath = audioPath;
			Volume = volume;
			Priority = priority;
		}
	}
}