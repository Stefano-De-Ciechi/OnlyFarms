using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Areas.Tokens.Pages;

[Authorize]     // e' sufficiente un utente autorizzato per accedere a questa pagina (Admin, FarmManager o WaterManager)
public class JwtToken : PageModel
{
    private readonly ITokenRepository _tokenRepository;
    
    [BindProperty]
    public Token UserToken { get; set; }

    public JwtToken(ITokenRepository tokenRepository)
    {
        _tokenRepository = tokenRepository;
    }
    
    public void OnGet(int days = 30)
    {
        UserToken = _tokenRepository.GenerateUserToken(days);
        // TODO accedere al tipo di utente, se e' un FarmManager bisogna anche generare un token per il sottosistema IoT
        /* qualcosa del tipo
        if (user.HasClaim(nameof(Roles), Roles.FarmManager))
        {
            IoTSubsystemToken = _tokenRepository.GenerateIotSubsystemToken      
        }
         */
    }
}