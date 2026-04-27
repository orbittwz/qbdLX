using QobuzApiSharp.Service;
using System;

namespace QobuzDownloaderX.Shared
{
    public static class QobuzApiServiceManager
    {
        private static QobuzApiService _apiService;

        public static QobuzApiService GetApiService()
        {
            if (_apiService == null)
                throw new InvalidOperationException("QobuzApiService not initialized");
            return _apiService;
        }

        public static void Initialize(string appId, string appSecret)
        {
            _apiService?.Dispose();
            _apiService = new QobuzApiService(appId, appSecret);
        }

        public static void Initialize()
        {
            _apiService?.Dispose();
            _apiService = new QobuzApiService();
        }

        public static void ReleaseApiService()
        {
            if (_apiService != null)
            {
                using (_apiService)
                {
                    _apiService = null;
                }
            }
        }
    }
}