using System.Linq;
using System.Threading.Tasks;
using IdentityNetCore.Models;
using IdentityNetCore.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityNetCore.Controllers;

public class IdentityController : Controller
{
    private readonly IEmailSender _emailSender;
    private readonly SignInManager<IdentityUser> _signInManager;

    private readonly UserManager<IdentityUser> _userManager;

    public IdentityController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailSender = emailSender;
    }

    public Task<IActionResult> Signup()
    {
        var model = new SignupViewModel();
        return Task.FromResult<IActionResult>(View(model));
    }

    [HttpPost]
    public async Task<IActionResult> Signup(SignupViewModel model)
    {
        if (ModelState.IsValid)
            if (await _userManager.FindByEmailAsync(model.Email) == null)
            {
                var user = new IdentityUser
                {
                    Email = model.Email,
                    UserName = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    if (result.Succeeded)
                    {
                        var confirmationLink =
                            Url.ActionLink("ConfirmEmail", "Identity", new { userId = user.Id, token });
                        await _emailSender.SendEmailAsync("info@mydomain.com", user.Email, "Confirm your email address",
                            confirmationLink);

                        return RedirectToAction("Signin");
                    }
                }

                ModelState.AddModelError("Signup", string.Join("", result.Errors.Select(x => x.Description)));
                return View(model);
            }

        return View(model);
    }


    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded) return RedirectToAction("Signin");
        }

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
                await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, false);
            if (result.Succeeded)
                return RedirectToAction("Signin");
            ModelState.AddModelError("Login", "Cannot login.");
        }

        return View(model);
    }

    public IActionResult AccessDenied()
    {
        return RedirectToAction("Signin");
    }
}