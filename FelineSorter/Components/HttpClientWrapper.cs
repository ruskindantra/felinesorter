using System;
using System.Net.Http;
using JetBrains.Annotations;

namespace FelineSorter.Components
{
    [UsedImplicitly]
    internal class HttpClientWrapper : HttpClient, IHttpClient
    {
        public HttpClientWrapper(Uri baseAddress)
        {
            BaseAddress = baseAddress;
        }
    }
}