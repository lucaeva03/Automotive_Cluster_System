using UnityEngine;

namespace ClusterAudiFeatures
{
	public static class DoorLockData
	{
		// Path prefab e risorse
		public const string DOOR_LOCK_PREFAB_PATH = "DoorLock/DoorLockPrefab";
		public const string LOCKED_IMAGE_PATH = "DoorLock/img/Lock";
		public const string UNLOCKED_IMAGE_PATH = "DoorLock/img/Unlock";

		// Path audio
		public const string LOCK_SOUND_PATH = "Audio/SFX/DoorLock/LockSound";
		public const string UNLOCK_SOUND_PATH = "Audio/SFX/DoorLock/UnlockSound";

		// Configurazioni
		public const bool DEFAULT_LOCKED_STATE = true;
		public const float LOCK_IMAGE_SCALE = 1.0f;
		public const float DOOR_LOCK_VOLUME = 0.8f;
		public const int DOOR_LOCK_AUDIO_PRIORITY = 4;
		public const KeyCode DOOR_LOCK_TOGGLE_KEY = KeyCode.L;
	}
}