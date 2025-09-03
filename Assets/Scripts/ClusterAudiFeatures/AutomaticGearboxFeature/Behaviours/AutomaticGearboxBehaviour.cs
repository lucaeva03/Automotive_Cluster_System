using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ClusterAudi;
using ClusterAudiFeatures;

namespace ClusterAudiFeatures
{
	public class AutomaticGearboxBehaviour : BaseMonoBehaviour<IAutomaticGearboxFeatureInternal>
	{
		// Componenti UI da assegnare nel prefab
		[Header("AutomaticGearbox Display")]
		[Tooltip("RPM numerici")]
		[SerializeField] private TextMeshProUGUI _rpmValueText;

		[Tooltip("Unità di misura")]
		[SerializeField] private TextMeshProUGUI _rpmUnitText;

		[Tooltip("ProgressBar RPM")]
		[SerializeField] private Slider _rpmProgressBar;

		[Tooltip("Barra ProgressBar")]
		[SerializeField] private Image _progressBarFill;

		[Tooltip("Marcia corrente")]
		[SerializeField] private TextMeshProUGUI _gearValueText;

		// Servizi e configurazione
		private IBroadcaster _broadcaster;
		private IVehicleDataService _vehicleDataService;
		private AutomaticGearboxConfig _currentConfiguration;

		// Valori per animazioni smooth
		private float _displayedRPM = 800f;
		private float _targetRPM = 800f;
		private int _displayedGear = 0;
		private int _targetGear = 0;

		// Colori per transizioni graduali
		private Color _currentRPMColor = Color.white;
		private Color _targetRPMColor = Color.white;

		// Inizializzazione - Ottiene servizi e configura UI
		protected override void ManagedAwake()
		{
			Debug.Log("[AUTOMATIC GEARBOX] AutomaticGearboxBehaviour inizializzato");

			// Ottiene servizi dal Client tramite feature
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();
			_vehicleDataService = client.Services.Get<IVehicleDataService>();

			// Ottiene configurazione iniziale dalla feature
			_currentConfiguration = _feature.GetCurrentConfiguration();

			// Setup componenti UI e eventi
			ValidateUIComponents();
			SetupInitialUI();
			SubscribeToEvents();
		}

		// Avvio - Imposta valori iniziali dal servizio veicolo
		protected override void ManagedStart()
		{
			Debug.Log("[AUTOMATIC GEARBOX] AutomaticGearbox UI avviata");

			// Sincronizza con stato corrente del veicolo
			_targetRPM = _vehicleDataService.CurrentRPM;
			_displayedRPM = _targetRPM;
			_targetGear = _vehicleDataService.CurrentGear;
			_displayedGear = _targetGear;

			UpdateUI();
		}

		// Update continuo - Smooth animations e aggiornamento UI
		protected override void ManagedUpdate()
		{
			// Aggiorna valori con smooth damping
			UpdateSmoothValues();

			// Aggiorna tutti i componenti UI
			UpdateUI();
		}

		// Cleanup - Rimuove sottoscrizioni eventi
		protected override void ManagedOnDestroy()
		{
			Debug.Log("[AUTOMATIC GEARBOX] AutomaticGearboxBehaviour distrutto");
			UnsubscribeFromEvents();
		}

		// Applica nuova configurazione (es. cambio modalità guida)
		public void ApplyConfiguration(AutomaticGearboxConfig config)
		{
			_currentConfiguration = config;

			// Aggiorna range massimo progress bar
			if (_rpmProgressBar != null)
			{
				_rpmProgressBar.maxValue = config.MaxDisplayRPM;
			}

			Debug.Log($"[AUTOMATIC GEARBOX] Configurazione applicata: MaxRPM={config.MaxDisplayRPM}");
		}

		// Validazione componenti UI essenziali
		private void ValidateUIComponents()
		{
			int missingComponents = 0;

			if (_rpmValueText == null)
			{
				Debug.LogError("[AUTOMATIC GEARBOX] _rpmValueText non assegnato nel prefab!");
				missingComponents++;
			}
			if (_rpmUnitText == null)
			{
				Debug.LogError("[AUTOMATIC GEARBOX] _rpmUnitText non assegnato nel prefab!");
				missingComponents++;
			}
			if (_rpmProgressBar == null)
			{
				Debug.LogError("[AUTOMATIC GEARBOX] _rpmProgressBar non assegnato nel prefab!");
				missingComponents++;
			}
			if (_gearValueText == null)
			{
				Debug.LogError("[AUTOMATIC GEARBOX] _gearValueText non assegnato nel prefab!");
				missingComponents++;
			}

			if (missingComponents == 0)
			{
				Debug.Log("[AUTOMATIC GEARBOX] Tutti i componenti essenziali assegnati");
			}
			else
			{
				Debug.LogError($"[AUTOMATIC GEARBOX] {missingComponents} componenti essenziali mancanti!");
			}
		}

