using UnityEngine;

namespace ClusterAudi
{
	// Classe che definisce struttura base per MonoBehaviour che comunica con Features
	public class BaseMonoBehaviour<TFeatureInternal> : MonoBehaviour where TFeatureInternal : IFeatureInternal
	{
		// Riferimento alla Feature che gestisce questo UI component
		protected TFeatureInternal _feature;

		// Broadcaster per eventi interni Feature - UI
		protected IBroadcaster _featureBroadcaster;

		// Variabile per evitare Update() prima dell'inizializzazione
		private bool _isInitialized = false;

		// Setup controllato dalla Feature
		public void Initialize(TFeatureInternal feature)
		{
			_feature = feature;
			_featureBroadcaster = feature.FeatureBroadcaster;

			// Lifecycle gestito da codice
			ManagedAwake();
			ManagedStart();

			_isInitialized = true;
		}

		// Unity Update con protezione - chiama solo se inizializzato
		private void Update()
		{
			if (_isInitialized)
			{
				ManagedUpdate();
			}
		}

		// Unity OnDestroy sempre attivo
		private void OnDestroy()
		{
			ManagedOnDestroy();
		}

		// Sostituisce Awake Unity
		protected virtual void ManagedAwake()
		{
		}

		// Sostituisce Start Unity
		protected virtual void ManagedStart()
		{

		}

		// Sostituisce Update Unity ma con controllo
		protected virtual void ManagedUpdate()
		{

		}

		// Sostituisce OnDestroy Unity
		protected virtual void ManagedOnDestroy()
		{

		}
	}
}