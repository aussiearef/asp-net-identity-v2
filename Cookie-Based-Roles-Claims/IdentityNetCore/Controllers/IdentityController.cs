using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityNetCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityNetCore.Controllers;

public class IdentityController(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    RoleManager<IdentityRole> roleManager,
    IUrlHelper urlHelper)
    : Controller
{
    public Task<IActionResult> Signup()
    {
        var model = new SignupViewModel { Role = "Member" };
        return Task.FromResult<IActionResult>(View(model));
    }

    [HttpPost]
    public async Task<IActionResult> Signup(SignupViewModel model)
    {
        if (ModelState.IsValid)
        {
            if (!await roleManager.RoleExistsAsync(model.Role))
            {
                var role = new IdentityRole { Name = model.Role };
                var roleResult = await roleManager.CreateAsync(role);
                if (!roleResult.Succeeded)
                {
                    var errors = roleResult.Errors.Select(s => s.Description);
                    ModelState.AddModelError("Role", string.Join(",", errors));
                    return View(model);
                }
            }


            if (await userManager.FindByEmailAsync(model.Email) == null)
            {
                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var claim = new Claim("Department", model.Department);
                    await userManager.AddClaimAsync(user, claim);
                    await userManager.AddToRoleAsync(user, model.Role);
                    return RedirectToAction("Signin");
                }

                ModelState.AddModelError("Signup", string.Join("", result.Errors.Select(x => x.Description)));
                return View(model);
            }
        }

        return View(model);
    }


    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId);

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (result.Succeeded) return RedirectToAction("Signin");

        return new NotFoundResult();
    }

    public IActionResult Signin()
    {
        return View(new SigninViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Signin(SigninViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result =
                await signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                var user = await userManager.FindByEmailAsync(model.Username);

                var userClaims = await userManager.GetClaimsAsync(user);

                if (await userManager.IsInRoleAsync(user, "Member")) return RedirectToAction("Member", "Home");
            }
            else
            {
                ModelState.AddModelError("Login", "Cannot login.");
            }
        }

        return View(model);
    }

    public Task<IActionResult> AccessDenied()
    {
        return Task.FromResult<IActionResult>(View());
    }

    public async Task<IActionResult> Signout()
    {
        // For custom sign out, use the code below
        // await signInManager.SignOutAsync();
        // return RedirectToAction("Signin");

        // or use the code below to let ASP.NET Core take care of sign-out
        return base.SignOut(new AuthenticationProperties
        {
            RedirectUri = urlHelper.ActionLink("Signin")
        });
    }
}