		// Setup stato iniziale di tutti i componenti UI
		private void SetupInitialUI()
		{
			// Configurazione testo RPM
			if (_rpmValueText != null)
			{
				_rpmValueText.text = "800";
				_rpmValueText.color = AutomaticGearboxData.DEFAULT_RPM_COLOR;
			}

			// Configurazione etichetta unità
			if (_rpmUnitText != null)
			{
				_rpmUnitText.text = AutomaticGearboxData.RPM_UNIT_LABEL;
				_rpmUnitText.color = AutomaticGearboxData.DEFAULT_RPM_COLOR;
			}

			// Configurazione progress bar
			if (_rpmProgressBar != null)
			{
				_rpmProgressBar.minValue = 0f;
				_rpmProgressBar.maxValue = _currentConfiguration?.MaxDisplayRPM ?? AutomaticGearboxData.MAX_DISPLAY_RPM;
				_rpmProgressBar.value = AutomaticGearboxData.IDLE_RPM;
				_rpmProgressBar.interactable = false;
			}

			// Configurazione colore fill progress bar
			if (_progressBarFill != null)
			{
				_progressBarFill.color = AutomaticGearboxData.IDLE_RPM_COLOR;
			}

			// Configurazione display marcia
			if (_gearValueText != null)
			{
				_gearValueText.text = "P";
				_gearValueText.color = AutomaticGearboxData.GEAR_ACTIVE_COLOR;
			}

			Debug.Log("[AUTOMATIC GEARBOX] UI iniziale configurata");
		}

		// Sottoscrizione a eventi VehicleDataService e sistema generale
		private void SubscribeToEvents()
		{
			_vehicleDataService.OnRPMChanged += OnRPMChanged;
			_vehicleDataService.OnGearChanged += OnGearChanged;
			_broadcaster.Add<DriveModeChangedEvent>(OnDriveModeChanged);

			Debug.Log("[AUTOMATIC GEARBOX] Eventi sottoscritti");
		}

		// Rimozione sottoscrizioni eventi
		private void UnsubscribeFromEvents()
		{
			if (_vehicleDataService != null)
			{
				_vehicleDataService.OnRPMChanged -= OnRPMChanged;
				_vehicleDataService.OnGearChanged -= OnGearChanged;
			}

			if (_broadcaster != null)
			{
				_broadcaster.Remove<DriveModeChangedEvent>(OnDriveModeChanged);
			}

			Debug.Log("[AUTOMATIC GEARBOX] Eventi rimossi");
		}

		// Handler cambio RPM - Aggiorna target e colore
		private void OnRPMChanged(float newRPM)
		{
			_targetRPM = newRPM;
			_targetRPMColor = AutomaticGearboxData.GetRPMColor(newRPM);
		}

		// Handler cambio marcia - Aggiorna target gear
		private void OnGearChanged(int newGear)
		{
			_targetGear = newGear;
		}

		// Handler cambio modalità guida
		private void OnDriveModeChanged(DriveModeChangedEvent e)
		{
			Debug.Log($"[AUTOMATIC GEARBOX] Modalità cambiata: {e.NewMode}");
		}

		// Update smooth con damping configurabile basato su modalità
		private void UpdateSmoothValues()
		{
			// Smooth transition RPM con damping dalla configurazione
			if (!Mathf.Approximately(_displayedRPM, _targetRPM))
			{
				float damping = _currentConfiguration?.ResponseDamping ?? AutomaticGearboxData.COMFORT_RESPONSE_DAMPING;
				_displayedRPM = Mathf.Lerp(_displayedRPM, _targetRPM, damping * Time.deltaTime * 10f);

				// Snap finale per evitare oscillazioni
				if (Mathf.Abs(_displayedRPM - _targetRPM) < 10f)
					_displayedRPM = _targetRPM;
			}

			// Smooth transition colori RPM
			if (_currentRPMColor != _targetRPMColor)
			{
				_currentRPMColor = Color.Lerp(_currentRPMColor, _targetRPMColor, Time.deltaTime * 5f);
			}

			// Marcia senza smooth (cambio immediato)
			_displayedGear = _targetGear;
		}

		// Aggiorna tutti i componenti UI con valori correnti
		private void UpdateUI()
		{
			// Aggiorna display RPM con colore corrente
			if (_rpmValueText != null)
			{
				_rpmValueText.text = AutomaticGearboxData.FormatRPM(_displayedRPM);
				_rpmValueText.color = _currentRPMColor;
			}

			// Aggiorna progress bar con valore clamped
			if (_rpmProgressBar != null)
			{
				_rpmProgressBar.value = Mathf.Clamp(_displayedRPM, 0f, _rpmProgressBar.maxValue);
			}

			// Aggiorna colore fill progress bar
			if (_progressBarFill != null)
			{
				_progressBarFill.color = _currentRPMColor;
			}

			// Aggiorna display marcia corrente
			if (_gearValueText != null)
			{
				_gearValueText.text = AutomaticGearboxData.FormatGear(_displayedGear);
			}
		}
	}
}