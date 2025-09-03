using ClusterAudi;

public abstract class ClusterBaseState : IState
{
	// Dati condivisi tra stati
	protected ClusterStateContext _context;

	//Riceve Client, VehicleService, Broadcaster
	public ClusterBaseState(ClusterStateContext context)
	{
		_context = context; 
	}

	// Ogni stato figlio deve implementare questi
	public abstract void StateOnEnter();
	public abstract void StateOnExit();
	public abstract void StateOnUpdate();
}