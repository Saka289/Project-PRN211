using Microsoft.AspNetCore.Mvc;

namespace PRN211_Project.Controllers
{
	public class MembersController : Controller
	{
		public IActionResult List()
		{
			List<Person> listP = new List<Person>();
			using (var context = new CenimaDBContext())
			{
				listP = context.Persons.ToList();
				ViewBag.listP = listP;
			}
			return View();
		}

		public IActionResult Status(int id, int pid)
		{
			Console.WriteLine(id.ToString());
			using (var context = new CenimaDBContext())
			{
				var person = context.Persons.Where(p => p.PersonId == pid).SingleOrDefault();
				if (person != null)
				{
					if (id == 1)
					{
						person.IsActive = true;
					}
					else
					{
						person.IsActive = false;
					}
					context.SaveChanges();
				}
			}
			return RedirectToAction("List");
		}

		[HttpPost]
		public IActionResult Create(Person person)
		{
			using (var context = new CenimaDBContext())
			{
				var check = context.Persons.Where(c => c.Email == person.Email).SingleOrDefault();
				if (check != null)
				{
					ViewBag.note = "Email Đã Tồn Tại !!!";
					return RedirectToAction("List");
				}
				else
				{
					var per = context.Persons.Add(new Person
					{
						Fullname = person.Fullname,
						Gender = person.Gender,
						Email = person.Email,
						Password = person.Password,
						Type = person.Type,
						IsActive = person.IsActive,
					});
					context.SaveChanges();
				}
			}
			return RedirectToAction("List");
		}
	}
}
