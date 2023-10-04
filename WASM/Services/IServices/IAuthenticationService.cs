using Common.DTOs;

namespace WASM.Services.IServices
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResponseDTO> Login(AuthenticationDTO userFromAuthentication);

        Task Logout();
    }
}
