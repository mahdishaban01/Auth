using Blazor.Providers;
using Blazored.LocalStorage;
using Common.DTOs;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Text;

namespace Blazor.Auth
{
	public interface IAuthenticationService
	{
		Task<bool> AuthenticateAsync(AuthenticationDTO authentication);
		Task Logout();
	}

	public class AuthenticationService : IAuthenticationService
	{
		private readonly HttpClient _client;
		private readonly ILocalStorageService _localStorage;
		private readonly AuthenticationStateProvider _authenticationStateProvider;
		public AuthenticationService(HttpClient client, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
		{
			_client = client;
			_localStorage = localStorage;
			_authenticationStateProvider = authenticationStateProvider;
		}

		public async Task<bool> AuthenticateAsync(AuthenticationDTO authentication)
		{
			var content = JsonConvert.SerializeObject(authentication);
			var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
			var response = await _client.PostAsync("account/signin", bodyContent);
			var contentTemp = await response.Content.ReadAsStringAsync();
			var result = JsonConvert.DeserializeObject<AuthenticationResponseDTO>(contentTemp);

			await _localStorage.SetItemAsync("accessToken", result.Token);

			await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedIn();
			return true;
		}

		public async Task Logout()
		{
			await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedOut();
		}
	}
}
