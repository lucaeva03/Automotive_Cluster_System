namespace ClusterAudi
{
	public class BaseFeature : IFeature, IFeatureInternal
	{
		// Variabili private e protette
		protected Client _client;
		protected IBroadcaster _broadcaster;
		protected IAssetService _assetService;
		protected IBroadcaster _featureBroadcaster;

		// Property readonly - controllo dell'accesso
		public IBroadcaster FeatureBroadcaster => _featureBroadcaster;

		// EQUIVALENTE A 
		//
		// public IBroadcaster FeatureBroadcaster
		// {
		//	 get
		//	 {
		//	 	return _featureBroadcaster;
		//	 }
		// }

		public BaseFeature(Client client)
		{
			// Accede a Client
			_client = client;

			// Ottiene i servizi base dal Locator
			_broadcaster = client.Services.Get<IBroadcaster>();
			_assetService = client.Services.Get<IAssetService>();

			// Crea broadcaster privato per comunicazione interna 
			_featureBroadcaster = new Broadcaster();
		}
	}
}