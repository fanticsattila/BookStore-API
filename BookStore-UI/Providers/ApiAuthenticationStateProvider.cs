using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookStore_UI.Providers
{
    public class ApiAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        
        public ApiAuthenticationStateProvider(ILocalStorageService localStorage, JwtSecurityTokenHandler tokenHandler)
        {
            _localStorage = localStorage;
            _tokenHandler = tokenHandler;
        }

        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                string savedToken = await _localStorage.GetItemAsync<string>("authToken");
                if(string.IsNullOrWhiteSpace(savedToken))
                {
                    //Nincs bejelentkezve senki
                    return new AuthenticationState(new System.Security.Claims.ClaimsPrincipal(new ClaimsIdentity()));
                }
                JwtSecurityToken tokenContent = _tokenHandler.ReadJwtToken(savedToken);
                DateTime expiry = tokenContent.ValidTo;
                if(expiry < DateTime.Now)
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    //Nincs bejelentkezve senki
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                //Get claims from token and build auth user object
                List<Claim> claims = ParseClaims(tokenContent).ToList();
                ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
                
                //return authenticated person
                return new AuthenticationState(user);
            }
            catch (Exception ex)
            {
                return new AuthenticationState(new System.Security.Claims.ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task LoggedIn()
        {
            string savedToken = await _localStorage.GetItemAsync<string>("authToken");
            JwtSecurityToken tokenContent = _tokenHandler.ReadJwtToken(savedToken);
            List<Claim> claims = ParseClaims(tokenContent).ToList();
            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            Task< AuthenticationState> authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public void LoggedOut()
        {
            ClaimsPrincipal nobody = new ClaimsPrincipal(new ClaimsIdentity());
            Task<AuthenticationState> authState = Task.FromResult(new AuthenticationState(nobody));
            NotifyAuthenticationStateChanged(authState);
        }

        private IList<Claim> ParseClaims(JwtSecurityToken tokentContent)
        {
            List<Claim> claims = tokentContent.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.Name, tokentContent.Subject));

            return claims;
        }
    }
}
