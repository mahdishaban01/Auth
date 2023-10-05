using Blazored.LocalStorage;
using Common.DTOs;
using Newtonsoft.Json;
using System.Text;

namespace Blazor.Auth
{
    public interface IAuthenticationService
    {
        Task<bool> AuthenticateAsync(AuthenticationDTO authentication);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _client;
        private readonly ILocalStorageService _localStorage;
        public AuthenticationService(HttpClient client,ILocalStorageService localStorage)
        {
            _client = client;
            _localStorage = localStorage;
        }

        public async Task<bool> AuthenticateAsync(AuthenticationDTO authentication)
        {
            var content = JsonConvert.SerializeObject(authentication);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/account/signin", bodyContent);
            var contentTemp = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AuthenticationResponseDTO>(contentTemp);

            await _localStorage.SetItemAsync("accessToken", result.Token);
        }
    }
}
