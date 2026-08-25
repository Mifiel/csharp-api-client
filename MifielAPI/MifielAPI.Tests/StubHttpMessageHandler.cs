using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MifielApiTests
{
    internal sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses = new Queue<HttpResponseMessage>();

        public HttpRequestMessage LastRequest { get; private set; }

        public int SendCount { get; private set; }

        public void EnqueueJson(string json, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _responses.Enqueue(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(json ?? "{}", Encoding.UTF8, "application/json")
            });
        }

        public void EnqueueBytes(byte[] bytes, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _responses.Enqueue(new HttpResponseMessage(statusCode)
            {
                Content = new ByteArrayContent(bytes ?? Array.Empty<byte>())
            });
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            SendCount++;

            if (_responses.Count == 0)
            {
                throw new InvalidOperationException("StubHttpMessageHandler has no queued response. Tests must not make real HTTP calls.");
            }

            var response = _responses.Dequeue();
            response.RequestMessage = request;
            return Task.FromResult(response);
        }
    }
}
