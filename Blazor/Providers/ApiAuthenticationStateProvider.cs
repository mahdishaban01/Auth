﻿using Blazor.Helper;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Blazor.Providers
{
	public class ApiAuthenticationStateProvider : AuthenticationStateProvider
	{
		private readonly ILocalStorageService _localStorage;
		private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
		public ApiAuthenticationStateProvider(ILocalStorageService localStorage, JwtSecurityTokenHandler jwtSecurityTokenHandler)
		{
			_localStorage = localStorage;
			_jwtSecurityTokenHandler = jwtSecurityTokenHandler;
		}

		public override async Task<AuthenticationState> GetAuthenticationStateAsync()
		{
			var user = new ClaimsPrincipal(new ClaimsIdentity());
			var savedToken = await _localStorage.GetItemAsync<string>("accessToken");
			if (savedToken == null)
				return new AuthenticationState(user);

			var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);

			if (tokenContent.ValidTo < DateTime.Now)
				return new AuthenticationState(user);

			var claims = await GetClaims();

			user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));

			return new AuthenticationState(user);
		}

		public async Task LoggedIn()
		{
			var claims = await GetClaims();
			var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
			var authState = Task.FromResult(new AuthenticationState(user));
			NotifyAuthenticationStateChanged(authState);
		}

		public async Task LoggedOut()
		{
			await _localStorage.RemoveItemAsync("accessToken");
			var nobody = new ClaimsPrincipal(new ClaimsIdentity());
			var authState = Task.FromResult(new AuthenticationState(nobody));
			NotifyAuthenticationStateChanged(authState);
		}

		private async Task<IEnumerable<Claim>> GetClaims()
		{
			var savedToken = await _localStorage.GetItemAsync<string>("accessToken");
			var claims = JwtParser.ParseClaimsFromJwt(savedToken);
			return claims;
		}
	}
}
