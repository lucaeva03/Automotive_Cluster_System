using ClusterAudi;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

namespace ClusterAudiFeatures
{
	public class SeatBeltBehaviour : BaseMonoBehaviour<ISeatBeltFeatureInternal>
	{
		// Componenti prefab
		[Header("SeatBelt Icons")]
		[SerializeField] private Image _driverIcon;
		[SerializeField] private Image _passengerIcon;
		[SerializeField] private Image _rearLeftIcon;
		[SerializeField] private Image _rearRightIcon;

		// Servizi e stato interno
		private IBroadcaster _broadcaster;
		private Image[] _seatBeltIcons;
		private SeatBeltData.SeatBeltStatus[] _displayedStates;
		private bool[] _iconFlashStates;

		// Sistema di lampeggio
		private bool _isFlashingActive = false;
		private Coroutine _iconFlashCoroutine;

		protected override void ManagedAwake()
		{
			Debug.Log("[SEATBELT UI] SeatBeltBehaviour inizializzato");

			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();

			// Arrays per gestione stati
			_seatBeltIcons = new Image[] { _driverIcon, _passengerIcon, _rearLeftIcon, _rearRightIcon };
			_displayedStates = new SeatBeltData.SeatBeltStatus[SeatBeltData.TOTAL_SEATBELTS];
			_iconFlashStates = new bool[SeatBeltData.TOTAL_SEATBELTS];

			// Setup iniziale
			ValidateComponents();
			SetupInitialUI();
			SubscribeToEvents();
		}

		protected override void ManagedUpdate()
		{
			// Gestione input
			HandleDebugInput();

		}

		protected override void ManagedOnDestroy()
		{
			Debug.Log("[SEATBELT UI] SeatBeltBehaviour distrutto");
			UnsubscribeFromEvents();
			StopAllCoroutines();
		}

		// Aggiornamento UI stato di una singola cintura
		public void UpdateSeatBeltStatus(SeatBeltData.SeatBeltPosition position, SeatBeltData.SeatBeltStatus status)
		{
			int index = (int)position;
			if (index < 0 || index >= SeatBeltData.TOTAL_SEATBELTS) return;

			if (_displayedStates[index] != status)
			{
				_displayedStates[index] = status;
				UpdateSingleIconVisual(position, status);
			}
		}

