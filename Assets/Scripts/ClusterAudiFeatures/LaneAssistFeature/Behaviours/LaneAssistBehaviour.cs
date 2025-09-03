using ClusterAudi;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace ClusterAudiFeatures
{
	public class LaneAssistBehaviour : BaseMonoBehaviour<ILaneAssistFeatureInternal>
	{
		// Componenti prefab
		[Header("Lane Assist Components")]
		[SerializeField] private Image _leftLaneLines;
		[SerializeField] private Image _rightLaneLines;
		[SerializeField] private Image _carIcon;
		[SerializeField] private TextMeshProUGUI _statusText;

		// Servizi e stato interno
		private IBroadcaster _broadcaster;
		private IVehicleDataService _vehicleDataService;

		// Tracking input A/D
		private float _leftKeyHoldTime = 0f;
		private float _rightKeyHoldTime = 0f;
		private bool _isLeftKeyHeld = false;
		private bool _isRightKeyHeld = false;

		// Stato sistema
		private LaneAssistData.LaneDepartureType _currentDeparture = LaneAssistData.LaneDepartureType.None;
		private bool _isSystemActive = false;
		private float _autoResetTimer = 0f;

		// Posizione originale icona auto
		private Vector3 _originalCarIconPosition;

		// Sistema audio ripetuto
		private Coroutine _audioCoroutine;

		protected override void ManagedAwake()
		{
			Debug.Log("[LANE ASSIST] LaneAssistBehaviour inizializzato");

			// Ottieni servizi dal client
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();
			_vehicleDataService = client.Services.Get<IVehicleDataService>();

			// Setup UI iniziale
			InitializeUI();
		}

		protected override void ManagedUpdate()
		{
			// Aggiorna stato sistema basato su velocità
			UpdateSystemState();

			// Monitor input A/D se sistema attivo
			if (_isSystemActive)
			{
				MonitorLaneDepartureInput();
			}

			// Gestisce auto-reset
			if (_currentDeparture != LaneAssistData.LaneDepartureType.None)
			{
				HandleAutoReset();
			}
		}

		protected override void ManagedOnDestroy()
		{
			Debug.Log("[LANE ASSIST] LaneAssist UI distrutta");
			StopAudio();
		}

		// Configura stato iniziale UI
		private void InitializeUI()
		{
			if (_carIcon != null)
				_originalCarIconPosition = _carIcon.transform.localPosition;

			SetLaneLineColors(LaneAssistData.NORMAL_LANE_COLOR);
			UpdateStatusText("Lane Assist: OFF");

			Debug.Log("[LANE ASSIST] UI inizializzata");
		}

		// Aggiorna attivazione sistema basata su velocità
		private void UpdateSystemState()
		{
			float currentSpeed = _vehicleDataService.CurrentSpeed;
			bool shouldBeActive = LaneAssistData.CanActivateLaneAssist(currentSpeed);

			if (_isSystemActive != shouldBeActive)
			{
				_isSystemActive = shouldBeActive;
				UpdateStatusText(_isSystemActive ? "Lane Assist: ON" : "Lane Assist: OFF");

				// Reset se sistema disattivato
				if (!_isSystemActive && _currentDeparture != LaneAssistData.LaneDepartureType.None)
				{
					ResetLaneDeparture();
				}
			}
		}

		// Monitor tasti A/D per rilevare lane departure (pressione > 2 secondi)
		private void MonitorLaneDepartureInput()
		{
			// Tasto A (LEFT DEPARTURE)
			if (Input.GetKey(LaneAssistData.LEFT_DEPARTURE_KEY))
			{
				_isLeftKeyHeld = true;
				_leftKeyHoldTime += Time.deltaTime;

				if (_currentDeparture != LaneAssistData.LaneDepartureType.Left &&
					LaneAssistData.IsLaneDeparture(_leftKeyHoldTime))
				{
					TriggerLaneDeparture(LaneAssistData.LaneDepartureType.Left);
				}
			}
			else if (_isLeftKeyHeld)
			{
				_isLeftKeyHeld = false;
				_leftKeyHoldTime = 0f;
			}

			// Tasto D (RIGHT DEPARTURE)
			if (Input.GetKey(LaneAssistData.RIGHT_DEPARTURE_KEY))
			{
				_isRightKeyHeld = true;
				_rightKeyHoldTime += Time.deltaTime;

				if (_currentDeparture != LaneAssistData.LaneDepartureType.Right &&
					LaneAssistData.IsLaneDeparture(_rightKeyHoldTime))
				{
					TriggerLaneDeparture(LaneAssistData.LaneDepartureType.Right);
				}
			}
			else if (_isRightKeyHeld)
			{
				_isRightKeyHeld = false;
				_rightKeyHoldTime = 0f;
			}
		}

		// Attiva lane departure warning con visual + audio
		private void TriggerLaneDeparture(LaneAssistData.LaneDepartureType departureType)
		{
			Debug.Log($"[LANE ASSIST] LANE DEPARTURE: {departureType}");

			_currentDeparture = departureType;
			_autoResetTimer = 0f;

			// Aggiorna visuals per warning
			UpdateStatusText("Lane Assist: ATTENZIONE");
			SetLaneLineColors(LaneAssistData.DEPARTURE_LANE_COLOR);
			SetCarIconShift(departureType == LaneAssistData.LaneDepartureType.Left ? -20f : 20f);

			// Avvia audio ripetuto ogni 2 secondi
			StartRepeatingAudio(departureType);

			// Notifica sistema via eventi
			_broadcaster.Broadcast(new LaneDepartureDetectedEvent(departureType, 2f));
		}

		// Gestione auto-reset quando tasti A/D vengono rilasciati
		private void HandleAutoReset()
		{
			if (!_isLeftKeyHeld && !_isRightKeyHeld)
			{
				_autoResetTimer += Time.deltaTime;

				if (_autoResetTimer >= LaneAssistData.AUTO_RESET_TIME)
				{
					ResetLaneDeparture();
				}
			}
			else
			{
				_autoResetTimer = 0f;
			}
		}

		// Reset completo lane departure a stato normale
		private void ResetLaneDeparture()
		{
			Debug.Log("[LANE ASSIST] Lane departure reset");

			_currentDeparture = LaneAssistData.LaneDepartureType.None;
			_autoResetTimer = 0f;

			// Ripristina visuals normali
			UpdateStatusText(_isSystemActive ? "Lane Assist: ON" : "Lane Assist: OFF");
			SetLaneLineColors(LaneAssistData.NORMAL_LANE_COLOR);
			ResetCarIconPosition();

			// Ferma audio
			StopAudio();

			// Notifica reset via eventi
			_broadcaster.Broadcast(new LaneDepartureResetEvent());
		}

		// Avvia coroutine per audio ripetuto ogni 2 secondi
		private void StartRepeatingAudio(LaneAssistData.LaneDepartureType departureType)
		{
			StopAudio();
			_audioCoroutine = StartCoroutine(AudioCoroutine(departureType));
		}

		// Ferma audio ripetuto
		private void StopAudio()
		{
			if (_audioCoroutine != null)
			{
				StopCoroutine(_audioCoroutine);
				_audioCoroutine = null;
			}
		}

		// Coroutine riproduce audio ogni 2 secondi durante warning
		private IEnumerator AudioCoroutine(LaneAssistData.LaneDepartureType departureType)
		{
			string audioPath = departureType == LaneAssistData.LaneDepartureType.Left
				? LaneAssistData.LANE_DEPARTURE_LEFT_AUDIO_PATH
				: LaneAssistData.LANE_DEPARTURE_RIGHT_AUDIO_PATH;

			while (_currentDeparture == departureType)
			{
				_broadcaster.Broadcast(new LaneAssistAudioRequestEvent(audioPath, departureType));
				yield return new WaitForSeconds(2f);
			}
		}

		// Imposta colore delle lane lines
		private void SetLaneLineColors(Color color)
		{
			if (_leftLaneLines != null) _leftLaneLines.color = color;
			if (_rightLaneLines != null) _rightLaneLines.color = color;
		}

		// Spostamento icona auto
		private void SetCarIconShift(float shiftAmount)
		{
			if (_carIcon != null)
				_carIcon.transform.localPosition = _originalCarIconPosition + new Vector3(shiftAmount, 0f, 0f);
		}

		// Ripristina posizione icona auto
		private void ResetCarIconPosition()
		{
			if (_carIcon != null)
				_carIcon.transform.localPosition = _originalCarIconPosition;
		}

		// Aggiorna testo stato sistema
		private void UpdateStatusText(string message)
		{
			if (_statusText != null)
				_statusText.text = message;
		}
	}
}