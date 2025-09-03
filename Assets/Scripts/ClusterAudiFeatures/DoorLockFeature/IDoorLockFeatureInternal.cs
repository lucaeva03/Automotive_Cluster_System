using ClusterAudi;
using UnityEngine;

namespace ClusterAudiFeatures
{
	public interface IDoorLockFeatureInternal : IFeatureInternal
	{
		Client GetClient();
		
		// Stato del lucchetto per il behaviour
		bool IsLocked { get; }

		// Imposta direttamente lo stato bloccato/sbloccato
		void SetLocked(bool locked);
		
		// Cambia lo stato del lucchetto
		void ToggleLock();
	}
}