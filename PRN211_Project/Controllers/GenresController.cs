﻿using Microsoft.AspNetCore.Mvc;
using PRN211_Project.Models;
using System.Collections.Specialized;
using System.Text.Json;

namespace PRN211_Project.Controllers
{
    public class GenresController : Controller
    {
        public IActionResult GenreManagement()
        {
            Person user = new Person();
            if (HttpContext.Session.GetString("account") != null)
            {
                user = (Person)JsonSerializer.Deserialize<Person>(HttpContext.Session.GetString("account"));
            }

            if (user.Type == 1)
            {
                using (var context = new CenimaDBContext())
                {
                    List<Genre> listG = context.Genres.ToList();
                    ViewBag.listG = listG;
                    if (TempData["shortMessage"] != null)
                    {
                        ViewBag.Message = TempData["shortMessage"].ToString();
                    }

                    return View(listG);
                }
            }
            ViewData["message"] = "Error 404 Not found!";
            return View("../Shared/Error");

        }

        public IActionResult Edit(int id)
        {
            using (var context = new CenimaDBContext())
            {
                Genre genre = context.Genres.SingleOrDefault(c => c.GenreId == id);
                return PartialView("editGenre", genre);
            }
        }

        [HttpPost]
        public IActionResult Edit(Genre genre)
        {
            using (var context = new CenimaDBContext())
            {
                context.Update(genre);
                context.SaveChanges();
                return RedirectToAction("GenreManagement");
            }
        }

        public IActionResult Delete(int id)
        {
            using (var context = new CenimaDBContext())
            {
                Genre genre = context.Genres.SingleOrDefault(c => c.GenreId == id);
                return PartialView("deleteGenre", genre);
            }
        }

        [HttpPost]
        public IActionResult Delete(Genre genre)
        {
            using (var context = new CenimaDBContext())
            {
                Movie movie = context.Movies.FirstOrDefault(c => c.GenreId == genre.GenreId);
                if (movie == null)
                {
                    context.Remove(genre);
                    context.SaveChanges();
                    return RedirectToAction("GenreManagement");
                }
                else
                {
                    TempData["shortMessage"] = "This Genre is currently in use";
                    return RedirectToAction("GenreManagement");
                }

            }
        }

        [HttpPost]
        public IActionResult Add()
        {
            using (var context = new CenimaDBContext())
            {
                string desc = this.Request.Form["des"];
                Genre genre = new Genre()
                {
                    Description = desc
                };
                context.Genres.Add(genre);
                context.SaveChanges();
                return RedirectToAction("GenreManagement");
            }
        }
    }
}
