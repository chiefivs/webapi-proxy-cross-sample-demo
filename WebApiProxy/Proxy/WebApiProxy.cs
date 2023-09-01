#if NET5_0_OR_GREATER
using Microsoft.AspNetCore.Http;
#endif
#if NET46_OR_GREATER
using HttpMethods = WebApiProxy.HttpMethods;
#endif
using WebApiProxy.Exceptions;
using WebApiProxy.Manager.ProxyGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
#if NET46_OR_GREATER
using System.Web;
#endif

namespace WebApiProxy.Proxy
{
    /// <summary>
    /// Proxy type for web api clients
    /// </summary>
    public class WebApiProxy : DynamicProxy
    {
        internal Uri Endpoint;
        internal HttpClient HttpClient;
        internal HttpContext HttpContext;
        internal CorrelationTokenFactory CorrelationTokenFactory;
        internal WebApiEndpointOptions Options;

        protected override bool TryInvokeMember(Type interfaceType, string name, object[] args, out object result)
        {
            var descriptor = new MethodDescriptor(interfaceType, name, args);
            var query = descriptor.CreateQueryString(args);
            var contentType = GetContentType();
            var headers = (Options.TransferHeaders ?? new string[] { })
                .Select(h => new
                {
                    name = h,
                    value = GetHeaderValue(h)
                })
                .Where(i => !string.IsNullOrEmpty(i.value))
                .ToDictionary(i => i.name, i => i.value);

            var body = ContentHelper.SerializeBody(descriptor.GetBodyParam(args), contentType);

            try
            {
                var task = ExecuteRequestAsync(query, descriptor.HttpMethod, contentType, headers, body);
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
                if (ex is WebApiException)
                    throw;
                
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

        private async Task<HttpResponseMessage> ExecuteRequestAsync(string url, string method, string contentType, Dictionary<string, string> headers, string body)
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
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }

                    if(method == HttpMethods.Post || method == HttpMethods.Put)
                        request.Content = new StringContent(body ?? "", Encoding.UTF8, contentType);

                    var responseTask = HttpClient.SendAsync(request);
                    responseTask.Wait();
                    return responseTask.Result; ;
                }
            }
        }

        private string GetContentType()
        {
            var contentType = HttpContext?.Request?.ContentType;
            return string.IsNullOrEmpty(contentType) ? Options.ContentType : contentType;
        }

        private string GetHeaderValue(string name)
        {
            return HttpContext?.Request?.Headers[name];
        }
    }
}
