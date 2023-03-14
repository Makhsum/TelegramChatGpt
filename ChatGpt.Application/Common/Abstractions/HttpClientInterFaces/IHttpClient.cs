using System.Net.Http.Headers;

namespace ChatGpt.Application.Common.Abstractions.HttpClientInterFaces;

public interface IHttpClient
{
    Task<HttpResponseMessage> GetAsync(string requestUrl);
    Task<HttpResponseMessage> PostAsync(string requestUrl,HttpContent content);
    HttpRequestHeaders DefaultRequestHeaders { get; set; }
}