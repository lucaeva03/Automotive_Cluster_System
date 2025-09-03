using ClusterAudi;
using System.Threading.Tasks;

namespace ClusterAudiFeatures
{
	// Interfaccia pubblica per sistema audio
	public interface IAudioFeature : IFeature
	{
		// Istanzia componente audio del cluster
		Task InstantiateAudioFeature();

		// Riproduce clip audio con volume e priorità personalizzabili
		void PlayAudioClip(string clipPath, float volume = 1f, int priority = 1);

		// Controllo volume master di tutto il sistema
		void SetMasterVolume(float volume);
	}
}