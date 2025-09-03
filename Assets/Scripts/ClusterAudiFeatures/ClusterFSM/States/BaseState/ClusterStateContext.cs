using ClusterAudi;

// Container per tutti i dati condivisi tra gli stati della State Machine
public class ClusterStateContext
{
	// Client per accesso alle feature
	public Client Client;

	// State Machine
	public ClusterFSM ClusterStateMachine;

	// Dati veicolo
	public IVehicleDataService VehicleData;

	// Sistema eventi 
	public IBroadcaster Broadcaster;
}