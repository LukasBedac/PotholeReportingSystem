using GUIApps.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace GUIApps.Shared.Services
{
    public class WebAuthService : IAuthService
    {
        private readonly NavigationManager _navigation;

        public WebAuthService(NavigationManager navigation)
        {
            _navigation = navigation;
        }

        public Task LoginAsync()
        {
            _navigation.NavigateTo("Account/Login", forceLoad: true);
            return Task.CompletedTask;
        }

        public Task LogoutAsync()
        {
            _navigation.NavigateTo("Account/Logout", forceLoad: true);
            return Task.CompletedTask;
        }
    }
}
