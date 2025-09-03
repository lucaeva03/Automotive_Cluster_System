namespace ClusterAudi
{
	// Collegamento automatico per MonoBehaviour da prefab che devono trovare la loro Feature
	public class BaseSelfInjectedBehaviour<TFeatureInternal, TFeature> : BaseMonoBehaviour<TFeatureInternal>
		where TFeatureInternal : IFeatureInternal
		where TFeature : IFeature
	{
		// Si autoinizializza quando Unity crea il prefab
		private void Awake()
		{
			// Trova l'istanza
			Client client = Client.Instance;

			if (client == null)
			{
				return;
			}

			// Trova la Feature nel Locator
			var myFeature = client.Features.Get<TFeature>();

			// Cast sicuro a interfaccia interna e auto-setup
			if (myFeature is TFeatureInternal myFeatureInternal)
			{
				Initialize(myFeatureInternal);
			}
		}
	}
}