using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace A01_FuNewsManagement_FE.Pages.Accounts
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnPost()
        {
            HttpContext.Session.Clear(); // Xoá toàn bộ session
            return RedirectToPage("/Index");
        }
    }
}
