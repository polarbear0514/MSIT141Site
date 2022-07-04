using Microsoft.AspNetCore.Mvc;
using MSIT141Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSIT141Site.Controllers
{
    public class ApiController : Controller
    {

        
        private readonly DemoContext _context;
        public ApiController(DemoContext context)
        {
            _context = context;
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
                if (datas.Count()==0)
                {
                    return Content("無此會員帳號");
                }
                else
                {
                    return Content("會員帳號已存在");
                }
            }
        }
    }
}
