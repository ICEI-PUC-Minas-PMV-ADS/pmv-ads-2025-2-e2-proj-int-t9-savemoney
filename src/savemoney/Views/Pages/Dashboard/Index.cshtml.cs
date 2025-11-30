using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

// Se você está redirecionando do servidor (handler), use RedirectToPage
public class IndexModel : PageModel
{
    public IActionResult OnPostOpenRelatorios()
    {
        return RedirectToPage("/Relatorios/Index");
    }
}