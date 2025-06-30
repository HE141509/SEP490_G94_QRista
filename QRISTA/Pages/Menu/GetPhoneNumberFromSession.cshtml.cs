using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QRB.Pages.Menu
{
    public class GetPhoneNumberFromSessionModel : PageModel
    {
        public IActionResult OnGet()
        {
            var phone = HttpContext.Session.GetString("PhoneNumber");
            return new JsonResult(new { phoneNumber = phone });
        }
    }
}
