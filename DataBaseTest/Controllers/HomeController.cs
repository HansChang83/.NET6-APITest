using DataBaseTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.EntityFrameworkCore;   // Async「非同步」會用到的命名空間
using Microsoft.Data.SqlClient;

namespace DataBaseTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly TestDataContext _db = new TestDataContext();

      
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger, TestDataContext context)
        {  //                                                                                          ****************************（自己動手加上）
            _logger = logger;
            _db = context;    //*****************************（自己動手加上）
                              // https://blog.givemin5.com/asp-net-mvc-core-tao-bi-hen-jiu-di-yi-lai-zhu-ru-dependency-injection/
        }
        public IActionResult Index()
        {
            return View();
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







        public IActionResult Create()
        {
            return View();
        }

        [HttpPost, ActionName("Create")]   // 把下面的動作名稱，改成 CreateConfirm 試試看？
        [ValidateAntiForgeryToken]   // 避免CSRF攻擊。在FormTagHelper - 如果有寫 action屬性、或是Method = Get就會啟動此功能。設定為true。
                                     // https://docs.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.mvc.taghelpers.formtaghelper
        public IActionResult Create(User _userTable)
        {
            if ((_userTable != null) && (ModelState.IsValid))   // ModelState.IsValid，通過表單驗證（Server-side validation）需搭配 Model底下類別檔的 [驗證]
            {  
                _db.Users.Add(_userTable);
                _db.SaveChanges();


                return Content(" 新增一筆記錄，成功！");    // 新增成功後，出現訊息（字串）。
                return RedirectToAction("List");
            }
            else
            {   // 搭配 ModelState.IsValid，如果驗證沒過，就出現錯誤訊息。
                ModelState.AddModelError("Value1", " 自訂錯誤訊息(1) ");   // 第一個輸入值是 key，第二個是錯誤訊息（字串）
                ModelState.AddModelError("Value2", " 自訂錯誤訊息(2) ");
                return View();   // 將錯誤訊息，返回並呈現在「新增」的檢視畫面上
            }
        }



        public async Task<IActionResult> List()
        {
            //第一種寫法：  //*** 查詢結果是一個 IQueryable **************************
            IQueryable<User> ListAll = from _userTable in _db.Users
                                            select _userTable;
               
               

            if (ListAll == null)
            {   // 找不到這一筆記錄
                //return HttpNotFound();   // .NET Core無此寫法。
                return NotFound();
                //return Content(" ** Sorry! 找不到任一筆記錄 ** ");
            }
            else
            {
                return View(await ListAll.ToListAsync());  // 執行 .ToList()方法後才真正運作，產生查詢的「結果(result)」。

              
            }

           
        }
        //===================================
        //== 刪除 ==  （不搭配路由 (Route) 就無法運作。必須自己手動加入 HTML Hidden欄位）
        //===================================

        //== 刪除前的 Double-Check，先讓您確認這筆記錄的內容？
        //[HttpGet]  加上這一句也無法解決「找不到 _ID」的問題
        public IActionResult Delete(int? _ID)    // 網址 http://xxxxxx/UserDB/Delete?_ID=1 
        {
            if (_ID == null)
            {   // 沒有輸入文章編號（_ID），就會報錯 - Bad Request
                //return new StatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);
                return Content(" 沒有輸入文章編號（_ID）");
            }

            // 使用上方 Details動作的程式，先列出這一筆的內容，給使用者確認
            User ut = _db.Users.Find(_ID);

            if (ut == null)
            {   // 找不到這一筆記錄
                // return NotFound(); 
                return Content(" 找不到這一筆記錄！");
            }
            else
            {
                return View(ut);
            }
        }

        //== 真正刪除這一筆，並回寫資料庫 ===============
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]   // 避免CSRF攻擊  https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/implementing-basic-crud-functionality-with-the-entity-framework-in-asp-net-mvc-application#overpost
                                     // 避免CSRF攻擊。在FormTagHelper - 如果有寫 action屬性、或是Method = Get就會啟動此功能。設定為true。
                                     // https://docs.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.mvc.taghelpers.formtaghelper  
                                     // 避免 "刪除" 一筆記錄的安全漏洞 http://stephenwalther.com/archive/2009/01/21/asp-net-mvc-tip-46-ndash-donrsquot-use-delete-links-because
                                     //           如果您希望將刪除的動作，合併在一起，一次解決，請看下面文章的最後一個範例（P.S. 可能有安全漏洞）。 
                                     //           https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/examining-the-details-and-delete-methods
        public IActionResult DeleteConfirm(int _ID)
        {
            //return Content(_ID.ToString());  //為什麼無法刪除？找不到這一筆記錄？是不是沒有傳遞_ID過來這裡呢？

            if (ModelState.IsValid)   // ModelState.IsValid，通過表單驗證（Server-side validation）需搭配 Model底下類別檔的 [驗證]
            {
                //// 第二種方法（作法類似後續的 Edit動作）

                //User ut = _db.Users.Find(_ID);
                //if (ut == null)
                //{   // 找不到這一筆記錄
                //    return Content(" 刪除時，找不到這一筆記錄！");
                //}
                //else
                //{
                //    _db.Entry(ut).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;  // 確認刪除一筆（狀態：Deleteed）
                //    _db.SaveChanges();
                //    //**** 刪除以後，必須執行 .SaveChanges()方法，才能真正去DB刪除這一筆記錄 ****
                //}


                // 第三種方法。必須先找到這一筆記錄。找得到，才能刪除！
                User ut = _db.Users.Find(_ID);   // https://www.youtube.com/watch?v=cINtgwbG8zo
                if (ut == null)
                {   // 找不到這一筆記錄
                    return Content(" 刪除時，找不到這一筆記錄！");
                }
                else
                {
                    _db.Users.Remove(ut);
                    _db.SaveChanges();  //**** 刪除以後，必須執行 .SaveChanges()方法，才能真正去DB刪除這一筆記錄 ****
                }

                return Content(" 刪除一筆記錄，成功！");    // 刪除成功後，出現訊息（字串）。
                //return RedirectToAction("List");
                return RedirectToAction(nameof(List));  // 提升程式的維護性，常用在"字串"上。

            }
            else
            {   // 搭配 ModelState.IsValid，如果驗證沒過，就出現錯誤訊息。
                ModelState.AddModelError("Value1", " 自訂錯誤訊息(1) ");
                ModelState.AddModelError("Value2", " 自訂錯誤訊息(2) ");
                return View();   // 將錯誤訊息，返回並呈現在「刪除」的檢視畫面上
            }
        }





        [HttpGet]
        public ActionResult Edit(int? _ID = 1)
        {
            if (_ID == null)
            {   // 沒有輸入文章編號（_ID），就會報錯 - Bad Request
                //return new StatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);
                return Content("請輸入 _ID 編號");
            }

            // 使用上方 Details動作的程式，先列出這一筆的內容，給使用者確認
            User ut = _db.Users.Find(_ID);

            if (ut == null)
            {   // 找不到這一筆記錄
                // return NotFound();
                return Content("找不到任何記錄");
            }
            else
            {
                return View(ut);
            }
        }

        //== 修改（更新），回寫資料庫 #1 ============ 注意！這裡的輸入值是一個 UserTable
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]   // 避免CSRF攻擊  https://docs.microsoft.com/en-us/aspnet/mvc/overview/getting-started/getting-started-with-ef-using-mvc/implementing-basic-crud-functionality-with-the-entity-framework-in-asp-net-mvc-application#overpost

        // [Bind(...)] 也可以寫在 Model的類別檔裡面，就不用重複地寫在新增、刪除、修改每個動作之中。
        // 可以避免 overposting attacks （過多發佈）攻擊  http://www.cnblogs.com/Erik_Xu/p/5497501.html
        // 參考資料 http://blog.kkbruce.net/2011/10/aspnet-mvc-model-binding6.html
        public ActionResult EditConfirm([Bind("UserId, UserName, UserPassWord, UserEmail")] User _user)
        {
            // https://docs.microsoft.com/zh-tw/aspnet/core/data/ef-mvc/crud  關於大量指派（overposting / 過多發佈）的安全性注意事項
            if (_user == null)
            {   // 沒有輸入內容，就會報錯 - Bad Request
                return new StatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)   // ModelState.IsValid，通過表單驗證（Server-side validation）需搭配 Model底下類別檔的 [驗證]
            {   
                //// 第一種寫法： *** 注意！.NET Core的 命名空間 改變了 ***
                _db.Entry(_user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;  // 確認被修改（狀態：Modified）
                _db.SaveChanges();

               
                return RedirectToAction(nameof(List));  // 提升程式的維護性，常用在"字串"上。
            }
            else
            {
                //return View(_userTable);  // 若沒有修改成功，則列出原本畫面
                return Content(" *** 更新失敗！！*** ");
            }
        }
    }










    
}

 
    
