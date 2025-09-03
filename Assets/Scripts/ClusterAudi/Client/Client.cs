using UnityEngine;

namespace ClusterAudi
{
	// Classe che definisce struttura base client
	public abstract class Client : MonoBehaviour
	{
		public static Client Instance { get; private set; }

		// Creazione Servizi e Feature
		public Locator<IService> Services;
		public Locator<IFeature> Features;

		protected IBroadcaster _broadcaster;

		// Metodi che devono essere implementati nei figli Cluster obbligatoriamente
		public abstract void InitFeatures();
		public abstract void StartClient();

		// Registrazione Servizi
		public void InitServices()
		{
			// Registrazione servizi base
			Services.Add<IBroadcaster>(new Broadcaster());
			Services.Add<IAssetService>(new AssetService());

			// Registrazione servizio specifico per auto
			Services.Add<IVehicleDataService>(new VehicleDataService());
		}

		// Garantisce che ci sia un solo Client attivo
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
				return;
			}

			Services = new Locator<IService>();
			Features = new Locator<IFeature>();

			InitServices();
			InitFeatures();

			_broadcaster = Services.Get<IBroadcaster>();
		}

		private void Start()
		{
			StartClient();
		}

		private void OnDestroy()
		{

		}
	}
}