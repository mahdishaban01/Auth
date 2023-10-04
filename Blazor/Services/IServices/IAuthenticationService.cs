using Common.DTOs;

namespace Blazor.Services.IServices
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponseDTO> Login(AuthenticationDTO userFromAuthentication);

        Task Logout();
    }
}
