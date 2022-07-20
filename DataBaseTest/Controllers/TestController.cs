using DataBaseTest.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient.Memcached;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections;

namespace DataBaseTest.Controllers
{
    public class TestController : Controller
    {
        // GET: TestController
        /// <summary>
        /// 用於顯示使用者資料
        /// </summary>
        /// <param name="_ID"></param>
        /// <returns></ returns>
        public async Task<IActionResult> Index(int? UserId)
        {

            using HttpClient client = new HttpClient();

            //BaseAddress = new Uri("http://localhost:5142/api/Database/")

            User? user = new User();

            if (UserId == null)
            {

                user = await client.GetFromJsonAsync<User>("http://localhost:5142/api/Database/1,1,1");


                //user.UserId = 0;
                //user.UserName = "查無資訊";
                //user.UserPassWord = "查無資訊";
                //user.UserEmail = "查無資訊";
            }
            else
            {
                user = await client.GetFromJsonAsync<User>(UserId.ToString() + ",1,1");
            }
            return View(user);

        }
        /// <summary>
        /// 搜尋使用者資料
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Index")]
        public async Task<IActionResult> Detail(int? UserId, string? UserName, string? UserEmail)
        {

            using HttpClient client = new()
            {
                BaseAddress = new Uri("http://localhost:5142/api/Database/")
            };
            try
            {

                User? user = new User();
                if (UserId == null)
                {
                    UserId = 0;
                }
                else
                {
                     user.UserId= (int)UserId;
                }


                if (UserName == null)
                    user.UserName = "null";
                else
                {
                    user.UserName = UserName;
                }
                if (UserEmail == null)
                {
                    user.UserEmail = "null";
                }

                else
                {
                    user.UserEmail = UserEmail;
                }
                user = await client.GetFromJsonAsync<User>((user.UserId.ToString() + "," + user.UserName + "," + user.UserEmail));

                return View(user);
            }
            catch (Exception ex)
            {

                return Content(ex.ToString());
            }

        }
        /// <summary>
        /// 開啟編輯頁面
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();

        }
        /// <summary>
        /// 接收資料後put至api
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Create")]   // 把下面的動作名稱，改成 CreateConfirm 試試看？
        [ValidateAntiForgeryToken]   // 避免CSRF攻擊。在FormTagHelper - 如果有寫 action屬性、或是Method = Get就會啟動此功能。設定為true。
                                     // https://docs.microsoft.com/zh-tw/dotnet/api/microsoft.aspnetcore.mvc.taghelpers.formtaghelper
        public async Task<IActionResult> CreateConfirm(User user)
        {
            using HttpClient client = new()
            {
                BaseAddress = new Uri("http://localhost:5142/api/Database/")
            };

            HttpResponseMessage response = await client.PostAsJsonAsync("", user);
            return View(user);
        }



        /// <summary>
        /// 刪除確認頁面
        /// </summary>
        /// <param name="_ID"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? _ID)    // 網址 http://xxxxxx/UserDB/Delete?_ID=1 
        {
            using HttpClient client = new()
            {
                BaseAddress = new Uri("http://localhost:5142/api/Database/")
            };
            if (_ID == null)
            {   // 沒有輸入文章編號（_ID），就會報錯 - Bad Request
                //return new StatusCodeResult((int)System.Net.HttpStatusCode.BadRequest);
                return Content(" 沒有輸入文章編號（_ID）");
            }

            // 使用上方 Details動作的程式，先列出這一筆的內容，給使用者確認
            User? user = await client.GetFromJsonAsync<User>(_ID.ToString()+",1,1");

            if (user == null)
            {   // 找不到這一筆記錄
                // return NotFound(); 
                return Content(" 找不到這一筆記錄！");
            }
            else
            {
                return View(user);
            }
        }
        /// <summary>
        /// 透過api做刪除
        /// </summary>
        /// <param name="_ID"></param>
        /// <returns></returns>
        //== 真正刪除這一筆，並回寫資料庫 ===============
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int _ID)
        {

            if (ModelState.IsValid)   // ModelState.IsValid，通過表單驗證（Server-side validation）需搭配 Model底下類別檔的 [驗證]
            {


                using HttpClient client = new()
                {
                    BaseAddress = new Uri("http://localhost:5142/api/Database/")
                };

                User? user = await client.GetFromJsonAsync<User>(_ID.ToString()+",1,1");

                if (user == null)
                {   // 找不到這一筆記錄
                    return Content(" 刪除時，找不到這一筆記錄！");
                }

                else
                {
                    HttpResponseMessage response = await client.DeleteAsync($"{_ID}");
                    return Content(" 刪除一筆記錄，成功！");
                }   // 刪除成功後，出現訊息（字串）。


            }
            else
            {   // 搭配 ModelState.IsValid，如果驗證沒過，就出現錯誤訊息。
                ModelState.AddModelError("Value1", " 自訂錯誤訊息(1) ");
                ModelState.AddModelError("Value2", " 自訂錯誤訊息(2) ");
                return View();   // 將錯誤訊息，返回並呈現在「刪除」的檢視畫面上
            }
        }

        /// <summary>
        /// 編輯輸入頁面
        /// </summary>
        /// <param name="_ID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int? _ID = 1)
        {
            using HttpClient client = new()
            {
                BaseAddress = new Uri("http://localhost:5142/api/Database/")
            };
            if (_ID == null)
            {
                return Content("請輸入 _ID 編號");
            }


            User? user = await client.GetFromJsonAsync<User>(_ID.ToString()+",1,1");
            if (user == null)
            {
                return Content("找不到任何記錄");
            }
            else
            {
                return View(user);
            }
        }

        /// <summary>
        /// 透過api編輯資料
        /// </summary>
        /// <param name="_user"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Edit")]


        public async Task<IActionResult> EditConfirm([Bind("UserId, UserName, UserPassWord, UserEmail")] User _user)
        {
            using HttpClient client = new()
            {
                BaseAddress = new Uri("http://localhost:5142/api/Database/")
            };






            if (ModelState.IsValid)   // ModelState.IsValid，通過表單驗證（Server-side validation）需搭配 Model底下類別檔的 [驗證]
            {

                HttpResponseMessage response = await client.PutAsJsonAsync<User>(_user.UserId.ToString(), _user);
                return Content(" *** 完成 *** ");
                //return RedirectToAction(nameof(List));  // 提升程式的維護性，常用在"字串"上。
            }
            else
            {
                //return View(_userTable);  // 若沒有修改成功，則列出原本畫面
                return Content(" *** 更新失敗！！**");
            }
        }
    }
}