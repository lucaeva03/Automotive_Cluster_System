using ClusterAudi;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public class DoorLockFeature : BaseFeature, IDoorLockFeature, IDoorLockFeatureInternal
	{
		// Stato serratura (false = sbloccato)
		private bool _isLocked = false;

		public DoorLockFeature(Client client) : base(client) { }

		// Istanzia prefab e inizializza behaviour
		public async Task InstantiateDoorLockFeature()
		{
			try
			{
				var instance = await _assetService.InstantiateAsset<DoorLockBehaviour>("DoorLock/DoorLockPrefab");
				instance?.Initialize(this);
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[DOOR LOCK] Error: {ex.Message}");
			}
		}

		// Proprietà per accesso stato
		public bool IsLocked => _isLocked;

		// Cambia stato bloccato - sbloccato
		public void ToggleLock() => SetLocked(!_isLocked);

		// Accesso al client per behaviour
		public Client GetClient() => _client;

		// Notifica cambio stato e evento audio
		public void SetLocked(bool locked)
		{
			if (_isLocked == locked) return;

			bool previous = _isLocked;
			_isLocked = locked;

			// Notifica cambio stato
			_broadcaster.Broadcast(new DoorLockStateChangedEvent(_isLocked, previous));

			// Richiesta di audio giusto
			string audioPath = locked ? "Audio/SFX/DoorLock/LockSound" : "Audio/SFX/DoorLock/UnlockSound";
			_broadcaster.Broadcast(new DoorLockAudioRequestEvent(audioPath, 0.8f, 3, locked));
		}
	}
}