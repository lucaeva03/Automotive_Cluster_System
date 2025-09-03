using ClusterAudi;
using ClusterAudiFeatures;
using UnityEngine;

public class WelcomeState : ClusterBaseState
{
	private IBroadcaster _broadcaster;
	private IWelcomeFeature _welcomeFeature;
	private bool _isInstantiated = false; // Soluzione problema istanziazioni multiple

	public WelcomeState(ClusterStateContext context) : base(context)
	{
		_broadcaster = context.Client.Services.Get<IBroadcaster>();
		_welcomeFeature = context.Client.Features.Get<IWelcomeFeature>();
	}

	// Attivazione Welcome
	public override void StateOnEnter()
	{
		Debug.Log("[WELCOME STATE] Ingresso in Welcome State");

		// Sottoscrivi eventi transizione dalla UI
		_broadcaster.Add<WelcomeTransitionEvent>(OnWelcomeTransition);

		// Istanzia scehrmata welcome
		InstantiateWelcome();
	}

	// Uscita da Welcome
	public override void StateOnExit()
	{
		Debug.Log("[WELCOME STATE] Uscita da Welcome State");
		_broadcaster.Remove<WelcomeTransitionEvent>(OnWelcomeTransition);
		_isInstantiated = false;
	}

	// Update: input debug per test rapidi modalità
	public override void StateOnUpdate()
	{
		// Input debug per transizioni rapide
		if (Input.GetKeyDown(KeyCode.F1))
			_context.ClusterStateMachine.GoTo(WelcomeData.ECO_MODE_STATE);
		else if (Input.GetKeyDown(KeyCode.F2))
			_context.ClusterStateMachine.GoTo(WelcomeData.COMFORT_MODE_STATE);
		else if (Input.GetKeyDown(KeyCode.F3))
			_context.ClusterStateMachine.GoTo(WelcomeData.SPORT_MODE_STATE);
	}

	// Istanzia UI Welcome
	private async void InstantiateWelcome()
	{
		if (_isInstantiated) return;

		try
		{
			await _welcomeFeature.InstantiateWelcomeFeature();
			_isInstantiated = true;
		}
		catch (System.Exception ex)
		{
			Debug.LogError($"[WELCOME STATE] Errore: {ex.Message}");
		}
	}

	// Gestione transizioni richieste dalla UI
	private void OnWelcomeTransition(WelcomeTransitionEvent e)
	{
		Debug.Log($"[WELCOME STATE] Transizione a: {e.TargetState}");

		// Validazione stato target prima della transizione
		if (WelcomeData.IsValidState(e.TargetState))
			_context.ClusterStateMachine.GoTo(e.TargetState);
		else
			Debug.LogWarning($"[WELCOME STATE] Stato non valido: {e.TargetState}");
	}
}