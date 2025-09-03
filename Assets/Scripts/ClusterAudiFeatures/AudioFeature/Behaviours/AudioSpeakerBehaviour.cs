using ClusterAudi;
using System.Collections;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public class AudioSpeakerBehaviour : BaseMonoBehaviour<IAudioFeatureInternal>
	{
		// Componente per riproduzione audio
		[SerializeField] private AudioSource _audioSource;

		// Riproduzione, priorità e coroutine (riproduzione asincrona)
		private bool _isPlaying = false;
		private int _currentPriority = 0;
		private Coroutine _playbackCoroutine;

		protected override void ManagedAwake()
		{
			Debug.Log("[AUDIO SPEAKER] AudioSpeaker inizializzato");

			// Validazione AudioSource assegnato a prefab
			if (_audioSource == null)
			{
				Debug.LogError("[AUDIO SPEAKER] AudioSource non assegnato nel prefab!");
				return;
			}

			// Configurazione iniziale AudioSource
			_audioSource.playOnAwake = false;
			_audioSource.loop = false;
		}

		// Pulizia quando viene distrutto
		protected override void ManagedOnDestroy()
		{
			StopCurrentClip();
		}

		// Riproduce clip con sistema priorità
		public void PlayClip(string clipPath, float volume, int priority)
		{
			// SISTEMA PRIORITÀ: Audio con priorità più alta interrompe quello corrente
			if (_isPlaying && priority <= _currentPriority)
			{
				Debug.Log($"[AUDIO SPEAKER] Audio ignorato (priorità {priority} <= {_currentPriority})");
				return;
			}

			// Stop clip corrente se necessario
			if (_isPlaying) StopCurrentClip();

			// Avvia nuovo clip
			_playbackCoroutine = StartCoroutine(PlayClipCoroutine(clipPath, volume, priority));
		}

		// Stop riproduzione clip
		public void StopCurrentClip()
		{
			// Stop coroutine se attiva
			if (_playbackCoroutine != null)
			{
				StopCoroutine(_playbackCoroutine);
				_playbackCoroutine = null;
			}

			// stop Audio se attivo
			if (_audioSource?.isPlaying == true)
				_audioSource.Stop();

			// Reset stato
			_isPlaying = false;
			_currentPriority = 0;
		}

		// Coroutine riproduzione
		private IEnumerator PlayClipCoroutine(string clipPath, float volume, int priority)
		{

			// Carica clip
			AudioClip clip = Resources.Load<AudioClip>(clipPath);
			if (clip == null)
			{
				Debug.LogError($"[AUDIO SPEAKER] Clip non trovato: {clipPath}");
				yield break;
			}

			// Setup riproduzione
			_audioSource.clip = clip;
			_audioSource.volume = volume;
			_currentPriority = priority;
			_isPlaying = true;

			// Avvia riproduzione
			_audioSource.Play();
			Debug.Log($"[AUDIO SPEAKER] Playing: {clipPath} (priorità: {priority})");

			// Attendi fine
			yield return new WaitUntil(() => !_audioSource.isPlaying);

			// Cleanup
			_isPlaying = false;
			_currentPriority = 0;
			_playbackCoroutine = null;
		}
	}
}