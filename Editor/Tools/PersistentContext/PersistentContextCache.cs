using System;
using System.Collections.Generic;
using System.IO;
using EasyToolkit.Core.Collections;
using EasyToolkit.Core.Editor;
using EasyToolkit.Core.Editor.Internal;
using EasyToolkit.Core.Patterns;
using EasyToolkit.Serialization;
using UnityEditor;
using UnityEngine;

namespace EasyToolkit.Inspector.Editor
{
    [InitializeOnLoad]
    public class PersistentContextCache : Singleton<PersistentContextCache>
    {
        private static readonly string CacheFileName = "PersistentContextCache.bytes";
        private static string s_cacheFilePath;
        private static string s_cacheDirectory;

        private static string CacheDirectory
        {
            get
            {
                if (s_cacheDirectory == null)
                    s_cacheDirectory = EditorAssetPaths.GetModuleTemporaryDirectory("Inspector");
                return s_cacheDirectory;
            }
        }
        private static string CacheFilePath
        {
            get
            {
                if (s_cacheFilePath == null)
                    s_cacheFilePath = CacheDirectory + "/" + CacheFileName;
                return s_cacheFilePath;
            }
        }

        private PersistentContextDirectory _cache;
        private float _cacheMemorySize;
        private bool _initialized = false;
        private DateTime _lastSaveTime = DateTime.MinValue;
        private double _lastUpdateTime;

        static PersistentContextCache()
        {
            UnityEditorEventUtility.DelayAction(() => Instance.EnsureInitialize());
        }

        private PersistentContextCache()
        {
            _cache = new PersistentContextDirectory();
        }

        private void EnsureInitialize()
        {
            if (_initialized) return;
            _initialized = true;

            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
            AppDomain.CurrentDomain.DomainUnload -= OnDomainUnload;
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            LoadCache();
        }

        public GlobalPersistentContext<TValue> GetContext<TValue>(string key, out bool isNew)
        {
            EnsureInitialize();

            if (_cache.TryGetValue(key, out var originCtx) && originCtx is GlobalPersistentContext<TValue> castedCtx)
            {
                isNew = false;
                return castedCtx;
            }

            var context = GlobalPersistentContext<TValue>.Create();
            _cache[key] = context;
            isNew = true;
            return context;
        }

        private void OnDomainUnload(object sender, EventArgs e)
        {
            SaveCache();
        }

        private void OnUpdate()
        {
            if (EditorApplication.timeSinceStartup - _lastUpdateTime < 1.0)
                return;
            _lastUpdateTime = EditorApplication.timeSinceStartup;
            //TODO clear unused cache
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            var now = DateTime.Now;
            if (now - _lastSaveTime > TimeSpan.FromSeconds(1))
            {
                _lastSaveTime = now;
                SaveCache();
            }
        }

        void LoadCache()
        {
            try
            {
                if (!Directory.Exists(CacheDirectory))
                    Directory.CreateDirectory(CacheDirectory);
                if (!File.Exists(CacheFilePath))
                    File.Create(CacheFilePath).Close();

                var data = File.ReadAllBytes(CacheFilePath);
                _cache = EasySerializer.DeserializeFromBinary<PersistentContextDirectory>(data);
                _cache ??= new PersistentContextDirectory();
                _cacheMemorySize = data.Length;
            }
            catch (UnauthorizedAccessException)
            {
                Debug.LogError(
                    $"Failed to load persistent context cache due to insufficient permissions. " +
                    $"Cache file path: {CacheFilePath}. " +
                    $"Please check file permissions or run Unity as administrator.");
            }
            catch (IOException e)
            {
                Debug.LogError(
                    $"Failed to load persistent context cache due to an I/O error. " +
                    $"Cache file path: {CacheFilePath}. Error: {e.Message}. " +
                    $"The file may be corrupted or locked by another process. " +
                    $"Try closing other Unity instances or delete the cache file to reset.");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError(
                    $"Failed to load persistent context cache due to an unexpected error. " +
                    $"Cache file path: {CacheFilePath}. Error: {e.Message}. " +
                    $"A new empty cache will be created instead.");
            }
            finally
            {
                _cache ??= new PersistentContextDirectory();
            }
        }

        void SaveCache()
        {
            var fileInfo = new FileInfo(CacheFilePath);
            try
            {
                if (_cache.Count == 0)
                {
                    if (!fileInfo.Exists)
                        return;
                    DeleteCache();
                    return;
                }

                if (!Directory.Exists(CacheDirectory))
                    Directory.CreateDirectory(CacheDirectory);

                var unityReferences = new List<UnityEngine.Object>();

                File.WriteAllBytes(CacheFilePath, EasySerializer.SerializeToBinary(_cache, ref unityReferences));
                if (unityReferences.IsNotNullOrEmpty())
                {
                    Debug.Log($"Reference unity objects({string.Join(", ", unityReferences)}) in persistent context is not supported.");
                }
                _cacheMemorySize = fileInfo.Length;
            }
            catch (UnauthorizedAccessException)
            {
                Debug.LogError(
                    $"Failed to save persistent context cache due to insufficient permissions. " +
                    $"Cache file path: {CacheFilePath}. " +
                    $"Please check file permissions or run Unity as administrator.");
            }
            catch (IOException e)
            {
                Debug.LogError(
                    $"Failed to save persistent context cache due to an I/O error. " +
                    $"Cache file path: {CacheFilePath}. Error: {e.Message}. " +
                    $"The disk may be full or the file may be locked by another process. " +
                    $"Try closing other Unity instances or free up disk space.");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError(
                    $"Failed to save persistent context cache due to an unexpected error. " +
                    $"Cache file path: {CacheFilePath}. Error: {e.Message}. " +
                    $"The cache will be saved again on the next attempt.");
            }
        }

        void DeleteCache()
        {
            _cacheMemorySize = 0;
            _cache.Clear();
            if (File.Exists(CacheFilePath))
                File.Delete(CacheFilePath);
        }
    }
}
