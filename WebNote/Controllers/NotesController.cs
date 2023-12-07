using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using WebNote.Entity;
using WebNote.Models;

namespace WebNote.Controllers
{

    public class NotesController : Controller
    {
        private readonly AppDbContext _dbContext;

        public NotesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            // Перевірити, чи користувач авторизований
            if (!IsUserAuthenticated())
            {
                // Якщо користувач не авторизований, переадресувати на сторінку авторизації
                return RedirectToAction("Login", "Account");
            }

            // Отримати всі нотатки для поточного користувача
            var userId = GetCurrentUserId();
            var notes = _dbContext.Notes.Where(n => n.UserId == userId).ToList();
            ViewBag.UserId = userId;
            return View(notes);
        }

        // GET: /Notes/Create
        public IActionResult Create()
        {
            // Перевірити, чи користувач авторизований
            if (!IsUserAuthenticated())
            {
                // Якщо користувач не авторизований, переадресувати на сторінку авторизації
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: /Notes/Create
        [HttpPost]
        public IActionResult Create(NoteViewModel model)
        {
            // Перевірити, чи користувач авторизований
            if (!IsUserAuthenticated())
            {
                // Якщо користувач не авторизований, переадресувати на сторінку авторизації
                return RedirectToAction("Login", "Account");
            }

            var userId = GetCurrentUserId();

            var note = new Note
            {
                Title = model.Title,
                Content = model.Content,
                CreatedAt = DateTime.Now,
                UserId = userId
            };

            _dbContext.Notes.Add(note);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: /Notes/Edit/{id}
        public IActionResult Edit(int id)
        {
            // Перевірити, чи користувач авторизований
            if (!IsUserAuthenticated())
            {
                // Якщо користувач не авторизований, переадресувати на сторінку авторизації
                return RedirectToAction("Login", "Account");
            }

            var note = _dbContext.Notes.Find(id);

            if (note == null)
            {
                return NotFound();
            }

            var model = new Note
            {
                Title = note.Title,
                Content = note.Content
            };

            return View(model);
        }

        // POST: /Notes/Edit/{id}
        [HttpPost]
        public IActionResult Edit(int id, Note model)
        {
            // Перевірити, чи користувач авторизований
            if (!IsUserAuthenticated())
            {
                // Якщо користувач не авторизований, переадресувати на сторінку авторизації
                return RedirectToAction("Login", "Account");
            }

            var note = _dbContext.Notes.Find(id);

            if (note == null)
            {
                return NotFound();
            }

            note.Title = model.Title;
            note.Content = model.Content;

            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }


        // GET: /Notes/Delete/{id}
        public IActionResult Delete(int id)
        {
            // Перевірити, чи користувач авторизований
            if (!IsUserAuthenticated())
            {
                // Якщо користувач не авторизований, переадресувати на сторінку авторизації
                return RedirectToAction("Login", "Account");
            }

            var note = _dbContext.Notes.Find(id);

            if (note == null)
            {
                return NotFound();
            }

            return View(note);
        }

        // POST: /Notes/Delete/{id}
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            // Перевірити, чи користувач авторизований
            if (!IsUserAuthenticated())
            {
                // Якщо користувач не авторизований, переадресувати на сторінку авторизації
                return RedirectToAction("Login", "Account");
            }

            var note = _dbContext.Notes.Find(id);
           
            if (note == null)
            {
                return NotFound();
            }

            _dbContext.Notes.Remove(note);
            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        // Перевірка, чи користувач авторизований
        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetInt32("UserId") != null;
        }

        // Отримання ідентифікатора поточного користувача
        private int GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }

        public IActionResult ViewNotes(int id)
        {
            // Отримати дані користувача за його кодом
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            // Передати код користувача в перегляд через ViewBag
            ViewBag.Name = user.FirstName+" "+user.LastName;
            
            // Отримати всі нотатки для поточного користувача
            var notes = _dbContext.Notes.Where(n => n.UserId == user.Id).ToList();

            return View(notes);
        }


    }

}
