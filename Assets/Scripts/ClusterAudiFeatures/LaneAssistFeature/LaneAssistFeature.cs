using System.Threading.Tasks;
using ClusterAudi;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public class LaneAssistFeature : BaseFeature, ILaneAssistFeature, ILaneAssistFeatureInternal
	{
		private LaneAssistData.LaneAssistState _currentState = LaneAssistData.LaneAssistState.Disabled;
		private bool _isSystemEnabled = true;
		private LaneAssistBehaviour _behaviour;

		public LaneAssistFeature(Client client) : base(client)
		{
			Debug.Log("[LANE ASSIST FEATURE] LaneAssistFeature inizializzata");

			// Sottoscrive eventi audio
			_broadcaster.Add<LaneAssistAudioRequestEvent>(OnAudioRequest);
		}

		// Istanzia UI lane assist da prefab
		public async Task InstantiateLaneAssistFeature()
		{
			Debug.Log("[LANE ASSIST FEATURE] Istanziazione LaneAssist...");

			try
			{
				var instance = await _assetService.InstantiateAsset<LaneAssistBehaviour>(
					LaneAssistData.LANE_ASSIST_PREFAB_PATH);

				if (instance != null)
				{
					instance.Initialize(this);
					_behaviour = instance;
					Debug.Log("[LANE ASSIST FEATURE] UI istanziata da prefab");
				}
				else
				{
					Debug.LogError($"[LANE ASSIST FEATURE] Prefab non trovato");
				}
			}
			catch (System.Exception ex)
			{
				Debug.LogError($"[LANE ASSIST FEATURE] Errore: {ex.Message}");
			}
		}

		// Abilita/disabilita sistema lane assist
		public void SetLaneAssistEnabled(bool enabled)
		{
			_isSystemEnabled = enabled;
			_currentState = enabled ? LaneAssistData.LaneAssistState.Active : LaneAssistData.LaneAssistState.Disabled;
			Debug.Log($"[LANE ASSIST FEATURE] Sistema: {(_isSystemEnabled ? "ON" : "OFF")}");
		}

		// Restituisce stato corrente del sistema
		public LaneAssistData.LaneAssistState GetCurrentState()
		{
			return _currentState;
		}

		// Accesso al client per behaviour
		public Client GetClient() => _client;

		// Gestione richieste audio
		private void OnAudioRequest(LaneAssistAudioRequestEvent e)
		{

		}
	}
}