using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace IDosGames
{
	public static class AzureService
	{
		public const string URL_MAKE_TRANSACTION = "https://walletapplication.azurewebsites.net/api/TryMakeTransaction?code=7PIUFqw3Ux5ruoK6aSB3_QM1ns4BRViTqakoSyal0KAjAzFu5-4E4Q==";

		public static async Task<string> TryMakeTransaction(JObject requestBody)
		{
			return await SendRequest(URL_MAKE_TRANSACTION, requestBody);
		}

		private static async Task<string> SendRequest(string functionURL, JObject requestBody)
		{
			byte[] bodyRaw = new UTF8Encoding(true).GetBytes(requestBody.ToString());

			UnityWebRequest webRequest = new()
			{
				url = functionURL,
				method = RequestMethodType.POST.ToString(),
				uploadHandler = new UploadHandlerRaw(bodyRaw),
				downloadHandler = new DownloadHandlerBuffer(),
			};

			webRequest.SetRequestHeader("Content-Type", "application/json");

			UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();

			while (!asyncOperation.isDone)
			{
				await Task.Yield();
			}

			if (webRequest.result == UnityWebRequest.Result.Success)
			{
				Debug.Log($"Success request: {webRequest.downloadHandler.text}");
				return webRequest.downloadHandler.text;
			}
			else
			{
				Debug.LogError($"Error request: {webRequest.error}");
				return null;
			}
		}
	}
}