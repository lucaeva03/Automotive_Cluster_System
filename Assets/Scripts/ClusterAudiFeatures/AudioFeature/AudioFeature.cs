using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public class AudioFeature : BaseFeature, IAudioFeature, IAudioFeatureInternal
	{
		// Riferimento Monobehaviour Audio
		private AudioSpeakerBehaviour _audioSpeaker;
		// Volume globale
		private float _masterVolume = 1f;
		// Variabile abilitazione sistema audio
		private bool _audioSystemEnabled = true;

		public AudioFeature(Client client) : base(client)
		{
			Debug.Log("[AUDIO FEATURE] AudioFeature inizializzata");

			// Sottoscrizione eventi audio da altre Features
			_broadcaster.Add<PlaySeatBeltAudioEvent>(OnPlaySeatBeltAudio);
			_broadcaster.Add<DoorLockAudioRequestEvent>(OnPlayDoorLockAudio);
			_broadcaster.Add<LaneAssistAudioRequestEvent>(OnPlayLaneAssistAudio);
			_broadcaster.Add<WelcomeAudioRequestEvent>(OnPlayWelcomeAudio);
		}

		// Carica e inizializza AudioSpeaker da prefab
		public async Task InstantiateAudioFeature()
		{
			Debug.Log("[AUDIO FEATURE] Istanziazione Audio Speaker...");

			try
			{
				// Caricamento prefab e verifica
				var instance = await _assetService.InstantiateAsset<AudioSpeakerBehaviour>(
					AudioData.AUDIO_SPEAKER_PREFAB_PATH);

				if (instance != null)
				{
					instance.Initialize(this);
					_audioSpeaker = instance;
					Debug.Log("[AUDIO FEATURE] Audio Speaker caricato da prefab");
				}
				else
				{
					Debug.LogError("[AUDIO FEATURE] Prefab AudioSpeaker non trovato!");
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[AUDIO FEATURE] Errore: {ex.Message}");
			}
		}

		// Riproduzione audio con controllo priorità
		public void PlayAudioClip(string clipPath, float volume = 1f, int priority = 1)
		{
			if (!_audioSystemEnabled || _audioSpeaker == null) return;

			float finalVolume = volume * _masterVolume;
			_audioSpeaker.PlayClip(clipPath, finalVolume, priority);
		}

		// Controllo volume generale
		public void SetMasterVolume(float volume)
		{
			_masterVolume = Mathf.Clamp01(volume);
		}

		// Accesso client per MonoBehaviour
		public Client GetClient() => _client;

		// Gestione eventi - riproduzione attraverso PlaAudioClip
		private void OnPlaySeatBeltAudio(PlaySeatBeltAudioEvent e)
		{
			PlayAudioClip(e.AudioClipPath, e.Volume, e.Priority);
		}

		private void OnPlayDoorLockAudio(DoorLockAudioRequestEvent e)
		{
			PlayAudioClip(e.AudioPath, e.Volume, e.Priority);
		}

		private void OnPlayLaneAssistAudio(LaneAssistAudioRequestEvent e)
		{
			PlayAudioClip(e.AudioPath, e.Volume, e.Priority);
		}

		private void OnPlayWelcomeAudio(WelcomeAudioRequestEvent e)
		{
			PlayAudioClip(e.AudioPath, e.Volume, e.Priority);
		}

		// Pulizia eventi quando AudioFeature viene distrutta
		~AudioFeature()
		{
			if (_broadcaster != null)
			{
				_broadcaster.Remove<PlaySeatBeltAudioEvent>(OnPlaySeatBeltAudio);
				_broadcaster.Remove<DoorLockAudioRequestEvent>(OnPlayDoorLockAudio);
				_broadcaster.Remove<LaneAssistAudioRequestEvent>(OnPlayLaneAssistAudio);
				_broadcaster.Remove<WelcomeAudioRequestEvent>(OnPlayWelcomeAudio);
			}
		}
	}
}