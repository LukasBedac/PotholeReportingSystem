using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIApps.Shared.Services.Interfaces
{
    public interface IAuthService
    {
        Task LoginAsync();
        Task LogoutAsync();
    }
}
