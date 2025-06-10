using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace A01_FuNewsManagement_FE.Pages.Accounts
{
    public class GoogleRedirectModel : PageModel
    {
        public IActionResult OnGet(string access_token, string refresh_token)
        {
            if (string.IsNullOrEmpty(access_token))
            {
                return RedirectToPage("/Accounts/Login");
            }

            // Lưu session
            HttpContext.Session.SetString("AccessToken", access_token);
            HttpContext.Session.SetString("RefreshToken", refresh_token);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(access_token);
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            if (!string.IsNullOrEmpty(email))
            {
                HttpContext.Session.SetString("UserEmail", email);
            }

            // Chuyển hướng theo role hoặc mặc định đến /Articles
            if (role == "Admin")
            {
                return RedirectToPage("/Admin/Users/Index");
            }
            else if (role == "Staff")
            {
                return RedirectToPage("/Staff/Categories/Index");
            }
            else
            {
                return Redirect("/Articles");
            }
        }
    }
}