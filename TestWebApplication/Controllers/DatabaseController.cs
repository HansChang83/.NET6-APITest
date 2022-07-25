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
        //定義DatabaseController以連接model中的連接字串
        private readonly TestDataContext _db;
        public DatabaseController(TestDataContext db)
        {
            _db = db;
        }

      //  GET: api/<DatabaseController>
         [HttpGet]
        public ActionResult<UserDTO> Get()//測試JOIN因此MODEL先改成UserDTO
        {
            var linqjoin = _db.Users.Join(_db.UserInformations,
                user => user.UserId,
                info => info.UserId,
                (user, info) => new
                {
                    UserID = user.UserId,
                    UserName = user.UserName,
                    UserPassword = user.UserPassWord,
                    UserEmail = user.UserEmail,
                    UserIDCard = info.UserIdentityCardNumber,
                    UserPhone = info.UserPhone,
                    UserAddress = info.UserAddress,
                }).FirstOrDefault();
            UserDTO dTO = new UserDTO();
            dTO.UserId = linqjoin.UserID;
            dTO.UserName = linqjoin.UserName;
            dTO.UserIdentityCardNumber = linqjoin.UserIDCard;
            dTO.UserPhone = linqjoin.UserPhone;
            dTO.UserAddress = linqjoin.UserAddress;
            dTO.UserPassWord = linqjoin.UserPassword;
            dTO.UserEmail = linqjoin.UserEmail;


            return dTO;

        }
        /// <summary>
        /// 根據ID取得資料
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        // GET api/<DatabaseController>/x,x,x
        [HttpGet("{UserId},{UserName},{UserEmail}")]
        public ActionResult<User> Get(int? UserId, string? UserName, string? UserEmail)
        {

            if (UserId == null)
            {
                //暫定處理方式，再更新為更好的處理方式
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
            catch (Exception ex)
            {

                User? resultID = new User();
                resultID.UserId = 9999;
                resultID.UserName = "查無資料";
                resultID.UserEmail = "查無資料";
                resultID.UserPassWord = "查無資料";
                return resultID;
            }



        }
        /////////////////////////////// 
        ///回傳JOIN後的資料
        //[HttpGet("userget")]
        //public ActionResult<UserDTO> Get2([FromForm] UserDTO dTO)
        //{

        //   // UserDTO? resultUserDTO = dTO;

        //   // if (resultUserDTO.UserId == null)
        //    {
        //   //     resultUserDTO.UserId = 9999;
        //    }
        // //   User? baseInfo = _db.Users.Where(a => a.UserId == resultUserDTO.UserId).FirstOrDefault();
        //    UserDTO userDTO = new UserDTO();
        //    //if (baseInfo == null)
        //    //{
        //    //    userDTO.UserId = 1;
        //    //    userDTO.UserName = "查無資訊";
        //    //    userDTO.UserIdentityCardNumber = "查無資訊";
        //    //    userDTO.UserEmail = "查無資訊";
        //    //    userDTO.UserPhone = 0;
        //    //    userDTO.UserAddress = "查無資訊";
        //    //    userDTO.UserPassWord = "查無資訊";

        //    //    return userDTO;
        //    //}
        //    //else
        //    //{
        //    //    _db.Users.ToList();
        //    //    _db.UserInformations.ToList();
        //    //    var linqjoin = _db.Users.Join(_db.UserInformations,
        //    //         user => user.UserId,
        //    //         info => info.UserId,
        //    //         (user, info) => new
        //    //         {
        //    //             UserID = user.UserId,
        //    //             UserName = user.UserName,
        //    //             UserPassword = user.UserPassWord,
        //    //             UserEmail = user.UserEmail,
        //    //             UserIDCard = info.UserIdentityCardNumber,
        //    //             UserPhone = info.UserPhone,
        //    //             UserAddress = info.UserAddress,
        //    //         }).ToList();


        //    //    //若無join資料，則傳送原user之資料
        //    //    if (linqjoin.Count == 0)
        //    //    {

        //    //        userDTO.UserId = baseInfo.UserId;
        //    //        userDTO.UserName = baseInfo.UserName;
        //    //        userDTO.UserEmail = baseInfo.UserEmail;
        //    //        userDTO.UserPassWord = baseInfo.UserPassWord;
        //    //        userDTO.UserIdentityCardNumber = "查無資訊";
        //    //        userDTO.UserPhone = 0;
        //    //        userDTO.UserAddress = "查無資訊";
        //    //        ;

        //    //        return userDTO;
        //    //    }
        //    //    //若有join資料，輸出join後的資料
        //    //    else
        //    //    {
        //    //        //確認查詢到的User筆數
        //    //        for (int a = 0; a < _db.Users.Count(); a++)
        //    //        {
        //    //            //LinqJoin的資料不一定是按照順序的，比如說id==1 和 id==10有對應的資料而對應的count數也只有2，因此要跑完查詢到的所有user資料去對應
        //    //            for (int b = 0; b < linqjoin.Count; b++)
        //    //            {
        //    //                //若有2筆查詢資料則linqjoin[0]和linqjoin[1]有值
        //    //                if (linqjoin[b].UserID == resultUserDTO.UserId)
        //    //                {
        //    //                    //將對應之資料傳入DTO中，回傳至client端
        //    //                    userDTO.UserId = linqjoin[b].UserID;
        //    //                    userDTO.UserName = linqjoin[b].UserName;
        //    //                    userDTO.UserIdentityCardNumber = linqjoin[a].UserIDCard;
        //    //                    userDTO.UserPhone = linqjoin[b].UserPhone;
        //    //                    userDTO.UserAddress = linqjoin[b].UserAddress;
        //    //                    userDTO.UserPassWord = linqjoin[b].UserPassword;
        //    //                    userDTO.UserEmail = linqjoin[b].UserEmail;
        //    //                    return userDTO;
        //    //                }
        //    //            }
        //    //        }
        //    //        //若執行至此而未return 則代表有user值 卻無匹配之join，因此回傳user之值，剩下補入"沒有資料"
        //    //        userDTO.UserId = baseInfo.UserId;
        //    //        userDTO.UserName = baseInfo.UserName;
        //    //        userDTO.UserEmail = baseInfo.UserEmail;
        //    //        userDTO.UserPassWord = baseInfo.UserPassWord;
        //    //        userDTO.UserIdentityCardNumber = "查無資訊";
        //    //        userDTO.UserPhone = 0;
        //    //        userDTO.UserAddress = "查無資訊";
        //    //        return userDTO;
        //    //    }

        //    //}
        //    userDTO.UserId = 1;
        //    userDTO.UserName = "查無資訊";
        //    userDTO.UserIdentityCardNumber = "查無資訊";
        //    userDTO.UserEmail = "查無資訊";
        //    userDTO.UserPhone = 0;
        //    userDTO.UserAddress = "查無資訊";
        //    userDTO.UserPassWord = "查無資訊";
        //    return userDTO;
        //}

       
        //新增資料
        [HttpPost]
        //若要強制 Web API 從要求本文讀取簡單類型，請將 [FromBody] 屬性新增至 參數
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
        [HttpPut]
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
                //判斷錯誤原因
                if (!_db.Users.Any(e => e.UserId == UserID))
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(500, "存取發生錯誤");
                }
            }
            //204
            return NoContent();
        }
        /// <summary>
        /// 刪除資料
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        [HttpDelete]
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
        


    }
}
