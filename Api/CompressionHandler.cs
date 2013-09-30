using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Cab9.Api.Compression
{
    public class CompressionHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(responseToCompleteTask =>
            {
                HttpResponseMessage response = responseToCompleteTask.Result;

                if (request.Headers.AcceptEncoding != null)
                {
                    string encodingType = request.Headers.AcceptEncoding.First().Value;                    
                    response.Content = new CompressedContent(response.Content, encodingType);
                }
 
                return response;
            },
            TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
 
}