using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ClusterAudi
{
    public class AssetService : IAssetService
    {
        // Dizionario risorse già caricate
        private Dictionary<string, Object> _myAssets;

        // Dizionario risorse in richiesta di caricamento
        private Dictionary<string, ResourceRequest> _currentlyLoadingAssets;

        // Impostazione dizionario vuoto + asset in caricamento
        public AssetService()
        {
            _myAssets = new Dictionary<string, Object>();
            _currentlyLoadingAssets = new Dictionary<string, ResourceRequest>();
        }

        public async Task<T> Load<T>(string path) where T : Object
        {
            // Controlla se è gia caricato
            if (_myAssets.ContainsKey(path))
            {
                return (T)_myAssets[path];
            }

            // controlla se lo sta caricando
            if (!_currentlyLoadingAssets.ContainsKey(path))
            {
                _currentlyLoadingAssets[path] = Resources.LoadAsync<T>(path);
            }

            // Attende caricamento
            ResourceRequest myLoadingProcess = _currentlyLoadingAssets[path];

            while (!myLoadingProcess.isDone)
            {
                await Task.Delay(1);
            }

            // Salva in cache per la prossima chiamata
            _currentlyLoadingAssets.Remove(path);

            _myAssets[path] = myLoadingProcess.asset;

            return (T)_myAssets[path];
        }

        // Metodo usato per caricare gli asset dalle feature in quanto crea una copia dell'originale da poter modificare
        public async Task<T> InstantiateAsset<T>(string path, Transform parent = null) where T : Object
        {
            T asset = await Load<T>(path);
            return asset == null ? null : Object.Instantiate(asset, parent);
        }
    }
}

