using Microsoft.AspNetCore.Mvc;
using System;
using WebNote.Entity;
using WebNote.Models;

namespace WebNote.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _dbContext;

        public AccountController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Зберегти інформацію про авторизованого користувача у сесії
                HttpContext.Session.SetInt32("UserId", user.Id);

                return RedirectToAction("Index", "Notes");
            }

            ModelState.AddModelError("", "Невірний пароль, або логін");
            return View();
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            // Перевірити, чи вже існує користувач з таким іменем користувача
            if (_dbContext.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("", "Такий логін вже використовується!");
                return View();
            }

            // Перевірка на заповнення полів
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName))
            {
                ModelState.AddModelError("", "Заповніть всі поля!");
                return View();
            }

            // Створити нового користувача
            var user = new User
            {
                Username = model.Username,
                Password = model.Password,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            // Зберегти інформацію про авторизованого користувача у сесії після реєстрації
            HttpContext.Session.SetInt32("UserId", user.Id);

            return RedirectToAction("Index", "Notes");
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            // Очистити інформацію про авторизованого користувача з сесії
            HttpContext.Session.Remove("UserId");

            return RedirectToAction("Login");
        }

    }

}