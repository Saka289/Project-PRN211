using GoogleAuthentication.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PRN211_Project.Models;
using System;
using System.Security.Principal;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace PRN211_Project.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Signin()
        {
            var ClientID = "625640582952-khuslr7j429aa0kb13eg93rql0nj95vc.apps.googleusercontent.com";
            var url = "http://localhost:5228/Login/LoginGoogle";
            var response = GoogleAuth.GetAuthUrl(ClientID, url);
            ViewBag.response = response;
            return View();
        }

        [HttpPost]
        public IActionResult Signin(Person person)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("account")))
            {
                using (var context = new CenimaDBContext())
                {
                    var account = context.Persons.Where(a => a.Email == person.Email && a.Password == person.Password).SingleOrDefault();
                    if (account != null)
                    {
                        HttpContext.Session.SetString("account", System.Text.Json.JsonSerializer.Serialize(account));
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Message"] = "Đăng Nhập Không Thành Công";
                        return View();
                    }
                }
            }
            else
            {
                TempData["Message"] = "Đã Đăng Nhập Tài Khoản Rồi !!!";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> LoginGoogle(string code)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("account")))
            {
                var ClientID = "625640582952-khuslr7j429aa0kb13eg93rql0nj95vc.apps.googleusercontent.com";
                var ClientSecret = "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";
                var url = "http://localhost:5228/Login/LoginGoogle";
                var token = await GoogleAuth.GetAuthAccessToken(code, ClientID, ClientSecret, url);
                var userProfile = await GoogleAuth.GetProfileResponseAsync(token.AccessToken.ToString());
                var googleUser = JsonConvert.DeserializeObject<userGoogle>(userProfile);
                if (googleUser.Verified_Email == true)
                {
                    using (var context = new CenimaDBContext())
                    {
                        var checkEmail = context.Persons.Where(c => c.Email == googleUser.Email).SingleOrDefault();
                        Person person = new Person();
                        if (checkEmail == null)
                        {
                            person.Fullname = googleUser.Name;
                            person.Email = googleUser.Email;
                            person.Gender = "";
                            person.Password = "";
                            person.Type = 2;
                            person.IsActive = true;
                            context.Add(person);
                            context.SaveChanges();
                            var account = context.Persons.Where(c => c.Email == googleUser.Email).SingleOrDefault();
                            HttpContext.Session.SetString("account", System.Text.Json.JsonSerializer.Serialize(account));
                            return RedirectToAction("Index", "Home");
                        }
                        else if (string.IsNullOrEmpty(checkEmail.Password))
                        {
                            HttpContext.Session.SetString("account", System.Text.Json.JsonSerializer.Serialize(checkEmail));
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            TempData["Message"] = "Email Đã Tồn Tại !!!";
                            return RedirectToAction("Signin");
                        }
                    }
                }
                else
                {
                    TempData["Message"] = "Đăng Nhập Không Thành Công";
                    return RedirectToAction("Signin");
                }
            }
            else
            {
                TempData["Message"] = "Đã Đăng Nhập Tài Khoản Rồi !!!";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Signout()
        {
            if ((HttpContext.Session.GetString("account") != null) || string.IsNullOrEmpty(HttpContext.Session.GetString("account")))
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
                        Type = 2,
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