		// Aggiorna stati delle cinture contemporaneamente
		public void UpdateAllSeatBeltStates(SeatBeltData.SeatBeltStatus[] states)
		{
			if (states == null || states.Length != SeatBeltData.TOTAL_SEATBELTS) return;

			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_displayedStates[i] != states[i])
				{
					_displayedStates[i] = states[i];
					UpdateSingleIconVisual((SeatBeltData.SeatBeltPosition)i, states[i]);
				}
			}
		}

		// Verifica componenti prefab
		private void ValidateComponents()
		{
			for (int i = 0; i < _seatBeltIcons.Length; i++)
			{
				if (_seatBeltIcons[i] == null)
				{
					Debug.LogError($"[SEATBELT UI] Icona {i} non assegnata!");
				}
			}
		}

		// Configura stato inziale icone
		private void SetupInitialUI()
		{
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_seatBeltIcons[i] != null)
				{
					_displayedStates[i] = SeatBeltData.SeatBeltStatus.Unknown;
					_seatBeltIcons[i].color = SeatBeltData.GetColorForStatus(SeatBeltData.SeatBeltStatus.Unknown);
					_iconFlashStates[i] = false;
				}
			}
		}

		// Aggiorna il colore di una singola icona se non sta lampeggiando
		private void UpdateSingleIconVisual(SeatBeltData.SeatBeltPosition position, SeatBeltData.SeatBeltStatus status)
		{
			int index = (int)position;
			if (index < 0 || index >= SeatBeltData.TOTAL_SEATBELTS || _seatBeltIcons[index] == null)
				return;

			// Aggiorna colore solo se non sta flashando
			if (!_iconFlashStates[index])
			{
				_seatBeltIcons[index].color = SeatBeltData.GetColorForStatus(status);
			}
		}

		// Sottoscrizione agli eventi del sistema
		private void SubscribeToEvents()
		{
			_broadcaster.Add<SeatBeltFlashIconsEvent>(OnFlashIcons);
			Debug.Log("[SEATBELT UI] Eventi sottoscritti");
		}

		// Rimozione sottoscrizioni eventi
		private void UnsubscribeFromEvents()
		{
			if (_broadcaster != null)
			{
				_broadcaster.Remove<SeatBeltFlashIconsEvent>(OnFlashIcons);
			}
		}

		// Gestione eventi di lampeggio icone
		private void OnFlashIcons(SeatBeltFlashIconsEvent e)
		{
			if (e.StartFlashing)
			{
				StartIconFlashing(e.PositionsToFlash, e.FlashInterval);
			}
			else
			{
				StopIconFlashing();
			}
		}

		// Avvia il lampeggio delle icone specificate
		private void StartIconFlashing(SeatBeltData.SeatBeltPosition[] positions, float interval)
		{
			StopIconFlashing();

			if (positions == null || positions.Length == 0) return;

			_isFlashingActive = true;

			// Resetta stati flash
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				_iconFlashStates[i] = false;
			}

			// Marca posizioni per lampeggio
			foreach (var pos in positions)
			{
				int index = (int)pos;
				if (index >= 0 && index < SeatBeltData.TOTAL_SEATBELTS)
				{
					_iconFlashStates[index] = true;
				}
			}

			// Avvia coroutine lampeggio
			_iconFlashCoroutine = StartCoroutine(FlashIconsCoroutine(positions, interval));
			Debug.Log($"[SEATBELT UI] Lampeggio avviato per {positions.Length} icone");
		}

		// Ferma lampeggio e ripristina colori normali
		private void StopIconFlashing()
		{
			if (_iconFlashCoroutine != null)
			{
				StopCoroutine(_iconFlashCoroutine);
				_iconFlashCoroutine = null;
			}

			_isFlashingActive = false;

			// Ripristina colori normali per icone che stavano lampeggiando
			for (int i = 0; i < SeatBeltData.TOTAL_SEATBELTS; i++)
			{
				if (_iconFlashStates[i] && _seatBeltIcons[i] != null)
				{
					_iconFlashStates[i] = false;
					_seatBeltIcons[i].color = SeatBeltData.GetColorForStatus(_displayedStates[i]);
				}
			}

			Debug.Log("[SEATBELT UI] Lampeggio fermato");
		}

		// Coroutine per alternare visibilità/invisibilità delle icone ogni secondo
		private IEnumerator FlashIconsCoroutine(SeatBeltData.SeatBeltPosition[] positions, float interval)
		{
			while (_isFlashingActive)
			{
				// Fase 1: Icone visibili (0.5 secondi)
				foreach (var pos in positions)
				{
					int index = (int)pos;
					if (index >= 0 && index < SeatBeltData.TOTAL_SEATBELTS && _seatBeltIcons[index] != null)
					{
						_seatBeltIcons[index].color = SeatBeltData.GetColorForStatus(_displayedStates[index]);
					}
				}

				yield return new WaitForSeconds(interval * 0.5f);

				// Fase 2: Icone invisibili (0.5 secondi)
				foreach (var pos in positions)
				{
					int index = (int)pos;
					if (index >= 0 && index < SeatBeltData.TOTAL_SEATBELTS && _seatBeltIcons[index] != null)
					{
						_seatBeltIcons[index].color = Color.clear; // Trasparente
					}
				}

				yield return new WaitForSeconds(interval * 0.5f);
			}
		}

		// Input stato cinture
		private void HandleDebugInput()
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Q)) ToggleSeatBelt(SeatBeltData.SeatBeltPosition.Driver, "DRIVER");
			if (UnityEngine.Input.GetKeyDown(KeyCode.E)) ToggleSeatBelt(SeatBeltData.SeatBeltPosition.Passenger, "PASSENGER");
			if (UnityEngine.Input.GetKeyDown(KeyCode.R)) ToggleSeatBelt(SeatBeltData.SeatBeltPosition.RearLeft, "REAR LEFT");
			if (UnityEngine.Input.GetKeyDown(KeyCode.T)) ToggleSeatBelt(SeatBeltData.SeatBeltPosition.RearRight, "REAR RIGHT");

			if (UnityEngine.Input.GetKeyDown(KeyCode.I))
			{
				LogAllSeatBeltStates();
			}
		}

		// Toggle stato cintura per debug
		private void ToggleSeatBelt(SeatBeltData.SeatBeltPosition position, string name)
		{
			var currentState = _feature.GetAllSeatBeltStates()[(int)position];
			var newState = currentState == SeatBeltData.SeatBeltStatus.Fastened
				? SeatBeltData.SeatBeltStatus.Unfastened
				: SeatBeltData.SeatBeltStatus.Fastened;

			var client = _feature.GetClient();
			var seatBeltFeature = client.Features.Get<ISeatBeltFeature>();
			seatBeltFeature.SetSeatBeltStatus(position, newState);

			string statusIcon = newState == SeatBeltData.SeatBeltStatus.Fastened ? "V" : "X";
		}

		// Log di tutti gli stati correnti per debug
		private void LogAllSeatBeltStates()
		{
			var states = _feature.GetAllSeatBeltStates();
			Debug.Log("=== SEATBELT STATUS ===");
			for (int i = 0; i < states.Length; i++)
			{
				string name = ((SeatBeltData.SeatBeltPosition)i).ToString();
				string icon = states[i] == SeatBeltData.SeatBeltStatus.Fastened ? "V" : "X";
				Debug.Log($"{name}: {icon} {states[i]}");
			}
			Debug.Log("Controls: Q=Driver | E=Passenger | R=RearLeft | T=RearRight | I=Info");
		}
	}
}