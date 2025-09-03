using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ClusterAudi;
using ClusterAudiFeatures;

namespace ClusterAudiFeatures
{
	public class SpeedometerBehaviour : BaseMonoBehaviour<ISpeedometerFeatureInternal>
	{
		// Componenti prefab
		[Header("Speedometer Display")]
		[SerializeField] private TextMeshProUGUI _speedValueText;
		[SerializeField] private TextMeshProUGUI _speedUnitText;
		[SerializeField] private Slider _speedProgressBar;
		[SerializeField] private Image _progressBarFill;

		// Servizi e configurazione
		private IBroadcaster _broadcaster;
		private IVehicleDataService _vehicleDataService;
		private SpeedometerConfig _currentConfiguration;

		// Var animazioni smooth per display velocità
		private float _displayedSpeed = 0f;
		private float _targetSpeed = 0f;
		private Color _currentSpeedColor = Color.white;
		private Color _targetSpeedColor = Color.white;

		protected override void ManagedAwake()
		{
			Debug.Log("[SPEEDOMETER] SpeedometerBehaviour inizializzato");

			// Servizi dal client
			var client = _feature.GetClient();
			_broadcaster = client.Services.Get<IBroadcaster>();
			_vehicleDataService = client.Services.Get<IVehicleDataService>();
			_currentConfiguration = _feature.GetCurrentConfiguration();

			// Setup iniziale
			SetupInitialUI();
			SubscribeToEvents();
		}

		protected override void ManagedStart()
		{
			Debug.Log("[SPEEDOMETER] Speedometer UI avviata");

			// Sincronizza con velocità attuale
			_targetSpeed = _vehicleDataService.CurrentSpeed;
			_displayedSpeed = _targetSpeed;
			UpdateUI();
		}

		protected override void ManagedUpdate()
		{
			// Aggiornamenti con animazioni
			UpdateSmoothSpeed();
			UpdateSpeedColors();
			UpdateUI();
		}

		protected override void ManagedOnDestroy()
		{
			Debug.Log("[SPEEDOMETER] SpeedometerBehaviour distrutto");
			UnsubscribeFromEvents();
		}

		// Applica nuova configurazione
		public void ApplyConfiguration(SpeedometerConfig config)
		{
			_currentConfiguration = config;

			if (_speedProgressBar != null)
				_speedProgressBar.maxValue = config.MaxDisplaySpeed;

			Debug.Log($"[SPEEDOMETER] Configurazione applicata: MaxSpeed={config.MaxDisplaySpeed}");
		}

		// Configura stato iniziale di tutti i componenti UI
		private void SetupInitialUI()
		{
			if (_speedValueText != null)
			{
				_speedValueText.text = "0";
				_speedValueText.color = SpeedometerData.DEFAULT_SPEED_COLOR;
			}

			if (_speedUnitText != null)
			{
				_speedUnitText.text = SpeedometerData.SPEED_UNIT_LABEL;
				_speedUnitText.color = SpeedometerData.DEFAULT_SPEED_COLOR;
			}

			if (_speedProgressBar != null)
			{
				_speedProgressBar.minValue = 0f;
				_speedProgressBar.maxValue = _currentConfiguration?.MaxDisplaySpeed ?? SpeedometerData.MAX_DISPLAY_SPEED;
				_speedProgressBar.value = 0f;
				_speedProgressBar.interactable = false;
			}

			if (_progressBarFill != null)
				_progressBarFill.color = SpeedometerData.DEFAULT_SPEED_COLOR;

			Debug.Log("[SPEEDOMETER] UI iniziale configurata");
		}

		// Sottoscrizione eventi
		private void SubscribeToEvents()
		{
			_vehicleDataService.OnSpeedChanged += OnSpeedChanged;
			_broadcaster.Add<DriveModeChangedEvent>(OnDriveModeChanged);
			Debug.Log("[SPEEDOMETER] Eventi sottoscritti");
		}

		// Rimozione sottoscrizioni eventi
		private void UnsubscribeFromEvents()
		{
			if (_vehicleDataService != null)
				_vehicleDataService.OnSpeedChanged -= OnSpeedChanged;

			if (_broadcaster != null)
				_broadcaster.Remove<DriveModeChangedEvent>(OnDriveModeChanged);

			Debug.Log("[SPEEDOMETER] Eventi rimossi");
		}

		// Gestione cambio velocità - aggiorna anche colore
		private void OnSpeedChanged(float newSpeed)
		{
			_targetSpeed = newSpeed;
			_targetSpeedColor = SpeedometerData.GetSpeedColor(newSpeed);
		}

		// Gestione cambio modalità guida
		private void OnDriveModeChanged(DriveModeChangedEvent e)
		{
			Debug.Log($"[SPEEDOMETER] Modalità cambiata: {e.NewMode}");
		}

		// Aggiorna velocità displayed con animazione
		private void UpdateSmoothSpeed()
		{
			if (Mathf.Approximately(_displayedSpeed, _targetSpeed)) return;

			float damping = _currentConfiguration?.ResponseDamping ?? SpeedometerData.COMFORT_RESPONSE_DAMPING;
			_displayedSpeed = Mathf.Lerp(_displayedSpeed, _targetSpeed, damping * Time.deltaTime * 10f);

			// Snap finale per evitare problemi
			if (Mathf.Abs(_displayedSpeed - _targetSpeed) < 0.1f)
				_displayedSpeed = _targetSpeed;
		}

		// Colori aggiornati con transizione
		private void UpdateSpeedColors()
		{
			if (_currentSpeedColor == _targetSpeedColor) return;

			_currentSpeedColor = Color.Lerp(_currentSpeedColor, _targetSpeedColor, Time.deltaTime * 5f);

			// Applica colore a testo e progress bar
			if (_speedValueText != null)
				_speedValueText.color = _currentSpeedColor;

			if (_progressBarFill != null)
				_progressBarFill.color = _currentSpeedColor;
		}

		// Aggiorna tutti gli elementi UI con valori correnti
		private void UpdateUI()
		{
			// Display valore velocità formattato
			if (_speedValueText != null)
				_speedValueText.text = SpeedometerData.FormatSpeed(_displayedSpeed);

			// Progress bar limitazione valori per sicurezza
			if (_speedProgressBar != null)
				_speedProgressBar.value = Mathf.Clamp(_displayedSpeed, 0f, _speedProgressBar.maxValue);
		}
	}
}