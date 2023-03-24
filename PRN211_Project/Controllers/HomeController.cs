using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN211_Project.Models;
using System.Diagnostics;


namespace PRN211_Project.Controllers
{
   public class HomeController : Controller
   {

      private readonly ILogger<HomeController> _logger;


      public HomeController(ILogger<HomeController> logger)
      {
         _logger = logger;
      }

      public IActionResult Index(int id)
      {
         using (var context = new CenimaDBContext())
         {
            List<Genre> genres = context.Genres.ToList();
            List<Movie> movies = new List<Movie>();
            if (id == 0)
            {
               movies = context.Movies.Include(c => c.Genre).Include(c => c.Rates).ToList();
            }
            else
            {
               movies = context.Movies.Include(c => c.Genre).Include(c => c.Rates).Where(c => c.GenreId == id).ToList();


            }


            

            ViewBag.genres = genres;
            ViewBag.movies = movies;

            return View();
         }
      }
      public IActionResult Search(string search){
         using (var context = new CenimaDBContext())
         {
            List<Genre> genres = context.Genres.ToList();
            List<Movie> movies = new List<Movie>();
            movies = context.Movies.Include(c => c.Genre).Include(c => c.Rates).Where(c=>c.Title.Contains(search)).ToList();


            




            ViewBag.genres = genres;
            ViewBag.movies = movies;

            return View("Index");
         }
      }
      

      public IActionResult Privacy()
      {
         return View();
      }

      [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
      public IActionResult Error()
      {
         return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
      }
   }
}