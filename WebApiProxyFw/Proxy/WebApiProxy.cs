using WebApiProxy.Exceptions;
using WebApiProxy.Manager.ProxyGenerator;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebApiProxy.Proxy
{
    /// <summary>
    /// Proxy type for web api clients
    /// </summary>
    public class WebApiProxy : DynamicProxy
    {
        internal Uri Endpoint;

        internal HttpClient HttpClient;
        //internal IServiceProvider ServiceProvider;
        internal HttpContext HttpContext;
        internal WebApiEndpointOptions Options;
        internal CorrelationTokenFactory CorrelationTokenFactory;

        protected override bool TryInvokeMember(Type interfaceType, string name, object[] args, out object result)
        {
            var descriptor = new MethodDescriptor(interfaceType, name, args);
            var query = descriptor.CreateQueryString(args);
            var contentType = GetContentType();
            var body = ContentHelper.SerializeBody(descriptor.GetBodyParam(args), Options.ContentType);

            try
            {
                var task = ExecuteRequestAsync(query, descriptor.HttpMethod, contentType, body);
                task.Wait();
                var message = task.Result;

                if (message.IsSuccessStatusCode)
                {
                    if (descriptor.MethodInfo.ReturnType.Name.StartsWith("Task"))
                    {
                        var type = descriptor.MethodInfo.ReturnType.GetGenericArguments().FirstOrDefault();
                        result = type != null 
                            ? Task.FromResult(ContentHelper.DeserializeBodyFromResponse(type, message))
                            : Task.CompletedTask;
                    }
                    else
                    {
                        result = ContentHelper.DeserializeBodyFromResponse(descriptor.MethodInfo.ReturnType, message);
                    }

                    return true;
                }
                else
                {
                    try
                    {
                        var info = (ExceptionInfo)ContentHelper.DeserializeBodyFromResponse(typeof(ExceptionInfo), message);
                        throw new WebApiException(info);
                    }
                    catch
                    {
                        var textTask = message.Content.ReadAsStringAsync();
                        textTask.Wait();
                        throw new WebApiException(new Exception(textTask.Result));
                    }
                }
            }
            catch(Exception ex)
            {
                throw new WebApiException(ex);
            }
        }

        #region Not implemented
        protected override bool TryGetMember(Type interfaceType, string name, out object result)
        {
            throw new NotImplementedException();
        }

        protected override bool TrySetEvent(Type interfaceType, string name, object value)
        {
            throw new NotImplementedException();
        }

        protected override bool TrySetMember(Type interfaceType, string name, object value)
        {
            throw new NotImplementedException();
        }
        #endregion

        private async Task<HttpResponseMessage> ExecuteRequestAsync(string url, string method, string contentType, string body)
        {
            using (HttpClient)
            {
                HttpClient.BaseAddress = Endpoint;
                using (var request = new HttpRequestMessage(new HttpMethod(method), url))
                {
                    if (CorrelationTokenFactory != null)
                    {
                        request.Headers.Add(
                            CorrelationTokenFactory.CorrelationTokenHeaderName,
                            CorrelationTokenFactory.GetCorrelationToken());
                    }

                    request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(contentType));
                    request.Content = new StringContent(body ?? "", Encoding.UTF8, contentType);

                    var responseTask = HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead);
                    responseTask.Wait();
                    var response = responseTask.Result;
                    return response;
                }
            }
        }

        private string GetContentType()
        {
            var contentType = HttpContext?.Request?.ContentType;
            return string.IsNullOrEmpty(contentType) ? Options.ContentType : contentType;
        }

        //private HttpClient CreateHttpClient()
        //{
        //    var httpClient = ServiceProvider.GetService(typeof(HttpClient)) as HttpClient ?? new HttpClient();

        //    return httpClient;
        //}
    }
}
