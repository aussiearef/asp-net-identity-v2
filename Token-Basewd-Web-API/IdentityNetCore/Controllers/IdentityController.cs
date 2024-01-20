using System.Linq;
using System.Threading.Tasks;
using IdentityNetCore.Models;
using IdentityNetCore.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityNetCore.Controllers;

public class IdentityController(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    IEmailSender emailSender)
    : Controller
{
    public Task<IActionResult> Signup()
    {
        var model = new SignupViewModel();
        return Task.FromResult<IActionResult>(View(model));
    }

    [HttpPost]
    public async Task<IActionResult> Signup(SignupViewModel model)
    {
        if (ModelState.IsValid)
            if (await userManager.FindByEmailAsync(model.Email) != null)
            {
                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await userManager.CreateAsync(user, model.Password);
                user = await userManager.FindByEmailAsync(model.Email);

                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                if (result.Succeeded)
                {
                    var confirmationLink = Url.ActionLink("ConfirmEmail", "Identity", new { userId = user.Id, token });
                    await emailSender.SendEmailAsync("info@mydomain.com", user.Email, "Confirm your email address",
                        confirmationLink);

                    return RedirectToAction("Signin");
                }

                ModelState.AddModelError("Signup", string.Join("", result.Errors.Select(x => x.Description)));
                return View(model);
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
                return RedirectToAction("Signin");
            ModelState.AddModelError("Login", "Cannot login.");
        }

        return View(model);
    }

    public Task<IActionResult> AccessDenied()
    {
        return Task.FromResult<IActionResult>(RedirectToAction("Signin"));
    }
}