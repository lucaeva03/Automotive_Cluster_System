using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public class SeatBeltFeature : BaseFeature, ISeatBeltFeature, ISeatBeltFeatureInternal
	{

		private SeatBeltConfig _currentConfiguration;
		private SeatBeltData.SeatBeltStatus[] _seatBeltStates;
		private SeatBeltBehaviour _seatBeltBehaviour;

		// Variabili sistema di warning
		private bool _isWarningSystemActive = false;
		private float _warningStartTime = 0f;
		private AudioEscalationLevel _currentAudioLevel = AudioEscalationLevel.None;

		// Servizio e audio continuo
		private IVehicleDataService _vehicleDataService;
		private System.Collections.IEnumerator _continuousAudioCoroutine;

		public SeatBeltFeature(Client client) : base(client)
		{
			Debug.Log("[SEATBELT FEATURE] SeatBeltFeature inizializzata");

			// Inizializza tutte le cinture come slacciate
			_seatBeltStates = new SeatBeltData.SeatBeltStatus[SeatBeltData.TOTAL_SEATBELTS];
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				_seatBeltStates[i] = SeatBeltData.SeatBeltStatus.Unfastened;
			}

			// Cache servizi e configurazione iniziale
			_vehicleDataService = client.Services.Get<IVehicleDataService>();
			_currentConfiguration = SeatBeltData.GetConfigForDriveMode(_vehicleDataService.CurrentDriveMode);

			// Sottoscrizione eventi cambio modalità
			_broadcaster.Add<DriveModeChangedEvent>(OnDriveModeChanged);

			// Sottoscrizione eventi velocità per sistema warning
			_vehicleDataService.OnSpeedChanged += OnSpeedChanged;

			Debug.Log("[SEATBELT FEATURE] Stati iniziali: tutte slacciate");
		}

		// Istanzia UI e inizializza behaviour
		public async Task InstantiateSeatBeltFeature()
		{
			Debug.Log("[SEATBELT FEATURE] Istanziazione SeatBelt UI...");

			try
			{
				var seatBeltInstance = await _assetService.InstantiateAsset<SeatBeltBehaviour>(
					SeatBeltData.SEATBELT_PREFAB_PATH);

				if (seatBeltInstance != null)
				{
					seatBeltInstance.Initialize(this);
					_seatBeltBehaviour = seatBeltInstance;
					_seatBeltBehaviour.UpdateAllSeatBeltStates(_seatBeltStates);
					Debug.Log("[SEATBELT FEATURE] UI istanziata da prefab");
				}
				else
				{
					Debug.LogWarning("[SEATBELT FEATURE] Prefab non trovato");
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[SEATBELT FEATURE] Errore istanziazione: {ex.Message}");
			}
		}

		// Imposta stato di una cintura specifica e valuta warning system
		public void SetSeatBeltStatus(SeatBeltData.SeatBeltPosition position, SeatBeltData.SeatBeltStatus status)
		{
			int index = (int)position;
			if (index < 0 || index >= SeatBeltData.TOTAL_SEATBELTS) return;

			var oldStatus = _seatBeltStates[index];
			if (oldStatus == status) return;

			_seatBeltStates[index] = status;

			// Notifica cambio stato
			_broadcaster.Broadcast(new SeatBeltStatusChangedEvent(position, oldStatus, status));

			// Aggiorna UI
			if (_seatBeltBehaviour != null)
			{
				_seatBeltBehaviour.UpdateSeatBeltStatus(position, status);
			}

			// Rivaluta sistema warning
			EvaluateWarningSystem();

			Debug.Log($"[SEATBELT FEATURE] Cintura {position}: {oldStatus} → {status}");
		}

		// Forza controllo sistema warning
		public void ForceWarningCheck()
		{
			Debug.Log("[SEATBELT FEATURE] Check forzato sistema cinture");
			EvaluateWarningSystem();
		}

		// Accesso al client per behaviour
		public Client GetClient() => _client;

		// Restituisce configurazione corrente
		public SeatBeltConfig GetCurrentConfiguration() => _currentConfiguration;

		// Copia degli stati correnti
		public SeatBeltData.SeatBeltStatus[] GetAllSeatBeltStates()
		{
			var copy = new SeatBeltData.SeatBeltStatus[SeatBeltData.TOTAL_SEATBELTS];
			System.Array.Copy(_seatBeltStates, copy, SeatBeltData.TOTAL_SEATBELTS);
			return copy;
		}

		// Stato del sistema warning
		public bool IsWarningSystemActive() => _isWarningSystemActive;

		// Valuta se attivare/disattivare sistema warning basato su velocità e cinture
		private void EvaluateWarningSystem()
		{
			float currentSpeed = _vehicleDataService.CurrentSpeed;
			float speedThreshold = _currentConfiguration.SpeedThreshold;

			bool shouldShowWarning = currentSpeed > speedThreshold && HasUnfastenedBelts();

			if (shouldShowWarning && !_isWarningSystemActive)
			{
				StartWarningSystem();
			}
			else if (!shouldShowWarning && _isWarningSystemActive)
			{
				StopWarningSystem();
			}
		}

		// Avvia sistema warning con lampeggio icone e audio continuo
		private void StartWarningSystem()
		{
			_isWarningSystemActive = true;
			_warningStartTime = Time.time;
			_currentAudioLevel = AudioEscalationLevel.Soft;

			var unfastenedBelts = GetUnfastenedBeltPositions();

			// Notifica inizio warning
			_broadcaster.Broadcast(new SeatBeltWarningStartedEvent(
				_vehicleDataService.CurrentSpeed,
				_currentConfiguration.SpeedThreshold,
				unfastenedBelts));

			// Attiva lampeggio icone cinture slacciate
			if (_currentConfiguration.FlashingEnabled)
			{
				_broadcaster.Broadcast(new SeatBeltFlashIconsEvent(unfastenedBelts, true, 1f));
			}

			// Avvia sistema audio continuo
			if (_currentConfiguration.EnableAudioWarning)
			{
				StartContinuousAudioWarning();
			}

			Debug.Log($"[SEATBELT FEATURE] WARNING ATTIVATO: {unfastenedBelts.Length} cinture slacciate - Lampeggio + Audio");
		}

		// Ferma sistema warning e ripristina stato normale
		private void StopWarningSystem()
		{
			if (!_isWarningSystemActive) return;

			float totalDuration = Time.time - _warningStartTime;
			_isWarningSystemActive = false;
			_currentAudioLevel = AudioEscalationLevel.None;

			// Ferma audio continuo
			StopContinuousAudioWarning();

			// Notifica fine warning
			_broadcaster.Broadcast(new SeatBeltWarningStoppedEvent(totalDuration));

			// Ferma lampeggio
			_broadcaster.Broadcast(new SeatBeltFlashIconsEvent(new SeatBeltData.SeatBeltPosition[0], false));

			Debug.Log($"[SEATBELT FEATURE] WARNING FERMATO - Audio e lampeggio fermati");
		}

		// Avvia coroutine per audio continuo con escalation
		private void StartContinuousAudioWarning()
		{
			if (_continuousAudioCoroutine != null)
			{
				_client.StopCoroutine(_continuousAudioCoroutine);
			}

			_continuousAudioCoroutine = ContinuousAudioCoroutine();
			_client.StartCoroutine(_continuousAudioCoroutine);
			Debug.Log("[SEATBELT FEATURE] Audio continuo avviato");
		}

		// Ferma sistema audio continuo
		private void StopContinuousAudioWarning()
		{
			if (_continuousAudioCoroutine != null)
			{
				_client.StopCoroutine(_continuousAudioCoroutine);
				_continuousAudioCoroutine = null;
			}

			_broadcaster.Broadcast(new StopSeatBeltAudioEvent());
			Debug.Log("[SEATBELT FEATURE] Audio continuo fermato");
		}

		// Coroutine che riproduce audio ogni 2 secondi con escalation basata su tempo
		private System.Collections.IEnumerator ContinuousAudioCoroutine()
		{
			const float AUDIO_INTERVAL = 2f;

			while (_isWarningSystemActive)
			{
				// Calcola escalation basata su durata warning
				float warningDuration = Time.time - _warningStartTime;
				var currentLevel = SeatBeltData.GetAudioEscalationLevel(warningDuration);

				if (currentLevel != _currentAudioLevel)
				{
					_currentAudioLevel = currentLevel;
					Debug.Log($"[SEATBELT FEATURE] Audio escalation: {currentLevel} (tempo: {warningDuration:F1}s)");
				}

				// Riproduce audio appropriato per livello escalation
				string audioPath = GetAudioPathForLevel(_currentAudioLevel);
				_broadcaster.Broadcast(new PlaySeatBeltAudioEvent(audioPath, 1f, 5));

				yield return new WaitForSeconds(AUDIO_INTERVAL);
			}

			_continuousAudioCoroutine = null;
		}

		// Restituisce path audio basato sul livello di escalation
		private string GetAudioPathForLevel(AudioEscalationLevel level)
		{
			return level switch
			{
				AudioEscalationLevel.Urgent => SeatBeltData.URGENT_BEEP_AUDIO_PATH,
				AudioEscalationLevel.Continuous => SeatBeltData.CONTINUOUS_BEEP_AUDIO_PATH,
				_ => SeatBeltData.SOFT_BEEP_AUDIO_PATH
			};
		}

		// Gestione cambio velocità - rivaluta warning system
		private void OnSpeedChanged(float newSpeed)
		{
			EvaluateWarningSystem();
		}

		// Verifica se ci sono cinture slacciate
		private bool HasUnfastenedBelts()
		{
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_seatBeltStates[i] == SeatBeltData.SeatBeltStatus.Unfastened)
					return true;
			}
			return false;
		}

		private void OnDriveModeChanged(DriveModeChangedEvent e)
		{
			_currentConfiguration = SeatBeltData.GetConfigForDriveMode(e.NewMode);
			Debug.Log($"[SEATBELT] Configurazione aggiornata per modalità: {e.NewMode}");

			// Re-evaluta warning system con nuove soglie
			EvaluateWarningSystem();
		}

		// Restituisce array delle posizioni con cinture slacciate
		private SeatBeltData.SeatBeltPosition[] GetUnfastenedBeltPositions()
		{
			var unfastened = new System.Collections.Generic.List<SeatBeltData.SeatBeltPosition>();

			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_seatBeltStates[i] == SeatBeltData.SeatBeltStatus.Unfastened)
				{
					unfastened.Add((SeatBeltData.SeatBeltPosition)i);
				}
			}

			return unfastened.ToArray();
		}
	}
}