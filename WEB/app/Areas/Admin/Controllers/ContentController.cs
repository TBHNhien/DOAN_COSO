﻿using Model.Dao;
//using Model.EF;
using app.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using app.Dao;

namespace app.Areas.Admin.Controllers
{
    public class ContentController : BaseController
    {

        private readonly ContentDao _contentDao;
        private readonly CategoryDao _categoryDao;

        public ContentController(ContentDao contentDao, CategoryDao categoryDao)
        {
            _contentDao = contentDao;
            _categoryDao = categoryDao;
        }
        // GET: Admin/Content
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Area("Admin")]
        public IActionResult Create()
        {
            SetViewBag();
            var model = new Content(); // Thay `YourModel` bằng tên class model của bạn
            return View(model);
        }

        [HttpGet]
        [Area("Admin")]
        public IActionResult Edit(long id)
        {

            var content = _contentDao.GetByID(id);
            SetViewBag(content.CategoryId);
            return View();
        }

        [HttpPost]
        [Area("Admin")]
        public IActionResult Edit(Content model)
        {
            if (ModelState.IsValid)
            {

            }
            SetViewBag(model.CategoryId);
            return View();
        }


        [HttpPost]
        [Area("Admin")]
        //[ValidateInput(false)]
        public IActionResult Create(Content model)
        {
            if (ModelState.IsValid)
            {

            }
            SetViewBag();
            return View();
        }

        public void SetViewBag(long? selectedId = null)
        
        {

            ViewBag.CategoryId = new SelectList(_categoryDao.ListAll(), "Id", "Name", selectedId);
        }
    }
}