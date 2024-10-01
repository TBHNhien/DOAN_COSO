
using app.Areas.Admin.Models;
using app.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Model.Dao;
using Newtonsoft.Json;
using app.Dao;

namespace app.Controllers
{
    [Area("Admin")]
    public class LoginController : Controller
    {
        
        private readonly UserDao _userDao;

        public LoginController(UserDao userDao)
        {
            _userDao = userDao;
        }

        // GET: Admin/Login

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {

                //var dao = new UserDao();
                var result = _userDao.Login(model.UserName, Encryptor.MD5Hash(model.Password));
                if (result == 1)
                {
                    var user = _userDao.GetById(model.UserName);
                    var userSession = new UserLogin();
                    userSession.UserName = user.UserName;
                    userSession.UserID = user.Id;
                    //Session.Add(CommonConstants.USER_SESSION, userSession);

                    // Gán giá trị cho session
                    HttpContext.Session.SetString(CommonConstants.USER_SESSION, JsonConvert.SerializeObject(userSession));


                    //return RedirectToAction("Index", "HomeAdmin", new { Area = "Admin" });
                    return Redirect("~/admin/HomeAdmin");


                }
                else if (result == 0)
                {
                    ModelState.AddModelError("", "Tài khoản không tồn tại .");
                }
                else if (result == -1)
                {
                    ModelState.AddModelError("", "Tài khoản đang bị khoá .");
                }
                else if (result == -2)
                {
                    ModelState.AddModelError("", "Mật khẩu không đúng .");
                }
                else
                {
                    ModelState.AddModelError("", "đăng nhập không đúng .");
                }
            }
            return View("Index");
        }
    }
}