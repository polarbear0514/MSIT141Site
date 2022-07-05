﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSIT141Site.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MSIT141Site.Controllers
{
    public class ApiController : Controller
    {


        private readonly DemoContext _context;
        private readonly IWebHostEnvironment _host;
        public ApiController(DemoContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _host = hostEnvironment;
        }


        public IActionResult Index(User user)
        {

            //System.Threading.Thread.Sleep(5000); //停止5秒鐘
            //return Content("<h2>Hello Ajax!!你好</h2>","text/html",System.Text.Encoding.UTF8);
            if (string.IsNullOrEmpty(user.name))
            {
                user.name = "Ajax";
            }
            return Content($"<h2>{user.name}!!你好,你的年紀是{user.age},你的Email是{user.email}</h2>", "text/plain", System.Text.Encoding.UTF8);
        }

        public IActionResult Register(Member member,IFormFile file)
        {
            //檔案上傳要有實際路徑
            //C:\Users\Student\Documents\Ajax\MSIT141Site\wwwroot\uploads
            //string path = _host.ContentRootPath; //會取得專案資料夾的實際路徑
            string path = Path.Combine(_host.WebRootPath, "uploads", file.FileName); //會取得專案資料夾下wwwroot的實際路徑
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(fileStream); //儲存檔案到uploads資料夾中
            }
            //寫進資料庫
            byte[] imByte = null;
            using (var memoryStream = new MemoryStream())
            {
                file.CopyTo(memoryStream);
                imByte = memoryStream.ToArray();
            }
            member.FileName = file.FileName;
            member.FileData = imByte;

            _context.Members.Add(member);
            _context.SaveChanges();

            string info = $"{file.FileName} - {file.ContentType} - {file.Length}";
            return Content(info, "text/plain", System.Text.Encoding.UTF8);
        }
        public IActionResult GetImageBytes(int id=2) 
        {
            Member member = _context.Members.Find(id);
            byte[] img = member.FileData;
            return File(img, "image/jpeg");
        }

        public IActionResult CheckName(User user)
        {
            IEnumerable<Member> datas = null;
            if (user.name == null)
            {
                return Content("請輸入會員帳號");
            }
            else
            {
                datas = _context.Members.Where(t => t.Name.Equals(user.name)).ToList();
                if (datas.Count() == 0)
                {
                    return Content("無此會員帳號");
                }
                else
                {
                    return Content("會員帳號已存在");
                }
            }
        }

        //讀取所有城市的資料
        public IActionResult City()
        {
            var cities = _context.Addresses.Select(a => a.City).Distinct();
            return Json(cities);
        }

        //根據城市名稱讀出鄉鎮區的資料
        public IActionResult Districts(string city)
        {
            var districts = _context.Addresses.Where(a => a.City == city).Select(a => a.SiteId).Distinct();
            return Json(districts);
        }

        //根據鄉鎮區的資料讀出路名
        public IActionResult Roads(string district)
        {
            var roads = _context.Addresses.Where(a => a.SiteId == district).Select(a => a.Road);
            return Json(roads);
        }



    }
}
