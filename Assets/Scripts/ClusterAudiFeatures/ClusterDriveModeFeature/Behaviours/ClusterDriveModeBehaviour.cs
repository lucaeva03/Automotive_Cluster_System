using ClusterAudi;
using ClusterAudiFeatures;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClusterAudiFeatures
{
	public class ClusterDriveModeBehaviour : BaseMonoBehaviour<IClusterDriveModeFeatureInternal>
	{
		// Componenti prefab
		[Header("Mode Indicator Images")]
		[SerializeField] private Image _ecoModeIndicator;
		[SerializeField] private Image _comfortModeIndicator;
		[SerializeField] private Image _sportModeIndicator;

		[Header("Mode Text Labels")]
		[SerializeField] private TextMeshProUGUI _ecoModeText;
		[SerializeField] private TextMeshProUGUI _comfortModeText;
		[SerializeField] private TextMeshProUGUI _sportModeText;

		[Header("BACKGROUND IMAGE")]
		[SerializeField] private Image _backgroundImage;

		// Servizi, stato veicolo e modalità iniziale
		private IBroadcaster _broadcaster;
		private IVehicleDataService _vehicleDataService;
		private DriveMode _currentDriveMode = DriveMode.Comfort;

		// Animazioni
		private Coroutine _modeTransitionCoroutine;
		private Coroutine _backgroundTransitionCoroutine;

		// Cache sprite background
		private Sprite _ecoBackgroundSprite;
		private Sprite _comfortBackgroundSprite;
		private Sprite _sportBackgroundSprite;
		private bool _backgroundResourcesLoaded = false;

		// Inizializzazione
		protected override void ManagedAwake()
		{
			Debug.Log("[CLUSTER DRIVE MODE] ClusterDriveModeBehaviour inizializzato");

			// Servizi Client 
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();
			_vehicleDataService = client.Services.Get<IVehicleDataService>();

			// Impostazione UI e risorse
			ValidateUIComponents();
			SubscribeToEvents();
			SetupInitialUI();
			LoadBackgroundResources();
		}

		// Avvio - modalità corrente del veicolo
		protected override void ManagedStart()
		{
			Debug.Log("[CLUSTER DRIVE MODE] Cluster Drive Mode UI avviata");

			// Modalità corrente
			var initialMode = _vehicleDataService.CurrentDriveMode;
			Debug.Log($"[CLUSTER DRIVE MODE] Modalità iniziale dal servizio: {initialMode}");

			// Reset per forzare aggiornamento
			_currentDriveMode = (DriveMode)(-1);

			// Aggiorna UI con modalità corrente
			UpdateModeDisplay(initialMode);
			ApplyBackgroundForMode(initialMode);
		}

		// Pulizia - Ferma animazioni e rimuovi eventi  
		protected override void ManagedOnDestroy()
		{
			Debug.Log("[CLUSTER DRIVE MODE] Cluster Drive Mode UI distrutta");

			UnsubscribeFromEvents();

			// Stop animazioni
			if (_modeTransitionCoroutine != null)
				StopCoroutine(_modeTransitionCoroutine);

			if (_backgroundTransitionCoroutine != null)
				StopCoroutine(_backgroundTransitionCoroutine);

			CleanupAnyRemainingOverlays();
		}

		// Validazione componenti UI essenziali
		private void ValidateUIComponents()
		{
			int missingComponents = 0;

			if (_ecoModeIndicator == null) { Debug.LogError("[CLUSTER DRIVE MODE] _ecoModeIndicator non assegnato nel prefab!"); missingComponents++; }
			if (_comfortModeIndicator == null) { Debug.LogError("[CLUSTER DRIVE MODE] _comfortModeIndicator non assegnato nel prefab!"); missingComponents++; }
			if (_sportModeIndicator == null) { Debug.LogError("[CLUSTER DRIVE MODE] _sportModeIndicator non assegnato nel prefab!"); missingComponents++; }

			if (_ecoModeText == null) { Debug.LogError("[CLUSTER DRIVE MODE] _ecoModeText non assegnato nel prefab!"); missingComponents++; }
			if (_comfortModeText == null) { Debug.LogError("[CLUSTER DRIVE MODE] _comfortModeText non assegnato nel prefab!"); missingComponents++; }
			if (_sportModeText == null) { Debug.LogError("[CLUSTER DRIVE MODE] _sportModeText non assegnato nel prefab!"); missingComponents++; }

			if (_backgroundImage == null) { Debug.LogError("[CLUSTER DRIVE MODE] _backgroundImage non assegnato nel prefab!"); missingComponents++; }

			if (missingComponents == 0)
			{
				Debug.Log("[CLUSTER DRIVE MODE] Tutti i componenti essenziali assegnati correttamente");
			}
			else
			{
				Debug.LogError($"[CLUSTER DRIVE MODE] {missingComponents} componenti mancanti!");
			}
		}

		private void SetupInitialUI()
		{
			Debug.Log("[CLUSTER DRIVE MODE] UI iniziale configurata");
		}


		// Carica tutti background
		private void LoadBackgroundResources()
		{
			Debug.Log("[CLUSTER DRIVE MODE] Caricamento risorse background...");

			try
			{
				_ecoBackgroundSprite = Resources.Load<Sprite>(ClusterDriveModeData.ECO_BACKGROUND_PATH);
				_comfortBackgroundSprite = Resources.Load<Sprite>(ClusterDriveModeData.COMFORT_BACKGROUND_PATH);
				_sportBackgroundSprite = Resources.Load<Sprite>(ClusterDriveModeData.SPORT_BACKGROUND_PATH);

				int loadedSprites = 0;
				if (_ecoBackgroundSprite != null) loadedSprites++;
				if (_comfortBackgroundSprite != null) loadedSprites++;
				if (_sportBackgroundSprite != null) loadedSprites++;

				if (loadedSprites == 3)
				{
					_backgroundResourcesLoaded = true;
					Debug.Log("[CLUSTER DRIVE MODE] Tutte le risorse background caricate con successo");
				}
				else
				{
					Debug.LogWarning($"[CLUSTER DRIVE MODE] Solo {loadedSprites}/3 background caricati");
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[CLUSTER DRIVE MODE] Errore caricamento background: {ex.Message}");
				_backgroundResourcesLoaded = false;
			}
		}

		// Applica background per modalità attuale con o senza animazione
		private void ApplyBackgroundForMode(DriveMode mode, bool animated = false)
		{
			if (_backgroundImage == null)
			{
				Debug.LogWarning("[CLUSTER DRIVE MODE] _backgroundImage non assegnato nel prefab");
				return;
			}

			if (!_backgroundResourcesLoaded)
			{
				Debug.LogWarning("[CLUSTER DRIVE MODE] Risorse background non caricate");
				return;
			}

			Debug.Log($"[CLUSTER DRIVE MODE] Applico background per modalità: {mode}");

			// Scelta background
			Sprite newBackgroundSprite = mode switch
			{
				DriveMode.Eco => _ecoBackgroundSprite,
				DriveMode.Comfort => _comfortBackgroundSprite,
				DriveMode.Sport => _sportBackgroundSprite,
				_ => _comfortBackgroundSprite
			};

			if (newBackgroundSprite == null)
			{
				Debug.LogError($"[CLUSTER DRIVE MODE] Background sprite per modalità {mode} è null!");
				return;
			}

			// Applica il cambio con o senza animazione
			if (animated)
			{
				StartLayeredBackgroundTransition(newBackgroundSprite);
			}
			else
			{
				_backgroundImage.sprite = newBackgroundSprite;
				Debug.Log($"[CLUSTER DRIVE MODE] Background cambiato immediatamente a: {newBackgroundSprite.name}");
			}
		}

		// Transizione con overlay temporaneo
		private void StartLayeredBackgroundTransition(Sprite newSprite)
		{
			if (_backgroundTransitionCoroutine != null)
				StopCoroutine(_backgroundTransitionCoroutine);

			_backgroundTransitionCoroutine = StartCoroutine(AnimateLayeredBackgroundTransition(newSprite));
		}

		// Coroutine animazione background
		private IEnumerator AnimateLayeredBackgroundTransition(Sprite newSprite)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] Inizio layered background transition");

			float transitionDuration = ClusterDriveModeData.BACKGROUND_TRANSITION_DURATION;

			// Crea overlay temporaneo
			GameObject overlayObject = CreateTemporaryOverlay(newSprite);
			if (overlayObject == null)
			{
				Debug.LogError("[CLUSTER DRIVE MODE] Impossibile creare overlay temporaneo");
				yield break;
			}

			Image overlayImage = overlayObject.GetComponent<Image>();
			Color overlayColor = overlayImage.color;

			// Overlay inizia trasparente
			overlayColor.a = 0f;
			overlayImage.color = overlayColor;

			Debug.Log($"[CLUSTER DRIVE MODE] Overlay creato per: {newSprite.name}");

			// Fade IN dell'overlay
			float elapsed = 0f;
			while (elapsed < transitionDuration)
			{
				elapsed += Time.deltaTime;
				float t = EaseInOut(elapsed / transitionDuration);
				overlayColor.a = Mathf.Lerp(0f, 1f, t);
				overlayImage.color = overlayColor;
				yield return null;
			}

			// Overlay completamente opaco
			overlayColor.a = 1f;
			overlayImage.color = overlayColor;

			Debug.Log($"[CLUSTER DRIVE MODE] Overlay fade-in completato");

			// Sostituisci background principale
			_backgroundImage.sprite = newSprite;

			Debug.Log($"[CLUSTER DRIVE MODE] Background principale sostituito con: {newSprite.name}");

			// Rimuovi overlay
			DestroyTemporaryOverlay(overlayObject);

			Debug.Log($"[CLUSTER DRIVE MODE] Layered transition completata per: {newSprite.name}");
			_backgroundTransitionCoroutine = null;
		}

		// Crea immagine temporanea per effetto overlay
		private GameObject CreateTemporaryOverlay(Sprite sprite)
		{
			try
			{
				Canvas parentCanvas = _backgroundImage.GetComponentInParent<Canvas>();
				if (parentCanvas == null)
				{
					Debug.LogError("[CLUSTER DRIVE MODE] Impossibile trovare parent Canvas");
					return null;
				}

				GameObject overlayObject = new GameObject("BackgroundOverlay_Temp");
				overlayObject.transform.SetParent(parentCanvas.transform, false);

				Image overlayImage = overlayObject.AddComponent<Image>();
				overlayImage.sprite = sprite;
				overlayImage.color = Color.white;

				RectTransform overlayRect = overlayObject.GetComponent<RectTransform>();
				RectTransform backgroundRect = _backgroundImage.GetComponent<RectTransform>();

				overlayRect.anchorMin = backgroundRect.anchorMin;
				overlayRect.anchorMax = backgroundRect.anchorMax;
				overlayRect.anchoredPosition = backgroundRect.anchoredPosition;
				overlayRect.sizeDelta = backgroundRect.sizeDelta;
				overlayRect.localScale = backgroundRect.localScale;

				overlayObject.transform.SetSiblingIndex(_backgroundImage.transform.GetSiblingIndex() + 1);

				Debug.Log("[CLUSTER DRIVE MODE] Overlay temporaneo creato e posizionato");
				return overlayObject;
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[CLUSTER DRIVE MODE] Errore creazione overlay: {ex.Message}");
				return null;
			}
		}

		// Rimuovi overlay temporaneo
		private void DestroyTemporaryOverlay(GameObject overlayObject)
		{
			if (overlayObject != null)
			{
				Debug.Log("[CLUSTER DRIVE MODE] Rimozione overlay temporaneo");
				Destroy(overlayObject);
			}
		}

		// Funzione smooth transitions
		private float EaseInOut(float t)
		{
			return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
		}

		// Pulizia overlay rimasti da transizioni incomplete - Problema risolto
		private void CleanupAnyRemainingOverlays()
		{
			GameObject[] overlays = GameObject.FindGameObjectsWithTag("Untagged");
			foreach (var obj in overlays)
			{
				if (obj.name == "BackgroundOverlay_Temp")
				{
					Debug.Log("[CLUSTER DRIVE MODE] Cleanup overlay rimasto");
					Destroy(obj);
				}
			}
		}

		// Aggiorna display modalità con animazione
		private void UpdateModeDisplay(DriveMode activeMode)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] Aggiornamento display: {_currentDriveMode} → {activeMode}");

			var previousMode = _currentDriveMode;
			_currentDriveMode = activeMode;

			// Stop animazione precedente
			if (_modeTransitionCoroutine != null)
			{
				StopCoroutine(_modeTransitionCoroutine);
			}

			// Avvia animazione transizione colori
			_modeTransitionCoroutine = StartCoroutine(AnimateModeTransition(activeMode));

			Debug.Log($"[CLUSTER DRIVE MODE] Display aggiornato da {previousMode} a {activeMode}");
		}

		// Coroutine animazione transizione modalità con effetto
		private IEnumerator AnimateModeTransition(DriveMode activeMode)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] INIZIO animazione transizione verso: {activeMode}");

			float duration = ClusterDriveModeData.ANIMATION_DURATION;
			float elapsed = 0f;

			// Tutti inattivi
			if (_ecoModeIndicator != null) _ecoModeIndicator.color = ClusterDriveModeData.INACTIVE_COLOR;
			if (_comfortModeIndicator != null) _comfortModeIndicator.color = ClusterDriveModeData.INACTIVE_COLOR;
			if (_sportModeIndicator != null) _sportModeIndicator.color = ClusterDriveModeData.INACTIVE_COLOR;

			// Piccola pausa per stabilizzare
			yield return new WaitForSeconds(0.05f);

			// Colori target basati su modalità
			Color ecoTarget = activeMode == DriveMode.Eco ? ClusterDriveModeData.ECO_COLOR : ClusterDriveModeData.INACTIVE_COLOR;
			Color comfortTarget = activeMode == DriveMode.Comfort ? ClusterDriveModeData.COMFORT_COLOR : ClusterDriveModeData.INACTIVE_COLOR;
			Color sportTarget = activeMode == DriveMode.Sport ? ClusterDriveModeData.SPORT_COLOR : ClusterDriveModeData.INACTIVE_COLOR;

			// Colori inattivi
			Color ecoStart = ClusterDriveModeData.INACTIVE_COLOR;
			Color comfortStart = ClusterDriveModeData.INACTIVE_COLOR;
			Color sportStart = ClusterDriveModeData.INACTIVE_COLOR;

			// Loop animazione smooth
			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;
				float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

				// Aggiorna colori con interpolazione smooth
				if (_ecoModeIndicator != null)
					_ecoModeIndicator.color = Color.Lerp(ecoStart, ecoTarget, t);

				if (_comfortModeIndicator != null)
					_comfortModeIndicator.color = Color.Lerp(comfortStart, comfortTarget, t);

				if (_sportModeIndicator != null)
					_sportModeIndicator.color = Color.Lerp(sportStart, sportTarget, t);

				yield return null;
			}

			// Assicura colori finali esatti
			if (_ecoModeIndicator != null) _ecoModeIndicator.color = ecoTarget;
			if (_comfortModeIndicator != null) _comfortModeIndicator.color = comfortTarget;
			if (_sportModeIndicator != null) _sportModeIndicator.color = sportTarget;

			Debug.Log($"[CLUSTER DRIVE MODE] Transizione completata per modalità: {activeMode}");
			_modeTransitionCoroutine = null;
		}

		// Sottoscrizione eventi
		private void SubscribeToEvents()
		{
			Debug.Log("[CLUSTER DRIVE MODE] Sottoscrizione eventi...");
			_broadcaster.Add<DriveModeChangedEvent>(OnDriveModeChanged);
			Debug.Log("[CLUSTER DRIVE MODE] Eventi sottoscritti");
		}

		// Rimozione sottoscrizioni eventi
		private void UnsubscribeFromEvents()
		{
			Debug.Log("[CLUSTER DRIVE MODE] Rimozione sottoscrizioni eventi...");

			if (_broadcaster != null)
			{
				_broadcaster.Remove<DriveModeChangedEvent>(OnDriveModeChanged);
			}

			Debug.Log("[CLUSTER DRIVE MODE] Sottoscrizioni rimosse");
		}

		// Cambio modalità di guida dalla State Machine gestita
		private void OnDriveModeChanged(DriveModeChangedEvent e)
		{
			Debug.Log($"[CLUSTER DRIVE MODE] Modalità cambiata via FSM: {e.NewMode}");

			// Aggiorna display modalità con animazione
			UpdateModeDisplay(e.NewMode);

			// Aggiorna background con animazione layered
			ApplyBackgroundForMode(e.NewMode, animated: true);
		}
	}
}