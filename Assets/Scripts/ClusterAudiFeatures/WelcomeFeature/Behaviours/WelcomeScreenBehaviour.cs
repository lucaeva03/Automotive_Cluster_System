using ClusterAudi;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ClusterAudiFeatures
{
	public class WelcomeScreenBehaviour : BaseMonoBehaviour<IWelcomeFeatureInternal>
	{
		// Componenti prefab
		[Header("Welcome Screen UI")]
		[SerializeField] private CanvasGroup _welcomeCanvasGroup;
		[SerializeField] private Image _audiLogo;
		[SerializeField] private TextMeshProUGUI _welcomeText;
		[SerializeField] private TextMeshProUGUI _timerText;
		[SerializeField] private Slider _progressSlider;

		// Stato interno e timer
		private float _welcomeTimer = 0f;
		private bool _isActive = true;
		private IBroadcaster _broadcaster;

		protected override void ManagedAwake()
		{
			Debug.Log("[WELCOME SCREEN] WelcomeScreenBehaviour inizializzato");

			// Broadcaster per transizione
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();

			// UI iniziale
			SetupUI();
		}

		protected override void ManagedStart()
		{
			Debug.Log("[WELCOME SCREEN] Avvio Welcome Screen...");

			// Sequenza Welcome
			StartCoroutine(WelcomeSequence());
		}

		protected override void ManagedUpdate()
		{
			if (!_isActive) return;

			// Aggiorna slider in base a timer
			if (_progressSlider != null)
				_progressSlider.value = _welcomeTimer;

		}

		// Configura stato iniziale componenti UI
		private void SetupUI()
		{
			// Setup canvas group per fade in/out
			if (_welcomeCanvasGroup == null)
				_welcomeCanvasGroup = GetComponentInParent<CanvasGroup>();

			if (_welcomeCanvasGroup != null)
				_welcomeCanvasGroup.alpha = 1f;

			// Logo inizia trasparente per fade in animato
			if (_audiLogo != null)
			{
				var color = _audiLogo.color;
				color.a = 0f;
				_audiLogo.color = color;
			}

			// Progress slider per visualizzare countdown
			if (_progressSlider != null)
			{
				_progressSlider.minValue = 0f;
				_progressSlider.maxValue = WelcomeData.WELCOME_SCREEN_DURATION;
				_progressSlider.value = 0f;
			}
		}

		// Sequenza Welcome completa
		private IEnumerator WelcomeSequence()
		{
			// Fade in del logo Audi
			yield return StartCoroutine(FadeLogo(0f, 1f, 1f));

			// Riproduzione audio dopo Fade-in
			RequestWelcomeAudio();

			// Loop timer per animazione
			while (_welcomeTimer < WelcomeData.WELCOME_SCREEN_DURATION && _isActive)
			{
				_welcomeTimer += Time.deltaTime;

				// Pulse logo ogni secondo
				if (Mathf.FloorToInt(_welcomeTimer) != Mathf.FloorToInt(_welcomeTimer - Time.deltaTime))
				{
					StartCoroutine(SimpleLogoPulse());
				}

				yield return null;
			}

			// Transizione automatica a Comfort Mode
			if (_isActive)
			{
				Debug.Log("[WELCOME SCREEN] Timer completato - transizione automatica");
				TransitionToComfort();
			}
		}

		// Animazione fade per logo con durata
		private IEnumerator FadeLogo(float fromAlpha, float toAlpha, float duration)
		{
			if (_audiLogo == null) yield break;

			float elapsed = 0f;
			Color color = _audiLogo.color;

			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				color.a = Mathf.Lerp(fromAlpha, toAlpha, elapsed / duration);
				_audiLogo.color = color;
				yield return null;
			}

			color.a = toAlpha;
			_audiLogo.color = color;
		}

		// Effetto pulse per logo
		private IEnumerator SimpleLogoPulse()
		{
			yield return StartCoroutine(FadeLogo(1f, 0.3f, 0.2f));
			yield return StartCoroutine(FadeLogo(0.3f, 1f, 0.2f));
		}

		// Transizione a Comfort Mode e Pulizia
		private void TransitionToComfort()
		{
			if (!_isActive) return;
			_isActive = false;

			Debug.Log("[WELCOME SCREEN] Transizione a Comfort Mode");
			_broadcaster.Broadcast(new WelcomeTransitionEvent(WelcomeData.COMFORT_MODE_STATE));

			StartCoroutine(FadeOutAndDestroy());
		}

		// Fade out completo e Pulizia
		private IEnumerator FadeOutAndDestroy()
		{
			// Fade out
			if (_welcomeCanvasGroup != null)
			{
				float elapsed = 0f;
				float duration = 1f;

				while (elapsed < duration)
				{
					elapsed += Time.deltaTime;
					_welcomeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
					yield return null;
				}
			}

			Debug.Log("[WELCOME SCREEN] Welcome Screen completato");
			Destroy(gameObject);
		}

		// Richiesta riproduzione audio
		private void RequestWelcomeAudio()
		{
			var audioEvent = new WelcomeAudioRequestEvent(
				WelcomeData.WELCOME_SOUND_PATH,
				WelcomeData.WELCOME_AUDIO_VOLUME,
				WelcomeData.WELCOME_AUDIO_PRIORITY
			);
			_broadcaster.Broadcast(audioEvent);
			Debug.Log("[WELCOME SCREEN] Audio welcome richiesto");
		}
	}
}