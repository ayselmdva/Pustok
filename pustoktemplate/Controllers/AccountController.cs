﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using pustoktemplate.Models;
using pustoktemplate.ViewModels.Account;

namespace pustoktemplate.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _usermanager;
        private readonly SignInManager<AppUser> _signinmanager;
        private readonly RoleManager<IdentityRole> _rolemanager;

        public AccountController(SignInManager<AppUser> signinmanager, UserManager<AppUser> usermanager, RoleManager<IdentityRole> rolemanager)
        {
            _signinmanager = signinmanager;
            _usermanager = usermanager;
            _rolemanager = rolemanager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm registerVm)
        {
            if (!ModelState.IsValid) { return View(); }
            AppUser newUser = new AppUser()
            {
                UserName = registerVm.UserName,
                Email = registerVm.Email,
                FullName = registerVm.FullName,
            };
            IdentityResult result = await _usermanager.CreateAsync(newUser, registerVm.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
                return View();
            }
            await _usermanager.AddToRoleAsync(newUser, "member");
            await _signinmanager.SignInAsync(newUser, false);
            return RedirectToAction("Index", "Home");
        }

         public async Task<IActionResult> Login()
        {
        	return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm loginVm)
        {
        	if (!ModelState.IsValid) { return View(); }
            if (loginVm.UserNameOrEmail.Contains("@"))
            {
                AppUser user = await _usermanager.FindByEmailAsync(loginVm.UserNameOrEmail);
                if (user != null)
                {
                    var result = await _signinmanager.PasswordSignInAsync(user, loginVm.Password, true, true);
                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(" ", "Please wait");
                    }
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError(" ", "Invalid credential");
                    }
                    await _usermanager.AddToRoleAsync(user, "Admin");
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {

            }
            return View();
        }
    }
}