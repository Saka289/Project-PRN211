using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN211_Project.Models;
using System;
using System.Text.Json;

namespace PRN211_Project.Controllers
{
    public class ProductDetail : Controller
    {
        public IActionResult movieDetail(int movieId)
        {
            using (var context = new CenimaDBContext())
            {
                var movie = context.Movies.Include(m => m.Genre).Include(m => m.Rates).SingleOrDefault(m => m.MovieId == movieId);
                List<Rate> Rate = context.Rates.Include(m => m.Person).Where(m => m.MovieId == movieId).ToList();
                ViewBag.Movie = movie;
                if (movie.Rates.Count > 0)
                {
                    ViewBag.NumericRate = movie.Rates.Average(r => r.NumericRating);
                }
                else
                {
                    ViewBag.NumericRate = "Not yet rated";
                }

                ViewBag.Rate = Rate;
            }
            return View();

        }
        [HttpPost]
        public IActionResult comment(Rate rate)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("movieDetail", new { movieId = rate.MovieId });
            }
            using (var context = new CenimaDBContext())
            {
                if (HttpContext.Session.GetString("account") != null)
                {
                    var user = (Person)JsonSerializer.Deserialize<Person>(HttpContext.Session.GetString("account"));
                    var checkRate = context.Rates.FirstOrDefault(r => r.MovieId == rate.MovieId && r.PersonId == user.PersonId);
                    if (checkRate != null)
                    {
                        checkRate.MovieId = rate.MovieId;
                        checkRate.PersonId = user.PersonId;
                        checkRate.NumericRating = rate.NumericRating;
                        checkRate.Comment = rate.Comment;
                        checkRate.Time = DateTime.Now;
                        context.SaveChanges();
                    }
                    else
                    {
                        rate.Time = DateTime.Now;
                        rate.PersonId = user.PersonId;
                        context.Rates.Add(rate);
                        context.SaveChanges();
                    }
                }
                return RedirectToAction("movieDetail", new { movieId = rate.MovieId, rated = rate });
            }

        }

    }
}
