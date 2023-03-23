using Microsoft.AspNetCore.Mvc;
using PRN211_Project.Models;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace PRN211_Project.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Signin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Signin(Person person)
        {
            using (var context = new CenimaDBContext())
            {
                var account = context.Persons.Where(a => a.Email == person.Email && a.Password == person.Password).SingleOrDefault();
                if (account != null)
                {
                    HttpContext.Session.SetString("account", JsonSerializer.Serialize(account));
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Message"] = "Đăng Nhập Không Thành Công";
                    return View();
                }
            }
        }

        public IActionResult Signout()
        {
            if (HttpContext.Session.GetString("account") != null)
            {
                HttpContext.Session.Remove("account");
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(Person person)
        {
            using (var context = new CenimaDBContext())
            {
                if (context.Persons.Where(p => p.Email == person.Email).SingleOrDefault() != null)
                {
                    ViewBag.exist = "Email Đã Tồn Tại !!!";
                    return View();
                }
                else
                {
                    var account = context.Persons.Add(new Person
                    {
                        Fullname = person.Fullname,
                        Gender = person.Gender,
                        Email = person.Email,
                        Password = person.Password,
                        Type = 1,
                        IsActive = true,
                    });
                    if (account != null)
                    {
                        TempData["Message"] = "Đăng Ký Thành Công";
                        context.SaveChanges();
                        return RedirectToAction("Signin");
                    }
                }
            }
            return View();
        }

        public IActionResult Reset()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Reset(Person person)
        {
            Random rd = new Random();
            using (var context = new CenimaDBContext())
            {
                var account = context.Persons.Where(a => a.Email == person.Email).SingleOrDefault();
                if (account != null)
                {
                    IEmailService emailService = new EmailService();
                    string otp = rd.Next(0, 1000000).ToString("D6");
                    HttpContext.Session.SetString("otp", otp);
                    emailService.SendEmail(person.Email, "OTP Change Password", otp);
                    HttpContext.Session.SetString("pid", account.PersonId.ToString());
                    return RedirectToAction("Otp");
                }
                else
                {
                    ViewBag.text = "Email Không Tồn Tại!!!";
                    return View();
                }
            }
        }

        public IActionResult Otp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Otp(Person person)
        {
            string otpValue = HttpContext.Request.Form["otpValue"].ToString().Trim();
            var otpSession = HttpContext.Session.GetString("otp");
            if (otpSession != null)
            {
                if (otpSession.Trim().Equals(otpValue))
                {
                    return RedirectToAction("Repassword");
                }
                else
                {
                    ViewBag.error = "OTP Không Chính Xác !!!";
                    return View();
                }
            }
            return View();
        }

        public IActionResult Repassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Repassword(Person person)
        {
            var id = HttpContext.Session.GetString("pid");
            string pass = HttpContext.Request.Form["pass"];
            string repass = HttpContext.Request.Form["repass"];
            if (pass.Equals(repass))
            {
                using (var context = new CenimaDBContext())
                {
                    var account = context.Persons.Where(a => a.PersonId == Convert.ToInt32(id)).SingleOrDefault();
                    if (account != null)
                    {
                        account.Password = pass;
                        TempData["Message"] = "Đổi Mật Khẩu Thành Công";
                        context.SaveChanges();
                        HttpContext.Session.Remove("pid");
                        return RedirectToAction("Signin");
                    }
                }
            }
            else
            {
                ViewBag.error = "Mật Khẩu Mới Không Khớp !!!";
                return View();
            }
            return View();
        }
    }
}
