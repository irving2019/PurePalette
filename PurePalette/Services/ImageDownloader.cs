using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PurePalette.Services
{
    public class ImageDownloader
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task<byte[]?> DownloadImageAsync(string url, CancellationToken token)
        {
            try
            {
                using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, token);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsByteArrayAsync(token);
            }

            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}