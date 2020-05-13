using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Charactify.API.InterFace;
using Charactify.API.Models;
using Charactify.API.Services;
using Charactify.API.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Charactify.API.Controllers
{
    [Route("api/User/")]
    [Produces("application/json")]
    [ApiController]
    public class UserController : ControllerBase
    {
        CServices srv = new CServices();
        [HttpPost]
        [Route("SetUser")]
        public IActionResult SetUser([FromBody] Users userObject)
        {
            int ret = 0;
            //var response = new SingleResponse<CResponse>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.SetUser(userObject, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                    response.Data = "There is some problem in the application, please try after some time";
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("SignUp")]
        public IActionResult SignUp([FromBody] Users signupObject)
        {
            // int ret = 0;
            //var response = new SingleResponse<CResponse>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                response.Data = srv.SignUpUser(signupObject, currentUserId);
                if (!string.IsNullOrEmpty(response.Data))
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Message = "Successfully";
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.OK;
                response.Message = "Successfully";
                //response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                //response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] Users signupObject)
        {

            var response = new SingleResponse<string>();
            try
            {
                response.Data = srv.Login(signupObject);
                if (response.Data == "Invalid Username or Password")
                {
                    response.Code = ResponseCodeEnum.UNAUTHORIZEUSER;
                    response.Message = "Invalid Username or Password";
                }
                else
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Message = "Successfully";
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNAUTHORIZEUSER;
                response.Message = "Invalid Username or Password";
                //response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                //response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword([FromBody] UserMaster Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            string res = string.Empty;
            try
            {
                res = srv.ForgotPassword(Obj.EmailId, Obj.Type, currentUserId);
                if (res != "Invalid EmailID")
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = res;
                    response.Message = "Successfully";
                }
                else if (ret <= 0)
                {
                    response.Data = res;
                    response.Code = ResponseCodeEnum.OK;
                    // response.Code = ResponseCodeEnum.UNAUTHORIZEUSER;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.OK;
                response.Message = "Successfully";
                //response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("VerifyCode")]
        public IActionResult VerifyCode([FromBody] UserMaster Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.VerifyCode(Obj.EmailId, Obj.VerificationCode, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                    response.Message = "Successfully";
                }
                else if (ret < 0)
                {
                    response.Data = "Invalid Verification Code";
                    response.Code = ResponseCodeEnum.INVALIDVERIFICATIONCODE;
                    response.Message = "Invalid Verification Code";
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("ResetPassword")]
        public IActionResult ResetPassword([FromBody] UserMaster Obj)
        {
            // int ret = 0;
            //var response = new SingleResponse<CResponse>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                response.Data = srv.ResetPassword(Obj.EmailId, Obj.Password, currentUserId);
                if (response.Data == "You can not reset  password with old Password")
                {
                    response.Code = 0;
                    response.Message = "You can not reset  password with old Password";
                }
                else
                {
                    if (!string.IsNullOrEmpty(response.Data))
                    {
                        response.Code = ResponseCodeEnum.OK;
                        response.Message = "Successfully";
                    }
                }

            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
            }
            return response.ToHttpResponse();
        }

        [HttpGet]
        [Route("GetUserById")]
        public IActionResult GetUserById(int userId)
        {
            var response = new SingleResponse<UserDetails>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetUser(userId, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
            }
            return response.ToHttpResponse();
        }

        [HttpGet]
        [Route("GetProfile")]
        public IActionResult GetProfile(int userId)
        {
            var response = new SingleResponse<UserDetails>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetUser(userId, currentUserId);
            }
            catch
            {
                response.Code = 1003;
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("ProfileUpdate")]
        public IActionResult ProfileUpdate([FromBody] UserDetailsRequest Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.ProfileUpdate(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                if (ret == 10)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = "User name Already Exist";
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                    response.Data = "Email Id Or Phone Number Already Exist";

                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpGet]
        [Route("GetContact")]
        public IActionResult GetContact(int userId)
        {
            var response = new SingleResponse<UserDetails>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetContact(userId, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                //response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("Invite")]
        public IActionResult Invite([FromBody] List<InviteRequest> objInvite)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.Invite(objInvite, currentUserId).ToString();
            }
            catch
            {
                response.Code = 1001;
                // response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                // response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("AddScore")]
        public IActionResult AddScore([FromBody] ScoreRequest objScore)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.AddScore(objScore, currentUserId).ToString();
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("MultipleTraits")]
        public IActionResult MultipleTraits([FromBody] List<TraitRequest> objTrait)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.AddTraits(objTrait, currentUserId).ToString();
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("UpdateScore")]
        public IActionResult UpdateScore([FromBody] ScoreRequest objScore)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.UpdateScore(objScore, currentUserId).ToString();
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("ApproveScore")]
        public IActionResult ApproveScore([FromBody] ScoreRequest objScore)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.ApproveScore(objScore, currentUserId).ToString();
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpGet]
        [Route("GetScore")]
        public IActionResult GetScore(int UserID)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetScore(UserID, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpPost]
        [Route("GetScoreWithProfile")]
        public IActionResult GetScoreWithProfile(ScoreRequest objScore)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetScoreWithProfile(objScore, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpGet]
        [Route("GetSelfScore")]
        public IActionResult GetSelfScore(int UserID)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetSelfScore(UserID, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("AddRequest")]
        public IActionResult AddRequest([FromBody] ScoreRequest objScore)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.AddRequest(objScore, currentUserId).ToString();
                response.Message = "Successfully";
            }
            catch
            {
                response.Code = ResponseCodeEnum.DBEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("UploadProfilePic")]
        public IActionResult UploadProfilePic(UserMaster UMReq)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.UploadProfilePic(UMReq.UserId, UMReq.UserProfilePic, currentUserId).ToString();
            }
            catch
            {
                response.Code = ResponseCodeEnum.USERNOTFOUND;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("GetConnections")]
        public IActionResult GetConnections([FromBody] Connections Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetConnections(Obj, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpPost]
        [Route("RatingApprove")]
        public IActionResult RatingApprove([FromBody] ScoreRequest objScore)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.AddRequest(objScore, currentUserId).ToString();
                response.Message = "Successfully";

            }
            catch
            {
                response.Code = ResponseCodeEnum.DBEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpGet]
        [Route("GetConnectionRating")]
        public IActionResult GetConnectionRating(int crid)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetConnectionRating(crid, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }
        [HttpPost]
        [Route("FilterEmail")]
        public IActionResult FilterEmail([FromBody] FilterEmailRequest Objreq)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.FilterEmail(Objreq, currentUserId).ToString();
                response.Message = "Successfully";

            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }
        [HttpPost]
        [Route("MyNotifications")]
        public IActionResult MyNotifications([FromBody] Connections Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.MyNotifications(Obj, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("ApproveRequest")]
        public IActionResult ApproveRequest([FromBody] ScoreRequest objScore)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.ApproveRequest(objScore, currentUserId).ToString();
                response.Message = "Successfully";

            }
            catch
            {
                response.Code = ResponseCodeEnum.DBEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("AddFeed")]
        //[DisableRequestSizeLimit]
        [RequestSizeLimit(long.MaxValue)]
        public IActionResult AddFeed([FromBody] FeedRequest Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.AddFeed(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("UpdateFeed")]
        public IActionResult UpdateFeed([FromBody] FeedRequest Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.UpdateFeed(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("AddStory")]
        public IActionResult AddStory([FromBody] StoryRequest Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.StoryFeed(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpGet]
        [Route("GetProfileDetails")]
        public IActionResult GetProfileDeatils(int UserID)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetProfileDeatils(UserID, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpPost]
        [Route("AddReactions")]
        public IActionResult AddReactions([FromBody] FeedReactionRequest Obj)
        {
            //int ret = 0;
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Data = srv.AddReactions(Obj, currentUserId);
                response.Code = ResponseCodeEnum.OK;
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpPost]
        [Route("UpdateComments")]
        public IActionResult UpdateComments([FromBody] FeedReactionRequest Obj)
        {
            int ret = 0;
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                // response = srv.UpdateComments(Obj);

                response.Code = ResponseCodeEnum.OK;
                response.Data = srv.UpdateComments(Obj, currentUserId);

            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpPost]
        [Route("GetFeedList")]
        public IActionResult GetFeedList([FromBody] GetFeedList Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetFeedList(Obj, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        //public IActionResult GetFeedList(int ReactionID)
        //{
        //    var response = new SingleResponse<string>();
        //    try
        //    {
        //        response.Code = 1001;
        //        response.Data = srv.GetFeedList(ReactionID);
        //    }
        //    catch
        //    {
        //        response.Code = 1003;
        //    }
        //    return response.ToHttpResponse();
        //}


        [HttpPost]
        [Route("ScoreSummarybycategory")]
        public IActionResult ScoreSummarybycategory([FromBody] GetScore Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.ScoreSummarybycategory(Obj, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("ScoreSummarybytraits")]
        public IActionResult ScoreSummarybytraits([FromBody] GetScore Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.ScoreSummarybytraits(Obj, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("GetAllCategory")]
        public IActionResult GetAllCategory([FromBody] AllCategoryUsers Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetAllCategory(Obj, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpGet]
        [Route("SendNotification")]
        public IActionResult SendNotification(int UserID)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                srv.SendNotification(currentUserId);
            }
            catch (Exception ex)
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("AddUserToken")]
        public IActionResult AddUserToken([FromBody] UserMaster Obj)
        {
            int ret = 0;
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            ret = srv.AddUserToken(Obj, currentUserId);
            try
            {
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpPost]
        [Route("GetFrindShip")]
        public IActionResult GetFrindShip([FromBody] FriendShipRequest Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetFriendShip(Obj, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("GetStoryList")]
        public IActionResult GetStoryList([FromBody] GetFeedList Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetStoryList(Obj, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        //[HttpPost]
        //[Route("GetStoryListForNode")]
        //public IActionResult GetStoryListForNode([FromBody] GetFeedList Obj)
        //{
        //    var response = new SingleResponse<string>();
        //    string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
        //    try
        //    {
        //        response.Code = 1001;
        //        response.Data = srv.GetStoryListForNode(Obj, currentUserId);
        //    }
        //    catch
        //    {
        //        response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
        //        response.Data = "There is some problem in the application, please try after some time";
        //    }
        //    return response.ToHttpResponse();
        //}

        [HttpPost]
        [Route("AddUpdateEducationDetails")]
        public IActionResult AddUpdateEducationDetails([FromBody] UserEducationDetails Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.AddUpdateEducationDetails(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("AddUpdateWorkDetails")]
        public IActionResult AddUpdateWorkDetails([FromBody] UserWorkDetails Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.AddUpdateWorkDetails(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        // 
        [HttpPost]
        [Route("GetSpecificFeedResponse")]
        public IActionResult GetSpecificFeedResponse([FromBody] GetFeedList Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetSpecificFeedResponse(Obj, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpGet]
        [Route("GetSearchList")]
        public IActionResult GetSearchList(int UserID)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetSearchList(UserID, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("UpdateMute")]
        public IActionResult UpdateMute([FromBody] ScoreRequest Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            int ret = -1;
            try
            {
                response.Code = 1001;
                ret = srv.UpdateMute(Obj, currentUserId);
                response.Data = ret.ToString();
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpPost]
        [Route("AddShare")]
        public IActionResult AddShare([FromBody] Share Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            int ret = -1;
            try
            {
                response.Code = 1001;
                ret = srv.addShare(Obj, currentUserId);
                response.Data = ret.ToString();
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("ConnectionRemove")]
        public IActionResult ConnectionRemove([FromBody] ScoreRequest Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            int ret = -1;
            try
            {
                response.Code = 1001;
                ret = srv.ConnectionRemove(Obj, currentUserId);
                response.Data = ret.ToString();
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpPost]
        [Route("UserWorkDelete")]
        public IActionResult UserWorkDelete([FromBody] UserWorkDetails Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.UserWorkDelete(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("DeleteEducationDetails")]
        public IActionResult DeleteEducationDetails([FromBody] UserEducationDetails Obj)
        {
            int ret = 0;
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                ret = srv.DeleteEducationDetails(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("GetConnectionSearchList")]
        public IActionResult GetConnectionSearchList([FromBody] ResUserDetail Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetConnectionSearchList(Obj, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpPost]
        [Route("UpdateUserName")]
        public IActionResult UpdateUserName([FromBody] UserDetailsRequest Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.UpdateUserName(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                if (ret == 10)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = "User name Already Exist";
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("DeleteComments")]
        public IActionResult DeleteComments([FromBody] FeedReactionRequest Obj)
        {
            int ret = 0;
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                // ret = srv.DeleteComments(Obj);

                response.Code = ResponseCodeEnum.OK;
                response.Data = srv.DeleteComments(Obj, currentUserId);

            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }



        [HttpPost]
        [Route("Addtagging")]
        public IActionResult Addtagging([FromBody] FeedRequest Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.Addtagging(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("DeleteFeed")]
        public IActionResult DeleteFeed([FromBody] FeedRequest Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.DeleteFeed(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpPost]
        [Route("AddNotifications")]
        public IActionResult AddNotifications([FromBody] Notification objNotification)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.AddNotifications(objNotification, currentUserId).ToString();
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpPost]
        [Route("LogOut")]
        public IActionResult LogOut([FromBody] UserMaster Obj)
        {
            int ret = 0;
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            ret = srv.LogOut(Obj, currentUserId);
            try
            {
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpGet]
        [Route("SelfScoreGet")]
        public IActionResult SelfScoreGet(int UserID)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.SelfScoreGet(UserID, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpGet]
        [Route("ConvertHtmlToImage")]
        public IActionResult ConvertHtmlToImage(int UserID)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.ConvertHtmlToImage(UserID, currentUserId);
            }
            catch (Exception ex)
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("GetFeedResponseOnFeedId")]
        public IActionResult GetFeedResponseOnFeedId([FromBody] GetFeedList Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetFeedResponseOnFeedId(Obj, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpGet]
        [Route("ScoreCard")]
        public IActionResult ScoreCard(int UserID)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.ScoreCard(UserID, currentUserId);
            }
            catch (Exception ex)
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("GetScoreStatus")]
        public IActionResult GetScoreStatus([FromBody] ScoreRequest objScore)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetScoreStatus(objScore, currentUserId).ToString();
                response.Message = "Successfully";

            }
            catch
            {
                response.Code = ResponseCodeEnum.DBEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpPost]
        [Route("GetFeedResponseForPost")]
        public IActionResult GetFeedResponseForPost([FromBody] GetFeedList Obj)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.GetFeedResponseForPost(Obj, currentUserId);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [Route("EmailSearchList")]
        public IActionResult EmailSearchList([FromBody] SearchList Objreq)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                response.Data = srv.EmailSearchList(Objreq, currentUserId).ToString();
                response.Message = "Successfully";

            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpGet]
        [Route("GetNotificationforCharactify")]
        public IActionResult GetNotificationforCharactify(int UserID)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                response.Code = 1001;
                srv.GetNotificationforCharactify(UserID, currentUserId);
            }
            catch (Exception ex)
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("AddVideoForIos")]
        public IActionResult AddVideoForIos([FromBody] FeedRequest Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.AddVideoForIos(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("DeActivateAccount")]
        public IActionResult DeActivateAccount([FromBody] UserDetailsRequest Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.DeActivateAccount(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("UserPrivacyDetails")]
        public IActionResult UserPrivacyDetails([FromBody] UserPrivacyDetails Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.UserPrivacyDetails(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("RemoveTagging")]
        public IActionResult RemoveTagging([FromBody] GetFeedList Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.RemoveTagging(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("getUserPrivacyDetails")]
        public IActionResult getUserPrivacyDetails([FromBody] UserPrivacyDetails Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                response.Code = ResponseCodeEnum.OK;
                response.Data = srv.getUserPrivacyDetails(Obj, currentUserId);

            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        //public ActionResult UserId(string UserId)
        //{
        //    int currentUserId = 0;
        //    if (!String.IsNullOrEmpty(UserId))
        //    {

        //        currentUserId = Convert.ToInt32(UserId);
        //    }

        //    return currentUserId;
        //}


        [HttpPost]
        [Route("AddNotificationsForIos")]
        public IActionResult AddNotificationsForIos([FromBody] Notification objNotification)
        {
            var response = new SingleResponse<string>();
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            try
            {
                string deviceid = "faMw6VhALK8:APA91bEzsvOulPnPSr6dGnwR3z_TNJ0xlc4RBjxlFxQYNdG9vh_FNSUi7arnJBzoefgvT7GUrUXoFMAB8ExmiICAFKaLjVZDWsVcNdFW5C9XcuQdD_6ezGmhqMATXyEkG7PCKvg1fDrT";
                response.Code = 1001;
                //  response.Data = srv.pushMessage(objNotification, currentUserId).ToString();
                // response.Data = 
                // srv.pushMessage(deviceid);
                srv.pushios(deviceid);
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("Updatetagging")]
        public IActionResult Updatetagging([FromBody] FeedRequest Obj)
        {
            int ret = 0;
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.Updatetagging(Obj, currentUserId);
                if (ret > 0)
                {
                    response.Code = ResponseCodeEnum.OK;
                    response.Data = ret.ToString();
                }
                else if (ret <= 0)
                {
                    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                }
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        //[HttpPost]
        //[Route("GetVideoContent")]
        //public IActionResult GetVideoContent([FromBody] FeedRequest Obj)
        //{
        //    var response = new SingleResponse<string>();
        //    string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
        //    try
        //    {
        //        response.Code = 1001;
        //        new PushStreamContent((Action<Stream, HttpContent, TransportContext>)srv.WriteContentToStream);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
        //        response.Data = "There is some problem in the application, please try after some time";
        //    }
        //    return response.ToHttpResponse();
        //}


        //[HttpPost]
        //[Route("GetFeedListForNode")]
        //public IActionResult GetFeedListForNode([FromBody] GetFeedList Obj)
        //{
        //    var response = new SingleResponse<string>();
        //    string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
        //    try
        //    {
        //        response.Code = 1001;
        //        response.Data = srv.GetFeedListForNode(Obj, currentUserId);
        //    }
        //    catch
        //    {
        //        response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
        //        response.Data = "There is some problem in the application, please try after some time";
        //    }
        //    return response.ToHttpResponse();
        //}

        //[HttpPost]
        //[Route("AddFeedForNode")]
        //public IActionResult AddFeedForNode([FromBody] FeedRequest Obj)
        //{
        //    int ret = 0;
        //    string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
        //    var response = new SingleResponse<string>();
        //    try
        //    {
        //        ret = srv.AddFeedForNode(Obj, currentUserId);
        //        if (ret > 0)
        //        {
        //            response.Code = ResponseCodeEnum.OK;
        //            response.Data = ret.ToString();
        //        }
        //        else if (ret <= 0)
        //        {
        //            response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
        //        }
        //    }
        //    catch
        //    {
        //        response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
        //        response.Data = "There is some problem in the application, please try after some time";
        //    }
        //    return response.ToHttpResponse();
        //}

        [HttpPost]
        [Route("Addvideofromurl")]
        public IActionResult Addvideofromurl([FromBody] FeedRequest objaf)
        {
            string ret = "0";
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.base64(objaf, currentUserId);
                //if (ret > 0)
                //{
                //    response.Code = ResponseCodeEnum.OK;
                //    response.Data = ret.ToString();
                //}
                //else if (ret <= 0)
                //{
                //    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                //}
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }


        [HttpPost]
        [Route("SendSms")]
        public IActionResult SendSms([FromBody] FeedRequest objaf)
        {
            string ret = "0";
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.sendinvitation("+918439728367", "2345");
                //if (ret > 0)
                //{
                //    response.Code = ResponseCodeEnum.OK;
                //    response.Data = ret.ToString();
                //}
                //else if (ret <= 0)
                //{
                //    response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                //}
            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

        [HttpPost]
        [Route("VerifyEmailOrPhone")]
        public IActionResult VerifyEmailOrPhone([FromBody] VerifyEmailOrPhone objaf)
        {
            string ret = "0";
            string currentUserId = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            var response = new SingleResponse<string>();
            try
            {
                ret = srv.VerifyEmailOrPhone(objaf, currentUserId);

            }
            catch
            {
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                response.Data = "There is some problem in the application, please try after some time";
            }
            return response.ToHttpResponse();
        }

    }
}