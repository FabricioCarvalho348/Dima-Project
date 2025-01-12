using Dima.Core.Handlers;
using Dima.Core.Requests.Account;
using Dima.Web.Security;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Identity;

public partial class LogoutPage : ComponentBase
{
    #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    
    [Inject] public IAccountHandler Handler { get; set; } = null!;
    
    [Inject] public NavigationManager Navigation { get; set; } = null!;
    
    [Inject] public ICookieAuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

    #endregion
    
    #region Properties

    public bool IsBusy { get; set; } = false;
    public LoginRequest InputModel { get; set; } = new();

    #endregion
    
    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        if (await AuthenticationStateProvider.CheckAuthenticatedAsync())
        {
            await Handler.LogoutAsync();
            await AuthenticationStateProvider.GetAuthenticationStateAsync();
            AuthenticationStateProvider.NotifyAuthenticationStateChanged();
        }

        await base.OnInitializedAsync();
    }

    #endregion
    
}