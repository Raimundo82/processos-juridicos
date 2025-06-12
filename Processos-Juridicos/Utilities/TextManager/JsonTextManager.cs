using System.Text.Json;
using Processos_Juridicos.Utilities.TextManager.Interfaces;

namespace Processos_Juridicos.Utilities.TextManager
{
    public class JsonTextManager : IJsonTextManager, IDisposable
    {
        private Dictionary<string, string> _resources = [];
        private readonly string _resourceFilePath;
        private FileSystemWatcher _fileWatcher = new();
        private readonly object _lock = new();
        private bool _disposed = false;

        public JsonTextManager(string resourceFilePath)
        {
            _resourceFilePath = resourceFilePath;
            LoadResources();
            SetupFileWatcher();
        }

        private void LoadResources()
        {
            try
            {
                if (File.Exists(_resourceFilePath))
                {
                    string jsonContent = File.ReadAllText(_resourceFilePath);
                    var resources = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent);
                    if (resources != null)
                    {
                        lock (_lock)
                        {
                            _resources = resources;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new FileNotFoundException(ex.Message);
            }
        }
        private void SetupFileWatcher()
        {

            var directory = Path.GetDirectoryName(_resourceFilePath);
            var fileName = Path.GetFileName(_resourceFilePath);

            if (directory == null || fileName == null)
            {
                throw new FileNotFoundException();
            }

            _fileWatcher = new FileSystemWatcher(directory, fileName)
            {
                NotifyFilter = NotifyFilters.LastWrite
            };

            _fileWatcher.Changed += (sender, e) =>
            {
                Thread.Sleep(100);
                LoadResources();
            };

            _fileWatcher.EnableRaisingEvents = true;
        }

        public string GetString(string key)
        {
            lock (_lock)
            {
                if (_resources.TryGetValue(key, out var value))
                {
                    return value;
                }
            }
            return $"[[{key}]]";
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _fileWatcher?.Dispose();
                }
                _disposed = true;
            }
        }

        ~JsonTextManager()
        {
            Dispose(false);
        }
    }
}