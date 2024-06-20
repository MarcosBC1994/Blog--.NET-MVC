using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using blogteste.Models;

[Authorize]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UsersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // Lista de usuários
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users;
        return View(users);
    }

    // Detalhes de um usuário
    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    // Adicionar, Editar, Excluir funcionalidades podem ser adicionadas aqui
}
