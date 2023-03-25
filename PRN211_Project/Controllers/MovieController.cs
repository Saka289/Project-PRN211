using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using PRN211_Project.Models;
using System.Collections.Generic;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using static System.Net.WebRequestMethods;
using System.Text.Json;

namespace PRN211_Project.Controllers
{
	public class MovieController : Controller
	{
		private readonly ILogger<HomeController> _logger;

		public IWebHostEnvironment HostEnvironment { get; }

		//private readonly CenimaDBContext _db;




		public MovieController(ILogger<HomeController> logger, IWebHostEnvironment hostEnvironment)
		{
			_logger = logger;
			HostEnvironment = hostEnvironment;



		}
		public IActionResult Index()
		{
			using (var context = new CenimaDBContext())
			{
				List<Movie> movies = context.Movies.ToList();
				Person user = new Person();
				if (HttpContext.Session.GetString("account") != null)
				{
					user = (Person)JsonSerializer.Deserialize<Person>(HttpContext.Session.GetString("account"));
					if (user.Type != 1)
					{
						return RedirectToAction("Signin", "Login");
					}
				}
				else
				{
					return RedirectToAction("Signin", "Login");

				}
				return View(movies);
			}

		}
		public IActionResult Create()
		{
			using (var context = new CenimaDBContext())
			{
				ViewData["Genre"] = new SelectList(context.Genres, "GenreId", "Desciption");
				List<Genre> g = context.Genres.ToList();
				ViewBag.Genres = g;
				return View();
			}

		}

		//public IActionResult Create()
		//{
		//   ViewData["CategoryId"] = new SelectList(_dbConText.Categories, "CategoryId", "CategoryName");
		//   return View();
		//}

		[HttpPost]
		[ValidateAntiForgeryToken]

		public async Task<IActionResult> Create(Movie product)
		{
			using (var context = new CenimaDBContext())
			{
				// Kiem tra du lieu tren form da hop le chua

				if (ModelState.IsValid)
				{


					var fileName = Path.GetFileName(product.imageFile.FileName);
					var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\image", fileName);
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await product.imageFile.CopyToAsync(fileStream);
					}


					//string wwwRootPath = HostEnvironment.WebRootPath;
					//string fileName = Path.GetFileNameWithoutExtension(product.imageFile.FileName);
					//string extension = Path.GetExtension(product.imageFile.FileName);

					//product.Image = fileName = fileName + DateTime.Now.ToString() + extension;
					//string path = Path.Combine(wwwRootPath + "/image/", fileName);
					//using (var fileStream = new FileStream(path, FileMode.Create))
					//{
					//   await product.imageFile.CopyToAsync(fileStream);
					//}
					product.Image = product.imageFile.FileName;
					context.Add(product);
					context.SaveChanges();
					return RedirectToAction("Index");
				}
				ViewData["GenreId"] = new SelectList(context.Genres, "GenreId", "Description");
				product.Image = Path.GetFileName(product.imageFile.FileName);
				List<Genre> g = context.Genres.ToList();

				ViewBag.Genres = g;

				return View(product);
			}
		}



		[HttpGet]
		public IActionResult Edit(int id)

		{
			using (var context = new CenimaDBContext())
			{
				List<Genre> list = context.Genres.ToList();
				var movie = context.Movies.Where(p => p.MovieId == id).SingleOrDefault();
				if (movie == null)
				{
					TempData["MessageUpdate"] = "phim khong ton tai";
					return RedirectToAction("Index");

				}
				else
				{
					ViewData["GenreId"] = new SelectList(list, "GenreId", "Description");
					ViewBag.image = "~/image/" + movie.Image;
					return View(movie);
				}
			}
		}
		[HttpPost]
		public async Task<IActionResult> Edit(Movie product)
		{


			using (var context = new CenimaDBContext())
			{
				Movie p = context.Movies.SingleOrDefault(p => p.MovieId == product.MovieId);
				if (p != null)
				{
					var fileName = Path.GetFileName(product.imageFile.FileName);
					var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\image", fileName);
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await product.imageFile.CopyToAsync(fileStream);
					}


					p.Image = product.imageFile.FileName;
					p.Title = product.Title;

					p.Description = product.Description;

					p.GenreId = product.GenreId;
					context.SaveChanges();
					TempData["MessageUpdate"] = "Update Successful";
					return RedirectToAction("Index");
				}

				ViewData["GenreId"] = new SelectList(context.Genres.ToList(), "GenreId", "Description");


				return View(p);
			}

			//public IActionResult Edit(int id)

			//{
			//   //using (var context = new CenimaDBContext())
			//   //{
			//      if (_db.Movies.SingleOrDefault(p => p.MovieId == id) == null)
			//      {
			//         ViewData["message"] = "this movie is not exist";
			//         return View("../error");

			//      }
			//      else
			//      {
			//         var p = _db.Movies.FirstOrDefault(p => p.MovieId == id);
			//         ViewData["GenreId"] = new SelectList(_db.Genres, "GenreId", "Description");
			//         //List<Genre> g = context.Genres.ToList();
			//         //ViewBag.Genres = g;
			//         return View(p);
			//      }


			//}
			//[HttpPost]
			//public IActionResult Edit(Movie product)
			//{


			//   //if (ModelState.IsValid)
			//   //{
			//   Movie p = _db.Movies.SingleOrDefault(p => p.MovieId == product.MovieId);
			//   if (p != null)
			//   {
			//      p.Title = product.Title;

			//      p.Description = product.Description;

			//      p.GenreId = product.GenreId;
			//      _db.SaveChanges();
			//      ViewData["GenreId"] = new SelectList(_db.Genres, "GenreId", "Desciption");
			//      return RedirectToAction("Index");
			//   }
			//   return View(p);





			//   //}
			//   //return View(product);
			//}

			//public IActionResult Edit(int id)

			//{
			//   //using (var context = new CenimaDBContext())
			//   //{
			//      if (_db.Movies.SingleOrDefault(p => p.MovieId == id) == null)
			//      {
			//         ViewData["message"] = "this movie is not exist";
			//         return View("../error");

			//      }
			//      else
			//      {
			//         var p = _db.Movies.FirstOrDefault(p => p.MovieId == id);
			//         ViewData["GenreId"] = new SelectList(_db.Genres, "GenreId", "Description");
			//         //List<Genre> g = context.Genres.ToList();
			//         //ViewBag.Genres = g;
			//         return View(p);
			//      }






			//}
			//return View(product);
		}
		public IActionResult Delete(int id)
		{
			using (var context = new CenimaDBContext())
			{

				var rate = context.Rates.FirstOrDefault(p => p.MovieId == id);
				if (rate == null)
				{
					var m = context.Movies.Find(id);
					if (m != null)
					{
						context.Movies.Remove(m);
						context.SaveChanges();
						TempData["MessageUpdate"] = "Delete Successful";
						//return RedirectToAction("index");
					}
				}
				else
				{
					TempData["Message"] = "phim co danh gia";

					//return RedirectToAction("index");


				}
				return RedirectToAction("index");



			}



		}


	}
}
