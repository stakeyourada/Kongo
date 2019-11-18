using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Kongo.Core.Extensions
{
	public static class HttpClientExtensions
	{
		/// <summary>
		/// ExtensionMethod to cast response to requested type T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="httpClient"></param>
		/// <param name="requestUri"></param>
		/// <returns></returns>
		public static async Task<T> GetAsync<T>(this HttpClient httpClient, string requestUri)
		{
			return await GetAsync<T>(httpClient, new Uri(requestUri));
		}

		/// <summary>
		/// ExtensionMethod to cast response to requested type T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="httpClient"></param>
		/// <param name="requestUri"></param>
		/// <returns></returns>
		public static async Task<T> GetAsync<T>(this HttpClient httpClient, Uri requestUri)
		{
			var response = await httpClient.GetAsync(requestUri);
			var responseContent = await response.Content.ReadAsStringAsync();

			return response == null ? default(T) : JsonConvert.DeserializeObject<T>(responseContent);
		}
	}
}
