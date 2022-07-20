using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TestWebApplication.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TestWebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly TestDataContext _db;
        public DatabaseController(TestDataContext db)
        {
            _db = db;
        }
        public class SearchInput
        {
            public string? _Input { get; set; }
            public int? _UserID { get; set; }
        }


        // GET: api/<DatabaseController>
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _db.Users.ToList();
        }
        /// <summary>
        /// 根據ID取得資料
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        // GET api/<DatabaseController>/5
        [HttpGet("{UserId},{UserName},{UserEmail}")]
        public ActionResult<User> Get(int? UserId, string? UserName, string? UserEmail)
        {
            if (UserId== null)
            {
                UserId = 9999;
            }
            try
            {
                User? resultID = _db.Users.Where(a => a.UserId == UserId).FirstOrDefault();
                if (resultID == null)
                {
                    User? resultName = _db.Users.Where(a => a.UserName == UserName).FirstOrDefault();
                    if (resultName == null)
                    {
                        User? resultEmail = _db.Users.Where(a => a.UserEmail == UserEmail).FirstOrDefault();
                        if (resultEmail == null)
                        {
                            resultEmail = new User();
                            resultEmail.UserId = 9999;
                            resultEmail.UserName = "查無資料";
                            resultEmail.UserEmail = "查無資料";
                            resultEmail.UserPassWord = "查無資料";
                            return resultEmail;

                        }
                        else
                        {
                            return resultEmail;
                        }
                    }
                    else
                    {
                        return resultName;
                    }
                }
                else
                {
                    return resultID;
                }
            }
            catch(Exception ex)
            {

                User? resultID = new User();
                resultID.UserId = 9999;
                resultID.UserName = "查無資料";
                resultID.UserEmail = "查無資料";
                resultID.UserPassWord = "查無資料";
                return resultID;
            }
            

            
        }


        //[HttpGet("{SearchString}")]
        //public IQueryable<User> GSearchet(string SearchString)
        //{
        //    var result = _db.Users.Where(a => a.UserName == SearchString);
        //    if (result == null)
        //    {
        //        return (IQueryable<User>)NotFound("沒有資料");
        //    }
        //    return result;
        //}
        //新增資料
        [HttpPost]
        public ActionResult<User> Post([FromBody] User value)
        {
            _db.Users.Add(value);
            _db.SaveChanges();

            return CreatedAtAction(nameof(Get), new { id = value.UserId }, value);
        }
        /// <summary>
        /// 修改資料
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="todoItem"></param>
        /// <returns></returns>
        [HttpPut("{UserID}")]
        public IActionResult Put(int UserID, [FromBody] User todoItem)
        {
            if (UserID != todoItem.UserId)
            {


                return BadRequest();
            }

            _db.Entry(todoItem).State = EntityState.Modified;

            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_db.Users.Any(e => e.UserId == UserID))
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(500, "存取發生錯誤");
                }
            }
            return NoContent();
        }
        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpDelete("{UserID}")]
        public IActionResult Delete(int UserID)
        {
            var result = _db.Users.Find(UserID);

            if (result == null)
            {
                return NotFound();
            }

            _db.Users.Remove(result);
            _db.SaveChanges();

            return NoContent();
        }
        /// <summary>
        /// 搜尋資料
        /// </summary>
        /// <param name="_searchword"></param>
        /// <returns></returns>


    }
}
