using ClusterAudi;
using UnityEngine;
using UnityEngine.UI;

namespace ClusterAudiFeatures
{
	public class DoorLockBehaviour : BaseMonoBehaviour<IDoorLockFeatureInternal>
	{
		// Componenti prefab
		[SerializeField] private Button _lockButton;
		[SerializeField] private Image _lockImage;

		// Definizione servizi e sprite
		private IBroadcaster _broadcaster;
		private Sprite _lockedSprite;
		private Sprite _unlockedSprite;

		protected override void ManagedAwake()
		{
			_broadcaster = _feature.GetClient().Services.Get<IBroadcaster>();

			// Carica sprite e configura pulsante
			LoadSprites();
			SetupButton();

			// Sottoscrive eventi di cambio stato
			_broadcaster.Add<DoorLockStateChangedEvent>(OnStateChanged);
		}

		protected override void ManagedUpdate()
		{
			// Input con tasto
			if (Input.GetKeyDown(KeyCode.L))
				_feature.ToggleLock();
		}

		protected override void ManagedOnDestroy()
		{
			// Rimuove sottoscrizioni eventi
			_broadcaster?.Remove<DoorLockStateChangedEvent>(OnStateChanged);
		}

		// Carica sprite da Resources per locked/unlocked
		private void LoadSprites()
		{
			_lockedSprite = Resources.Load<Sprite>(DoorLockData.LOCKED_IMAGE_PATH);
			_unlockedSprite = Resources.Load<Sprite>(DoorLockData.UNLOCKED_IMAGE_PATH);
		}

		// Click pulsante e display inziale
		private void SetupButton()
		{
			_lockButton?.onClick.AddListener(() => _feature.ToggleLock());
			UpdateDisplay();
		}

		// Gestore cambio di stato
		private void OnStateChanged(DoorLockStateChangedEvent e) => UpdateDisplay();

		// Aggiorna immaine sprite in base a stato
		private void UpdateDisplay()
		{
			if (_lockImage && _lockedSprite && _unlockedSprite)
				_lockImage.sprite = _feature.IsLocked ? _lockedSprite : _unlockedSprite;
		}
	}
}