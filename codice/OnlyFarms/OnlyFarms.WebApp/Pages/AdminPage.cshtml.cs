using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlyFarms.WebApp.Pages;

[Authorize(Policy = Roles.Admin)]
public class AdminPage : PageModel
{
    private  UserManager<ApplicationUser> _userManager { get; set; }
    private DataContext _context { get; set; }
    
    public IQueryable<ApplicationUser> UserList { get; set; }
    
    [BindProperty]
    public ApplicationUser CurrentUser { get; set; }
    
    public AdminPage([FromServices] UserManager<ApplicationUser> userManager, [FromServices] DataContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    
    public void OnGet()
    {
        UserList =
            from user in _userManager.Users
            where user.UserName != "admin@admin.com" && user.EmailConfirmed == false
            select user;
    }
    
    public async Task<IActionResult> OnPostSubmit()
    {
        var usr = await _context.Users.FindAsync(CurrentUser.Id);
        usr!.EmailConfirmed = true;
        
        _context.Users.Entry(usr).CurrentValues.SetValues(usr);
        
        await _userManager.UpdateAsync(usr);
        await _context.SaveChangesAsync();

        return RedirectToPage("./AdminPage");
    }
}