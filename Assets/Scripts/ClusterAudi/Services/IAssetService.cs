using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudi
{
	// Interfaccia caricamento asset da Resources
    public interface IAssetService : IService
    {
		// Carica asset generico da path Resources
		public Task<T> Load<T>(string path) where T : Object;
		// Carica e istanzia prefab con MonoBehaviour come figlio (opz)
		public Task<T> InstantiateAsset<T>(string path, Transform parent = null) where T : Object;
    }
}
