using Cysharp.Threading.Tasks;
using System;
using System.IO;
using UnityEngine;
using XDay.Asset;
using XDay.WorldAPI;

namespace Game.Asset
{
    internal class RuntimeAssetLoader : IWorldAssetLoader
    {
        public RuntimeAssetLoader()
        {
        }

        public async UniTask InitASync()
        {
            //AssetManager.Init("Bundle");
            AssetManager.Init("Editor");

            var startInfo = new BundleAssetLoaderStartInfo()
            {
                //CDNURL = "http://10.234.112.24/cdntest",
                //CDNURL = "http://192.168.124.12/cdntest/",
                //CDNURL = "http://192.168.124.21/cdntest/",
                CDNURL = "",
            };

            await AssetManager.StartAsync(startInfo);
        }

        public bool Exists(string path)
        {
            return AssetManager.Exists(path);
        }

        public T Load<T>(string path) where T : UnityEngine.Object
        {
            return AssetManager.Load<T>(path);
        }

        public async UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object
        {
            return await AssetManager.LoadAsync<T>(path);
        }

        public byte[] LoadBytes(string path)
        {
            return AssetManager.LoadBytes(path);
        }

        public GameObject LoadGameObject(string path)
        {
            return AssetManager.LoadGameObject(path);
        }

        public async UniTask<GameObject> LoadGameObjectAsync(string path)
        {
            return await AssetManager.LoadGameObjectAsync(path);
        }

        public void LoadGameObjectAsync(string path, Action<GameObject> onLoaded)
        {
            AssetManager.LoadGameObjectAsync(path, onLoaded);
        }

        public string LoadText(string path)
        {
            return AssetManager.LoadText(path);
        }

        public Stream LoadTextStream(string path)
        {
            return AssetManager.LoadTextStream(path);
        }

        public void OnDestroy()
        {
        }

        public bool UnloadAsset(string path)
        {
            AssetManager.UnloadAsset(path);
            return true;
        }
    }
}
