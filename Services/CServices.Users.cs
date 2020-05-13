using Charactify.API.DataModels;
using Charactify.API.Models;
using Charactify.API.Shared;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Charactify.API.InterFace;
using System.Diagnostics;
using System.Drawing;
using CoreHtmlToImage;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Drawing.Drawing2D;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using PushSharp.Apple;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.SqlServer.Server;
using Microsoft.AspNetCore.Http;
using System.Web;

namespace Charactify.API.Services
{



    public partial class CServices
    {
        Manager mn = new Manager();
        //MediaServer.Media media = new MediaServer.Media();
        //media ClassLibrary1.Class1 objmedia = new ClassLibrary1.Class1();

        // MediaConvertorClass objCll=new MediaConvertorClass()
        public string otp = null;
        public DateTime Currentdatetime = DateTime.UtcNow;
        // string URL = "http://webdot-001-site5.ctempurl.com";
        string URL = "https://www.charactify.net";
        string UrlNew = "www.charactify.me/video?url=";
        // string FullUrl = "http://www.charactify.me/video?url=";
        string FullUrl = "https://www.charactify.me:8443/video?url=";
        string baseDir = "";
        //Boolean Apilog = CResources.Apilog();
        // int CardWhite = CResources.CardWhite();
        //int CardGray = CResources.CardGray();
        //int CardGolden = CResources.CardGolden();
        Boolean Apilog = true;
        int CardWhite = 0;
        int CardGray = 100;
        int CardGolden = 500;

        public int SetUser(Users users, string currentUserId)
        {
            // Request.hea
            //string clientCode = Convert.ToString(Request.Headers["currentUserId"], CultureInfo.InvariantCulture);
            //string securityKey = Convert.ToString(Request.Headers[Constants.HEADER_SECURITYKEY_NAME], CultureInfo.InvariantCulture);
            int ret = 0;
            int success = 0;
            using (var db = new CContext())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var u = new UserMasters()
                        {
                            UserId = users.UserId,
                            FirstName = users.FirstName,
                            LastName = users.LastName,
                            Gender = users.Gender,
                            DateOfBirth = users.DateOfBirth,
                            EmailId = users.EmailId,
                            Address1 = users.Address1,
                            Address2 = users.Address2,
                            City = users.City,
                            StateId = users.StateId,
                            CountryId = users.CountryId,
                            Zip = users.Zip,
                            Phone = users.Phone,
                            Status = "A",
                            CreatedVia = users.CreatedVia,
                            ViaId = "1",
                            CreatedBy = 1,
                            CreatedDate = Currentdatetime
                        };
                        db.UserMaster.Add(u);
                        ret = db.SaveChanges();
                        success = 1;
                        if (success > 0)
                        {
                            dbContextTransaction.Commit();
                            ret = u.UserId;
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                    catch (Exception e)
                    {
                        mn.LogError(e.ToString());
                        dbContextTransaction.Rollback();
                        throw e;

                    }
                }
            }
            return ret;
        }

        public string SignUpUser(Users users, string currentUser_Id)
        {
            int ret = 0;
            int success = 0;
            int RequestID = 0;
            string data = string.Empty;
            ResUserDetail res = new ResUserDetail();
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "SignUpUser";
            objLog.Request = JsonConvert.SerializeObject(users).ToString();
            objLog.currentUserId = currentUser_Id;
            RequestID = RequestLog(objLog);
            using (var db = new CContext())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        var result = (from usr in db.UserMaster where (usr.Phone == users.Phone || usr.UserName == users.Phone) && ((usr.Status == "A")) select usr).FirstOrDefault();
                        users.Phone = users.PhoneNo;
                        if (users.CreatedVia == "PH")
                        {
                            result = (from usr in db.UserMaster where (usr.Phone == users.Phone || usr.UserName == users.Phone) && ((usr.Status == "A")) select usr).FirstOrDefault();

                        }
                        else
                        {
                            result = (from usr in db.UserMaster where (usr.EmailId == users.EmailId || usr.UserName == users.EmailId) && ((usr.Status == "A")) select usr).FirstOrDefault();

                        }
                        if (result == null)
                        {
                            // mn.LogError(users.Phone);
                            int i = 0;
                            string[] username = new string[3];
                            string aapname = null;
                            var Appusername = (dynamic)null;
                            if (users.EmailId == null || users.EmailId == "")
                            {
                                username[0] = users.FirstName.ToString();
                            }
                            else
                            {
                                username = users.EmailId.Split("@");
                            }
                            Appusername = (from usr in db.UserMaster where (usr.AppUserName == username[0].ToString()) select usr).FirstOrDefault();
                            users.AppUserName = appUserName(username[0]);
                            while (Appusername != null)
                            {

                                aapname = appUserName(username[0]);
                                users.AppUserName = aapname;
                                Appusername = (from usr in db.UserMaster where (usr.AppUserName.ToLower() == aapname.ToLower()) select usr).FirstOrDefault();

                            }


                            //users.AppUserName = appUserName(username[0]);
                            if (!string.IsNullOrEmpty(users.Password) && (users.CreatedVia == "EM" || users.CreatedVia == "PH"))
                            {
                                users.Password = Manager.Encrypt(users.Password);
                                users.Status = "P";
                                users.Phoneverify = false;
                                users.Emailverify = false;
                            }

                            else
                            {
                                users.Status = "A";
                                //users.veri = "A";
                                users.Phoneverify = false;
                                users.Emailverify = true;
                            }

                            if (users.FirstName == null || users.FirstName.ToUpper() == "NULL" || users.FirstName == "")
                            {
                                users.FirstName = "";
                            }
                            else
                            {
                                users.FirstName = users.FirstName.First().ToString().ToUpper() + users.FirstName.Substring(1);
                            }
                            if (users.LastName == null || users.LastName.ToUpper() == "NULL" || users.LastName == "")
                            {
                                users.LastName = "";
                            }
                            else
                            {
                                users.LastName = users.LastName.First().ToString().ToUpper() + users.LastName.Substring(1);
                            }
                            if (users.CreatedVia == "PH")
                            {
                                users.UserName = users.Phone;
                            }
                            else
                            {
                                users.UserName = users.EmailId;
                            }
                            var u = new UserMasters()
                            {
                                UserName = users.UserName,

                                EmailId = users.EmailId,
                                Password = users.Password,
                                Status = users.Status,
                                //  Status = "A",
                                CreatedVia = users.CreatedVia,
                                CreatedBy = 1,
                                CreatedDate = Currentdatetime,
                                FirstName = users.FirstName,
                                LastName = users.LastName,
                                UserProfilePic = users.UserProfilePic,
                                Phone = users.Phone,
                                UniqueId = users.UniqueId,
                                AppUserName = users.AppUserName,
                                VerifiedPhone = users.Phoneverify,
                                VerifiedEmail = users.Emailverify
                            };
                            db.UserMaster.Add(u);
                            ret = db.SaveChanges();
                            if (ret > 0)
                            {
                                using (var db1 = new TPContext())
                                {
                                    var u1 = new Models.UserPrivacyDetails()
                                    {
                                        UserId = u.UserId,
                                        FamilyStaus = false,// false mean public on frontend 
                                        FriendsStaus = false,
                                        CoWorkersStaus = false,
                                        AcquaintancesStaus = false,
                                        AllCategory = false,
                                        CreatedDate = Currentdatetime,
                                        CreatedBy = u.UserId,


                                    };
                                    db1.UserPrivacyDetails.Add(u1);
                                    db1.SaveChanges();
                                    ret = u1.Id;
                                    success = 1;
                                }
                            }
                            if (success > 0)
                            {
                                dbContextTransaction.Commit();
                                res.UserID = u.UserId;
                                if (users.CreatedVia == "PH")
                                {
                                    res.UserName = u.Phone;
                                }
                                else
                                {
                                    res.UserName = u.EmailId;
                                }
                                res.FirstName = u.FirstName;
                                res.EmailD = u.EmailId;
                                if (!string.IsNullOrEmpty(u.UserProfilePic))
                                {
                                    res.IsProfilePic = true;
                                }
                                res.UserProfilePic = u.UserProfilePic;
                                var connreq = (from usr in db.ConnectionRequest where usr.ToUserId == u.UserId && usr.FromUserId == u.UserId select usr).FirstOrDefault();
                                if (connreq != null)
                                {
                                    res.IsSelfRated = true;
                                }
                                else
                                {
                                    res.IsSelfRated = false;
                                }
                                res.CreatedVia = u.CreatedVia;
                                data = JsonConvert.SerializeObject(res);
                            }
                            else
                            {
                                dbContextTransaction.Rollback();
                            }
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            // mn.LogError(users.CreatedVia);
                            if (!string.IsNullOrEmpty(users.Password) && (users.CreatedVia == "EM" || users.CreatedVia == "PH"))
                            {
                                users.Password = Manager.Encrypt(users.Password);
                            }
                            string connStr = CResources.GetConnectionString();
                            ArrayList arrList = new ArrayList();
                            if (!string.IsNullOrEmpty(users.EmailId))
                            {
                                SP.spArgumentsCollection(arrList, "@EmailID", users.EmailId.ToString(), "varchar", "I");
                            }
                            if (!string.IsNullOrEmpty(users.Phone))
                            {
                                SP.spArgumentsCollection(arrList, "@Phone", users.Phone.ToString(), "varchar", "I");
                            }
                            SP.spArgumentsCollection(arrList, "@Password", users.Password.ToString(), "varchar", "I");
                            SP.spArgumentsCollection(arrList, "@type", users.CreatedVia.ToString(), "varchar", "I");
                            DataSet ds = new DataSet();
                            ds = SP.RunStoredProcedure(connStr, ds, "SP_GetLoginDetails", arrList);
                            mn.LogError(ds.ToString());
                            if (ds.Tables.Count > 0)
                            {
                                data = JsonConvert.SerializeObject(ds.Tables[0]);
                                data = data.Replace("[", "");
                                data = data.Replace("]", "");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        data = "-1";
                        objLog.Response = e.ToString();
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        mn.LogError(e.ToString());
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var reslogin = RequestLog1(objLog);
                            });
                        }
                        dbContextTransaction.Rollback();
                        throw e;
                    }
                    finally
                    {
                        objLog.Response = data;
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        // mn.LogError("Finally");
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var reslogin = RequestLog1(objLog);
                            });
                        }
                    }

                }
            }
            return data;
        }

        //int count = 0;
        string appUsrName = null;

        public object Request { get; private set; }
        //public object HtmlRenderer { get; set; }

        public string appUserName(string Emailid)
        {

            appUsrName = Emailid + RandomNumber(1000, 9999);
            // appUsrName = Emailid + "2";

            return appUsrName;
        }

        public string Login(Users users)
        {
            string data = string.Empty;
            ResUserDetail res = new ResUserDetail();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "Login";
            objLog.Request = JsonConvert.SerializeObject(users).ToString();
            RequestID = RequestLog(objLog);
            try
            {
                if (!string.IsNullOrEmpty(users.Password))
                {
                    users.Password = Manager.Encrypt(users.Password);
                }
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@EmailID", users.UserName.ToString(), "varchar", "I");
                SP.spArgumentsCollection(arrList, "@Password", users.Password.ToString(), "varchar", "I");
                SP.spArgumentsCollection(arrList, "@type", users.type.ToString(), "varchar", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "SP_GetLoginDetails", arrList);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        data = JsonConvert.SerializeObject(ds.Tables[0]);
                        data = data.Replace("[", "");
                        data = data.Replace("]", "");
                    }
                    else
                    {
                        data = "Invalid Username or Password";
                    }
                }
                else
                {
                    data = "Invalid Username or Password";
                }

            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var reslogin = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var reslogin = RequestLog1(objLog);
                    });
                }
            }

            return data;
        }

        public string ForgotPassword(string EmailID, string type, string currentUserId)
        {

            int ret = 0;
            string res = string.Empty;
            UserMaster objMaster = new UserMaster();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "ForgotPassword";
            objLog.Request = JsonConvert.SerializeObject(EmailID + "," + type).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            using (var db = new CContext())
            {

                try
                {
                    var result = (from usr in db.UserMaster where ((usr.EmailId == EmailID) || (usr.Phone == EmailID) || (usr.UserName == EmailID)) && ((usr.Status == "A") || (usr.Status == "P")) orderby usr.UserId descending select usr).FirstOrDefault();
                    if (type == "reset")
                    {
                        result = (from usr in db.UserMaster where ((usr.EmailId == EmailID && usr.VerifiedEmail == true) || (usr.Phone == EmailID && usr.VerifiedPhone == true) || (usr.UserName == EmailID)) && ((usr.Status == "A")) orderby usr.UserId descending select usr).FirstOrDefault();
                    }
                    if (result != null)
                    {
                        ret = RandomNumber(1000, 9999);
                        otp = ret.ToString();
                        if (type == "otp-verification")
                        {
                            if (EmailID.Contains("@"))
                            {
                                MailMessage Mail = new MailMessage();

                                string htmlString = null;
                                htmlString = "<html><body><p>Dear User,</p><p>Your verification code for the registration is:  </p> <b> {0} </b><p>Thanks, <br><b>Charactify Team <b></br></p></body></html>";
                                Mail.Subject = "Verification Code for registration!";

                                htmlString = string.Format(htmlString, otp);
                                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                                SmtpServer.Port = Convert.ToInt16(587);
                                Mail.To.Add(EmailID);
                                Mail.From = new MailAddress("charactifymail@gmail.com");

                                Mail.Body = htmlString;
                                Mail.IsBodyHtml = true;
                                SmtpServer.UseDefaultCredentials = false;
                                SmtpServer.Credentials = new System.Net.NetworkCredential("charactifymail@gmail.com", "Char123456");
                                // SmtpServer.Credentials = new System.Net.NetworkCredential("democlientmail@gmail.com", "gnxt@123");
                                SmtpServer.EnableSsl = true;
                                Object state = ret;
                                //event handler for asynchronous call
                                SmtpServer.SendCompleted += new SendCompletedEventHandler(smtpClient_SendCompleted);
                                //SmtpServer.Send(Mail);
                                SmtpServer.SendAsync(Mail, state);
                            }
                            else
                            {
                                sendOtp(EmailID, otp);
                            }
                        }
                        else if (type == "reset")
                        {
                            if (result.VerifiedEmail == true && result.VerifiedPhone == true)
                            {
                                sendOtp(result.Phone, otp);
                                SendEmail(result.EmailId, type, otp);
                            }
                            else if (result.VerifiedEmail == true)
                            {
                                SendEmail(result.EmailId, type, otp);
                            }
                            else if (result.VerifiedPhone == true)
                            {
                                sendOtp(result.Phone, otp);
                            }
                            else
                            {
                                if (EmailID.Contains("@"))
                                {
                                    MailMessage Mail = new MailMessage();

                                    string htmlString = null;
                                    if (type == "reset")
                                    {
                                        htmlString = "<html><body><p>Dear User,</p><p>Your verification code to reset the password is: </p> <b> {0} </b><p>Thanks, <br><b>Charactify Team <b></br></p></body></html>";
                                        Mail.Subject = "Verification Code to reset the password!";
                                    }
                                    htmlString = string.Format(htmlString, otp);
                                    SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                                    SmtpServer.Port = Convert.ToInt16(587);
                                    Mail.To.Add(EmailID);
                                    Mail.From = new MailAddress("charactifymail@gmail.com");

                                    Mail.Body = htmlString;
                                    Mail.IsBodyHtml = true;
                                    SmtpServer.UseDefaultCredentials = false;
                                    SmtpServer.Credentials = new System.Net.NetworkCredential("charactifymail@gmail.com", "Char123456");
                                    // SmtpServer.Credentials = new System.Net.NetworkCredential("democlientmail@gmail.com", "gnxt@123");
                                    SmtpServer.EnableSsl = true;
                                    Object state = ret;
                                    //event handler for asynchronous call
                                    SmtpServer.SendCompleted += new SendCompletedEventHandler(smtpClient_SendCompleted);
                                    //SmtpServer.Send(Mail);
                                    SmtpServer.SendAsync(Mail, state);
                                }
                                else
                                {
                                    sendOtp(EmailID, otp);
                                }
                            }
                        }
                        else if (type == "verify")
                        {
                            if (EmailID.Contains("@"))
                            {
                                MailMessage Mail = new MailMessage();

                                string htmlString = null;
                                htmlString = "<html><body><p>Dear User,</p><p>Your verification code to verify the email:</p> <b> {0} </b><p>Thanks, <br><b>Charactify Team <b></br></p></body></html>";
                                Mail.Subject = "Verification Code to verify your email";

                                htmlString = string.Format(htmlString, otp);
                                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                                SmtpServer.Port = Convert.ToInt16(587);
                                Mail.To.Add(EmailID);
                                Mail.From = new MailAddress("charactifymail@gmail.com");

                                Mail.Body = htmlString;
                                Mail.IsBodyHtml = true;
                                SmtpServer.UseDefaultCredentials = false;
                                SmtpServer.Credentials = new System.Net.NetworkCredential("charactifymail@gmail.com", "Char123456");
                                // SmtpServer.Credentials = new System.Net.NetworkCredential("democlientmail@gmail.com", "gnxt@123");
                                SmtpServer.EnableSsl = true;
                                Object state = ret;
                                //event handler for asynchronous call
                                SmtpServer.SendCompleted += new SendCompletedEventHandler(smtpClient_SendCompleted);
                                //SmtpServer.Send(Mail);
                                SmtpServer.SendAsync(Mail, state);
                            }
                            else
                            {
                                sendOtp(EmailID, otp);
                            }
                        }


                        result.VerificationCode = ret.ToString();
                        db.UserMaster.Update(result);
                        ret = db.SaveChanges();
                        res = ret.ToString();
                    }
                    else
                    {
                        res = "Invalid EmailID or Phone Number";
                    }

                }
                catch (Exception e)
                {
                    res = "-1";
                    objLog.Response = e.ToString();
                    objLog.LogId = RequestID;
                    ResponseLog(objLog);
                    mn.LogError(e.ToString());
                    if (Apilog == true)
                    {
                        Task.Run(() =>
                        {
                            var reslogin = RequestLog1(objLog);
                        });
                    }
                    throw e;
                }
                finally
                {
                    objLog.Response = ret.ToString();
                    objLog.LogId = RequestID;
                    ResponseLog(objLog);
                    if (Apilog == true)
                    {
                        Task.Run(() =>
                        {
                            var resul = RequestLog1(objLog);
                        });
                    }
                }
                // }
            }
            return res;
        }

        public void SendEmail(string EmailID, string type, string otp)
        {
            MailMessage Mail = new MailMessage();

            string htmlString = null;
            if (type == "reset")
            {
                htmlString = "<html><body><p>Dear User,</p><p>Your verification code to reset the password is: </p> <b> {0} </b><p>Thanks, <br><b>Charactify Team <b></br></p></body></html>";
                Mail.Subject = "Verification Code to reset the password!";
            }
            else
            {
                htmlString = "<html><body><p>Dear User,</p><p>Your verification code for the registration is:  </p> <b> {0} </b><p>Thanks, <br><b>Charactify Team <b></br></p></body></html>";
                Mail.Subject = "Verification Code for registration!";
            }
            htmlString = string.Format(htmlString, otp);
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            SmtpServer.Port = Convert.ToInt16(587);
            Mail.To.Add(EmailID);
            Mail.From = new MailAddress("charactifymail@gmail.com");

            Mail.Body = htmlString;
            Mail.IsBodyHtml = true;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("charactifymail@gmail.com", "Char123456");
            // SmtpServer.Credentials = new System.Net.NetworkCredential("democlientmail@gmail.com", "gnxt@123");
            SmtpServer.EnableSsl = true;
            Object state = otp;
            //event handler for asynchronous call
            SmtpServer.SendCompleted += new SendCompletedEventHandler(smtpClient_SendCompleted);
            //SmtpServer.Send(Mail);
            SmtpServer.SendAsync(Mail, state);
        }
        public int VerifyCode(string EmailID, string VerificationCode, string currentUserId)
        {
            int ret = 0;
            UserMaster objMaster = new UserMaster();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "VerifyCode";
            objLog.Request = EmailID + ";" + VerificationCode;
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            using (var db = new CContext())
            {

                try
                {
                    var result = (from usr in db.UserMaster where (usr.EmailId == EmailID || usr.Phone == EmailID || usr.UserName == EmailID) && (usr.VerificationCode == VerificationCode) orderby usr.UserId descending select usr).FirstOrDefault();
                    if (result != null)
                    {
                        if (EmailID.Contains("@") && EmailID == result.EmailId)
                        {
                            result.VerifiedEmail = true;
                            result.UserName = EmailID;
                        }
                        else
                        {
                            if (EmailID == result.Phone)
                            {
                                result.VerifiedPhone = true;
                                result.UserName = EmailID;
                            }
                        }
                        if ((result.CreatedVia == "EM" || result.CreatedVia == "PH") && result.Status == "P")
                        {
                            result.Status = "A";

                        }
                        db.UserMaster.Update(result);
                        db.SaveChanges();
                        ret = result.UserId;
                    }
                    else
                    {
                        ret = -1; //user not found 
                    }

                }
                catch (Exception e)
                {
                    ret = -1;
                    objLog.Response = e.ToString();
                    objLog.LogId = RequestID;
                    ResponseLog(objLog);
                    mn.LogError(e.ToString());
                    if (Apilog == true)
                    {
                        Task.Run(() =>
                        {
                            var reslogin = RequestLog1(objLog);
                        });
                    }
                    throw e;
                }
                finally
                {
                    objLog.Response = ret.ToString();
                    objLog.LogId = RequestID;
                    ResponseLog(objLog);
                    if (Apilog == true)
                    {
                        Task.Run(() =>
                        {
                            var resul = RequestLog1(objLog);
                        });
                    }
                }
            }
            return ret;
        }

        void smtpClient_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            int ID = Convert.ToInt32(e.UserState);
            string status = string.Empty;
            string msg = string.Empty;
            if (!e.Cancelled && e.Error == null)
            {
                status = "S";
                //message.Text = "Mail sent successfully"; need to handle this after completion
            }
            else
            {
                status = "F";
                msg = e.Error.Message;
            }
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "smtpClient_SendCompleted";
            objLog.Request = JsonConvert.SerializeObject(msg).ToString();
            RequestID = RequestLog(objLog);
            // UpdateEmailStatus(emailHistoryId, status, msg);
        }

        public UserDetails GetUser(int userId, string currentUserId)
        {
            string data = string.Empty;
            UserDetails usr = new UserDetails();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetUser";
            objLog.Request = JsonConvert.SerializeObject(userId).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@Id", userId.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "sp_GetUser", arrList);
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "users";
                }
                data = JsonConvert.SerializeObject(ds);
                usr = JsonConvert.DeserializeObject<UserDetails>(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var reslogin = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return usr;
        }

        public int UpdatePassword(int userId, String newPwd)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "UpdatePassword";
            objLog.Request = JsonConvert.SerializeObject(userId).ToString();
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    var userMaster = (from usr in db.UserMaster where (usr.UserId == userId) && (usr.Status == "A") select usr).FirstOrDefault();
                    if (userMaster != null)
                    {
                        userMaster.Password = Manager.Encrypt(newPwd);
                        db.UserMaster.Update(userMaster);
                        db.SaveChanges();
                        ret = userMaster.UserId;
                    }
                    else
                    {
                        ret = -1; //user not found 
                    }
                }
            }
            catch (Exception e)
            {
                ret = -1;
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public int ProfileUpdate(UserDetailsRequest users, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "ProfileUpdate";
            objLog.Request = JsonConvert.SerializeObject(users).ToString();
            objLog.currentUserId = currentUserId;
            var response = new SingleResponse<string>();
            if (users.DateOfBirth == null || users.DateOfBirth == "")
            {
                users.DateOfBirth = "01-01-1900";
            }
            RequestID = RequestLog(objLog);
            try
            {
                CultureInfo culture = new CultureInfo("en-US");
                //DateTime tempDate = Convert.ToDateTime("1/1/2010 12:10:15 PM", culture);
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        var EmailOrPhone = (from usr in db.UserMaster where (((usr.UserName == users.EmailId || usr.EmailId == users.EmailId) && usr.UserId != users.UserID && !string.IsNullOrEmpty(usr.EmailId)) || ((usr.UserName == users.Phone || usr.Phone == users.Phone) && usr.UserId != users.UserID && !string.IsNullOrEmpty(usr.Phone))) && (usr.Status == "A") select usr).ToList();
                        if (EmailOrPhone.Count == 0)
                        {
                            var Dataex = (from usr in db.UserMaster where (usr.AppUserName == users.AppUserName && usr.UserId != users.UserID) && (usr.Status == "A") select usr).ToList();
                            if (Dataex.Count == 0)
                            {
                                var Log = (from usr in db.UserMaster where (usr.UserId == users.UserID) && (usr.Status == "A") select usr).FirstOrDefault();
                                if (Log != null)
                                {
                                    if (Log.Phone != users.Phone)
                                    {
                                        Log.VerifiedPhone = false;
                                    }
                                    if (Log.EmailId != users.EmailId)
                                    {
                                        Log.VerifiedEmail = false;
                                    }

                                    Log.FirstName = users.FirstName;
                                    Log.LastName = users.LastName;
                                    //Log.DateOfBirth = Convert.ToDateTime(users.DateOfBirth, culture);
                                    // Log.DateOfBirth = Convert.ToDateTime(users.DateOfBirth.ToString("MM/dd/YYYY"));
                                    string[] date = users.DateOfBirth.Split("T");
                                    // SendPushNotification(date[0].ToString(), userDeviceId[1].ToString());
                                    Log.DateOfBirth = Convert.ToDateTime(date[0].ToString());
                                    Log.City = users.City;
                                    Log.Phone = users.Phone;
                                    Log.EmailId = users.EmailId;
                                    Log.Gender = users.Gender;
                                    Log.ModifiedDate = Currentdatetime;
                                    Log.ModifiedBy = users.UserID;
                                    Log.AppUserName = users.AppUserName;
                                    db.UserMaster.Update(Log);
                                    ret = db.SaveChanges();
                                    //if (users.FirstName != null)
                                    //{
                                    //    Log.FirstName = users.FirstName;
                                    //    Log.ModifiedDate = DateTime.Now;
                                    //    Log.ModifiedBy = users.UserID;
                                    //    db.UserMaster.Update(Log);
                                    //    ret = db.SaveChanges();
                                    //}
                                    //else if (users.LastName != null)
                                    //{
                                    //    Log.LastName = users.LastName;
                                    //    Log.ModifiedDate = DateTime.Now;
                                    //    Log.ModifiedBy = users.UserID;
                                    //    db.UserMaster.Update(Log);
                                    //    ret = db.SaveChanges();
                                    //}
                                    //else if (users.DateOfBirth != null)
                                    //{
                                    //    Log.DateOfBirth = Convert.ToDateTime(users.DateOfBirth.ToString());
                                    //    Log.ModifiedDate = DateTime.Now;
                                    //    Log.ModifiedBy = users.UserID;
                                    //    db.UserMaster.Update(Log);
                                    //    ret = db.SaveChanges();
                                    //}
                                    //else if (users.City != null)
                                    //{
                                    //    Log.City = users.City;
                                    //    Log.ModifiedDate = DateTime.Now;
                                    //    Log.ModifiedBy = users.UserID;
                                    //    db.UserMaster.Update(Log);
                                    //    ret = db.SaveChanges();
                                    //}
                                    //else if (users.Phone != null)
                                    //{
                                    //    Log.Phone = users.Phone;
                                    //    Log.ModifiedDate = DateTime.Now;
                                    //    Log.ModifiedBy = users.UserID;
                                    //    db.UserMaster.Update(Log);
                                    //    ret = db.SaveChanges();
                                    //}

                                }
                            }
                            else
                            {
                                ret = 10;
                            }
                        }
                        else
                        {
                            ret = 0;
                        }
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }

                    }
                }
            }
            catch (Exception e)
            {
                ret = -1;
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                //throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public string ResetPassword(string key, string newPwd, string currentUserId)
        {
            string data = string.Empty;
            ResUserDetail res = new ResUserDetail();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "ResetPassword";
            objLog.Request = JsonConvert.SerializeObject(key).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {


                    var userMaster = (from usr in db.UserMaster where (usr.EmailId == key || usr.Phone == key || usr.UserName == key) && (usr.Status == "A") orderby usr.UserId descending select usr).FirstOrDefault();
                    if (userMaster != null)
                    {
                        userMaster.Password = Manager.Encrypt(newPwd);
                        var userMasterpass = (from usr in db.UserMaster where (usr.EmailId == key || usr.Phone == key || usr.UserName == key) && (usr.Password == userMaster.Password) && (usr.Status == "A") orderby usr.UserId descending select usr).FirstOrDefault();
                        if (userMasterpass == null)
                        {
                            db.UserMaster.Update(userMaster);
                            db.SaveChanges();
                            res.UserID = userMaster.UserId;
                            res.UserName = userMaster.UserName;
                            res.FirstName = userMaster.FirstName;
                            res.EmailD = userMaster.EmailId;
                            if (!string.IsNullOrEmpty(userMaster.UserProfilePic))
                            {
                                res.IsProfilePic = true;
                            }
                            res.UserProfilePic = userMaster.UserProfilePic;
                            var connreq = (from usr in db.ConnectionRequest where usr.ToUserId == userMaster.UserId && usr.FromUserId == userMaster.UserId select usr).FirstOrDefault();
                            if (connreq != null)
                            {
                                res.IsSelfRated = true;
                            }
                            else
                            {
                                res.IsSelfRated = false;
                            }
                            res.CreatedVia = userMaster.CreatedVia;
                            data = JsonConvert.SerializeObject(res);
                        }
                        else
                        {
                            data = "You can not reset  password with old Password";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public UserDetails GetContact(int userId, string currentUserId)
        {
            string data = string.Empty;
            UserDetails usr = new UserDetails();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetContact";
            objLog.Request = JsonConvert.SerializeObject(userId).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", userId.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetContacts", arrList);
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "contacts";
                }
                data = JsonConvert.SerializeObject(ds);
                usr = JsonConvert.DeserializeObject<UserDetails>(data, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {

                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return usr;
        }

        public int Invite(List<InviteRequest> Invreq, string currentUserId)
        {
            int success = -1;
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "Invite";
            objLog.Request = JsonConvert.SerializeObject(Invreq).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            mn.LogError(JsonConvert.SerializeObject(Invreq).ToString());
            using (var db = new CContext())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var res in Invreq)
                        {
                            if (string.IsNullOrEmpty(res.InvitedEmailID) && string.IsNullOrEmpty(res.InvitedPhone))
                            {
                                ret = -1;
                            }
                            else
                            {
                                var u = new DataModels.InvitesMaster()
                                {
                                    InviteeId = res.InviteeID,
                                    InviteVia = res.InviteVia,
                                    InviteViaId = res.InviteViaID,
                                    InvitedName = res.InvitedName,
                                    InvitedPhone = res.InvitedPhone,
                                    InvitedEmailId = res.InvitedEmailID,
                                    InviteReSent = res.InviteReSent,
                                    InviteSentDate = Currentdatetime,
                                    //  InviteReSentDate = DateTime.Now,
                                    FromEmailId = "charactifymail@gmail.com",
                                    Status = false,
                                    CreatedBy = Convert.ToInt32(res.InviteViaID),
                                    CreatedDate = Currentdatetime
                                };
                                db.InvitesMaster.Add(u);
                                ret = db.SaveChanges();
                                if (ret == 0)
                                {
                                    break;
                                }
                                success = 1;
                                if (!string.IsNullOrEmpty(res.InvitedEmailID) && res.InvitedEmailID != null)
                                {
                                    if (success > 0)
                                    {
                                        //dbContextTransaction.Commit();
                                        string HtmlBody = " <html > " + " <body>" +
                                            " <style>  @media only screen and (max-width:767px) { .top{padding:20px 10px !important;height: 81px !important;}.bnr{position: absolute; right: 10px !important;top: 40px !important;width: 100px !important;}.top h1{ margin:5px 0 0 7px !important; font-size: 15px !important;}}</style >" +
                                            "<div id=\"emailer\" style=\"width: auto; max-width: 600px; margin: 0 auto; font-family: arial; padding: 0 7px; \">" +
                                            "<div class=\"top\" style=\"position:relative; \">" +

                                            //" <div class=\"MainBanner\" style=\"position:relative; height:100px;  background: url(https://www.charactify.net/Upload/banner-new.png); background-size: cover; \">" +
                                            //"<div class=\"logo\" style=\"float:left;width:48%;\" > <img src=\"https://www.charactify.net//Upload//logo-white.png\" alt=\"\" width=\"125px\"> " +

                                            //"<h1 style=\"color: #fff; font-size: 18px; line-height: 23px; margin: 18px 0 0 7px; font-family: arial\">Character is everything</h1>" +
                                            //"</div>" +
                                            // "</div>" +

                                            "<img class=\"bnr\" src=\"https://www.charactify.net//Upload//banner-news.png\" alt=\"\" style=\"top: 0;width:100%; \"> " +
                                            "</div>" +
                                            "<div class=\"middle\">" +
                                            "<p style=\"font-size: 16px; line-height: 22px; color: #756c6c;padding:0 0 20px;margin:0;\">An innovative platform enabling users to rate themselves and others based on select character traits.</p > " +
                                            "</div>" +
                                            "<div class=\"bottom\" style=\"background:#f5f5f5; overflow:hidden; padding: 3px 10px 10px 10px\">" +
                                            "<p style=\"text-align: center; font-size:16px; line-height:22px;margin: 15px 0 15px; \">You have received this e-mail because you are invited to join Charactify group.</p>" +
                                            "<p style=\"margin: 10px 0 0 0; font-size:19px; border-top: 1px dashed #ccc;padding-top:15px;text-align:center; font-weight:bold\">Charactify is available for Android </p > " +
                                            " <aside style=\"text-align: center; margin-top:16px; margin-bottom:10px;\">" +
                                            "<a style=\"display:inline-block; width: 165px; margin: 0 5px 0 0;\" href=\"https://play.google.com/store/apps/details?id=com.app.charactify&hl=en\">" +
                                            "<img src=\"https://www.charactify.net/Upload/googleplay.png\" alt=\"\" style=\"width:100%;\"></a>" +
                                            // "<a style=\"display: inline-block; width:165px; \"><img src=\"https://www.charactify.net/Upload/header-appstore.png\" alt=\"\" style=\"width:100%;\"></a>" +
                                            "</aside></div > " +
                                            "<div class=\"footer\" style=\"background: #000;overflow: hidden;padding: 10px 7px; text-align:center;\">" +
                                            "<p style=\"padding: 0; line-height: 15px; font-size: 12px; margin: 0; color: #fff;\">© 2020 <a class=\"link\" href = \"https://www.charactify.com/\" style = \" color: #fff; font-size:11px;\" > Charactify </a> " +
                                            ". All Rights Reserved <a href = \"https://www.charactify.com/tnc/\" style = \"color: #fff;\" > Terms & Conditions </a ></p > " +
                                            "</div></div> </body ></html>";

                                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
                                        MailMessage Mail = new MailMessage();
                                        SmtpServer.Port = Convert.ToInt16(587);
                                        Mail.To.Add(res.InvitedEmailID);
                                        Mail.From = new MailAddress("charactifymail@gmail.com");
                                        Mail.Subject = "Invitation Notification";
                                        Mail.Body = HtmlBody;
                                        Mail.IsBodyHtml = true;
                                        SmtpServer.UseDefaultCredentials = false;
                                        SmtpServer.Credentials = new System.Net.NetworkCredential("charactifymail@gmail.com", "Char123456");
                                        SmtpServer.EnableSsl = true;
                                        Object state = ret;
                                        //event handler for asynchronous call
                                        SmtpServer.SendCompleted += new SendCompletedEventHandler(smtpClient_SendCompleted);
                                        //SmtpServer.Send(Mail);
                                        SmtpServer.SendAsync(Mail, state);
                                        ret = u.InvitesId;
                                    }
                                }
                                if (!string.IsNullOrEmpty(res.InvitedPhone))
                                {
                                    sendinvitation(res.InvitedPhone, "");
                                    ret = u.InvitesId;
                                }

                            }
                        }
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                    catch (Exception e)
                    {
                        mn.LogError(e.ToString());
                        ret = -1;
                        dbContextTransaction.Rollback();
                        objLog.Response = e.ToString();
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var resul = RequestLog1(objLog);
                            });
                        }
                       // throw e;
                    }
                    finally
                    {
                        objLog.Response = ret.ToString();
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var resul = RequestLog1(objLog);
                            });
                        }
                    }
                }
                return ret;
            }

        }

        public int AddScore(ScoreRequest Scoreq, string currentUserId)
        {
            double weightedAvg = 0;
            int success = -1;
            int ret = -1;
            int RequestID = 0;
            decimal avgscore = 0;
            int category = 0;
            int OldCrid = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "AddScore";
            objLog.Request = JsonConvert.SerializeObject(Scoreq).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            using (var db = new CContext())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        category = Convert.ToInt32(Scoreq.Category);
                        var ConnReq = (from usr in db.ConnectionRequest where (usr.FromUserId == Scoreq.FromUserID && usr.ToUserId == Scoreq.ToUserID) orderby usr.Crid descending select usr).FirstOrDefault();
                        if (ConnReq != null)
                        {
                            OldCrid = ConnReq.Crid;
                            ConnReq.Status = "DisApproved";
                            ConnReq.ModifiedBy = Scoreq.FromUserID;
                            ConnReq.ModifiedDate = Currentdatetime;

                        }
                        var u = new DataModels.ConnectionRequest()
                        {
                            FromUserId = Scoreq.FromUserID,
                            ToUserId = Scoreq.ToUserID,
                            ConnectionType = Scoreq.Category,
                            LengthOfRelationship = Convert.ToInt32(Scoreq.SubCategory),
                            CreatedBy = Convert.ToInt32(Scoreq.FromUserID),
                            Status = "Pending",
                            CreatedDate = Currentdatetime,
                        };
                        db.ConnectionRequest.Add(u);
                        db.SaveChanges();
                        ret = u.Crid;
                        success = 1;


                        if (Scoreq.Category == "1" || Convert.ToInt32(Scoreq.SubCategory) == 204)
                        {
                            weightedAvg = .40;

                        }
                        else if (Scoreq.Category == "2" && Convert.ToInt32(Scoreq.SubCategory) < 204)
                        {
                            weightedAvg = .25;
                        }
                        else if (Scoreq.Category == "3" && Convert.ToInt32(Scoreq.SubCategory) < 204)
                        {
                            weightedAvg = .20;
                        }
                        else if (Scoreq.Category == "4" && Convert.ToInt32(Scoreq.SubCategory) < 204)
                        {
                            weightedAvg = .15;
                        }
                        if (success > 0)
                        {
                            int feedid = 0;
                            //if (Scoreq.FromUserID!= Scoreq.ToUserID)
                            //{ 
                            feedid = addCharactify(Scoreq.FromUserID, Scoreq.ToUserID, ret, OldCrid);
                            // }
                            foreach (var res in Scoreq.ScoreTrait)
                            {
                                var Scoremaster = new DataModels.ScoreMaster()
                                {

                                    FeedId = feedid,
                                    Score = Convert.ToDecimal(res.Score),
                                    TraitsId = res.TraitID,
                                    UserId = Scoreq.ToUserID,
                                    ConnectionReqID = ret,
                                    ScoredDate = Currentdatetime,
                                    Approved = 0,
                                    weightedavg = weightedAvg,
                                    CreatedBy = Scoreq.FromUserID,
                                    CreatedDate = Currentdatetime


                                };

                                db.ScoreMaster.Add(Scoremaster);
                                db.SaveChanges();
                                avgscore = avgscore + Convert.ToDecimal(res.Score);
                            }
                            dbContextTransaction.Commit();
                            avgscore = avgscore / 10;
                            string connStr = CResources.GetConnectionString();
                            ArrayList arrList = new ArrayList();
                            SP.spArgumentsCollection(arrList, "@FromUserID", Scoreq.FromUserID.ToString(), "INT", "I");
                            SP.spArgumentsCollection(arrList, "@ToUserID", Scoreq.ToUserID.ToString(), "INT", "I");
                            SP.spArgumentsCollection(arrList, "@Score", avgscore.ToString(), "DECIMAL", "I");
                            SP.spArgumentsCollection(arrList, "@Categoryid", category.ToString(), "INT", "I");
                            SP.spArgumentsCollection(arrList, "@feedId", feedid.ToString(), "INT", "I");
                            SP.spArgumentsCollection(arrList, "@ConnectionReqID", ret.ToString(), "INT", "I");
                            DataSet ds = new DataSet();
                            //if (Scoreq.FromUserID != Scoreq.ToUserID)
                            //{
                            ds = SP.RunStoredProcedure(connStr, ds, "Sp_addscore", arrList);
                            DataTable dt = new DataTable();
                            if (ds.Tables.Count > 0)
                            {
                                dt = ds.Tables[0];
                                foreach (DataRow dr in dt.Rows)
                                {
                                    string[] userDeviceId = dr["token"].ToString().Split(",");
                                    SendPushNotification(userDeviceId[0].ToString(), userDeviceId[1].ToString());
                                }

                            }
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }

                    }
                    catch (Exception e)
                    {
                        ret = -1;
                        objLog.Response = e.ToString();
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        mn.LogError(e.ToString());
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var resul = RequestLog1(objLog);
                            });
                        }
                        dbContextTransaction.Rollback();
                        throw e;
                    }
                    finally
                    {
                        objLog.Response = ret.ToString();
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var resul = RequestLog1(objLog);
                            });
                        }
                    }
                }
                return ret;


            }

        }

        public int AddTraits(List<TraitRequest> TraitReq, string currentUserId)
        {
            int success = -1;
            int ret = -1;
            int RequestID = 0;

            LogRequest objLog = new LogRequest();
            objLog.MethodName = "AddTraits";
            objLog.Request = JsonConvert.SerializeObject(TraitReq).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            using (var db = new CContext())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (TraitRequest tr in TraitReq)
                        {
                            var u = new DataModels.TraitsMaster()
                            {
                                TraitName = tr.TraitName,
                                MinimumScore = tr.MinimumScore,
                                MaximumScore = tr.MaximumScore,
                                CreatedBy = tr.CreatedBy,
                                ModifiedBy = tr.ModifiedBy,
                                CreatedDate = Currentdatetime,
                                ModifiedDate = Currentdatetime,
                            };
                            db.TraitsMaster.Add(u);
                            ret = db.SaveChanges();
                            success = 1;

                        }
                        if (success > 0)
                        {
                            dbContextTransaction.Commit();
                            ret = 1;
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                    catch (Exception e)
                    {
                        ret = -1;
                        objLog.Response = e.ToString();
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        mn.LogError(e.ToString());
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var resul = RequestLog1(objLog);
                            });
                        }
                        dbContextTransaction.Rollback();
                        throw e;
                    }
                    finally
                    {
                        objLog.Response = ret.ToString();
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var resul = RequestLog1(objLog);
                            });
                        }
                    }

                }
                return ret;


            }

        }

        public int ApproveScore(ScoreRequest SocRequest, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "ApproveScore";
            objLog.Request = JsonConvert.SerializeObject(SocRequest).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    var ConnReq = (from usr in db.ConnectionRequest where (usr.Crid == SocRequest.CRID) select usr).FirstOrDefault();
                    if (ConnReq != null)
                    {
                        ConnReq.Status = SocRequest.Status;
                        ConnReq.ModifiedBy = SocRequest.FromUserID;
                        ConnReq.ModifiedDate = Currentdatetime;
                        db.ConnectionRequest.Update(ConnReq);
                        ret = db.SaveChanges();
                        ret = SocRequest.CRID;
                        string connStr = CResources.GetConnectionString();
                        ArrayList arrList = new ArrayList();
                        SP.spArgumentsCollection(arrList, "@FromUserID", SocRequest.FromUserID.ToString(), "INT", "I");
                        SP.spArgumentsCollection(arrList, "@ToUserID", SocRequest.ToUserID.ToString(), "INT", "I");
                        SP.spArgumentsCollection(arrList, "@Status", SocRequest.Status.ToString(), "VARCHAR", "I");
                        SP.spArgumentsCollection(arrList, "@Crid", SocRequest.CRID.ToString(), "INT", "I");
                        DataSet ds = new DataSet();
                        ds = SP.RunStoredProcedure(connStr, ds, "Approverating_sp", arrList);
                        if (ds.Tables.Count > 0)
                        {
                            ret = 1;
                        }
                    }
                    else
                    {
                        ret = -1; //user not found 
                    }
                }

            }
            catch (Exception e)
            {
                ret = -1;
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public int UpdateScore(ScoreRequest SocRequest, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "UpdateScore";
            objLog.Request = JsonConvert.SerializeObject(SocRequest).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var ConnReq = (from usr in db.ScoreMaster where (usr.ConnectionReqId == SocRequest.CRID) select usr).FirstOrDefault();
                            if (ConnReq != null)
                            {
                                db.ScoreMaster.Remove(ConnReq);
                                foreach (var res in SocRequest.ScoreTrait)
                                {
                                    var Scoremaster = new Models.ScoreMaster()
                                    {
                                        Score = Convert.ToDecimal(res.Score),
                                        TraitsId = res.TraitID,
                                        UserId = SocRequest.ToUserID,
                                        ConnectionReqId = SocRequest.CRID,
                                        ScoredDate = Currentdatetime,
                                        Approved = 0,
                                        CreatedBy = SocRequest.FromUserID,
                                        CreatedDate = Currentdatetime
                                    };

                                    db.ScoreMaster.Add(Scoremaster);
                                    ret = db.SaveChanges();
                                }
                                dbContextTransaction.Commit();
                                ret = SocRequest.CRID;
                            }
                            else
                            {
                                ret = -1; //user not found 
                            }
                        }
                        catch (Exception e)
                        {
                            ret = -1;
                            objLog.Response = e.ToString();
                            objLog.LogId = RequestID;
                            ResponseLog(objLog);
                            mn.LogError(e.ToString());
                            if (Apilog == true)
                            {
                                Task.Run(() =>
                                {
                                    var resul = RequestLog1(objLog);
                                });
                            }
                            throw e;
                        }
                        finally
                        {
                            objLog.Response = ret.ToString();
                            objLog.LogId = RequestID;
                            ResponseLog(objLog);
                            if (Apilog == true)
                            {
                                Task.Run(() =>
                                {
                                    var resul = RequestLog1(objLog);
                                });
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                mn.LogError(ex.ToString());
                throw ex;
                //mgr.LogFileWrite(ex);
                //  ret = TPResources.DBEXCEPTIONRETVALUE;
            }
            return ret;


        }

        //public string GetScore(int UserID)
        //{
        //    string data = string.Empty;
        //    //  Scores usr = new Scores();
        //    try
        //    {
        //        string connStr = CResources.GetConnectionString();
        //        ArrayList arrList = new ArrayList();
        //        SP.spArgumentsCollection(arrList, "@UserID", UserID.ToString(), "INT", "I");
        //        DataSet ds = new DataSet();
        //        ds = SP.RunStoredProcedure(connStr, ds, "sp_GetScore", arrList);
        //        if (ds.Tables.Count > 0)
        //        {
        //            ds.Tables[0].TableName = "SelfScore";

        //            ds.Tables[1].TableName = "FamilyScore";
        //            ds.Tables[2].TableName = "FriendScore";
        //            ds.Tables[3].TableName = "CoWorkerScore";
        //            ds.Tables[4].TableName = "AcquaintanceScore";
        //            ds.Tables[6].TableName = "AverageScore";
        //            ds.Tables[5].TableName = "Traits";
        //            ds.Tables[7].TableName = "RatedBy";
        //        }
        //        data = JsonConvert.SerializeObject(ds);
        //        //// usr = JsonConvert.DeserializeObject<Scores>(data, new JsonSerializerSettings
        //        // {
        //        //     NullValueHandling = NullValueHandling.Ignore
        //        // });
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return data;
        //}

        public string ScoreSummarybycategory(GetScore Obj, string currentUserId)
        {
            string data = string.Empty;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "ScoreSummarybycategory";
            objLog.Request = JsonConvert.SerializeObject(Obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                List<CategoryScore> categoryScorelst = new List<CategoryScore>();
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", Obj.UserId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@cat_Id", Obj.CatId.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetScorebyUserAndCat_Sp", arrList);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        CategoryScore ObjCatScore = new CategoryScore();
                        ObjCatScore.AverageScoreAllTrait = Convert.ToDecimal(dr["AvgScoreoftraits"].ToString());

                        ObjCatScore.ScorebyCategourylst = new List<ScorebyCategoury>();
                        ObjCatScore.Traitslst = new List<Traits>();
                        ObjCatScore.TopCharactifierslst = new List<TopCharactifiers>();
                        ObjCatScore.threeMonthslst = new List<ThreeMonths>();
                        ObjCatScore.sixMonthslst = new List<SixMonths>();
                        ObjCatScore.oneYearlst = new List<OneYear>();
                        ObjCatScore.begnninglst = new List<Begnning>();
                        foreach (DataRow drCat in ds.Tables[1].Rows)
                        {
                            ScorebyCategoury ObjScorebyCategoury = new ScorebyCategoury();
                            ObjScorebyCategoury.CatId = Convert.ToInt32(drCat["ConfigurationsID"].ToString());
                            ObjScorebyCategoury.CatScore = Convert.ToDecimal(drCat["ScorebyCat"].ToString());
                            ObjCatScore.ScorebyCategourylst.Add(ObjScorebyCategoury);
                        }
                        foreach (DataRow drtrait in ds.Tables[2].Rows)
                        {
                            Traits ObjTraits = new Traits();
                            ObjTraits.TraitsId = Convert.ToInt32(drtrait["TraitsID"].ToString());
                            ObjTraits.TraitsScore = Convert.ToDecimal(drtrait["ScorebyTraits"].ToString());
                            ObjCatScore.Traitslst.Add(ObjTraits);
                        }
                        foreach (DataRow drtopchar in ds.Tables[3].Rows)
                        {
                            TopCharactifiers ObjTraits = new TopCharactifiers();
                            ObjTraits.username = drtopchar["name"].ToString();
                            ObjTraits.FirstName = drtopchar["firstname"].ToString();
                            ObjTraits.LastName = drtopchar["LastName"].ToString();
                            ObjTraits.Score = Convert.ToDecimal(drtopchar["ScoreByuser"].ToString());
                            ObjTraits.userProfilePic = drtopchar["UserProfilePic"].ToString();
                            ObjTraits.userId = Convert.ToInt32(drtopchar["userid"].ToString());
                            ObjCatScore.TopCharactifierslst.Add(ObjTraits);
                        }
                        foreach (DataRow dtr in ds.Tables[4].Rows)
                        {
                            ThreeMonths ObjthreeMonth = new ThreeMonths();
                            ObjthreeMonth.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            ObjthreeMonth.Monthname = dtr["monthname"].ToString();
                            ObjCatScore.threeMonthslst.Add(ObjthreeMonth);
                        }
                        foreach (DataRow dtr in ds.Tables[5].Rows)
                        {
                            SixMonths ObjsixMonths = new SixMonths();
                            ObjsixMonths.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            ObjsixMonths.Monthname = dtr["monthname"].ToString();
                            ObjCatScore.sixMonthslst.Add(ObjsixMonths);
                        }
                        foreach (DataRow dtr in ds.Tables[6].Rows)
                        {
                            OneYear ObjoneYear = new OneYear();
                            ObjoneYear.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            ObjoneYear.Monthname = dtr["monthname"].ToString();
                            ObjCatScore.oneYearlst.Add(ObjoneYear);
                        }
                        foreach (DataRow dtr in ds.Tables[7].Rows)
                        {
                            Begnning Objbegnning = new Begnning();
                            Objbegnning.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            Objbegnning.Monthname = dtr["monthname"].ToString();
                            ObjCatScore.begnninglst.Add(Objbegnning);
                        }
                        categoryScorelst.Add(ObjCatScore);
                    }
                }
                data = JsonConvert.SerializeObject(categoryScorelst);
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;

        }

        public string ScoreSummarybytraits(GetScore Obj, string currentUserId)
        {
            string data = string.Empty;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "ScoreSummarybytraits";
            objLog.Request = JsonConvert.SerializeObject(Obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                List<TraitScoreSummary> traitScoreSummarylst = new List<TraitScoreSummary>();
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", Obj.UserId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Trait_Id", Obj.TraitId.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetScorebyUserAndTrait_Sp", arrList);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        TraitScoreSummary ObjTraitScore = new TraitScoreSummary();
                        ObjTraitScore.AverageScoreAllTrait = Convert.ToDecimal(dr["AvgScoreoftraits"].ToString());

                        ObjTraitScore.ScorebyCategourylst = new List<ScorebyCategoury>();

                        ObjTraitScore.TopCharactifierslst = new List<TopCharactifiers>();
                        ObjTraitScore.threeMonthslst = new List<ThreeMonths>();
                        ObjTraitScore.sixMonthslst = new List<SixMonths>();
                        ObjTraitScore.oneYearlst = new List<OneYear>();
                        ObjTraitScore.begnninglst = new List<Begnning>();
                        foreach (DataRow drCat in ds.Tables[1].Rows)
                        {
                            ScorebyCategoury ObjScorebyCategoury = new ScorebyCategoury();
                            ObjScorebyCategoury.CatId = Convert.ToInt32(drCat["ConfigurationsID"].ToString());
                            ObjScorebyCategoury.CatScore = Convert.ToDecimal(drCat["ScorebyCat"].ToString());
                            ObjTraitScore.ScorebyCategourylst.Add(ObjScorebyCategoury);
                        }
                        foreach (DataRow drtopchar in ds.Tables[2].Rows)
                        {
                            TopCharactifiers ObjTraits = new TopCharactifiers();
                            ObjTraits.username = drtopchar["name"].ToString();
                            ObjTraits.FirstName = drtopchar["firstname"].ToString();
                            ObjTraits.LastName = drtopchar["LastName"].ToString();
                            ObjTraits.Score = Convert.ToDecimal(drtopchar["ScoreByuser"].ToString());
                            ObjTraits.userProfilePic = drtopchar["UserProfilePic"].ToString();
                            ObjTraits.userId = Convert.ToInt32(drtopchar["userid"].ToString());
                            ObjTraitScore.TopCharactifierslst.Add(ObjTraits);
                        }

                        foreach (DataRow dtr in ds.Tables[3].Rows)
                        {
                            ThreeMonths ObjthreeMonth = new ThreeMonths();
                            ObjthreeMonth.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            ObjthreeMonth.Monthname = dtr["monthname"].ToString();
                            ObjTraitScore.threeMonthslst.Add(ObjthreeMonth);
                        }
                        foreach (DataRow dtr in ds.Tables[4].Rows)
                        {
                            SixMonths ObjsixMonths = new SixMonths();
                            ObjsixMonths.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            ObjsixMonths.Monthname = dtr["monthname"].ToString();
                            ObjTraitScore.sixMonthslst.Add(ObjsixMonths);
                        }
                        foreach (DataRow dtr in ds.Tables[5].Rows)
                        {
                            OneYear ObjoneYear = new OneYear();
                            ObjoneYear.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            ObjoneYear.Monthname = dtr["monthname"].ToString();
                            ObjTraitScore.oneYearlst.Add(ObjoneYear);
                        }
                        foreach (DataRow dtr in ds.Tables[6].Rows)
                        {
                            Begnning Objbegnning = new Begnning();
                            Objbegnning.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            Objbegnning.Monthname = dtr["monthname"].ToString();
                            ObjTraitScore.begnninglst.Add(Objbegnning);
                        }
                        traitScoreSummarylst.Add(ObjTraitScore);
                    }
                }
                data = JsonConvert.SerializeObject(traitScoreSummarylst);
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }

            }
            return data;

        }

        public string GetScore(int userId, string currentUserId)
        {
            List<Home> homelst = new List<Home>();
            string data = string.Empty;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetScore";
            objLog.Request = JsonConvert.SerializeObject(userId).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", userId.ToString(), "INT", "I");
                // SP.spArgumentsCollection(arrList, "@currentUserId", currentUserId.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetScorebyUser_Sp", arrList);
                if (ds.Tables.Count > 0)
                {

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Home Objhome = new Home();
                        Objhome.CountCharactifiers = Convert.ToInt32(dr["CharCount"].ToString());
                        Objhome.CountPost = Convert.ToInt32(dr["Post"].ToString());
                        Objhome.AverageScoreAllTrait = Convert.ToDecimal(dr["AvgScoreoftraits"].ToString());
                        Objhome.UserProfilePic = dr["UserProfilePic"].ToString();
                        Objhome.showProfile = Convert.ToBoolean(dr["showProfile"]);
                        Objhome.ScorebyCategourylst = new List<ScorebyCategoury>();
                        Objhome.Traitslst = new List<Traits>();
                        Objhome.TopCharactifierslst = new List<TopCharactifiers>();
                        Objhome.threeMonthslst = new List<ThreeMonths>();
                        Objhome.sixMonthslst = new List<SixMonths>();
                        Objhome.oneYearlst = new List<OneYear>();
                        Objhome.begnninglst = new List<Begnning>();
                        foreach (DataRow drCat in ds.Tables[1].Rows)
                        {
                            ScorebyCategoury ObjScorebyCategoury = new ScorebyCategoury();
                            ObjScorebyCategoury.CatId = Convert.ToInt32(drCat["ConfigurationsID"].ToString());
                            ObjScorebyCategoury.CatScore = Convert.ToDecimal(drCat["ScorebyCat"].ToString());
                            Objhome.ScorebyCategourylst.Add(ObjScorebyCategoury);
                        }

                        foreach (DataRow drtrait in ds.Tables[2].Rows)
                        {
                            Traits ObjTraits = new Traits();
                            ObjTraits.TraitsId = Convert.ToInt32(drtrait["TraitsID"].ToString());
                            ObjTraits.TraitsScore = Convert.ToDecimal(drtrait["ScorebyTraits"].ToString());
                            Objhome.Traitslst.Add(ObjTraits);
                        }

                        foreach (DataRow drtopchar in ds.Tables[3].Rows)
                        {
                            TopCharactifiers ObjTraits = new TopCharactifiers();
                            ObjTraits.userId = Convert.ToInt32(drtopchar["userid"].ToString());
                            ObjTraits.username = drtopchar["name"].ToString();
                            ObjTraits.FirstName = drtopchar["firstname"].ToString();
                            ObjTraits.LastName = drtopchar["LastName"].ToString();
                            ObjTraits.Score = Convert.ToDecimal(drtopchar["ScoreByuser"].ToString());
                            ObjTraits.userProfilePic = drtopchar["UserProfilePic"].ToString();
                            Objhome.TopCharactifierslst.Add(ObjTraits);
                        }
                        foreach (DataRow dtr in ds.Tables[4].Rows)
                        {
                            ThreeMonths ObjthreeMonth = new ThreeMonths();
                            ObjthreeMonth.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            ObjthreeMonth.Monthname = dtr["monthname"].ToString();
                            Objhome.threeMonthslst.Add(ObjthreeMonth);
                        }
                        foreach (DataRow dtr in ds.Tables[5].Rows)
                        {
                            SixMonths ObjsixMonths = new SixMonths();
                            ObjsixMonths.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            ObjsixMonths.Monthname = dtr["monthname"].ToString();
                            Objhome.sixMonthslst.Add(ObjsixMonths);
                        }
                        foreach (DataRow dtr in ds.Tables[6].Rows)
                        {
                            OneYear ObjoneYear = new OneYear();
                            ObjoneYear.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            ObjoneYear.Monthname = dtr["monthname"].ToString();
                            Objhome.oneYearlst.Add(ObjoneYear);
                        }
                        foreach (DataRow dtr in ds.Tables[7].Rows)
                        {
                            Begnning Objbegnning = new Begnning();
                            Objbegnning.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            Objbegnning.Monthname = dtr["monthname"].ToString();
                            Objhome.begnninglst.Add(Objbegnning);
                        }
                        homelst.Add(Objhome);
                    }
                }
                data = JsonConvert.SerializeObject(homelst);
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = data;
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public string GetScoreWithProfile(ScoreRequest objScore, string currentUserId)
        {
            List<Home> homelst = new List<Home>();
            string data = string.Empty;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetScoreWithProfile";
            objLog.Request = JsonConvert.SerializeObject(objScore).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", objScore.UserID.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@currentUserId", objScore.currentUser.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetScorebyUser_Sp", arrList);
                if (ds.Tables.Count > 0)
                {

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        Home Objhome = new Home();
                        Objhome.CountCharactifiers = Convert.ToInt32(dr["CharCount"].ToString());
                        Objhome.CountPost = Convert.ToInt32(dr["Post"].ToString());
                        Objhome.AverageScoreAllTrait = Convert.ToDecimal(dr["AvgScoreoftraits"].ToString());
                        Objhome.UserProfilePic = dr["UserProfilePic"].ToString();
                        Objhome.showProfile = Convert.ToBoolean(dr["showProfile"]);
                        Objhome.ScorebyCategourylst = new List<ScorebyCategoury>();
                        Objhome.Traitslst = new List<Traits>();
                        Objhome.TopCharactifierslst = new List<TopCharactifiers>();
                        Objhome.threeMonthslst = new List<ThreeMonths>();
                        Objhome.sixMonthslst = new List<SixMonths>();
                        Objhome.oneYearlst = new List<OneYear>();
                        Objhome.begnninglst = new List<Begnning>();
                        foreach (DataRow drCat in ds.Tables[1].Rows)
                        {
                            ScorebyCategoury ObjScorebyCategoury = new ScorebyCategoury();
                            ObjScorebyCategoury.CatId = Convert.ToInt32(drCat["ConfigurationsID"].ToString());
                            ObjScorebyCategoury.CatScore = Convert.ToDecimal(drCat["ScorebyCat"].ToString());
                            Objhome.ScorebyCategourylst.Add(ObjScorebyCategoury);
                        }

                        foreach (DataRow drtrait in ds.Tables[2].Rows)
                        {
                            Traits ObjTraits = new Traits();
                            ObjTraits.TraitsId = Convert.ToInt32(drtrait["TraitsID"].ToString());
                            ObjTraits.TraitsScore = Convert.ToDecimal(drtrait["ScorebyTraits"].ToString());
                            Objhome.Traitslst.Add(ObjTraits);
                        }

                        foreach (DataRow drtopchar in ds.Tables[3].Rows)
                        {
                            TopCharactifiers ObjTraits = new TopCharactifiers();
                            ObjTraits.userId = Convert.ToInt32(drtopchar["userid"].ToString());
                            ObjTraits.username = drtopchar["name"].ToString();
                            ObjTraits.FirstName = drtopchar["firstname"].ToString();
                            ObjTraits.LastName = drtopchar["LastName"].ToString();
                            ObjTraits.Score = Convert.ToDecimal(drtopchar["ScoreByuser"].ToString());
                            ObjTraits.userProfilePic = drtopchar["UserProfilePic"].ToString();
                            Objhome.TopCharactifierslst.Add(ObjTraits);
                        }
                        foreach (DataRow dtr in ds.Tables[4].Rows)
                        {
                            ThreeMonths ObjthreeMonth = new ThreeMonths();
                            ObjthreeMonth.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            ObjthreeMonth.Monthname = dtr["monthname"].ToString();
                            Objhome.threeMonthslst.Add(ObjthreeMonth);
                        }
                        foreach (DataRow dtr in ds.Tables[5].Rows)
                        {
                            SixMonths ObjsixMonths = new SixMonths();
                            ObjsixMonths.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            ObjsixMonths.Monthname = dtr["monthname"].ToString();
                            Objhome.sixMonthslst.Add(ObjsixMonths);
                        }
                        foreach (DataRow dtr in ds.Tables[6].Rows)
                        {
                            OneYear ObjoneYear = new OneYear();
                            ObjoneYear.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            ObjoneYear.Monthname = dtr["monthname"].ToString();
                            Objhome.oneYearlst.Add(ObjoneYear);
                        }
                        foreach (DataRow dtr in ds.Tables[7].Rows)
                        {
                            Begnning Objbegnning = new Begnning();
                            Objbegnning.value = Convert.ToDecimal(dtr["AvgScore"].ToString());
                            Objbegnning.Monthname = dtr["monthname"].ToString();
                            Objhome.begnninglst.Add(Objbegnning);
                        }
                        homelst.Add(Objhome);
                    }
                }
                data = JsonConvert.SerializeObject(homelst);
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = data;
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public string GetSelfScore(int UserID, string currentUserId)
        {
            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetSelfScore";
            objLog.Request = JsonConvert.SerializeObject(UserID).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", UserID.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetSelfScore_Sp", arrList);
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "SelfScore";
                    ds.Tables[1].TableName = "Traits";
                }
                data = JsonConvert.SerializeObject(ds);
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = data;
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public string UploadProfilePic(int USerID, String UserProfilePic, string currentUserId)
        {
            int ret = 0;
            string Data = null;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "UploadProfilePic";
            objLog.Request = JsonConvert.SerializeObject(USerID).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    var userMaster = (from usr in db.UserMaster where (usr.UserId == USerID) select usr).FirstOrDefault();
                    if (userMaster != null)
                    {
                        if (UserProfilePic.Contains("https://"))
                        {
                            userMaster.UserProfilePic = UserProfilePic;
                        }
                        else
                        {
                            userMaster.UserProfilePic = Base64ToImage(UserProfilePic, USerID);
                        }
                        db.UserMaster.Update(userMaster);
                        ret = db.SaveChanges();
                        Data = userMaster.UserProfilePic;
                    }
                    else
                    {
                        ret = -1; //user not found 
                    }
                }
            }
            catch (Exception e)
            {
                ret = -1;
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return Data;
        }

        public int AddRequest(ScoreRequest objScore, string currentUserId)
        {
            int ret = 0;
            int RequestID, removeCount = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "AddRequest";
            objLog.Request = JsonConvert.SerializeObject(objScore).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            using (var db = new CContext())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {

                        var AddReq = new DataModels.ConnectionMaster();
                        {
                            var ud = (from fm in db.ConnectionMaster where ((fm.FromUserId == objScore.FromUserID) && (fm.ToUserId == objScore.ToUserID)) || ((fm.FromUserId == objScore.ToUserID) && (fm.ToUserId == objScore.FromUserID)) select fm).FirstOrDefault();
                            if (ud != null)
                            {
                                AddReq.Status = "Remove";
                                db.ConnectionMaster.Update(ud);
                                ret = db.SaveChanges();
                                // dbContextTransaction.Commit();
                                //if (ret >0)
                                //{
                                //    string connStr = CResources.GetConnectionString();
                                //    ArrayList arrList = new ArrayList();
                                //    // SP.spArgumentsCollection(arrList, "@ret", "0", "INT", "O");
                                //    SP.spArgumentsCollection(arrList, "@RequestID", ud.ConnectionId.ToString(), "INT", "I");
                                //    SP.spArgumentsCollection(arrList, "@RequestType", "AR", "VARCHAR", "I");
                                //    SP.spArgumentsCollection(arrList, "@Status", "Remove", "VARCHAR", "I");
                                //    SP.spArgumentsCollection(arrList, "@userId", ud.FromUserId.ToString(), "INT", "I");

                                //    DataSet ds = new DataSet();
                                //    DataTable dt = new DataTable();
                                //    ds = SP.RunStoredProcedure(connStr, ds, "ApproveRequest_SP", arrList);
                                //}
                            }

                            AddReq.FromUserId = objScore.FromUserID;
                            AddReq.ToUserId = objScore.ToUserID;
                            AddReq.Status = "Pending";
                            AddReq.ConnectedDate = Currentdatetime;
                            AddReq.CreatedBy = objScore.FromUserID;
                            AddReq.CreatedDate = Currentdatetime;
                        }

                        db.ConnectionMaster.Add(AddReq);
                        ret = db.SaveChanges();
                        ret = AddReq.ConnectionId;
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                            addNotification(objScore.FromUserID, objScore.ToUserID, "AddRequest", ret, 0);
                            ret = 1;
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            ret = -1;
                        }

                    }
                    catch (Exception ex)
                    {
                        ret = -1;
                        dbContextTransaction.Rollback();
                        objLog.Response = ex.ToString();
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        mn.LogError(ex.ToString());
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var resul = RequestLog1(objLog);
                            });
                        }
                        throw ex;
                    }
                    finally
                    {
                        objLog.Response = ret.ToString();
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var resul = RequestLog1(objLog);
                            });
                        }
                    }
                }
            }

            return ret;
        }

        public string GetConnections(Connections obj, string currentUserId)
        {
            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetConnections";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", obj.UserID.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageNo", obj.pageNo.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageSize", obj.pageSize.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetConnections_SP", arrList);
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "Connections";
                }
                data = JsonConvert.SerializeObject(ds);
                //// usr = JsonConvert.DeserializeObject<Scores>(data, new JsonSerializerSettings
                // {
                //     NullValueHandling = NullValueHandling.Ignore
                // });
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public int RatingApprove(ScoreRequest objScore)
        {
            int ret = 0;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "RatingApprove";
            objLog.Request = JsonConvert.SerializeObject(objScore).ToString();
            RequestID = RequestLog(objLog);
            using (var db = new CContext())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {


                        //  db.ConnectionMaster.Add(AddReq);
                        ret = db.SaveChanges();
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                            ret = 1;
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            ret = -1;
                        }

                    }

                    catch (Exception e)
                    {
                        ret = -1;
                        dbContextTransaction.Rollback();
                        objLog.Response = e.ToString();
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        mn.LogError(e.ToString());
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var resul = RequestLog1(objLog);
                            });
                        }
                        throw e;
                    }
                    finally
                    {
                        objLog.Response = ret.ToString();
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var resul = RequestLog1(objLog);
                            });
                        }
                    }
                }
            }

            return ret;
        }

        public string GetConnectionRating(int CRID, string currentUserId)
        {
            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetConnectionRating";
            objLog.Request = JsonConvert.SerializeObject(CRID).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@CRID", CRID.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetConnRating_Sp", arrList);
                if (ds.Tables.Count > 1)
                {
                    ds.Tables[0].TableName = "Category";
                    ds.Tables[1].TableName = "Traits";
                }
                else
                {

                }
                data = JsonConvert.SerializeObject(ds);
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;

        }

        public string FilterEmail(FilterEmailRequest Objreq, string currentUserId)
        {
            string data = string.Empty;
            List<EmailStatus> obj = new List<EmailStatus>();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "FilterEmail";
            objLog.Request = JsonConvert.SerializeObject(Objreq).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    foreach (string EmailID in Objreq.EmailID)
                    {
                        EmailStatus ObjEmailList = new EmailStatus();
                        var userMaster = (from usr in db.UserMaster where (usr.EmailId == EmailID) && (usr.Status == "A") select usr).FirstOrDefault();
                        if (userMaster != null)
                        {
                            int UserID = userMaster.UserId;
                            var connmaster = (from usr in db.ConnectionMaster where (usr.FromUserId == Objreq.UserID) && (usr.ToUserId == UserID) select usr).FirstOrDefault();
                            if (connmaster == null)
                            {

                                var checkEmailStatus = (from usr in db.UserMaster where (usr.EmailId == EmailID) && (usr.Status == "A") select usr).FirstOrDefault();

                                ObjEmailList.EmailID = EmailID;
                                if (checkEmailStatus != null)
                                {
                                    ObjEmailList.Status = true;
                                }
                                else
                                {
                                    ObjEmailList.Status = false;
                                }

                            }
                            else
                            {
                                continue;
                            }

                        }
                        else
                        {

                            ObjEmailList.EmailID = EmailID;
                            ObjEmailList.Status = false;
                        }
                        obj.Add(ObjEmailList);
                    }

                    data = JsonConvert.SerializeObject(obj);
                }
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;


        }

        public string MyNotifications(Connections Obj, string currentUserId)
        {
            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "MyNotifications";
            objLog.Request = JsonConvert.SerializeObject(Obj).ToString();
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", Obj.UserID.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageNo", Obj.pageNo.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageSize", Obj.pageSize.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "MyNotification_SP", arrList);
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "ConnectionNotification";
                }
                else
                {

                }
                data = JsonConvert.SerializeObject(ds);
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;

        }

        public int ApproveRequest(ScoreRequest objScore, string currentUserId)
        {
            string data = null;
            int ret = 0;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "ApproveRequest";
            objLog.Request = JsonConvert.SerializeObject(objScore).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            using (var db = new CContext())
            {
                //using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        string connStr = CResources.GetConnectionString();
                        ArrayList arrList = new ArrayList();
                        // SP.spArgumentsCollection(arrList, "@ret", "0", "INT", "O");
                        SP.spArgumentsCollection(arrList, "@RequestID", objScore.RequestId.ToString(), "INT", "I");
                        SP.spArgumentsCollection(arrList, "@RequestType", objScore.RequestType.ToString(), "VARCHAR", "I");
                        SP.spArgumentsCollection(arrList, "@Status", objScore.Status.ToString(), "VARCHAR", "I");
                        SP.spArgumentsCollection(arrList, "@userId", objScore.UserID.ToString(), "INT", "I");

                        DataSet ds = new DataSet();
                        DataTable dt = new DataTable();
                        ds = SP.RunStoredProcedure(connStr, ds, "ApproveRequest_SP", arrList);

                        if (ds.Tables.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                Usercomments userComments = new Usercomments();
                                string[] userDeviceId = dr["token"].ToString().Split(",");
                                SendPushNotification(userDeviceId[0].ToString(), userDeviceId[1].ToString());
                            }
                            ds.Tables[0].TableName = "ApproveRequest";
                        }
                        data = JsonConvert.SerializeObject(ds);

                        ret = 1;


                    }
                    catch (Exception ex)
                    {
                        ret = -1;
                        objLog.Response = ex.ToString();
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        mn.LogError(ex.ToString());
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var resul = RequestLog1(objLog);
                            });
                        }
                        throw ex;
                    }
                    finally
                    {
                        objLog.Response = ret.ToString();
                        objLog.LogId = RequestID;
                        ResponseLog(objLog);
                        if (Apilog == true)
                        {
                            Task.Run(() =>
                            {
                                var resul = RequestLog1(objLog);
                            });
                        }
                    }
                }
            }

            return ret;
        }

        public string MyEmails(int CompantID)
        {
            string data = string.Empty;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "ApproveRequest";
            objLog.Request = JsonConvert.SerializeObject(CompantID).ToString();
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@CompantID", CompantID.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "getemails_sp", arrList);
                if (ds.Tables.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string Password = ds.Tables[0].Rows[i]["Password"].ToString();

                        foreach (DataRow row in ds.Tables[0].Select("Password = '" + ds.Tables[0].Rows[i]["Password"].ToString() + "'"))
                        {
                            Manager mgr = new Manager();
                            // Password = "P94A1vFvS5A4vBBj1oRNFA==";
                            Password = "12345";
                            // Password = "tradepay";
                            Password = GetMD5Dcryption(Password);
                            // Password = Manager.Decrypt(Password);

                            row[i] = Password;
                        }

                        ds.Tables[0].AcceptChanges();
                    }
                }
                else
                {

                }
                data = JsonConvert.SerializeObject(ds);
            }
            catch (Exception ex)
            {
                data = "-1";
                objLog.Response = ex.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(ex.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw ex;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;

        }

        public string GetMD5Dcryption(string val)
        {
            StringBuilder sb = new StringBuilder();
            MD5CryptoServiceProvider md5obj = new MD5CryptoServiceProvider();
            md5obj.ComputeHash(ASCIIEncoding.ASCII.GetBytes(val));
            byte[] result = md5obj.Hash;
            for (int i = 0; i < val.Length; i++)
            {
                sb.Append(result[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public int AddFeed(FeedRequest objaf, string currentUserId)
        {
            int feedid = 0;
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "AddFeed";
            objLog.Request = JsonConvert.SerializeObject(objaf).ToString();

            objLog.currentUserId = currentUserId;
            //dynamic res =null;
            //RequestID = Convert.ToInt32(res);
            //RequestID = Task.Run(ResponseLog1(objLog));
            try
            {

                // 
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {

                        var ud = new Models.FeedMaster()
                        {
                            FromUserId = objaf.FromUserID,
                            ToUserId = objaf.ToUserID,
                            FeedType = objaf.FeedType,
                            FileType = objaf.FileType,
                            Description = objaf.Description,
                            CreatedDate = Currentdatetime,
                            CreatedBy = objaf.FromUserID
                        };
                        db.FeedMaster.Add(ud);
                        db.SaveChanges();
                        feedid = ud.FeedId;
                        if (objaf.FileType == "text")
                        {
                            ret = ud.FeedId;
                        }
                        foreach (var uwdr in objaf.feedImagePathslst)
                        {
                            string filename = null;
                            if (uwdr.FileType == "video")
                            {
                                //  mn.LogError(uwdr.filePath);
                                // filename = ConvertToVideo(uwdr.filePath, objaf.FromUserID, feedid, uwdr.Fileformat);
                                filename = ConvertToVideoFornode(uwdr.filePath, objaf.FromUserID, feedid, uwdr.Fileformat);

                            }
                            else
                            {
                                filename = Base64ToImage1(uwdr.filePath, objaf.FromUserID, feedid, uwdr.Fileformat);
                            }

                            var u = new Models.FeedImagePath()
                            {
                                FeedId = feedid,
                                ImagePath = @"\Upload\" + filename,
                                Filter = uwdr.Filter,
                                Description = uwdr.Description,
                                CreatedDate = Currentdatetime,
                                CreatedBy = objaf.FromUserID
                            };
                            db.FeedImagePath.Add(u);
                            ret = db.SaveChanges();
                        }

                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                            if (objaf.FeedType != "Story")
                            {
                                foreach (var uwdr in objaf.taggingslst)
                                {
                                    var ut = new Models.Tagging()
                                    {
                                        FeedId = feedid,
                                        UserId = uwdr.Touserid,
                                        //Description = uwdr.Description,
                                        CreatedDate = Currentdatetime,
                                        CreatedBy = uwdr.userid.ToString()
                                    };
                                    db.Tagging.Add(ut);
                                    ret = db.SaveChanges();
                                    addNotification(uwdr.userid, uwdr.Touserid, "Tagging", feedid, objaf.FeedID);
                                }
                            }
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ret = -1;
                objLog.Response = ex.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(ex.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var res = RequestLog1(objLog);
                    });
                }
                throw ex;
            }
            finally
            {

                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                //ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var res = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public int UpdateFeed(FeedRequest objaf, string currentUserId)
        {
            int feedid = 0;
            int ret = -1;
            //int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "UpdateFeed";
            objLog.Request = JsonConvert.SerializeObject(objaf).ToString();
            objLog.currentUserId = currentUserId;
            //RequestID = RequestLog(objLog);
            try
            {

                // 
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        //  var userMaster = (from usr in db.FeedMaster where (usr.FeedId == objaf.FeedID) select usr).FirstOrDefault();
                        var ud = (from fm in db.FeedMaster where (fm.FeedId == objaf.FeedID) select fm).FirstOrDefault();
                        {
                            ud.FeedId = objaf.FeedID;
                            ud.Description = objaf.Description;
                            ud.ModifiedDate = Currentdatetime;
                            ud.ModifiedBy = objaf.FromUserID;
                            db.FeedMaster.Update(ud);
                            ret = db.SaveChanges();
                        }
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ret = -1;
                objLog.Response = ex.ToString();
                //mgr.LogFileWrite(ex);
                //  ret = TPResources.DBEXCEPTIONRETVALUE;
                mn.LogError(ex.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw ex;
            }
            finally
            {
                objLog.Response = ret.ToString();
                // objLog.LogId = RequestID;
                //ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public int DeleteFeed(FeedRequest objaf, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "DeleteFeed";
            objLog.Request = JsonConvert.SerializeObject(objaf).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {

                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        //users.usereducationdetails
                        var Log = (from usr in db.FeedMaster where (usr.FeedId == objaf.FeedID) select usr).FirstOrDefault();
                        if (Log != null)
                        {
                            Log.IsDelete = objaf.IsDelete;
                            Log.ModifiedBy = objaf.FromUserID;
                            Log.ModifiedDate = Currentdatetime;
                            db.FeedMaster.Update(Log);
                            ret = db.SaveChanges();
                        }
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }

                    }
                }
            }
            catch (Exception e)
            {
                ret = -1;
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;

        }

        public int StoryFeed(StoryRequest objaf, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "StoryFeed";
            objLog.Request = JsonConvert.SerializeObject(objaf).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        var u = new Models.StoryMaster()
                        {
                            FromUserId = objaf.FromUserID,
                            ToUserId = objaf.ToUserID,
                            StoryType = objaf.StoryType,
                            Description = objaf.Description,
                            FileType = objaf.FileType,
                            FilePath = objaf.FilePath,
                            CreatedDate = Currentdatetime,
                            CreatedBy = objaf.FromUserID
                        };
                        db.StoryMaster.Add(u);
                        ret = db.SaveChanges();

                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ret = -1;
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public string GetProfileDeatils(int UserID, string currentUserId)
        {
            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetProfileDeatils";
            objLog.Request = JsonConvert.SerializeObject(UserID).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", UserID.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "getuserProfile_Sp", arrList);
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "User";
                    ds.Tables[1].TableName = "UserEducationDetails";
                    ds.Tables[2].TableName = "UserWorkDetails";
                }
                data = JsonConvert.SerializeObject(ds);
                //// usr = JsonConvert.DeserializeObject<Scores>(data, new JsonSerializerSettings
                // {
                //     NullValueHandling = NullValueHandling.Ignore
                // });
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public string AddReactions(FeedReactionRequest objaf, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "AddReactions";
            objLog.Request = JsonConvert.SerializeObject(objaf).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            string data = string.Empty;
            try
            {

                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {

                        var feedreactions = (from Fr in db.FeedReactions where (Fr.FeedId == objaf.FeedID && Fr.CreatedBy == objaf.UserID && Fr.ReactionTypeId != 65) select Fr).FirstOrDefault();

                        if (feedreactions != null && Convert.ToInt32(objaf.ReactionType) != 65)
                        {
                            feedreactions.ReactionTypeId = Convert.ToInt32(objaf.ReactionType);
                            feedreactions.Description = objaf.Description;
                            feedreactions.ModifiedBy = objaf.UserID;
                            feedreactions.ModifiedDate = Currentdatetime;
                            db.FeedReactions.Update(feedreactions);
                            db.SaveChanges();
                            ret = feedreactions.ReactionId;
                        }

                        else
                        {
                            if (((objaf.Description != null) && Convert.ToInt32(objaf.ReactionType) == 65) || ((objaf.Description == null) && Convert.ToInt32(objaf.ReactionType) != 65))
                            {
                                var u = new Models.FeedReactions()
                                {
                                    FeedId = objaf.FeedID,
                                    ReactionTypeId = Convert.ToInt32(objaf.ReactionType),
                                    Description = objaf.Description,
                                    CreatedDate = Currentdatetime,
                                    CreatedBy = objaf.UserID

                                };
                                db.FeedReactions.Add(u);
                            }
                            ret = db.SaveChanges();
                        }
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                            GetFeedList Obj = new GetFeedList();
                            Obj.UserId = objaf.UserID;
                            Obj.FeedId = objaf.FeedID;
                            data = GetSpecificFeedResponse(Obj, currentUserId);

                            //string connStr = CResources.GetConnectionString();
                            //ArrayList arrList = new ArrayList();
                            //SP.spArgumentsCollection(arrList, "@ret", "0", "INT", "O");
                            //SP.spArgumentsCollection(arrList, "@FromUserID", objaf.UserID.ToString(), "INT", "I");
                            //SP.spArgumentsCollection(arrList, "@feedid", objaf.FeedID.ToString(), "INT", "I");
                            //SP.spArgumentsCollection(arrList, "@CommandName", "Reaction", "VARCHAR", "I");
                            //DataSet ds = new DataSet();
                            //SP.RunStoredProcedureRet(connStr, "Sp_addNotification", arrList);
                            addNotification(objaf.UserID, 0, "Reaction", Convert.ToInt32(objaf.ReactionType), objaf.FeedID);

                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;


        }

        public string UpdateComments(FeedReactionRequest objaf, string currentUserId)
        {
            string data = string.Empty;
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "UpdateComments";
            objLog.Request = JsonConvert.SerializeObject(objaf).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {

                        var feedreactions = (from Fr in db.FeedReactions where (Fr.ReactionId == objaf.ReactionID) select Fr).FirstOrDefault();
                        if (feedreactions != null)
                        {
                            feedreactions.ReactionTypeId = Convert.ToInt32(objaf.ReactionType);
                            feedreactions.Description = objaf.Description;
                            feedreactions.ModifiedBy = objaf.UserID;
                            feedreactions.ModifiedDate = Currentdatetime;
                            db.FeedReactions.Update(feedreactions);
                            db.SaveChanges();
                            ret = feedreactions.ReactionId;
                            dbContextTransaction.Commit();
                            GetFeedList Obj = new GetFeedList();
                            Obj.UserId = objaf.UserID;
                            Obj.FeedId = objaf.FeedID;
                            data = GetSpecificFeedResponse(Obj, currentUserId);
                        }
                        else
                        {
                            ret = -1; //user not found 
                        }
                    }
                }
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }

            }
            return data;
        }

        public string DeleteComments(FeedReactionRequest objaf, string currentUserId)
        {
            string data = string.Empty;
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "DeleteComments";
            objLog.Request = JsonConvert.SerializeObject(objaf).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {

                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        var feedreactions = (from Fr in db.FeedReactions where (Fr.ReactionId == objaf.ReactionID) select Fr).FirstOrDefault();
                        if (feedreactions != null)
                        {

                            feedreactions.IsDelete = true;
                            feedreactions.ModifiedBy = objaf.UserID;
                            feedreactions.ModifiedDate = Currentdatetime;
                            db.FeedReactions.Update(feedreactions);
                            // db.FeedReactions.Remove(feedreactions);
                            db.SaveChanges();
                            ret = feedreactions.ReactionId;
                            dbContextTransaction.Commit();
                            GetFeedList Obj = new GetFeedList();
                            Obj.UserId = objaf.UserID;
                            Obj.FeedId = objaf.FeedID;
                            data = GetSpecificFeedResponse(Obj, currentUserId);
                        }
                        else
                        {
                            ret = -1; //user not found 
                        }
                    }
                }
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }

            }
            return data;
        }

        public string GetFeedList(GetFeedList obj, string currentUserId)
        {
            List<GetFeedRections> getFeedReactions = new List<GetFeedRections>();
            List<UserRectionsType> getUserRectionsType = new List<UserRectionsType>();

            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetFeedList";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserId", obj.UserId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageNo", obj.pageNo.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageSize", obj.pageSize.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Story", obj.Story.ToString(), "VARCHAR", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "getFeedWithReactions_SP", arrList);
                //if (ds.Tables.Count > 0)
                //{
                //    ds.Tables[0].TableName = "Feed";
                //    ds.Tables[1].TableName = "FeedReactions";
                //}
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    GetFeedRections getFeedReaction = new GetFeedRections();
                    getFeedReaction.UserID = Convert.ToInt32(dr["createdby"].ToString());
                    getFeedReaction.feedID = Convert.ToInt32(dr["feedid"].ToString());
                    getFeedReaction.feedType = dr["feedtype"].ToString();
                    getFeedReaction.Name = dr["Name"].ToString();
                    getFeedReaction.UserProfilePic = dr["ProfilePic"].ToString();
                    getFeedReaction.description = dr["Description"].ToString();
                    getFeedReaction.filePath = dr["FilePath"].ToString();
                    getFeedReaction.createdDate = dr["CreatedDate"].ToString();
                    getFeedReaction.noRections = Convert.ToInt32(dr["React_count"].ToString());
                    getFeedReaction.noComments = Convert.ToInt32(dr["Comments_count"].ToString());
                    getFeedReaction.noShares = Convert.ToInt32(dr["Share_count"].ToString());
                    getFeedReaction.FirstName = dr["firstname"].ToString();
                    getFeedReaction.LastName = dr["LastName"].ToString();
                    DataRow[] dataRowsCurReactions = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + "  and CreatedBy='" + obj.UserId.ToString() + "'  and Name <>'CM'  ");
                    foreach (DataRow drReaction in dataRowsCurReactions)
                    {
                        getFeedReaction.currentUserRection = drReaction["Name"].ToString();
                    }
                    getFeedReaction.userRectionsTypeLst = new List<UserRectionsType>();
                    getFeedReaction.filePathlst = new List<filePath>();

                    if (getFeedReaction.feedType == "Charactify")
                    {
                        getFeedReaction.charactifyScores = new List<CharactifyScore>();

                        DataRow[] dataRorschar = ds.Tables[4].Select("Crid=" + dr["Crid"].ToString());
                        foreach (DataRow dataRorsch in dataRorschar)
                        {
                            CharactifyScore charactifyScore = new CharactifyScore();
                            charactifyScore.Score = dataRorsch["Score"].ToString();
                            charactifyScore.TraitsID = dataRorsch["TraitsID"].ToString();
                            getFeedReaction.charactifyScores.Add(charactifyScore);
                        }
                    }
                    else
                    {
                        DataRow[] dataRorspath = ds.Tables[2].Select("feedid=" + dr["feedid"].ToString() + " and feedtype<>'Charactify'");
                        //DataRow[] dataRorspath = ds.Tables[2].Select("feedid=" + dr["feedid"].ToString());
                        foreach (DataRow dataRorspa in dataRorspath)
                        {
                            filePath filepath = new filePath();
                            filepath.filter = dataRorspa["Filter"].ToString();
                            filepath.Description = dataRorspa["Description"].ToString();
                            // filepath.Path = URL + dataRorspa["ImagePath"].ToString();
                            if (getFeedReaction.feedType == "video")
                            {
                                string myString = dataRorspa["ImagePath"].ToString();
                                myString = myString.Remove(0, 1);
                                filepath.Path = FullUrl + myString;
                                string imgurl = myString.Replace(@".MP4", @".jpg");
                                filepath.Thumbnailurl = @"C:\NITIN\1\1\" + imgurl.ToString();

                                if (File.Exists(filepath.Thumbnailurl))
                                {
                                    filepath.Thumbnailurl = URL + "/" + imgurl.ToString();
                                }
                                else
                                {
                                    // \Upload\2512_3903_c20ae34d-52cd-4cdf-b23d-902c84b23562.jpeg
                                    filepath.Thumbnailurl = URL + "/Upload/Thumbnail.jpg";
                                }

                            }
                            else
                            {
                                filepath.Path = URL + dataRorspa["ImagePath"].ToString();
                            }

                            getFeedReaction.filePathlst.Add(filepath);

                        }
                    }
                    //DataRow[] data1 = ds.Tables[3].Select(y => y.feedid = dr["feedid"].ToString());
                    DataRow[] dataRorsRec = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + " and Name<>'CM' ");
                    var x = (from r in dataRorsRec.AsEnumerable()
                             select new
                             {
                                 feedid = r.Field<int>("feedid"),
                                 Name = r.Field<string>("Name")
                             }).Distinct().ToList();
                    foreach (var i in x)
                    {
                        UserRectionsType userRectionsType = new UserRectionsType();

                        userRectionsType.ReactionType = i.Name.ToString();

                        userRectionsType.userRectionsLst = new List<UserRections>();

                        DataRow[] dataRowsReactions = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + "  and Name='" + i.Name.ToString() + "'  and Name <>'CM'  ");
                        foreach (DataRow drReaction in dataRowsReactions)
                        {
                            UserRections userRections = new UserRections();
                            userRections.ReactionType = drReaction["Name"].ToString();
                            userRections.UserID = Convert.ToInt32(drReaction["CreatedBy"].ToString());
                            userRections.ReactionID = Convert.ToInt32(drReaction["ReactionID"].ToString());
                            userRections.feedID = Convert.ToInt32(drReaction["feedid"].ToString());
                            userRections.Name = drReaction["UName"].ToString();
                            userRections.UserProfilePic = drReaction["ProfilePic"].ToString();
                            userRections.FirstName = drReaction["FirstName"].ToString();
                            userRections.LastName = drReaction["LastName"].ToString();

                            userRectionsType.userRectionsLst.Add(userRections);
                        }
                        //getFeedReaction.userRectionsLst.add
                        getFeedReaction.userRectionsTypeLst.Add(userRectionsType);
                    }
                    getFeedReaction.usercommentLst = new List<Usercomments>();
                    DataRow[] dataRowsComment = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + " and Name='CM' ");
                    foreach (DataRow drReaction in dataRowsComment)
                    {
                        Usercomments userComments = new Usercomments();
                        userComments.UserID = Convert.ToInt32(drReaction["createdby"].ToString());
                        userComments.ReactionID = Convert.ToInt32(drReaction["ReactionID"].ToString());
                        userComments.feedID = Convert.ToInt32(drReaction["feedid"].ToString());
                        userComments.Name = drReaction["UName"].ToString();
                        userComments.ReactionType = drReaction["Name"].ToString();
                        userComments.Description = drReaction["description"].ToString();
                        userComments.UserProfilePic = drReaction["ProfilePic"].ToString();
                        userComments.createdDate = drReaction["CreatedDate"].ToString();
                        getFeedReaction.usercommentLst.Add(userComments);
                    }
                    getFeedReaction.taggingslst = new List<tagging>();
                    DataRow[] dataRowstagging = ds.Tables[5].Select("feedid=" + dr["feedid"].ToString());
                    foreach (DataRow drTagging in dataRowstagging)
                    {
                        tagging objtagging = new tagging();
                        objtagging.userid = Convert.ToInt32(dr["createdby"].ToString());
                        objtagging.Touserid = Convert.ToInt32(drTagging["toUserId"].ToString());
                        objtagging.FirstName = drTagging["FirstName"].ToString();
                        objtagging.LastName = drTagging["LastName"].ToString();
                        getFeedReaction.taggingslst.Add(objtagging);
                    }

                    getFeedReaction.takenScorelst = new List<takenScore>();
                    DataRow[] dataRowstakenScore = ds.Tables[6].Select("feedid=" + dr["feedid"].ToString());
                    foreach (DataRow drTagging in dataRowstakenScore)
                    {
                        takenScore objtakenby = new takenScore();
                        objtakenby.userid = Convert.ToInt32(drTagging["UserID"].ToString());
                        objtakenby.FirstName = drTagging["FirstName"].ToString();
                        objtakenby.LastName = drTagging["LastName"].ToString();
                        objtakenby.UserProfilePic = drTagging["UserProfilePic"].ToString();
                        getFeedReaction.takenScorelst.Add(objtakenby);
                    }


                    getFeedReactions.Add(getFeedReaction);
                }



                data = JsonConvert.SerializeObject(getFeedReactions);
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = "Success";
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }



        //    public string GetFeedList(int UserID)
        //    {
        //        string data = string.Empty;
        //        //  Scores usr = new Scores();
        //        try
        //        {

        //            using (var db = new TPContext())
        //            {
        //                var dataset = db.FeedReactions
        //.Where(x => x.ReactionId == UserID).Select(x => new { x.ReactionType, x.Description, x.ModifiedBy }).ToList();
        //                //var feedreactions = (from Fr in db.FeedReactions where (Fr.ReactionId == UserID) select(Fr => new { Fr. })).FirstOrDefault();
        //                if (dataset != null)
        //                {

        //                    //data = dataset.ToString();
        //                    data = JsonConvert.SerializeObject(dataset);
        //                }
        //                else
        //                {
        //                   // data = -1; //user not found 
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //        return data;
        //    }

        public int RequestLog(LogRequest ObjLog)
        {
            int ret = 0;
            //using (var db = new CContext())
            //{
            //    using (var dbContextTransaction = db.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            var Log = new DataModels.ApiLogs();
            //            {
            //                Log.MethodName = ObjLog.MethodName;
            //                Log.Request = ObjLog.Request;
            //                Log.CreatedDate = DateTime.UtcNow;
            //            }

            //            db.ApiLogs.Add(Log);
            //            db.SaveChanges();
            //            ret = Log.Id;
            //            if (ret > 0)
            //            {
            //                dbContextTransaction.Commit();
            //                //ret = 1;
            //            }
            //            else
            //            {
            //                dbContextTransaction.Rollback();
            //                // ret = -1;
            //            }

            //        }
            //        catch (Exception ex)
            //        {
            //            dbContextTransaction.Rollback();
            //            throw ex;
            //        }
            //    }
            //}
            return ret;
        }

        public async Task<int> RequestLog1(LogRequest ObjLog)
        {
            int ret = 0;

            using (var db = new CContext())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Log = new DataModels.ApiLogs();
                        {
                            Log.MethodName = ObjLog.MethodName;
                            Log.Request = ObjLog.Request;
                            Log.Response = ObjLog.Response;
                            Log.ModifiedDate = DateTime.UtcNow;
                            Log.CreatedDate = Currentdatetime;
                            Log.CreatedBy = Convert.ToString(ObjLog.currentUserId);
                        }

                        db.ApiLogs.Add(Log);
                        db.SaveChanges();
                        ret = Log.Id;
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                            //ret = 1;
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            // ret = -1;
                        }

                    }
                    catch (Exception ex)
                    {
                        mn.LogError(ex.ToString());
                        dbContextTransaction.Rollback();
                        throw ex;
                    }
                }
            }
            return ret;
        }

        public int ResponseLog(LogRequest ObjLog)
        {
            int ret = 0;
            //using (var db = new CContext())
            //{
            //    using (var dbContextTransaction = db.Database.BeginTransaction())
            //    {
            //        try
            //        {
            //            var Log = (from usr in db.ApiLogs where (usr.Id == ObjLog.LogId) select usr).FirstOrDefault();
            //            if (Log != null)
            //            {
            //                Log.Response = ObjLog.Response;
            //                Log.ModifiedDate = DateTime.UtcNow;
            //                db.ApiLogs.Update(Log);
            //                ret = db.SaveChanges();
            //            }
            //            if (ret > 0)
            //            {
            //                dbContextTransaction.Commit();
            //                ret = 1;
            //            }
            //            else
            //            {
            //                dbContextTransaction.Rollback();
            //                ret = -1;
            //            }

            //        }
            //        catch (Exception ex)
            //        {
            //            dbContextTransaction.Rollback();
            //            throw ex;
            //        }
            //    }
            //}
            return ret;
        }

        /// <summary>
        ///   For base64String
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>

        public string ConvertToVideo(string data, int userid, int Feedid, string Fileformat)
        {
            if (Fileformat == null || Fileformat == "" || Fileformat == ".3gp")
            {
                Fileformat = ".mp4";
            }
            string filename = null;
            Guid obj = Guid.NewGuid();
            string path = Directory.GetCurrentDirectory();
            baseDir = path + "\\Upload\\";
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }
            byte[] Bytes = Convert.FromBase64String(data);
            string date = DateTime.Now.ToString().Replace(@"/", @"_").Replace(@":", @"_").Replace(@" ", @"_");
            // FileInfo fil = new FileInfo(baseDir + Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + obj.ToString() + Fileformat);
            FileInfo fil = new FileInfo(baseDir + Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + date.ToString() + Fileformat);
            // ret= Compress(ret);
            using (Stream sw = fil.OpenWrite())
            {
                sw.Write(Bytes, 0, Bytes.Length);
                sw.Close();
            }
            // var bytes = Convert.FromBase64String(base64encodedstring);
            //var contents = new MemoryStream(ret);
            //StreamReader sw = null;
            //using (sw = fil.OpenWrite())
            //{
            //    sw.Read(ret, 0, ret.Length);
            //    sw.Close();
            //}
            //GZipStream myStreamZip = new GZipStream(contents, CompressionMode.Compress);
            //myStreamZip.Write(ret, 0, ret.Length);
            //myStreamZip.Close();
            //  string pathToVideoFile = @"C:\Ravi\CharactifyAPI\Charactify.API\Upload\2274_959_ec6a749e-a45b-4f73-872a-8e4c396c625d.MP4";
            //var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            //ffMpeg.ConvertMedia(pathToVideoFile, "video.mp4", Format.mp4);            
            //filename = Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + obj.ToString() + Fileformat;
            filename = Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + date.ToString() + Fileformat;
            return filename;
        }

        public async Task ConvertMp4Video(string Url)
        {
            // HttpResponseMessage response = await client.PutAsJsonAsync(
            //    $"api/products/{Url}", Url);
            //response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            //await response.Content.ReadAsAsync();
            string newUrl = "http://localhost/mapi/values/Get?Url=" + Url;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(newUrl);

                //HTTP POST
                //  var postTask = client.PostAsJsonAsync("value", Url);
                var postTask = client.GetAsync("");
                postTask.Wait();

                var result = postTask.Result;
                mn.LogError(result.ToString());
            }

        }

        //public string  ConvertMp4Videonew(string Url)
        //{
        //    // HttpResponseMessage response = await client.PumtAsJsonAsync(
        //    //    $"api/products/{Url}", Url);
        //    //response.EnsureSuccessStatusCode();

        //    // Deserialize the updated product from the response body.
        //    //await response.Content.ReadAsAsync();
        //    string newUrl = "http://localhost/mapi/values/Get?Url=" + Url;
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(newUrl);

        //        //HTTP POST
        //        //  var postTask = client.PostAsJsonAsync("value", Url);
        //        var postTask = client.GetAsync("");
        //        postTask.Wait();

        //        var result = postTask.Result;
        //        mn.LogError(result.ToString());
        //    }

        //}


        public string ConvertToVideoFornode(string data, int userid, int Feedid, string Fileformat)
        {
            string filename = null;
            Guid obj = Guid.NewGuid();
            string path = Directory.GetCurrentDirectory();
            //if (Fileformat == null || Fileformat == "" || Fileformat == ".3gp")
            {
                Fileformat = ".MP4";
            }

            //if (Fileformat == ".3gp")
            //{

            baseDir = path + "\\Video\\";
            //}
            //else
            //{
            //    baseDir = path + "\\media-server\\Upload\\";
            //}

            //baseDir = path + "\\Upload\\";
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }

            byte[] Bytes = Convert.FromBase64String(data.Trim());
            // BitArray[] Bytes = Convert.FromBase64String(data.Trim());
            // byte[] Bytes = Convert.FromBase64String(Encoding.ASCII.GetString(data));

            string date = DateTime.Now.ToString().Replace(@"/", @"_").Replace(@":", @"_").Replace(@" ", @"_");
            // FileInfo fil = new FileInfo(baseDir + Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + obj.ToString() + Fileformat);
            FileInfo fil = new FileInfo(baseDir + Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + date.ToString() + Fileformat);
            using (Stream sw = fil.OpenWrite())
            {
                sw.Write(Bytes, 0, Bytes.Length);
                sw.Close();
            }
            // filename = Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + obj.ToString() + Fileformat;
            filename = Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + date.ToString() + Fileformat;
            //if (Fileformat == ".3gp")
            {
                Task.Run(() =>
            {
                var resul = ConvertMp4Video(filename);
            }
            );
            }



            return filename;
        }

        public string ConvertToVideoFornodeold(string data, int userid, int Feedid, string Fileformat)
        {
            if (Fileformat == null || Fileformat == "" || Fileformat == ".3gp")
            {
                Fileformat = ".MP4";
            }
            string filename = null;
            Guid obj = Guid.NewGuid();
            string path = Directory.GetCurrentDirectory();
            baseDir = path + "\\media-server\\Upload\\";
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }

            string base64data = data.Replace("data:video/.3gp;base64,", "");
            // byte[] ret = Convert.FromBase64String(data);
            byte[] Bytes = Convert.FromBase64String(data);

            string date = DateTime.Now.ToString().Replace(@"/", @"_").Replace(@":", @"_").Replace(@" ", @"_");
            // FileInfo fil = new FileInfo(baseDir + Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + obj.ToString() + Fileformat);
            FileInfo fil = new FileInfo(baseDir + Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + date.ToString() + Fileformat);
            //obj.mp4convert_new();

            /// new cod
            // Fileformat = ".3gp";



            // media.mp4convert();
            // var convert = new NReco.VideoConverter.FFMpegConverter();
            // convert.ConvertMedia(@"C:\Users\gNxt007\Desktop\Image\2274_2373_1_21_2020_10_00_22_AM.3gp", @"C:\Users\gNxt007\Desktop\Image\new.mp4", NReco.VideoConverter.Format.mp4);
            //filename = Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + date.ToString() + Fileformat;
            //File.WriteAllBytes(baseDir + "//" + filename , Bytes);

            //var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
            //ffMpeg.ConvertMedia(baseDir + "//" + filename, "video.mp4","MP4");


            //byte[] data1 = Convert.FromBase64String(data); 
            //if (data1 != null)
            //{
            //    WriteByteArrayToFile("temp.mp4", data1);
            //    videoPlayer.url = "temp.mp4";
            //    videoPlayer.source = VideoSource.Url;
            //}
            string fileName = baseDir + "VID_20200128_145427.3gp";
            //if (fileName.PostedFile.ContentType == "video/mp4")

            byte[] buff = null;
            FileStream fs = new FileStream(fileName,
                                           FileMode.Open,
                                           FileAccess.Read);

            BinaryReader br = new BinaryReader(fs);
            long numBytes = new FileInfo(fileName).Length;
            buff = br.ReadBytes((int)numBytes);
            string base64String = Convert.ToBase64String(buff, 0, buff.Length);
            ///




            using (Stream sw = fil.OpenWrite())
            {
                sw.Write(buff, 0, buff.Length);
                sw.Close();
            }


            // filename = Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + obj.ToString() + Fileformat;
            filename = Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + date.ToString() + Fileformat;
            // ReturnVideo(filename);
            Task.Run(() =>
            {
                var resul = ConvertMp4Video(filename);
            });
            return filename;
        }

        /// <summary>   
        /// For Video 


        private bool ReturnVideo(string fileName)
        {
            string html = string.Empty;
            //rename if file already exists

            int j = 0;
            string AppPath;
            string inputPath;
            string outputPath;
            string imgpath;
            //string path = Directory.GetCurrentDirectory();
            //baseDir = path + "\\media-server\\Upload\\";

            AppPath = Directory.GetCurrentDirectory();
            //Get the application path
            inputPath = AppPath + "\\media-server\\Upload\\";
            //Path of the original file
            outputPath = AppPath + "\\media-server";
            //Path of the converted file
            imgpath = AppPath + "Thumbs";
            //Path of the preview file
            string filepath = inputPath + fileName;
            //while (File.Exists(filepath))
            //{
            //    j = j + 1;
            //    int dotPos = fileName.LastIndexOf(".");
            //    string namewithoutext = fileName.Substring(0, dotPos);
            //    string ext = fileName.Substring(dotPos + 1);
            //    fileName = namewithoutext + j + "." + ext;
            //    filepath = inputPath + fileName;
            //}
            //try
            //{
            //    //this.fileuploadImageVideo.SaveAs(filepath);


            //}
            //catch
            //{
            //    return false;
            //}
            string outPutFile;
            outPutFile = "\\media-server\\Upload\\" + fileName;
            // int i = this.fileuploadImageVideo.PostedFile.ContentLength;

            System.IO.FileInfo a = new System.IO.FileInfo(AppPath + outPutFile);
            long i = a.Length;
            //while (a.Exists == false)
            //{

            //}
            long b = a.Length;
            //while (i != b)
            //{

            //}


            string cmd = " -i \"" + inputPath + "\\" + fileName + "\" \"" + outputPath + "\\" + fileName.Remove(fileName.IndexOf(".")) + ".flv" + "\"";
            ConvertNow(cmd);
            string imgargs = " -i \"" + outputPath + "\\" + fileName.Remove(fileName.IndexOf(".")) + ".flv" + "\" -f image2 -ss 1 -vframes 1 -s 280x200 -an \"" + imgpath + "\\" + fileName.Remove(fileName.IndexOf(".")) + ".jpg" + "\"";
            ConvertNow(imgargs);


            return true;
        }

        private void ConvertNow(string cmd)
        {
            string exepath;
            string AppPath = Directory.GetCurrentDirectory();
            //Get the application path
            exepath = AppPath + "\\media-server\\Upload\\" + "ffmpeg.exe";
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo.FileName = exepath;
            //Path of exe that will be executed, only for "filebuffer" it will be "flvtool2.exe"
            proc.StartInfo.Arguments = cmd;
            //The command which will be executed
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.Start();

            while (proc.HasExited == false)
            {

            }
        }


        /// </summary>



        public void WriteByteArrayToFile(string fileName, byte[] data)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            fileStream.Write(data, 0, data.Length);
        }


        private void CreateMovie(DateTime startDate, DateTime endDate)
        {
            int width = 320;
            int height = 240;
            var framRate = 200;


            //a LINQ-query for getting the desired images
            //var query = from d in container.ImageSet
            //            where d.Date >= startDate && d.Date <= endDate
            //            select d;

            // create instance of video writer

            //VideoWriter vw = new VideoWriter("test.avi", 30, 500, 500, true);
            ////Then write your frame
            //vw.WriteFrame(frame);

            //using (var vFWriter = new VideoFileWriter())
            //{
            ////    // create new video file
            ////    vFWriter.Open("nameOfMyVideoFile.avi", width, height, framRate, VideoCodec.Raw);

            ////    var imageEntities = query.ToList();

            ////    //loop throught all images in the collection
            ////    foreach (var imageEntity in imageEntities)
            ////    {
            ////        //what's the current image data?
            ////        var imageByteArray = imageEntity.Data;
            ////        var bmp = ToBitmap(imageByteArray);
            //       var bmpReduced = ReduceBitmap(bmp, width, height);

            //       vFWriter.WriteVideoFrame(bmpReduced);
            //}
            //vFWriter.Close();
        }

        //public bool CreateVideo(List<Bitmap> bitmaps, string outputFile, double fps)
        //{
        //    int width = 640;
        //    int height = 480;
        //    if (bitmaps == null || bitmaps.Count == 0) return false;
        //    try
        //    {
        //        using (ITimeline timeline = new DefaultTimeline(fps))
        //        {
        //            IGroup group = timeline.AddVideoGroup(32, width, height);
        //            ITrack videoTrack = group.AddTrack();

        //            int i = 0;
        //            double miniDuration = 1.0 / fps;
        //            foreach (var bmp in bitmaps)
        //            {
        //                IClip clip = videoTrack.AddImage(bmp, 0, i * miniDuration, (i + 1) * miniDuration);
        //                System.Diagnostics.Debug.WriteLine(++i);

        //            }
        //            timeline.AddAudioGroup();
        //            IRenderer renderer = new WindowsMediaRenderer(timeline, outputFile, WindowsMediaProfiles.HighQualityVideo);
        //            renderer.Render();
        //        }
        //    }
        //    catch { return false; }
        //    return true;
        //}


        //public static string GetEncodeVideoFFMpegArgs(string sSourceFile, MP4Info objMp4Info, double nMbps, int iWidth, int iHeight, bool bIncludeAudio, string sOutputFile)
        //{
        //    //Ensure file contains a video stream, otherwise this command will fail
        //    if (objMp4Info != null && objMp4Info.VideoStreamCount == 0)
        //    {
        //        throw new Exception("FFMpegArgUtils::GetEncodeVideoFFMpegArgs - mp4 does not contain a video stream");
        //    }

        //    int iBitRateInKbps = (int)(nMbps * 1000);


        //    StringBuilder sbArgs = new StringBuilder();
        //    sbArgs.Append(" -y -threads 2 -i \"" + sSourceFile + "\" -strict -2 "); // 0 tells it to choose how many threads to use

        //    if (bIncludeAudio == true)
        //    {
        //        //sbArgs.Append(" -acodec libmp3lame -ab 96k");
        //        sbArgs.Append(" -acodec aac -ar 44100 -ab 96k");
        //    }
        //    else
        //    {
        //        sbArgs.Append(" -an");
        //    }


        //    sbArgs.Append(" -vcodec libx264 -level 41 -r 15 -crf 25 -g 15  -keyint_min 45 -bf 0");

        //    //sbArgs.Append(" -vf pad=" + iWidth + ":" + iHeight + ":" + iVideoOffsetX + ":" + iVideoOffsetY);
        //    sbArgs.Append(String.Format(" -vf \"scale=iw*min({0}/iw\\,{1}/ih):ih*min({0}/iw\\,{1}/ih),pad={0}:{1}:({0}-iw)/2:({1}-ih)/2\"", iWidth, iHeight));

        //    //Output File
        //    sbArgs.Append(" \"" + sOutputFile + "\"");
        //    return sbArgs.ToString();
        //}

        public static byte[] Compress(byte[] buffer)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream zip = new GZipStream(ms, CompressionMode.Compress, true);
            zip.Write(buffer, 0, buffer.Length);
            zip.Close();
            ms.Position = 0;

            MemoryStream outStream = new MemoryStream();

            byte[] compressed = new byte[ms.Length];
            ms.Read(compressed, 0, compressed.Length);

            byte[] gzBuffer = new byte[compressed.Length + 4];
            Buffer.BlockCopy(compressed, 0, gzBuffer, 4, compressed.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gzBuffer, 0, 4);
            return gzBuffer;

        }

        public string GetAllCategory(AllCategoryUsers Obj, string currentUserId)
        {
            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetAllCategory";
            objLog.Request = JsonConvert.SerializeObject(Obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@userId", Obj.UserId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@cat_Id", Obj.CatId.ToString(), "VARCHAR", "I");
                SP.spArgumentsCollection(arrList, "@orderby", Obj.Orderby.ToString(), "VARCHAR", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetAllCategory", arrList);
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "AllCategory";
                }
                data = JsonConvert.SerializeObject(ds);
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public string SendNotification(string currentUserId)
        {
            // SendMessage();
            //SendNotification("f6OvcKg2QFE:APA91bEQscuGjmDCxs_13GULxPRYM8Pm7IT2d1UrIEtCyX8P33Mvfy5bMCEfsBpmbudOwDqLqcvKUM9oN3TP3Kxh26Iu5Cr5smwc4BzrIAemjG7hK26aX3TjdKwAchmOf72CKxGrCmyA", "hi this is ravbi");
            // SendPushNotification();
            return "ok";
        }

        public static void SendMessage()
        {
            string serverKey = "AIzaSyCSEj9mlyHlcSnxhQjav1XdBK6pI10qayU";
            string senderId = "1076006751428";
            try
            {
                var result = "-1";
                var webAddr = "https://fcm.googleapis.com/fcm/send";

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Headers.Add("Authorization:key=" + serverKey);
                httpWebRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    //string json = "{\"to\": \"fBMmeaopHsU:APA91bGvVLYbj_OKjcPS5b3QJ1OSk3aKxR0aZq2XVXk8X2Ib3biamJ_3Cs-7bHNn2phYB3Qxmjkttrf2g6a5rjdhpPN-jmuZyYlKylJnH09Tm2BuiUGse4PO8MI3_hm4NLRHNniZ1OeJ\",\"data\": {\"message\": \"This is a Firebase Cloud Messaging Topic Message!\",}}";
                    string json = "{\"to\": \"fBMmeaopHsU:APA91bGvVLYbj_OKjcPS5b3QJ1OSk3aKxR0aZq2XVXk8X2Ib3biamJ_3Cs-7bHNn2phYB3Qxmjkttrf2g6a5rjdhpPN-jmuZyYlKylJnH09Tm2BuiUGse4PO8MI3_hm4NLRHNniZ1OeJ\",\"data\": {\"message\": \"This is a Firebase Cloud Messaging Topic Message!\",\"data\": {\"message\": \"This is a Firebase Cloud Messaging Topic Message!\",\"notification\": {\"title\": \"Charactify !\",\"body\": \"There is a new message in FriendlyCha2\",\"sound\":\"default\",\"click_action\": \"FCM_PLUGIN_ACTIVITY\", },}}";
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                // return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddUserToken(UserMaster obj, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "AddUserToken";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    var UserMaster = (from Fr in db.UserMaster where (Fr.UserId == obj.UserId) select Fr).FirstOrDefault();
                    if (UserMaster != null)
                    {
                        UserMaster.Device = obj.Device;
                        UserMaster.IslogOff = false;
                        //feedreactions.Description = objaf.Description;
                      //UserMaster.CreatedVia = UserMaster.CreatedVia;
                        UserMaster.UserToken = obj.UserToken;
                        db.UserMaster.Update(UserMaster);
                        db.SaveChanges();
                        ret = UserMaster.UserId;
                    }
                    else
                    {
                        ret = -1; //user not found 
                    }
                }
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        //public string GetFriendShip(FriendShipRequest Obj)
        //{
        //    string data = string.Empty;
        //    List<FriendShip> friendShiplst = new List<FriendShip>();
        //    List<GetFeedRections> getFeedRectionslst = new List<GetFeedRections>();
        //    int RequestID = 0;
        //    LogRequest objLog = new LogRequest();
        //    objLog.MethodName = "GetFriendShip";
        //    objLog.Request = JsonConvert.SerializeObject(Obj).ToString();
        //    RequestID = RequestLog(objLog);
        //    try
        //    {
        //        string connStr = CResources.GetConnectionString();
        //        ArrayList arrList = new ArrayList();
        //        SP.spArgumentsCollection(arrList, "@FromUserId", Obj.FromUserId.ToString(), "INT", "I");
        //        SP.spArgumentsCollection(arrList, "@ToUserId", Obj.ToUserId.ToString(), "INT", "I");
        //        DataSet ds = new DataSet();
        //        ds = SP.RunStoredProcedure(connStr, ds, "getFriendShip", arrList);
        //        if (ds.Tables.Count > 0)
        //        {
        //            foreach (DataRow dr in ds.Tables[0].Rows)
        //            {
        //                FriendShip friendShip = new FriendShip();
        //                friendShip.ConnectedDate = dr["ConnectedDate"].ToString();
        //                friendShip.friendDetaillst = new List<FriendDetail>();
        //                foreach (DataRow drUd in ds.Tables[1].Rows)
        //                {
        //                    FriendDetail friendDetail = new FriendDetail();
        //                    friendDetail.userId = Convert.ToInt32(drUd["userid"].ToString());
        //                    friendDetail.userName = drUd["UserName"].ToString();
        //                    friendDetail.useProfilePic = drUd["UserProfilePic"].ToString();
        //                    friendDetail.ScoreDate = drUd["ScoredDate"].ToString();
        //                    friendDetail.AvgScore = Convert.ToDecimal(drUd["AvgScore"].ToString());
        //                    friendDetail.traitslst = new List<Traits>();

        //                    DataRow[] dataRowsComment = ds.Tables[2].Select("userid=" + drUd["userid"].ToString());
        //                    foreach (DataRow drtraitval in dataRowsComment)
        //                    {
        //                        Traits traits = new Traits();
        //                        traits.TraitsId = Convert.ToInt32(drtraitval["TraitsID"].ToString());
        //                        traits.TraitsScore = Convert.ToDecimal(drtraitval["TraitScore"].ToString());
        //                        friendDetail.traitslst.Add(traits);
        //                    }
        //                    friendShip.friendDetaillst.Add(friendDetail);
        //                }

        //                //----------
        //                friendShip.getFeedRectionslst = new List<GetFeedRections>();
        //                foreach (DataRow drFeed in ds.Tables[3].Rows)
        //                {
        //                    GetFeedRections getFeedReaction = new GetFeedRections();
        //                    getFeedReaction.UserID = Convert.ToInt32(drFeed["FromUserID"].ToString());
        //                    getFeedReaction.feedID = Convert.ToInt32(drFeed["feedid"].ToString());
        //                    getFeedReaction.feedType = drFeed["feedtype"].ToString();
        //                    getFeedReaction.Name = drFeed["Name"].ToString();
        //                    getFeedReaction.UserProfilePic = drFeed["UserProfilePic"].ToString();
        //                    getFeedReaction.createdDate = drFeed["CreatedDate"].ToString();
        //                    //getFeedReaction.noRections = Convert.ToInt32(drFeed["React_count"].ToString());
        //                    //getFeedReaction.noComments = Convert.ToInt32(drFeed["Comments_count"].ToString());


        //                    getFeedReaction.userRectionsTypeLst = new List<UserRectionsType>();
        //                    getFeedReaction.filePathlst = new List<filePath>();


        //                    DataRow[] dataRorspath = ds.Tables[4].Select("feedid=" + drFeed["feedid"].ToString());
        //                    foreach (DataRow dataRorspa in dataRorspath)
        //                    {
        //                        filePath filepath = new filePath();
        //                        filepath.filter = dataRorspa["Filter"].ToString();
        //                        filepath.Path = "http://webdot-001-site5.ctempurl.com" + dataRorspa["ImagePath"].ToString();
        //                        getFeedReaction.filePathlst.Add(filepath);

        //                    }

        //                    friendShip.getFeedRectionslst.Add(getFeedReaction);
        //                }

        //                //------------------
        //                friendShiplst.Add(friendShip);
        //            }
        //        }
        //        data = JsonConvert.SerializeObject(friendShiplst);
        //    }
        //    catch (Exception e)
        //    {
        //        objLog.Response = e.ToString();
        //        objLog.LogId = RequestID;
        //        ResponseLog(objLog);
        //        throw e;
        //    }
        //    finally
        //    {
        //        objLog.Response = data.ToString();
        //        objLog.LogId = RequestID;
        //        ResponseLog(objLog);
        //    }
        //    return data;
        //}

        public string GetFriendShip(FriendShipRequest Obj, string currentUserId)
        {
            string data = string.Empty;
            List<FriendShip> friendShiplst = new List<FriendShip>();
            List<GetFeedRections> getFeedRectionslst = new List<GetFeedRections>();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetFriendShip";
            objLog.Request = JsonConvert.SerializeObject(Obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@currentUserId", Obj.currentUserId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@friendUserId", Obj.friendUserId.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "getFriendShipScore", arrList);
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "connectedSince";
                    ds.Tables[1].TableName = "currentUser";
                    ds.Tables[2].TableName = "friend";
                    ds.Tables[3].TableName = "scoreGiven";
                    ds.Tables[4].TableName = "scoreRecive";

                    //foreach (DataRow dr in ds.Tables[0].Rows)
                    //{





                    //}
                }
                data = JsonConvert.SerializeObject(ds);
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        private long ConvertToTimestamp(DateTime value)
        {
            TimeZoneInfo NYTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime NyTime = TimeZoneInfo.ConvertTime(value, NYTimeZone);
            TimeZone localZone = TimeZone.CurrentTimeZone;
            System.Globalization.DaylightTime dst = localZone.GetDaylightChanges(NyTime.Year);
            NyTime = NyTime.AddHours(-1);
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
            TimeSpan span = (NyTime - epoch);
            return (long)Convert.ToDouble(span.TotalSeconds);
        }

        public string GetStoryList(GetFeedList obj, string currentUserId)
        {
            List<Story> storylst = new List<Story>();
            string data = string.Empty;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetStoryList";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserId", obj.UserId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageNo", obj.pageNo.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageSize", obj.pageSize.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Story", obj.Story.ToString(), "VARCHAR", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetStory_Sp", arrList);
                foreach (DataRow drStory in ds.Tables[1].Rows)
                {
                    Story story = new Story();
                    List<Story> storieslst = new List<Story>();
                    Usercomments userComments = new Usercomments();
                    story.Userid = Convert.ToInt32(drStory["UserID"].ToString());
                    story.UserName = drStory["UserName"].ToString();
                    story.UserProfilePic = drStory["UserProfilePic"].ToString();
                    story.FristName = drStory["FirstName"].ToString();
                    story.LastName = drStory["LastName"].ToString();
                    story.items = new List<Items>();
                    DataRow[] dataRorsItem = ds.Tables[0].Select("FromUserID=" + drStory["UserID"].ToString());
                    foreach (DataRow drItem in dataRorsItem)
                    {
                        Items items = new Items();
                        items.id = Convert.ToInt32(drItem["Id"].ToString());
                        // long timestamp = ConvertToTimestamp(Convert.ToDateTime(drItem["Createddate"].ToString()));
                        items.Date = drItem["Createddate"].ToString();
                        // items.Date = drItem["Createddate"].ToString();
                        items.FileType = drItem["FileType"].ToString();
                        //items.Path = URL + drItem["ImagePath"].ToString();
                        if (items.FileType == "video")
                        {
                            string myString = drItem["ImagePath"].ToString();
                            myString = myString.Remove(0, 1);
                            // items.Path = "http://www.charactify.me/video?url=" + myString; 
                            items.Path = FullUrl + myString;
                            // items.Path = UrlNew + myString;
                            string imgurl = myString.Replace(@".MP4", @".jpg");
                            items.Thumbnailurl = @"C:\NITIN\1\1\" + imgurl.ToString();

                            if (File.Exists(items.Thumbnailurl))
                            {
                                items.Thumbnailurl = URL + "/" + imgurl.ToString();
                            }
                            else
                            {
                                // \Upload\2512_3903_c20ae34d-52cd-4cdf-b23d-902c84b23562.jpeg
                                items.Thumbnailurl = URL + "/Upload/Thumbnail.jpg";
                            }
                        }
                        else
                        {
                            items.Path = URL + drItem["ImagePath"].ToString();
                        }
                        items.Description = drItem["Description"].ToString();
                        story.items.Add(items);
                    }
                    storylst.Add(story);
                }


                data = JsonConvert.SerializeObject(storylst);
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = "Success";
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public int AddUpdateEducationDetails(UserEducationDetails users, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "AddUpdateEducationDetails";
            objLog.Request = JsonConvert.SerializeObject(users).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {

                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        //users.usereducationdetails
                        if (users.Fromdate == null || users.Fromdate == "" || users.Fromdate == "NaN/NaN/NaN")
                        {
                            users.Fromdate = "01-01-1900";
                        }
                        if (users.ToDate == null || users.ToDate == "" || users.ToDate == "NaN/NaN/NaN")
                        {
                            users.ToDate = "01-01-1900";
                        }

                        var Log = (from usr in db.UserEducationDetails where (usr.UserSchoolId == users.UserSchoolID) select usr).FirstOrDefault();
                        if (Log != null)
                        {
                            Log.SchoolName = users.SchoolName;
                            Log.TypeOfSchool = users.TypeofSchool;
                            Log.FromDate = Convert.ToDateTime(users.Fromdate);
                            Log.ToDate = Convert.ToDateTime(users.ToDate);
                            Log.IsPublic = Convert.ToInt32(users.Ispublic);
                            Log.IsGraduated = Convert.ToInt32(users.Isgraduated);
                            Log.Description = users.Description;
                            Log.ModifiedDate = Currentdatetime;
                            db.UserEducationDetails.Update(Log);
                            ret = db.SaveChanges();
                        }

                        else
                        {
                            var u = new Models.UserEducationDetails()
                            {
                                UserId = users.UserID,
                                SchoolName = users.SchoolName,
                                FromDate = Convert.ToDateTime(users.Fromdate),
                                ToDate = Convert.ToDateTime(users.ToDate),
                                IsGraduated = Convert.ToInt32(users.Isgraduated),
                                Description = users.Description,
                                TypeOfSchool = users.TypeofSchool,
                                IsPublic = Convert.ToInt32(users.Ispublic),
                                CreatedBy = users.UserID,
                                CreatedDate = Currentdatetime,
                            };
                            db.UserEducationDetails.Add(u);
                            ret = db.SaveChanges();
                        }



                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }

                    }
                }
            }
            catch (Exception e)
            {

                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public int AddUpdateWorkDetails(UserWorkDetails users, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "AddUpdateWorkDetails";
            objLog.Request = JsonConvert.SerializeObject(users).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                try
                {
                    if (users.Fromdate == null || users.Fromdate == "" || users.Fromdate == "NaN/NaN/NaN")
                    {
                        users.Fromdate = "01-01-1900";
                    }
                }
                catch (Exception ex)
                {
                    users.Fromdate = "01-01-1900";
                }
                try
                {
                    if (users.ToDate == null || users.ToDate == "" || users.ToDate == "NaN/NaN/NaN")
                    {
                        users.ToDate = "01-01-1900";
                    }
                }
                catch (Exception ex)
                {
                    users.ToDate = "01-01-1900";
                }

                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        //users.usereducationdetails
                        var Log = (from usr in db.UserWorkDetails where (usr.UserEducationId == users.UserEducationID) select usr).FirstOrDefault();
                        if (Log != null)
                        {
                            Log.CompanyName = users.CompanyName;
                            Log.Address = users.Address;
                            Log.Position = users.Position;
                            Log.FromDate = Convert.ToDateTime(users.Fromdate);
                            Log.ModifiedDate = Currentdatetime;
                            Log.ToDate = Convert.ToDateTime(users.ToDate);
                            Log.IsStillWorking = Convert.ToInt32(users.Isstillworking);
                            Log.Description = users.Description;
                            Log.IsPublic = Convert.ToInt32(users.Ispublic);
                            Log.ModifiedBy = users.UserID;
                            db.UserWorkDetails.Update(Log);
                            ret = db.SaveChanges();
                        }

                        else
                        {
                            var u = new Models.UserWorkDetails()
                            {
                                UserId = users.UserID,
                                CompanyName = users.CompanyName,
                                Address = users.Address,
                                Position = users.Position,
                                FromDate = Convert.ToDateTime(users.Fromdate),
                                CreatedDate = Currentdatetime,
                                ToDate = Convert.ToDateTime(users.ToDate),
                                IsStillWorking = Convert.ToInt32(users.Isstillworking),
                                Description = users.Description,
                                IsPublic = Convert.ToInt32(users.Ispublic),
                                CreatedBy = users.UserID
                            };
                            db.UserWorkDetails.Add(u);
                            ret = db.SaveChanges();
                        }



                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }

                    }
                }
            }
            catch (Exception e)
            {

                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        private string Base64ToImage1(string base64String, int userid, int Feedid, string Fileformat)
        {
            if (Fileformat.Contains("whatsapp"))
            {
                Fileformat = ".jpg";
            }
            else if (Fileformat.Contains("undefined"))
            {
                Fileformat = ".jpg";
            }
            else if (Fileformat.Contains("facebook"))
            {
                Fileformat = ".jpg";
            }
            else if (Fileformat.Contains("miui"))
            {
                Fileformat = ".jpg";
            }
            else if (Fileformat.Contains("app"))
            {
                Fileformat = ".jpg";
            }
            else if (Fileformat.Contains("instagram"))
            {
                Fileformat = ".jpg";
            }
            else if (Fileformat.Contains("google"))
            {
                Fileformat = ".jpg";
            }
            string filename = null;
            Guid obj = Guid.NewGuid();
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            image = ResizeImage(image, new Size(500, 600), true);
            string date = DateTime.Now.ToString().Replace(@"/", @"_").Replace(@":", @"_").Replace(@" ", @"_");
            string path = Directory.GetCurrentDirectory();
            // string rootPath = TPResources.env.ContentRootPath;
            baseDir = path + "\\Upload\\";
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }
            string mainpath = baseDir + Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + obj.ToString() + Fileformat;
            image.Save(mainpath, System.Drawing.Imaging.ImageFormat.Jpeg);
            filename = Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + obj.ToString() + Fileformat;
            return filename;
        }

        public static Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        private string Base64ToImage(string base64String, int userid)
        {
            string filename = null;
            string Fileformat = null;
            if (base64String.Contains("base64"))
            {
                string[] base64 = base64String.Split(",");
                string[] Imagetype = base64String.Split(";");
                string[] imagetype = Imagetype[0].Split("/");
                Fileformat = imagetype[1];
                Guid obj = Guid.NewGuid();
                byte[] imageBytes = Convert.FromBase64String(base64[1]);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
                ms.Write(imageBytes, 0, imageBytes.Length);
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                string date = DateTime.Now.ToString().Replace(@"/", @"_").Replace(@":", @"_").Replace(@" ", @"_");
                string path = Directory.GetCurrentDirectory();
                // string rootPath = TPResources.env.ContentRootPath;
                baseDir = path + "\\Upload\\";
                if (!Directory.Exists(baseDir))
                {
                    Directory.CreateDirectory(baseDir);
                }
                string mainpath = baseDir + Convert.ToString(userid) + "-" + obj.ToString() + "." + Fileformat;
                //Imager.PerformImageResizeAndPutOnCanvas(mainpath, filename, 300, 400, System.Drawing.Imaging.ImageFormat.Jpeg);
                image.Save(mainpath, System.Drawing.Imaging.ImageFormat.Jpeg);
                filename = @URL + "\\Upload\\" + Convert.ToString(userid) + "-" + obj.ToString() + "." + Fileformat;

            }

            return filename;
        }
        //  ravi 9140095326
        public string GetSpecificFeedResponse(GetFeedList Obj, string currentUserId)
        {
            List<GetFeedRections> getFeedReactions = new List<GetFeedRections>();
            List<UserRectionsType> getUserRectionsType = new List<UserRectionsType>();

            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetSpecificFeedResponse";
            objLog.Request = JsonConvert.SerializeObject(Obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserId", Obj.UserId.ToString(), "INT", "I");
                //SP.spArgumentsCollection(arrList, "@pageNo", obj.pageNo.ToString(), "INT", "I");
                //SP.spArgumentsCollection(arrList, "@pageSize", obj.pageSize.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@FeedID", Obj.FeedId.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "getFeedWithReactionsForFeed_SP", arrList);
                //if (ds.Tables.Count > 0)
                //{
                //    ds.Tables[0].TableName = "Feed";
                //    ds.Tables[1].TableName = "FeedReactions";
                //}
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    GetFeedRections getFeedReaction = new GetFeedRections();
                    getFeedReaction.UserID = Convert.ToInt32(dr["createdby"].ToString());
                    getFeedReaction.feedID = Convert.ToInt32(dr["feedid"].ToString());
                    getFeedReaction.feedType = dr["feedtype"].ToString();
                    getFeedReaction.Name = dr["Name"].ToString();
                    getFeedReaction.FirstName = dr["FirstName"].ToString();
                    getFeedReaction.LastName = dr["LastName"].ToString();
                    getFeedReaction.UserProfilePic = dr["ProfilePic"].ToString();
                    getFeedReaction.description = dr["Description"].ToString();
                    getFeedReaction.filePath = dr["FilePath"].ToString();
                    getFeedReaction.createdDate = dr["CreatedDate"].ToString();
                    getFeedReaction.noRections = Convert.ToInt32(dr["React_count"].ToString());
                    getFeedReaction.noComments = Convert.ToInt32(dr["Comments_count"].ToString());
                    getFeedReaction.noShares = Convert.ToInt32(dr["feedid"].ToString());

                    getFeedReaction.userRectionsTypeLst = new List<UserRectionsType>();
                    getFeedReaction.filePathlst = new List<filePath>();

                    //DataRow[] dataRorspath = ds.Tables[2].Rows);
                    foreach (DataRow dataRorspa in ds.Tables[2].Rows)
                    {
                        filePath filepath = new filePath();
                        filepath.filter = dataRorspa["Filter"].ToString();
                        filepath.Path = URL + dataRorspa["ImagePath"].ToString();
                        getFeedReaction.filePathlst.Add(filepath);

                    }
                    //DataRow[] data1 = ds.Tables[3].Select(y => y.feedid = dr["feedid"].ToString());
                    DataRow[] dataRorsRec = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + " and Name<>'CM' ");
                    var x = (from r in dataRorsRec.AsEnumerable()
                             select new
                             {
                                 feedid = r.Field<int>("feedid"),
                                 Name = r.Field<string>("Name")
                             }).Distinct().ToList();
                    foreach (var i in x)
                    {
                        UserRectionsType userRectionsType = new UserRectionsType();

                        userRectionsType.ReactionType = i.Name.ToString();

                        userRectionsType.userRectionsLst = new List<UserRections>();

                        DataRow[] dataRowsReactions = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + "  and Name='" + i.Name.ToString() + "'  and Name <>'CM'  ");
                        foreach (DataRow drReaction in dataRowsReactions)
                        {
                            UserRections userRections = new UserRections();
                            userRections.ReactionType = drReaction["Name"].ToString();
                            userRections.UserID = Convert.ToInt32(drReaction["CreatedBy"].ToString());
                            userRections.ReactionID = Convert.ToInt32(drReaction["ReactionID"].ToString());
                            userRections.feedID = Convert.ToInt32(drReaction["feedid"].ToString());
                            userRections.Name = drReaction["UName"].ToString();
                            userRections.UserProfilePic = drReaction["ProfilePic"].ToString();
                            userRections.FirstName = drReaction["FirstName"].ToString();
                            userRections.LastName = drReaction["LastName"].ToString();
                            userRectionsType.userRectionsLst.Add(userRections);
                        }
                        //getFeedReaction.userRectionsLst.add
                        getFeedReaction.userRectionsTypeLst.Add(userRectionsType);
                    }
                    getFeedReaction.usercommentLst = new List<Usercomments>();
                    DataRow[] dataRowsComment = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + " and Name='CM' ");
                    foreach (DataRow drReaction in dataRowsComment)
                    {
                        Usercomments userComments = new Usercomments();
                        userComments.UserID = Convert.ToInt32(drReaction["createdby"].ToString());
                        userComments.ReactionID = Convert.ToInt32(drReaction["ReactionID"].ToString());
                        userComments.feedID = Convert.ToInt32(drReaction["feedid"].ToString());
                        userComments.Name = drReaction["UName"].ToString();
                        userComments.ReactionType = drReaction["Name"].ToString();
                        userComments.Description = drReaction["description"].ToString();
                        userComments.UserProfilePic = drReaction["ProfilePic"].ToString();
                        userComments.createdDate = drReaction["Createddate"].ToString();
                        getFeedReaction.usercommentLst.Add(userComments);
                    }

                    getFeedReactions.Add(getFeedReaction);
                }



                data = JsonConvert.SerializeObject(getFeedReactions);
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = "Success";
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public string GetSearchList(int UserID, string currentUserId)
        {
            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetSearchList";
            objLog.Request = JsonConvert.SerializeObject(UserID).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", UserID.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetSearchList", arrList);
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "SearchList";
                }
                data = JsonConvert.SerializeObject(ds);

            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public int UpdateMute(ScoreRequest obj, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "UpdateMute";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        //users.usereducationdetails
                        var Log = (from usr in db.ConnectionMaster where (usr.ConnectionId == obj.connectionid) select usr).FirstOrDefault();
                        if (Log != null)
                        {
                            Log.IsMute = obj.mute;
                            Log.ModifiedDate = Currentdatetime;
                            Log.ModifiedBy = obj.UserID;
                            db.ConnectionMaster.Update(Log);
                            ret = db.SaveChanges();
                        }
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }

                    }
                }
            }
            catch (Exception e)
            {

                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public int addShare(Share obj, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "addShare";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            int feedid;
            try
            {
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {

                        var Log = (from usr in db.FeedMaster where (usr.FeedId == obj.FeedId) select usr).FirstOrDefault();
                        if (Log != null)
                        {
                            var ud = new Models.FeedMaster()
                            {
                                FromUserId = obj.UserId,
                                ToUserId = 0,
                                FeedType = "Share",
                                FileType = Log.FileType,
                                Crid = Log.Crid,
                                Description = obj.description,
                                CreatedDate = Currentdatetime,
                                CreatedBy = obj.UserId
                            };
                            db.FeedMaster.Add(ud);
                            db.SaveChanges();
                            feedid = ud.FeedId;
                            var FeedImagePath = (from usr in db.FeedImagePath where (usr.FeedId == obj.FeedId) select usr).ToList();
                            for (int i = 0; i < FeedImagePath.Count; i++)

                            {
                                var u1 = new Models.FeedImagePath()
                                {
                                    FeedId = feedid,
                                    ImagePath = FeedImagePath[i].ImagePath,
                                    Filter = FeedImagePath[i].Filter,
                                    CreatedDate = Currentdatetime,
                                    CreatedBy = obj.UserId
                                };
                                db.FeedImagePath.Add(u1);
                                ret = db.SaveChanges();
                            }


                            var u = new Models.ShareMaster()
                            {
                                UserId = obj.UserId,
                                FeedId = obj.FeedId,
                                Description = obj.description,
                                CreatedDate = Currentdatetime,
                                CreatedBy = obj.UserId.ToString()
                            };
                            db.ShareMaster.Add(u);
                            ret = db.SaveChanges();

                            if (ret > 0)
                            {
                                dbContextTransaction.Commit();
                                if (obj.UserId != Convert.ToInt32(Log.FromUserId))
                                {
                                    addNotification(obj.UserId, Convert.ToInt32(Log.FromUserId), "Share", ret, feedid);
                                }

                            }
                            else
                            {
                                dbContextTransaction.Rollback();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;
        }

        public int ConnectionRemove(ScoreRequest obj, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "ConnectionRemove";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        //users.usereducationdetails
                        var Log = (from usr in db.ConnectionMaster where (usr.ConnectionId == obj.connectionid) select usr).FirstOrDefault();
                        if (Log != null)
                        {
                            Log.Status = "Remove";
                            Log.ModifiedDate = Currentdatetime;
                            Log.ModifiedBy = obj.UserID;
                            db.ConnectionMaster.Update(Log);
                            ret = db.SaveChanges();
                        }
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                            string connStr = CResources.GetConnectionString();
                            ArrayList arrList = new ArrayList();
                            SP.spArgumentsCollection(arrList, "@@connectionId", obj.connectionid.ToString(), "INT", "I");
                            DataSet ds = new DataSet();
                            ds = SP.RunStoredProcedure(connStr, ds, "updateNotification_Sp", arrList);
                        }

                    }
                }
            }
            catch (Exception e)
            {

                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public int UserWorkDelete(UserWorkDetails users, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "UserWorkDelete";
            objLog.Request = JsonConvert.SerializeObject(users).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        var Log = (from usr in db.UserWorkDetails where (usr.UserEducationId == users.UserEducationID) select usr).FirstOrDefault();
                        if (Log != null)
                        {
                            db.UserWorkDetails.Remove(Log);
                            ret = db.SaveChanges();
                        }
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }

                    }
                }
            }
            catch (Exception e)
            {

                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public int DeleteEducationDetails(UserEducationDetails users, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "DeleteEducationDetails";
            objLog.Request = JsonConvert.SerializeObject(users).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {

                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        //users.usereducationdetails
                        var Log = (from usr in db.UserEducationDetails where (usr.UserSchoolId == users.UserSchoolID) select usr).FirstOrDefault();
                        if (Log != null)
                        {
                            db.UserEducationDetails.Remove(Log);
                            ret = db.SaveChanges();
                        }
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }

                    }
                }
            }
            catch (Exception e)
            {

                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public string GetConnectionSearchList(ResUserDetail obj, string currentUserId)
        {
            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetConnectionSearchList";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", obj.UserID.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@UserName", obj.UserName.ToString(), "VARCHAR", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetConnectionSearchList", arrList);
                if (ds.Tables.Count > 0)
                {
                    ds.Tables[0].TableName = "ConnectionSearchList";
                }
                data = JsonConvert.SerializeObject(ds);

            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public int addCharactify(int FromUserID, int ToUserID, int Crid, int OldCrid)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "addCharactify";
            objLog.Request = JsonConvert.SerializeObject(FromUserID).ToString();
            RequestID = RequestLog(objLog);
            int feedid;
            try
            {
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {

                        var Log = (from usr in db.FeedMaster where (usr.FromUserId == FromUserID) && (usr.ToUserId == ToUserID) && (usr.Crid == OldCrid) && usr.FeedType == "Charactify" orderby usr.FeedId descending select usr).FirstOrDefault();
                        if (Log != null)
                        {

                            Log.IsDelete = true;
                            Log.ModifiedBy = FromUserID;
                            Log.ModifiedDate = Currentdatetime;
                        }

                        {
                            var ud = new Models.FeedMaster()
                            {
                                FromUserId = FromUserID,
                                ToUserId = ToUserID,
                                FileType = "Charactify",
                                FeedType = "Charactify",
                                Description = "",
                                Crid = Crid,
                                CreatedDate = Currentdatetime,
                                CreatedBy = FromUserID
                            };
                            db.FeedMaster.Add(ud);
                            db.SaveChanges();
                            feedid = ud.FeedId;
                            var u1 = new Models.FeedImagePath()
                            {
                                FeedId = feedid,
                                ImagePath = "",
                                Filter = "",
                                CreatedDate = Currentdatetime,
                                CreatedBy = FromUserID
                            };
                            db.FeedImagePath.Add(u1);
                            ret = db.SaveChanges();
                            if (ret > 0)
                            {
                                dbContextTransaction.Commit();
                                ret = feedid;
                            }
                            else
                            {
                                dbContextTransaction.Rollback();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;
        }

        public int UpdateUserName(UserDetailsRequest users, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "UpdateUserName";
            objLog.Request = JsonConvert.SerializeObject(users).ToString();
            var response = new SingleResponse<string>();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {

                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        var Dataex = (from usr in db.UserMaster where (usr.AppUserName == users.AppUserName) select usr).FirstOrDefault();
                        if (Dataex == null)
                        {
                            var Log = (from usr in db.UserMaster where (usr.UserId == users.UserID) select usr).FirstOrDefault();
                            if (Log != null)
                            {
                                Log.AppUserName = users.AppUserName;
                                Log.ModifiedDate = Currentdatetime;
                                Log.ModifiedBy = users.UserID;
                                db.UserMaster.Update(Log);
                                ret = db.SaveChanges();
                            }
                        }
                        else
                        {
                            ret = 10;
                        }
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }

                    }
                }
            }
            catch (Exception e)
            {
                ret = -1;
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                response.Code = ResponseCodeEnum.UNHANDELEDEXCEPTION;
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;
        }

        public int Addtagging(FeedRequest objaf, string currentUserId)
        {
            int feedid = 0;
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "Addtagging";
            objLog.Request = JsonConvert.SerializeObject(objaf).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {

                // 
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {

                        var ud = new Models.FeedMaster()
                        {
                            FromUserId = objaf.FromUserID,
                            ToUserId = objaf.ToUserID,
                            FeedType = objaf.FeedType,
                            FileType = objaf.FileType,
                            Description = objaf.Description,
                            CreatedDate = Currentdatetime,
                            CreatedBy = objaf.FromUserID
                        };
                        db.FeedMaster.Add(ud);
                        db.SaveChanges();
                        feedid = ud.FeedId;
                        foreach (var uwdr in objaf.feedImagePathslst)
                        {
                            string filename = null;
                            if (uwdr.FileType == "video")
                            {
                                filename = ConvertToVideo(uwdr.filePath, objaf.FromUserID, feedid, uwdr.Fileformat);
                            }
                            else
                            {
                                filename = Base64ToImage1(uwdr.filePath, objaf.FromUserID, feedid, uwdr.Fileformat);
                            }

                            var u = new Models.FeedImagePath()
                            {
                                FeedId = feedid,
                                ImagePath = @"\Upload\" + filename,
                                Filter = uwdr.Filter,
                                CreatedDate = Currentdatetime,
                                CreatedBy = objaf.FromUserID
                            };
                            db.FeedImagePath.Add(u);
                            ret = db.SaveChanges();
                        }

                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                            foreach (var uwdr in objaf.taggingslst)
                            {
                                addNotification(uwdr.userid, uwdr.Touserid, "Tagging", ret, 0);
                                //string connStr = CResources.GetConnectionString();
                                //ArrayList arrList = new ArrayList();
                                //SP.spArgumentsCollection(arrList, "@ret", "0", "INT", "O");
                                //SP.spArgumentsCollection(arrList, "@FromUserID", uwdr.userid.ToString(), "INT", "I");
                                //SP.spArgumentsCollection(arrList, "@ToUserID", uwdr.Touserid.ToString(), "INT", "I");
                                //SP.spArgumentsCollection(arrList, "@CommandName", "Tagging", "VARCHAR", "I");
                                //DataSet ds = new DataSet();
                                //SP.RunStoredProcedureRet(connStr, "Sp_addNotification", arrList);

                            }
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.Response = ex.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(ex.ToString());
                throw ex;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public string addNotification(int fromuserid, int Touserid, string CommandName, int id, int FeedID)
        {
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "AddNotifications";
            objLog.Request = JsonConvert.SerializeObject(FeedID + CommandName).ToString();
            RequestID = RequestLog(objLog);
            string res = null;
            try
            {
                DataTable dt = new DataTable();
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@ret", "0", "INT", "O");
                SP.spArgumentsCollection(arrList, "@FromUserID", fromuserid.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@ToUserID", Touserid.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@CommandName", CommandName.ToString(), "VARCHAR", "I");
                SP.spArgumentsCollection(arrList, "@Id", id.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@feedid", FeedID.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedureRetclass(connStr, "Sp_addNotification", arrList);
                //if (ds.Tables.Count > 0)
                //{
                //    dt = ds.Tables[0];
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Usercomments userComments = new Usercomments();
                    string[] userDeviceId = dr["token"].ToString().Split(",");
                    SendPushNotification(userDeviceId[0].ToString(), userDeviceId[1].ToString());
                }
                //}



            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                //objLog.Response = res.ToString();
                //objLog.LogId = RequestID;
                //ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return res;
        }

        public string SendNotification(string deviceId, string message)
        {
            string serverKey = "AIzaSyCSEj9mlyHlcSnxhQjav1XdBK6pI10qayU";
            string senderId = "1076006751428";
            //            string SERVER_API_KEY = "AIzaSyCSEj9mlyHlcSnxhQjav1XdBK6pI10qayU";
            //            var SENDER_ID = "1076006751428";
            //            var value = message;
            //            WebRequest tRequest;
            //            tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            //            tRequest.Method = "post";
            //            tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            //            tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));

            //            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));

            //            string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + value + "&data.time=" + System.DateTime.Now.ToString() + "&registration_id=" + deviceId + "";
            //            Console.WriteLine(postData);
            //            Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            //            tRequest.ContentLength = byteArray.Length;

            //            Stream dataStream = tRequest.GetRequestStream();
            //            dataStream.Write(byteArray, 0, byteArray.Length);
            //            dataStream.Close();

            //            WebResponse tResponse = tRequest.GetResponse();

            //            dataStream = tResponse.GetResponseStream();

            //            StreamReader tReader = new StreamReader(dataStream);

            //            String sResponseFromServer = tReader.ReadToEnd();


            //            tReader.Close();
            //            dataStream.Close();
            //            tResponse.Close();
            //            return sResponseFromServer;

            WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
            tRequest.Method = "post";
            //serverKey - Key from Firebase cloud messaging server  
            tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
            //Sender Id - From firebase project setting  
            tRequest.Headers.Add(string.Format("Sender: id={0}", serverKey));
            tRequest.ContentType = "application/json";
            var payload = new
            {
                to = deviceId,
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = "Test",
                    title = "Test",
                    badge = 1
                },
            };

            string postbody = JsonConvert.SerializeObject(payload).ToString();
            Byte[] byteArray = Encoding.UTF8.GetBytes(postbody);
            tRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = tRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream dataStreamResponse = tResponse.GetResponseStream())
                    {
                        if (dataStreamResponse != null) using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                //result.Response = sResponseFromServer;
                            }
                    }
                }
            }

            return "";

        }

        public string SendPushNotification(string NotificationToken, string Description)
        {
            string res = null;
            string response;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "SendPushNotification";
            objLog.Request = JsonConvert.SerializeObject(NotificationToken).ToString();
            RequestID = RequestLog(objLog);
            try
            {
                // From: https://console.firebase.google.com/project/x.y.z/settings/general/android:x.y.z

                // Projekt-ID: x.y.z
                // Web-API-Key: A...Y (39 chars)
                // App-ID: 1:...:android:...

                // From https://console.firebase.google.com/project/x.y.z/settings/
                // cloudmessaging/android:x,y,z
                // Server-Key: AAAA0...    ...._4
                string serverKey = "AIzaSyCSEj9mlyHlcSnxhQjav1XdBK6pI10qayU";
                string senderId = "1076006751428";
                //string serverKey = "AIzaSyDJek0t5XXbwbuU4YaXSdIbLXHe267PvLQ"; // Something very long
                //string senderId = "115848326921";
                string deviceId = NotificationToken; // Also something very long, 
                                                     // got from android
                                                     //string deviceId = "//topics/all";             // Use this to notify all devices, 
                                                     // but App must be subscribed to 
                                                     // topic notification
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = Description,
                        title = "Charactify",
                        sound = "Enabled",
                        click_action = "FCM_PLUGIN_ACTIVITY",
                        Type = "Notification"
                    }
                };

                //string postbody = JsonConvert.SerializeObject(payload).ToString();
                //var serializer = new JavaScriptSerializer();
                //var json = serializer.Serialize(data);
                var json = JsonConvert.SerializeObject(data).ToString();
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                response = sResponseFromServer;
                                res = response;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.Response = ex.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(ex.ToString());
                throw ex;
            }
            finally
            {
                objLog.Response = res;
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }

            return response;
        }

        public string AddNotifications(Notification objaf, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "AddNotifications";
            objLog.Request = JsonConvert.SerializeObject(objaf).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            string data = string.Empty;
            try
            {
                // string Msg = "N'" + objaf.message +"'";
                DataTable dt = new DataTable();
                string res = null;
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@ret", "0", "INT", "O");
                SP.spArgumentsCollection(arrList, "@FromUserID", objaf.FromUserID.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@ToUserID", objaf.ToUserID.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Description", objaf.message.ToString(), "NVARCHAR", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedureRetclass(connStr, "Sp_addChat", arrList);
                if (ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        Usercomments userComments = new Usercomments();
                        string[] userDeviceId = dr["token"].ToString().Split(",");
                        if (userDeviceId[0] != null)
                        {
                            res = SendPushNotification(userDeviceId[0].ToString(), userDeviceId[1].ToString());
                        }

                    }
                    res = "1";
                }
                return res;
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;


        }

        public int LogOut(UserMaster obj, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "LogOut";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    var UserMaster = (from Fr in db.UserMaster where (Fr.UserId == obj.UserId) select Fr).FirstOrDefault();
                    if (UserMaster != null)
                    {
                        UserMaster.IslogOff = true;
                        db.UserMaster.Update(UserMaster);
                        db.SaveChanges();
                        ret = UserMaster.UserId;
                    }
                    else
                    {
                        ret = -1; //user not found 
                    }
                }
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public string SelfScoreGet(int UserID, string currentUserId)
        {
            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetSelfScore";
            objLog.Request = JsonConvert.SerializeObject(UserID).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", UserID.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@sptype", "1", "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetSelfScore_Sp", arrList);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    data = "1";
                }
                else
                {
                    data = "0";
                }
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = data;
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public static System.Drawing.Bitmap GetThumbnail(string video, string thumbnail)
        {
            var cmd = "ffmpeg  -itsoffset -1  -i " + '"' + video + '"' + " -vcodec mjpeg -vframes 1 -an -f rawvideo -s 320x240 " + '"' + thumbnail + '"';

            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C " + cmd
            };

            var process = new Process
            {
                StartInfo = startInfo
            };

            process.Start();
            process.WaitForExit(5000);

            return LoadImage(thumbnail);
        }

        static Bitmap LoadImage(string path)
        {
            var ms = new MemoryStream(File.ReadAllBytes(path));
            return (Bitmap)Image.FromStream(ms);
        }


        public string ConvertHtmlToImage(int userId, string currentUserId)
        {


            string Data = string.Empty;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "ConvertHtmlToImage";
            objLog.Request = JsonConvert.SerializeObject(userId).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", userId.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetScoreForCardbyUser_Sp", arrList);
                int No_Con = 0;
                No_Con = Convert.ToInt32(ds.Tables[0].Rows[0]["ConCount"].ToString());
                string str = null;
                string strWithVal = null;
                if (No_Con >= CardGolden)
                {
                    str =
                       "<html style = \"font-family:arial;max-width:370px;height:280px; background-color:transparent;\"><body >" +

                   "<div class=\"card\" style=\"width:350px;height:230px; overflow: hidden; margin: 0 auto; padding:15px; border-radius:30px; background: #d4af37; border:2px solid #41465c;\" > " +
                     "<link href=\"https://fonts.googleapis.com/css?family=Muli&display=swap\" rel=\"stylesheet\">" +
                   " <div class=\"one\" style=\"float: left; width: 100px; text - align: center; \">" +
                     "<img src ='https://www.charactify.net\\Upload\\header-logo.png' alt=\"\" style=\"width:100%; \"  > " +
                     //"<b> Final Schedule of Participations </b > " +
                     " <aside>" +
                      "<div style = \"margin-top:5px;display:block;border:2px solid #41465c; border-radius:10px;overflow:hidden;height:96px;\" >" +
                       "<img src='#Url1' alt=\"\" style=\"width:100%; \' alt=\"\" style=\"width:100%; \">" +
                       // "<img src='http://webdot-001-site5.ctempurl.com\\Upload\\15_47_9a883179-fc2b-462b-9b3b-3cbe8a8302d8.jpg' alt=\"\" style=\"width:100%; \' alt=\"\" style=\"width:100%; \">" +
                       "</div>" +

                       "<span style=\"display: block; font-size: 16px; line-height: 15px; color: #41465c;font-weight: bold; margin: 5px 0 0 0;\">#UserName " +
                       "</span>" +
                       " </aside> </div> " +
                       "<div class=\"two\" style=\"float: left;margin: 10px 10px 0 20px;width: 131px;\">" +

                       "<ul style=\"list-style:none; float:left; text-align: center; width:50%; margin:5px 0 0 0; padding: 0; overflow:hidden; box-sizing:border-box;\">" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\hont.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T1" +
                       "</span > </li >" +

                      "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\confident.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T2" +
                       "</span > </li >" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\courage.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T7" +
                       "</span > </li >" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\adaptability.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T8" +
                       "</span > </li >" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\compasion.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T9" +
                       "</span > </li > </ul>" +


                       "<ul style=\"list - style:none; float:left; text-align: center; width:50%; margin: 5px 0 0 0; padding: 0; overflow: hidden; box-sizing:border-box;\">" +

                       "<li style=\"float:left; width: 100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\respectful.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T3" +
                       "</span > </li >" +

                      "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\generosity.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T6" +
                       "</span > </li >" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\forgiving.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T5" +
                       "</span > </li >" +

                       "<li style=\"float:left; width: 100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\fairness.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T4" +
                       "</span > </li >" +

                       "<li style=\"float:left; width: 100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\loyalty.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#Tloy" +
                       "</span > </li > </ul> </div>" +

                       "<div class=\"three\" style=\"float: right;margin: 40px 10px 0 0;width: 78px;text-align: center;\">" +
                       " <span style=\"display: block;  line-height: 80px; color: #41465c; font-size: 35px; position:relative;font-weight: bold;\"> " +

                       "<cite style=\"position: absolute; left: 0px; top: 2px; font-style:normal; font-size:25px; width:100%;text-align:center; \"> #AvgScore</cite>" +
                       "<img src='https://www.charactify.net\\Upload\\big.png' style=\"float:left; ; width:80px; \">" +
                       "</span>" +
                       "<span style=\"margin:-10px 2px 0 0; font-weight: bold; font-size: 20px; color:#41465c;display:block;\">" +
                       //"<cite style=\"float:right; font-style:normal; margin: -14px 0 0 0;\"> #NoCh </cite>" +
                       "</span> </div>" +
                       "<ul style=\"list-style:none; float:left; text - align: center; width: 100%; margin: 5px 0 0 0; padding: 0; overflow: hidden; box-sizing:border-box; \">" +
                        //"<li style=\"background: url(C:\\Ravi\\family.png) no - repeat; margin - right: 38px; line - height: 69px; width: 69px; height: 69px; float:left; color: #41465c;font-weight: bold;\"># 9.2"
                        //+ "</li>" +

                        "<li style=\"float: left;width: 65px;margin: 0 28px 0 0;position: relative;\">" +
                       "<img src='https://www.charactify.net\\Upload\\family.png' style=\"float:left; width: 69px; height:69px; \">" +
                       "<span style=\"margin: 4px 0 0 0px; color: #41465c; font-weight: bold; font-size: 14px;position:absolute;left: 0px;top: 22px; text-align:center;width: 100%;\">#family" +
                       "</span > </li >" +

                        "<li style=\"float:left; width: 69px; margin: 0 23px 0 0; position:relative;\">" +
                       "<img src='https://www.charactify.net\\Upload\\friends.png' style=\"float:left; width: 69px; height:69px; \">" +
                       "<span style=\"margin: 4px 0 0 0px; color: #41465c; font-weight: bold; font-size: 14px;position:absolute;left: 0px;top: 22px; text-align:center;width: 100%;\">#friends" +
                       "</span > </li >" +
                       "<li style=\"float: left;width: 67px;margin: 0 27px 0 0;position: relative;\">" +
                       "<img src='https://www.charactify.net\\Upload\\co-worker.png' style=\"float:left; width: 69px; height:69px;\">" +
                       "<span style=\"margin: 4px 0 0 0px; color: #41465c; font-weight: bold; font-size: 14px;position:absolute;left: 0px;top: 22px; text-align:center;width: 100%;\">#co-worker" +
                       "</span> </li >" +

                       "<li style=\"float: left;width: 68px;margin: 0;position: relative; \">" +
                       "<img src='https://www.charactify.net\\Upload\\acquaintances.png' style=\"float:left; width: 69px; height:69px;\">" +
                       "<span style=\"margin: 4px 0 0 0px; color: #41465c; font-weight: bold; font-size: 14px;position:absolute;left: 0px;top: 22px; text-align:center;width: 100%;\">#acquaintances" +
                       "</span > </li> </ul>" +

                     " </div>";

                    strWithVal = str.Replace(@"#UserName", ds.Tables[0].Rows[0]["UserName"].ToString()).Replace(@"#Url1", ds.Tables[0].Rows[0]["UserProfilePic"].ToString()).Replace(@"#T1", ds.Tables[2].Rows[0]["Honesty"].ToString()).Replace(@"#T2", ds.Tables[2].Rows[0]["Confidence"].ToString()).Replace(@"#T3", ds.Tables[2].Rows[0]["Respectful"].ToString()).Replace(@"#T4", ds.Tables[2].Rows[0]["Fairness"].ToString()).Replace(@"#T5", ds.Tables[2].Rows[0]["Forgiving"].ToString())
                    .Replace(@"#T6", ds.Tables[2].Rows[0]["Generosity"].ToString()).Replace(@"#T7", ds.Tables[2].Rows[0]["Courage"].ToString()).Replace(@"#T8", ds.Tables[2].Rows[0]["Adaptability"].ToString()).Replace(@"#T9", ds.Tables[2].Rows[0]["Compassion"].ToString()).Replace(@"#Tloy", ds.Tables[2].Rows[0]["Loyalty"].ToString()).Replace(@"#AvgScore", ds.Tables[0].Rows[0]["AvgScoreoftraits"].ToString()).Replace(@"#NoCh", ds.Tables[0].Rows[0]["CharCount"].ToString())
                    .Replace(@"#family", ds.Tables[1].Rows[0]["Family"].ToString()).Replace(@"#friends", ds.Tables[1].Rows[0]["Friends"].ToString()).Replace(@"#co-worker", ds.Tables[1].Rows[0]["CoWorkers"].ToString()).Replace(@"#acquaintances", ds.Tables[1].Rows[0]["Acquaintances"].ToString());


                }
                else if (No_Con >= CardGray)
                {
                    str =
                       "<html style = \"font-family:arial;max-width:370px;height:280px; background-color:transparent;\"><body >" +

                   "<div class=\"card\" style=\"width:350px;height:230px; overflow: hidden; margin: 0 auto; padding:15px; border-radius:30px; background:#aaa9ad; border:2px solid #41465c;\" > " +
                     "<link href=\"https://fonts.googleapis.com/css?family=Muli&display=swap\" rel=\"stylesheet\">" +
                   " <div class=\"one\" style=\"float: left; width: 100px; text - align: center; \">" +
                     "<img src ='https://www.charactify.net\\Upload\\header-logo.png' alt=\"\" style=\"width:100%; \"  > " +
                     //"<b> Final Schedule of Participations </b > " +
                     " <aside>" +
                      "<div style = \"margin-top:5px;display:block;border:2px solid #41465c; border-radius:10px;overflow:hidden;height:96px;\" >" +
                       "<img src='#Url1' alt=\"\" style=\"width:100%; \' alt=\"\" style=\"width:100%; \">" +
                       // "<img src='http://webdot-001-site5.ctempurl.com\\Upload\\15_47_9a883179-fc2b-462b-9b3b-3cbe8a8302d8.jpg' alt=\"\" style=\"width:100%; \' alt=\"\" style=\"width:100%; \">" +
                       "</div>" +

                       "<span style=\"display: block; font-size: 16px; line-height: 15px; color: #41465c;font-weight: bold; margin: 5px 0 0 0;\">#UserName " +
                       "</span>" +
                       " </aside> </div> " +
                       "<div class=\"two\" style=\"float: left;margin: 10px 10px 0 20px;width: 131px;\">" +

                       "<ul style=\"list-style:none; float:left; text-align: center; width:50%; margin:5px 0 0 0; padding: 0; overflow:hidden; box-sizing:border-box;\">" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\hont.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T1" +
                       "</span > </li >" +

                      "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\confident.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T2" +
                       "</span > </li >" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\courage.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T7" +
                       "</span > </li >" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\adaptability.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T8" +
                       "</span > </li >" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\compasion.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T9" +
                       "</span > </li > </ul>" +


                       "<ul style=\"list - style:none; float:left; text-align: center; width:50%; margin: 5px 0 0 0; padding: 0; overflow: hidden; box-sizing:border-box;\">" +

                       "<li style=\"float:left; width: 100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\respectful.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T3" +
                       "</span > </li >" +

                      "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\generosity.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T6" +
                       "</span > </li >" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\forgiving.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T5" +
                       "</span > </li >" +

                       "<li style=\"float:left; width: 100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\fairness.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T4" +
                       "</span > </li >" +

                       "<li style=\"float:left; width: 100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\loyalty.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#Tloy" +
                       "</span > </li > </ul> </div>" +

                       "<div class=\"three\" style=\"float: right;margin: 40px 10px 0 0;width: 78px;text-align: center;\">" +
                       " <span style=\"display: block;  line-height: 80px; color: #41465c; font-size: 35px; position:relative;font-weight: bold;\"> " +

                       "<cite style=\"position: absolute; left: 0px; top: 2px; font-style:normal; font-size:25px; width:100%;text-align:center;\"> #AvgScore</cite>" +
                       "<img src='https://www.charactify.net\\Upload\\big.png' style=\"float:left; ; width:80px; \">" +
                       "</span>" +
                       "<span style=\"margin:-10px 2px 0 0; font-weight: bold; font-size: 20px; color:#41465c;display:block;\">" +
                       //"<cite style=\"float:right; font-style:normal; margin: -14px 0 0 0;\"> #NoCh </cite>" +
                       "</span> </div>" +
                       "<ul style=\"list-style:none; float:left; text - align: center; width: 100%; margin: 5px 0 0 0; padding: 0; overflow: hidden; box-sizing:border-box; \">" +
                        //"<li style=\"background: url(C:\\Ravi\\family.png) no - repeat; margin - right: 38px; line - height: 69px; width: 69px; height: 69px; float:left; color: #41465c;font-weight: bold;\"># 9.2"
                        //+ "</li>" +

                        "<li style=\"float: left;width: 67px;margin: 0 28px 0 0;position: relative;\">" +
                       "<img src='https://www.charactify.net\\Upload\\family.png' style=\"float:left; width: 69px; height:69px; \">" +
                       "<span style=\"float:left; margin: 4px 0 0 0px; color: #41465c; font-weight: bold; font-size: 14px;position:absolute;left: 0px;top: 22px; text-align:center;width: 100%;\">#family" +
                       "</span > </li >" +

                        "<li style=\"float:left; width: 69px; margin: 0 23px 0 0; position:relative;\">" +
                       "<img src='https://www.charactify.net\\Upload\\friends.png' style=\"float:left; width: 69px; height:69px; \">" +
                       "<span style=\"margin: 4px 0 0 0px; color: #41465c; font-weight: bold; font-size: 14px;position:absolute;left: 0px;top: 22px; text-align:center;width: 100%;\">#friends" +
                       "</span > </li >" +
                       "<li style=\"float: left;width: 67px;margin: 0 27px 0 0;position: relative;\">" +
                       "<img src='https://www.charactify.net\\Upload\\co-worker.png' style=\"float:left; width: 69px; height:69px;\">" +
                       "<span style=\"margin: 4px 0 0 0px; color: #41465c; font-weight: bold; font-size: 14px;position:absolute;left: 0px;top: 22px; text-align:center;width: 100%;\">#co-worker" +
                       "</span> </li >" +

                       "<li style=\"float: left;width: 68px;margin: 0;position: relative; \">" +
                       "<img src='https://www.charactify.net\\Upload\\acquaintances.png' style=\"float:left; width: 69px; height:69px;\">" +
                       "<span style=\"margin: 4px 0 0 0px; color: #41465c; font-weight: bold; font-size: 14px;position:absolute;left: 0px;top: 22px; text-align:center;width: 100%;\">#acquaintances" +
                       "</span > </li> </ul>" +

                     " </div>";

                    strWithVal = str.Replace(@"#UserName", ds.Tables[0].Rows[0]["UserName"].ToString()).Replace(@"#Url1", ds.Tables[0].Rows[0]["UserProfilePic"].ToString()).Replace(@"#T1", ds.Tables[2].Rows[0]["Honesty"].ToString()).Replace(@"#T2", ds.Tables[2].Rows[0]["Confidence"].ToString()).Replace(@"#T3", ds.Tables[2].Rows[0]["Respectful"].ToString()).Replace(@"#T4", ds.Tables[2].Rows[0]["Fairness"].ToString()).Replace(@"#T5", ds.Tables[2].Rows[0]["Forgiving"].ToString())
                   .Replace(@"#T6", ds.Tables[2].Rows[0]["Generosity"].ToString()).Replace(@"#T7", ds.Tables[2].Rows[0]["Courage"].ToString()).Replace(@"#T8", ds.Tables[2].Rows[0]["Adaptability"].ToString()).Replace(@"#T9", ds.Tables[2].Rows[0]["Compassion"].ToString()).Replace(@"#Tloy", ds.Tables[2].Rows[0]["Loyalty"].ToString()).Replace(@"#AvgScore", ds.Tables[0].Rows[0]["AvgScoreoftraits"].ToString()).Replace(@"#NoCh", ds.Tables[0].Rows[0]["CharCount"].ToString())
                   .Replace(@"#family", ds.Tables[1].Rows[0]["Family"].ToString()).Replace(@"#friends", ds.Tables[1].Rows[0]["Friends"].ToString()).Replace(@"#co-worker", ds.Tables[1].Rows[0]["CoWorkers"].ToString()).Replace(@"#acquaintances", ds.Tables[1].Rows[0]["Acquaintances"].ToString());

                }
                else if (No_Con >= CardWhite)
                {
                    str =
                       "<html style = \"font-family:arial;max-width:370px;height:280px; background-color:transparent;\"><body >" +

                   "<div class=\"card\" style=\"width:350px;height:230px; overflow: hidden; margin: 0 auto; padding:15px; border-radius:30px; background:#fff; border:2px solid #41465c;\" > " +
                     "<link href=\"https://fonts.googleapis.com/css?family=Muli&display=swap\" rel=\"stylesheet\">" +
                   " <div class=\"one\" style=\"float: left; width: 100px; text - align: center; \">" +
                     "<img src ='https://www.charactify.net\\Upload\\header-logo.png' alt=\"\" style=\"width:100%; \"  > " +
                     //"<b> Final Schedule of Participations </b > " +
                     " <aside>" +
                      "<div style = \"margin-top:5px;display:block;border:2px solid #41465c; border-radius:10px;overflow:hidden;height:96px;\" >" +
                       "<img src='#Url1' alt=\"\" style=\"width:100%; \' alt=\"\" style=\"width:100%; \">" +
                       // "<img src='http://webdot-001-site5.ctempurl.com\\Upload\\15_47_9a883179-fc2b-462b-9b3b-3cbe8a8302d8.jpg' alt=\"\" style=\"width:100%; \' alt=\"\" style=\"width:100%; \">" +
                       "</div>" +

                       "<span style=\"display: block; font-size: 16px; line-height: 15px; color: #41465c;font-weight: bold; margin: 5px 0 0 0;\">#UserName " +
                       "</span>" +
                       " </aside> </div> " +
                       "<div class=\"two\" style=\"float: left;margin: 10px 10px 0 20px;width: 131px;\">" +

                       "<ul style=\"list-style:none; float:left; text-align: center; width:50%; margin:5px 0 0 0; padding: 0; overflow:hidden; box-sizing:border-box;\">" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\hont.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T1" +
                       "</span > </li >" +

                      "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\confident.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T2" +
                       "</span > </li >" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\courage.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T7" +
                       "</span > </li >" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\adaptability.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T8" +
                       "</span > </li >" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\compasion.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T9" +
                       "</span > </li > </ul>" +


                       "<ul style=\"list - style:none; float:left; text-align: center; width:50%; margin: 5px 0 0 0; padding: 0; overflow: hidden; box-sizing:border-box;\">" +

                       "<li style=\"float:left; width: 100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\respectful.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T3" +
                       "</span > </li >" +

                      "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\generosity.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T6" +
                       "</span > </li >" +

                       "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\forgiving.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T5" +
                       "</span > </li >" +

                       "<li style=\"float:left; width: 100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\fairness.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T4" +
                       "</span > </li >" +

                       "<li style=\"float:left; width: 100%; margin-bottom: 5px; \">" +
                       "<img src='https://www.charactify.net\\Upload\\loyalty.png' style=\"float:left; width: 20px; \">" +
                       "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#Tloy" +
                       "</span > </li > </ul> </div>" +

                       "<div class=\"three\" style=\"float: right;margin: 40px 10px 0 0;width: 78px;text-align: center;\">" +
                       " <span style=\"display: block;  line-height: 80px; color: #41465c; font-size: 35px; position:relative;font-weight: bold;\"> " +

                       "<cite style=\"position: absolute; left: 0px; top: 2px; font-style:normal; font-size:25px; width:100%;text-align:center;\"> #AvgScore</cite>" +
                       "<img src='https://www.charactify.net\\Upload\\big.png' style=\"float:left; ; width:80px; \">" +
                       "</span>" +
                       "<span style=\"margin:-10px 2px 0 0; font-weight: bold; font-size: 20px; color:#41465c;display:block;\">" +
                       //"<cite style=\"float:right; font-style:normal; margin: -14px 0 0 0;\"> #NoCh </cite>" +
                       "</span> </div>" +
                       "<ul style=\"list-style:none; float:left; text - align: center; width: 100%; margin: 5px 0 0 0; padding: 0; overflow: hidden; box-sizing:border-box; \">" +
                        //"<li style=\"background: url(C:\\Ravi\\family.png) no - repeat; margin - right: 38px; line - height: 69px; width: 69px; height: 69px; float:left; color: #41465c;font-weight: bold;\"># 9.2"
                        //+ "</li>" +

                        "<li style=\"float: left;width: 67px;margin: 0 28px 0 0;position: relative;\">" +
                       "<img src='https://www.charactify.net\\Upload\\family.png' style=\"float:left; width: 69px; height:69px; \">" +
                       "<span style=\"float:left; margin: 4px 0 0 0px; color: #41465c; font-weight: bold; font-size: 14px;position:absolute;left: 0px;top: 22px; text-align:center;width: 100%;\">#family" +
                       "</span > </li >" +

                        "<li style=\"float:left; width: 69px; margin: 0 23px 0 0; position:relative;\">" +
                       "<img src='https://www.charactify.net\\Upload\\friends.png' style=\"float:left; width: 69px; height:69px; \">" +
                       "<span style=\"margin: 4px 0 0 0px; color: #41465c; font-weight: bold; font-size: 14px;position:absolute;left: 0px;top: 22px; text-align:center;width: 100%; \">#friends" +
                       "</span > </li >" +
                       "<li style=\"float: left;width: 67px;margin: 0 27px 0 0;position: relative;\">" +
                       "<img src='https://www.charactify.net\\Upload\\co-worker.png' style=\"float:left; width: 69px; height:69px;\">" +
                       "<span style=\"margin: 4px 0 0 0px; color: #41465c; font-weight: bold; font-size: 14px;position:absolute;left: 0px;top: 22px; text-align:center;width: 100%;\">#co-worker" +
                       "</span> </li >" +

                       "<li style=\"float: left;width: 68px;margin: 0;position: relative; \">" +
                       "<img src='https://www.charactify.net\\Upload\\acquaintances.png' style=\"float:left; width: 69px; height:69px;\">" +
                       "<span style=\"margin: 4px 0 0 0px; color: #41465c; font-weight: bold; font-size: 14px;position:absolute;left: 0px;top: 22px; text-align:center;width: 100%;\">#acquaintances" +
                       "</span > </li> </ul>" +

                     " </div>";

                    strWithVal = str.Replace(@"#UserName", ds.Tables[0].Rows[0]["UserName"].ToString()).Replace(@"#Url1", ds.Tables[0].Rows[0]["UserProfilePic"].ToString()).Replace(@"#T1", ds.Tables[2].Rows[0]["Honesty"].ToString()).Replace(@"#T2", ds.Tables[2].Rows[0]["Confidence"].ToString()).Replace(@"#T3", ds.Tables[2].Rows[0]["Respectful"].ToString()).Replace(@"#T4", ds.Tables[2].Rows[0]["Fairness"].ToString()).Replace(@"#T5", ds.Tables[2].Rows[0]["Forgiving"].ToString())
                   .Replace(@"#T6", ds.Tables[2].Rows[0]["Generosity"].ToString()).Replace(@"#T7", ds.Tables[2].Rows[0]["Courage"].ToString()).Replace(@"#T8", ds.Tables[2].Rows[0]["Adaptability"].ToString()).Replace(@"#T9", ds.Tables[2].Rows[0]["Compassion"].ToString()).Replace(@"#Tloy", ds.Tables[2].Rows[0]["Loyalty"].ToString()).Replace(@"#AvgScore", ds.Tables[0].Rows[0]["AvgScoreoftraits"].ToString()).Replace(@"#NoCh", ds.Tables[0].Rows[0]["CharCount"].ToString())
                   .Replace(@"#family", ds.Tables[1].Rows[0]["Family"].ToString()).Replace(@"#friends", ds.Tables[1].Rows[0]["Friends"].ToString()).Replace(@"#co-worker", ds.Tables[1].Rows[0]["CoWorkers"].ToString()).Replace(@"#acquaintances", ds.Tables[1].Rows[0]["Acquaintances"].ToString());


                }
                var converter = new HtmlConverter();
                var html = strWithVal;
                if (html != null)
                {
                    var bytes = converter.FromHtmlString(html, 400);
                    Data = Convert.ToBase64String(bytes);
                }
                else
                {
                    Data = null;
                }
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = Data;
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return Data;
        }

        public string ScoreCard(int userId, string currentUserId)
        {


            string Data = string.Empty;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "ScoreCard";
            objLog.Request = JsonConvert.SerializeObject(userId).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", userId.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetScoreForCardbyUser_Sp", arrList);
                // string str  = "<html > < body > <div style="font-size:50px; font - color:red"> <div><p>Dear User,</br></br> Your verification code to rest the password is: <b>  96953  </b></br></br>Thanks,</br> Charactify Team</p></div></div> </body></html>";
                // string str = " < html >< body >< p > Dear User,</ p >< p > Your verification code to reset the password is: </ p > < b > { 0}  </ b >  < b > { 1}  </ b > < b > { 2}  </ b >< p > Thanks, < br >< b > Charactify Team<b> </ br ></ p ></ body ></ html > ";
                string str =
                    "<html style = \"font-family:arial;max-width:370px;height:280px; background-color:transparent;\"><body >" +

                "<div class=\"card\" style=\"width:350px;height:230px; overflow: hidden; margin: 0 auto; padding:15px; border-radius:30px; background:#fff; border:2px solid #41465c;\" > " +
                  "<link href=\"https://fonts.googleapis.com/css?family=Muli&display=swap\" rel=\"stylesheet\">" +
                " <div class=\"one\" style=\"float: left; width: 100px; text - align: center; \">" +
                  "<img src ='https://www.charactify.net\\Upload\\header-logo.png' alt=\"\" style=\"width:100%; \"  > " +
                  //"<b> Final Schedule of Participations </b > " +
                  " <aside>" +
                   "<div style = \"margin-top:5px;display:block;border:2px solid #41465c; border-radius:10px;overflow:hidden;height:96px;\" >" +
                    "<img src='#Url1' alt=\"\" style=\"width:100%; \' alt=\"\" style=\"width:100%; \">" +
                    // "<img src='http://webdot-001-site5.ctempurl.com\\Upload\\15_47_9a883179-fc2b-462b-9b3b-3cbe8a8302d8.jpg' alt=\"\" style=\"width:100%; \' alt=\"\" style=\"width:100%; \">" +
                    "</div>" +

                    "<span style=\"display: block; font-size: 16px; line-height: 15px; color: #41465c;font-weight: bold; margin: 5px 0 0 0;\">#UserName " +
                    "</span>" +
                    " </aside> </div> " +
                    "<div class=\"two\" style=\"float: left;margin: 10px 10px 0 20px;width: 131px;\">" +

                    "<ul style=\"list-style:none; float:left; text-align: center; width:50%; margin:5px 0 0 0; padding: 0; overflow:hidden; box-sizing:border-box;\">" +

                    "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                    "<img src='https://www.charactify.net\\Upload\\hont.png' style=\"float:left; width: 20px; \">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T1" +
                    "</span > </li >" +

                   "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                    "<img src='https://www.charactify.net\\Upload\\confident.png' style=\"float:left; width: 20px; \">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T2" +
                    "</span > </li >" +

                    "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                    "<img src='https://www.charactify.net\\Upload\\courage.png' style=\"float:left; width: 20px; \">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T7" +
                    "</span > </li >" +

                    "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                    "<img src='https://www.charactify.net\\Upload\\adaptability.png' style=\"float:left; width: 20px; \">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T8" +
                    "</span > </li >" +

                    "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                    "<img src='https://www.charactify.net\\Upload\\compasion.png' style=\"float:left; width: 20px; \">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T9" +
                    "</span > </li > </ul>" +


                    "<ul style=\"list - style:none; float:left; text-align: center; width:50%; margin: 5px 0 0 0; padding: 0; overflow: hidden; box-sizing:border-box;\">" +

                    "<li style=\"float:left; width: 100%; margin-bottom: 5px; \">" +
                    "<img src='https://www.charactify.net\\Upload\\respectful.png' style=\"float:left; width: 20px; \">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T3" +
                    "</span > </li >" +

                   "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                    "<img src='https://www.charactify.net\\Upload\\generosity.png' style=\"float:left; width: 20px; \">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T6" +
                    "</span > </li >" +

                    "<li style=\"float:left; width:100%; margin-bottom: 5px; \">" +
                    "<img src='https://www.charactify.net\\Upload\\forgiving.png' style=\"float:left; width: 20px; \">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T5" +
                    "</span > </li >" +

                    "<li style=\"float:left; width: 100%; margin-bottom: 5px; \">" +
                    "<img src='https://www.charactify.net\\Upload\\fairness.png' style=\"float:left; width: 20px; \">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#T4" +
                    "</span > </li >" +

                    "<li style=\"float:left; width: 100%; margin-bottom: 5px; \">" +
                    "<img src='https://www.charactify.net\\Upload\\loyalty.png' style=\"float:left; width: 20px; \">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;\">#Tloy" +
                    "</span > </li > </ul> </div>" +

                    "<div class=\"three\" style=\"float: right;margin: 40px 10px 0 0;width: 78px;text-align: center;\">" +
                    " <span style=\"display: block;  line-height: 80px; color: #41465c; font-size: 35px; position:relative;font-weight: bold;\"> " +

                    "<cite style=\"position: absolute; left: 21px; top: 2px; font-style:normal; font-size:26px; \"> #AvgScore</cite>" +
                    "<img src='https://www.charactify.net\\Upload\\big.png' style=\"float:left; ; width:80px; \">" +
                    "</span>" +
                    "<span style=\"margin:-10px 2px 0 0; font-weight: bold; font-size: 20px; color:#41465c;display:block;\">" +
                    //"<cite style=\"float:right; font-style:normal; margin: -14px 0 0 0;\"> #NoCh </cite>" +
                    "</span> </div>" +
                    "<ul style=\"list-style:none; float:left; text - align: center; width: 100%; margin: 5px 0 0 0; padding: 0; overflow: hidden; box-sizing:border-box; \">" +
                     //"<li style=\"background: url(C:\\Ravi\\family.png) no - repeat; margin - right: 38px; line - height: 69px; width: 69px; height: 69px; float:left; color: #41465c;font-weight: bold;\"># 9.2"
                     //+ "</li>" +

                     "<li style=\"float: left;width: 67px;margin: 0 28px 0 0;position: relative;\">" +
                    "<img src='https://www.charactify.net\\Upload\\family.png' style=\"float:left; width: 69px; height:69px; \">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;position: absolute;left: 15px;top: 22px;\">#family" +
                    "</span > </li >" +

                     "<li style=\"float:left; width: 69px; margin: 0 25px 0 0; position:relative;\">" +
                    "<img src='https://www.charactify.net\\Upload\\friends.png' style=\"float:left; width: 69px; height:69px; \">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;position: absolute;left: 15px;top: 22px;\">#friends" +
                    "</span > </li >" +
                    "<li style=\"float: left;width: 64px;margin: 0 27px 0 0;position: relative;\">" +
                    "<img src='https://www.charactify.net\\Upload\\co-worker.png' style=\"float:left; width: 69px; height:69px;\">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;position: absolute;left: 15px;top: 22px;\">#co-worker" +
                    "</span> </li >" +

                    "<li style=\"float: left;width: 58px;margin: 0;position: relative; \">" +
                    "<img src='https://www.charactify.net\\Upload\\acquaintances.png' style=\"float:left; width: 69px; height:69px;\">" +
                    "<span style=\"float:left; margin: 5px 0 0 10px; color: #41465c; font-weight: bold; font-size: 14px;position: absolute;left: 15px;top: 22px;\">#acquaintances" +
                    "</span > </li> </ul>" +

                  " </div>";



                string strWithVal = str.Replace(@"#UserName", ds.Tables[0].Rows[0]["UserName"].ToString()).Replace(@"#Url1", ds.Tables[0].Rows[0]["UserProfilePic"].ToString()).Replace(@"#T1", ds.Tables[2].Rows[0]["Honesty"].ToString()).Replace(@"#T2", ds.Tables[2].Rows[0]["Confidence"].ToString()).Replace(@"#T3", ds.Tables[2].Rows[0]["Respectful"].ToString()).Replace(@"#T4", ds.Tables[2].Rows[0]["Fairness"].ToString()).Replace(@"#T5", ds.Tables[2].Rows[0]["Forgiving"].ToString())
                    .Replace(@"#T6", ds.Tables[2].Rows[0]["Generosity"].ToString()).Replace(@"#T7", ds.Tables[2].Rows[0]["Courage"].ToString()).Replace(@"#T8", ds.Tables[2].Rows[0]["Adaptability"].ToString()).Replace(@"#T9", ds.Tables[2].Rows[0]["Compassion"].ToString()).Replace(@"#Tloy", ds.Tables[2].Rows[0]["Loyalty"].ToString()).Replace(@"#AvgScore", ds.Tables[0].Rows[0]["AvgScoreoftraits"].ToString()).Replace(@"#NoCh", ds.Tables[0].Rows[0]["CharCount"].ToString())
                    .Replace(@"#family", ds.Tables[1].Rows[0]["Family"].ToString()).Replace(@"#friends", ds.Tables[1].Rows[0]["Friends"].ToString()).Replace(@"#co-worker", ds.Tables[1].Rows[0]["CoWorkers"].ToString()).Replace(@"#acquaintances", ds.Tables[1].Rows[0]["Acquaintances"].ToString());
                var converter = new HtmlConverter();
                var html = strWithVal;
                //var htmlToImageConv = new NReco.ImageGenerator.HtmlToImageConverter();
                //var jpegBytes = htmlToImageConv.GenerateImage(html, "Jpeg");
                //var bytes = Encoding.ASCII.GetBytes(html); //Encoding.Default.GetBytes(html);
                var bytes = converter.FromHtmlString(html, 400);
                Card card = new Card();
                card.Link = Convert.ToBase64String(bytes);
                card.Msg = "Your Charactify Scorecard !";
                // File.WriteAllBytes("C:/Ravi/image1.jpg", bytes);
                Data = JsonConvert.SerializeObject(card);
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = Data;
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return Data;
        }


        public string GetFeedResponseOnFeedId(GetFeedList obj, string currentUserId)
        {
            List<GetFeedRections> getFeedReactions = new List<GetFeedRections>();
            List<UserRectionsType> getUserRectionsType = new List<UserRectionsType>();

            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetFeedResponseOnFeedId";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserId", obj.UserId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageNo", obj.pageNo.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageSize", obj.pageSize.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Story", obj.Story.ToString(), "VARCHAR", "I");
                SP.spArgumentsCollection(arrList, "@FeedID", obj.FeedId.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetSpecificFeedResponse_SP", arrList);
                //if (ds.Tables.Count > 0)
                //{
                //    ds.Tables[0].TableName = "Feed";
                //    ds.Tables[1].TableName = "FeedReactions";
                //}
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    GetFeedRections getFeedReaction = new GetFeedRections();
                    getFeedReaction.UserID = Convert.ToInt32(dr["createdby"].ToString());
                    getFeedReaction.feedID = Convert.ToInt32(dr["feedid"].ToString());
                    getFeedReaction.feedType = dr["feedtype"].ToString();
                    getFeedReaction.Name = dr["Name"].ToString();
                    getFeedReaction.UserProfilePic = dr["ProfilePic"].ToString();
                    getFeedReaction.description = dr["Description"].ToString();
                    getFeedReaction.filePath = dr["FilePath"].ToString();
                    getFeedReaction.createdDate = dr["CreatedDate"].ToString();
                    getFeedReaction.noRections = Convert.ToInt32(dr["React_count"].ToString());
                    getFeedReaction.noComments = Convert.ToInt32(dr["Comments_count"].ToString());
                    getFeedReaction.noShares = Convert.ToInt32(dr["Share_count"].ToString());
                    getFeedReaction.FirstName = dr["firstname"].ToString();
                    getFeedReaction.LastName = dr["LastName"].ToString();
                    DataRow[] dataRowsCurReactions = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + "  and CreatedBy='" + obj.UserId.ToString() + "'  and Name <>'CM'  ");
                    foreach (DataRow drReaction in dataRowsCurReactions)
                    {
                        getFeedReaction.currentUserRection = drReaction["Name"].ToString();
                    }
                    getFeedReaction.userRectionsTypeLst = new List<UserRectionsType>();
                    getFeedReaction.filePathlst = new List<filePath>();

                    if (getFeedReaction.feedType == "Charactify")
                    {
                        getFeedReaction.charactifyScores = new List<CharactifyScore>();

                        DataRow[] dataRorschar = ds.Tables[4].Select("Crid=" + dr["Crid"].ToString());
                        foreach (DataRow dataRorsch in dataRorschar)
                        {
                            CharactifyScore charactifyScore = new CharactifyScore();
                            charactifyScore.Score = dataRorsch["Score"].ToString();
                            charactifyScore.TraitsID = dataRorsch["TraitsID"].ToString();
                            getFeedReaction.charactifyScores.Add(charactifyScore);
                        }
                    }
                    else
                    {
                        DataRow[] dataRorspath = ds.Tables[2].Select("feedid=" + dr["feedid"].ToString() + " and feedtype<>'Charactify'");
                        //DataRow[] dataRorspath = ds.Tables[2].Select("feedid=" + dr["feedid"].ToString());
                        foreach (DataRow dataRorspa in dataRorspath)
                        {
                            filePath filepath = new filePath();
                            filepath.filter = dataRorspa["Filter"].ToString();
                            filepath.Description = dataRorspa["Description"].ToString();
                            // filepath.Path = URL + dataRorspa["ImagePath"].ToString();
                            if (getFeedReaction.feedType == "video")
                            {
                                string myString = dataRorspa["ImagePath"].ToString();
                                myString = myString.Remove(0, 1);
                                filepath.Path = FullUrl + myString;
                                string imgurl = myString.Replace(@".MP4", @".jpg");
                                filepath.Thumbnailurl = @"C:\NITIN\1\1\" + imgurl.ToString();

                                if (File.Exists(filepath.Thumbnailurl))
                                {
                                    filepath.Thumbnailurl = URL + "/" + imgurl.ToString();
                                }
                                else
                                {
                                    // \Upload\2512_3903_c20ae34d-52cd-4cdf-b23d-902c84b23562.jpeg
                                    filepath.Thumbnailurl = URL + "/Upload/Thumbnail.jpg";
                                }
                            }
                            else
                            {
                                filepath.Path = URL + dataRorspa["ImagePath"].ToString();
                            }
                            getFeedReaction.filePathlst.Add(filepath);

                        }
                    }
                    //DataRow[] data1 = ds.Tables[3].Select(y => y.feedid = dr["feedid"].ToString());
                    DataRow[] dataRorsRec = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + " and Name<>'CM' ");
                    var x = (from r in dataRorsRec.AsEnumerable()
                             select new
                             {
                                 feedid = r.Field<int>("feedid"),
                                 Name = r.Field<string>("Name")
                             }).Distinct().ToList();
                    foreach (var i in x)
                    {
                        UserRectionsType userRectionsType = new UserRectionsType();

                        userRectionsType.ReactionType = i.Name.ToString();

                        userRectionsType.userRectionsLst = new List<UserRections>();

                        DataRow[] dataRowsReactions = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + "  and Name='" + i.Name.ToString() + "'  and Name <>'CM'  ");
                        foreach (DataRow drReaction in dataRowsReactions)
                        {
                            UserRections userRections = new UserRections();
                            userRections.ReactionType = drReaction["Name"].ToString();
                            userRections.UserID = Convert.ToInt32(drReaction["CreatedBy"].ToString());
                            userRections.ReactionID = Convert.ToInt32(drReaction["ReactionID"].ToString());
                            userRections.feedID = Convert.ToInt32(drReaction["feedid"].ToString());
                            userRections.Name = drReaction["UName"].ToString();
                            userRections.UserProfilePic = drReaction["ProfilePic"].ToString();
                            userRections.FirstName = drReaction["FirstName"].ToString();
                            userRections.LastName = drReaction["LastName"].ToString();
                            userRectionsType.userRectionsLst.Add(userRections);
                        }
                        //getFeedReaction.userRectionsLst.add
                        getFeedReaction.userRectionsTypeLst.Add(userRectionsType);
                    }
                    getFeedReaction.usercommentLst = new List<Usercomments>();
                    DataRow[] dataRowsComment = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + " and Name='CM' ");
                    foreach (DataRow drReaction in dataRowsComment)
                    {
                        Usercomments userComments = new Usercomments();
                        userComments.UserID = Convert.ToInt32(drReaction["createdby"].ToString());
                        userComments.ReactionID = Convert.ToInt32(drReaction["ReactionID"].ToString());
                        userComments.feedID = Convert.ToInt32(drReaction["feedid"].ToString());
                        userComments.Name = drReaction["UName"].ToString();
                        userComments.ReactionType = drReaction["Name"].ToString();
                        userComments.Description = drReaction["description"].ToString();
                        userComments.UserProfilePic = drReaction["ProfilePic"].ToString();
                        userComments.createdDate = drReaction["CreatedDate"].ToString();
                        getFeedReaction.usercommentLst.Add(userComments);
                    }
                    getFeedReaction.taggingslst = new List<tagging>();
                    DataRow[] dataRowstagging = ds.Tables[5].Select("feedid=" + dr["feedid"].ToString());
                    foreach (DataRow drTagging in dataRowstagging)
                    {
                        tagging objtagging = new tagging();
                        objtagging.userid = Convert.ToInt32(dr["createdby"].ToString());
                        objtagging.Touserid = Convert.ToInt32(drTagging["toUserId"].ToString());
                        objtagging.FirstName = drTagging["FirstName"].ToString();
                        objtagging.LastName = drTagging["LastName"].ToString();
                        getFeedReaction.taggingslst.Add(objtagging);
                    }

                    getFeedReaction.takenScorelst = new List<takenScore>();
                    DataRow[] dataRowstakenScore = ds.Tables[6].Select("feedid=" + dr["feedid"].ToString());
                    foreach (DataRow drTagging in dataRowstakenScore)
                    {
                        takenScore objtakenby = new takenScore();
                        objtakenby.userid = Convert.ToInt32(drTagging["UserID"].ToString());
                        objtakenby.FirstName = drTagging["FirstName"].ToString();
                        objtakenby.LastName = drTagging["LastName"].ToString();
                        objtakenby.UserProfilePic = drTagging["UserProfilePic"].ToString();
                        getFeedReaction.takenScorelst.Add(objtakenby);
                    }


                    getFeedReactions.Add(getFeedReaction);
                }



                data = JsonConvert.SerializeObject(getFeedReactions);
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = "Success";
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public string EmailSearchList(SearchList Objreq, string currentUserId)
        {

            string data = string.Empty;
            string data1 = string.Empty;
            List<EmailStatus> obj = new List<EmailStatus>();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "IntersectedJson";
            objLog.currentUserId = currentUserId;
            objLog.Request = JsonConvert.SerializeObject(Objreq).ToString();
            try
            {
                using (var db = new TPContext())
                {
                    Email email = new Email();
                    DataTable dt = new DataTable();
                    dt.Columns.AddRange(new DataColumn[4]
                    { new DataColumn("UserId", typeof(int)),
                            new DataColumn("Name", typeof(string)),
                            new DataColumn("EmailId",typeof(string)),
                            new DataColumn("Phone",typeof(string))
                    });

                    if (Objreq.emails.Count > 0)
                    {
                        for (int a = 0; a < Objreq.emails.Count; a++)
                        {
                            dt.Rows.Add(Objreq.Userid, Objreq.emails[a].name, Objreq.emails[a].emailid.Replace(" ", String.Empty), (Objreq.emails[a].Phone.Replace(" ", String.Empty)).Replace("+91", " "));
                        }
                    }
                    else
                    {
                        dt.Rows.Add(Objreq.Userid, "", "", "");
                    }

                    string connStr = CResources.GetConnectionString();
                    DataSet ds = new DataSet();
                    SearchList searchList = new SearchList();

                    using (SqlConnection connect = new SqlConnection(connStr))
                    {
                        connect.Open();
                        SqlCommand cmd = new SqlCommand("GetSearchList_new", connect)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.AddWithValue("@DataTable", dt);
                        SqlDataAdapter adap = new SqlDataAdapter(cmd);
                        adap.Fill(ds);
                        dt.Clear();
                    }
                    if (ds.Tables.Count > 0)
                    {
                        //foreach (DataRow Sdr in ds.Tables[0].Rows)
                        //{

                        //    searchList.Username = Sdr["name"].ToString();
                        //    searchList.EmailID = Sdr["email"].ToString();
                        //    searchList.Userid = Convert.ToInt32(Sdr["userid"].ToString());
                        //    searchList.Status = Sdr["email"].ToString();
                        //}
                        ds.Tables[0].TableName = "SearchList";
                        Manager mn = new Manager();

                        data = JsonConvert.SerializeObject(ds);
                        mn.Response(ds.Tables[0].Rows.Count.ToString());

                    }
                    else
                    {
                        data = null;
                    }

                }
            }
            catch (Exception e)
            {
                Manager mn = new Manager();
                mn.LogError(e.ToString());
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                mn.Response(data.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                Manager mn = new Manager();
                mn.Request(JsonConvert.SerializeObject(Objreq).ToString());
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public string GetScoreStatus(ScoreRequest obj, string currentUserId)
        {
            string data = null;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetScoreStatus";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@FromUserID", obj.FromUserID.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@ToUserID", obj.ToUserID.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetScoreStatus", arrList);
                Boolean Status = false;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Status = true;
                }
                data = JsonConvert.SerializeObject(Status);
            }
            catch (Exception e)
            {
                data = "-1";
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                throw e;
            }
            finally
            {
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;


        }

        public string GetFeedResponseForPost(GetFeedList obj, string currentUserId)
        {
            List<GetFeedRections> getFeedReactions = new List<GetFeedRections>();
            List<UserRectionsType> getUserRectionsType = new List<UserRectionsType>();

            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetFeedResponseForPost";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserId", obj.UserId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageNo", obj.pageNo.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageSize", obj.pageSize.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Story", obj.Story.ToString(), "VARCHAR", "I");
                SP.spArgumentsCollection(arrList, "@FeedID", obj.FeedId.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetSpecificForUserPost_SP", arrList);
                //if (ds.Tables.Count > 0)
                //{
                //    ds.Tables[0].TableName = "Feed";
                //    ds.Tables[1].TableName = "FeedReactions";
                //}
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    GetFeedRections getFeedReaction = new GetFeedRections();
                    getFeedReaction.UserID = Convert.ToInt32(dr["createdby"].ToString());
                    getFeedReaction.feedID = Convert.ToInt32(dr["feedid"].ToString());
                    getFeedReaction.feedType = dr["feedtype"].ToString();
                    getFeedReaction.Name = dr["Name"].ToString();
                    getFeedReaction.UserProfilePic = dr["ProfilePic"].ToString();
                    getFeedReaction.description = dr["Description"].ToString();
                    getFeedReaction.filePath = dr["FilePath"].ToString();
                    getFeedReaction.createdDate = dr["CreatedDate"].ToString();
                    getFeedReaction.noRections = Convert.ToInt32(dr["React_count"].ToString());
                    getFeedReaction.noComments = Convert.ToInt32(dr["Comments_count"].ToString());
                    getFeedReaction.noShares = Convert.ToInt32(dr["Share_count"].ToString());
                    getFeedReaction.FirstName = dr["firstname"].ToString();
                    getFeedReaction.LastName = dr["LastName"].ToString();
                    DataRow[] dataRowsCurReactions = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + "  and CreatedBy='" + obj.UserId.ToString() + "'  and Name <>'CM'  ");
                    foreach (DataRow drReaction in dataRowsCurReactions)
                    {
                        getFeedReaction.currentUserRection = drReaction["Name"].ToString();
                    }
                    getFeedReaction.userRectionsTypeLst = new List<UserRectionsType>();
                    getFeedReaction.filePathlst = new List<filePath>();

                    if (getFeedReaction.feedType == "Charactify")
                    {
                        getFeedReaction.charactifyScores = new List<CharactifyScore>();

                        DataRow[] dataRorschar = ds.Tables[4].Select("Crid=" + dr["Crid"].ToString());
                        foreach (DataRow dataRorsch in dataRorschar)
                        {
                            CharactifyScore charactifyScore = new CharactifyScore();
                            charactifyScore.Score = dataRorsch["Score"].ToString();
                            charactifyScore.TraitsID = dataRorsch["TraitsID"].ToString();
                            getFeedReaction.charactifyScores.Add(charactifyScore);
                        }
                    }
                    else
                    {
                        DataRow[] dataRorspath = ds.Tables[2].Select("feedid=" + dr["feedid"].ToString() + " and feedtype<>'Charactify'");
                        //DataRow[] dataRorspath = ds.Tables[2].Select("feedid=" + dr["feedid"].ToString());
                        foreach (DataRow dataRorspa in dataRorspath)
                        {
                            filePath filepath = new filePath();
                            filepath.filter = dataRorspa["Filter"].ToString();
                            filepath.Description = dataRorspa["Description"].ToString();
                            //filepath.Path = URL + dataRorspa["ImagePath"].ToString();
                            if (getFeedReaction.feedType == "video")
                            {
                                string myString = dataRorspa["ImagePath"].ToString();
                                myString = myString.Remove(0, 1);
                                filepath.Path = FullUrl + myString;
                                string imgurl = myString.Replace(@".MP4", @".jpg");
                                filepath.Thumbnailurl = @"C:\NITIN\1\1\" + imgurl.ToString();

                                if (File.Exists(filepath.Thumbnailurl))
                                {
                                    filepath.Thumbnailurl = URL + "/" + imgurl.ToString();
                                }
                                else
                                {
                                    // \Upload\2512_3903_c20ae34d-52cd-4cdf-b23d-902c84b23562.jpeg
                                    filepath.Thumbnailurl = URL + "/Upload/Thumbnail.jpg";
                                }
                            }
                            else
                            {
                                filepath.Path = URL + dataRorspa["ImagePath"].ToString();
                            }
                            getFeedReaction.filePathlst.Add(filepath);

                        }
                    }
                    //DataRow[] data1 = ds.Tables[3].Select(y => y.feedid = dr["feedid"].ToString());
                    DataRow[] dataRorsRec = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + " and Name<>'CM' ");
                    var x = (from r in dataRorsRec.AsEnumerable()
                             select new
                             {
                                 feedid = r.Field<int>("feedid"),
                                 Name = r.Field<string>("Name")
                             }).Distinct().ToList();
                    foreach (var i in x)
                    {
                        UserRectionsType userRectionsType = new UserRectionsType();

                        userRectionsType.ReactionType = i.Name.ToString();

                        userRectionsType.userRectionsLst = new List<UserRections>();

                        DataRow[] dataRowsReactions = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + "  and Name='" + i.Name.ToString() + "'  and Name <>'CM'  ");
                        foreach (DataRow drReaction in dataRowsReactions)
                        {
                            UserRections userRections = new UserRections();
                            userRections.ReactionType = drReaction["Name"].ToString();
                            userRections.UserID = Convert.ToInt32(drReaction["CreatedBy"].ToString());
                            userRections.ReactionID = Convert.ToInt32(drReaction["ReactionID"].ToString());
                            userRections.feedID = Convert.ToInt32(drReaction["feedid"].ToString());
                            userRections.Name = drReaction["UName"].ToString();
                            userRections.UserProfilePic = drReaction["ProfilePic"].ToString();
                            getFeedReaction.FirstName = dr["firstname"].ToString();
                            getFeedReaction.LastName = dr["LastName"].ToString();
                            userRectionsType.userRectionsLst.Add(userRections);
                        }
                        //getFeedReaction.userRectionsLst.add
                        getFeedReaction.userRectionsTypeLst.Add(userRectionsType);
                    }
                    getFeedReaction.usercommentLst = new List<Usercomments>();
                    DataRow[] dataRowsComment = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + " and Name='CM' ");
                    foreach (DataRow drReaction in dataRowsComment)
                    {
                        Usercomments userComments = new Usercomments();
                        userComments.UserID = Convert.ToInt32(drReaction["createdby"].ToString());
                        userComments.ReactionID = Convert.ToInt32(drReaction["ReactionID"].ToString());
                        userComments.feedID = Convert.ToInt32(drReaction["feedid"].ToString());
                        userComments.Name = drReaction["UName"].ToString();
                        userComments.ReactionType = drReaction["Name"].ToString();
                        userComments.Description = drReaction["description"].ToString();
                        userComments.UserProfilePic = drReaction["ProfilePic"].ToString();
                        userComments.createdDate = drReaction["CreatedDate"].ToString();
                        getFeedReaction.usercommentLst.Add(userComments);
                    }
                    getFeedReaction.taggingslst = new List<tagging>();
                    DataRow[] dataRowstagging = ds.Tables[5].Select("feedid=" + dr["feedid"].ToString());
                    foreach (DataRow drTagging in dataRowstagging)
                    {
                        tagging objtagging = new tagging();
                        objtagging.userid = Convert.ToInt32(dr["createdby"].ToString());
                        objtagging.Touserid = Convert.ToInt32(drTagging["toUserId"].ToString());
                        objtagging.FirstName = drTagging["FirstName"].ToString();
                        objtagging.LastName = drTagging["LastName"].ToString();
                        getFeedReaction.taggingslst.Add(objtagging);
                    }

                    getFeedReaction.takenScorelst = new List<takenScore>();
                    DataRow[] dataRowstakenScore = ds.Tables[6].Select("feedid=" + dr["feedid"].ToString());
                    foreach (DataRow drTagging in dataRowstakenScore)
                    {
                        takenScore objtakenby = new takenScore();
                        objtakenby.userid = Convert.ToInt32(drTagging["UserID"].ToString());
                        objtakenby.FirstName = drTagging["FirstName"].ToString();
                        objtakenby.LastName = drTagging["LastName"].ToString();
                        objtakenby.UserProfilePic = drTagging["UserProfilePic"].ToString();
                        getFeedReaction.takenScorelst.Add(objtakenby);
                    }


                    getFeedReactions.Add(getFeedReaction);
                }



                data = JsonConvert.SerializeObject(getFeedReactions);
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                // throw e;
            }
            finally
            {
                objLog.Response = "Success";
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }


        public string GetNotificationforCharactify(int userId, string currentUserId)
        {


            string Data = string.Empty;
            int RequestID = 0;
            string res = null;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetNotificationforCharactify";
            objLog.Request = JsonConvert.SerializeObject(userId).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserID", userId.ToString(), "INT", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetNotificationforCharactify_sp", arrList);
                if (ds.Tables.Count > 0)
                {
                    DataTable dt = new DataTable();
                    dt = ds.Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        Usercomments userComments = new Usercomments();
                        string[] userDeviceId = dr["token"].ToString().Split(",");
                        if (userDeviceId[0] != null)
                        {
                            res = SendPushNotification(userDeviceId[0].ToString(), userDeviceId[1].ToString());
                        }

                    }
                    res = "1";
                }
                // return res;

                Data = JsonConvert.SerializeObject(res);
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = Data;
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return Data;
        }

        public string saveVideoByIos(FeedRequest objaf)
        {
            // string fn = System.IO.Path.GetFileName();
            // string SaveLocation = Server.MapPath("Data") + "\\" + fn;

            FileInfo file = null;
            string filename = null;
            Guid obj = Guid.NewGuid();
            string path = Directory.GetCurrentDirectory();
            baseDir = path + "\\Upload\\";
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }
            // File.
            //string date = DateTime.Now.ToString().Replace(@"/", @"_").Replace(@":", @"_").Replace(@" ", @"_");
            //FileInfo SaveLocation = new FileInfo(baseDir + Convert.ToString(objaf.FromUserID) + "_" + Convert.ToString(objaf.FeedID) + "_" + obj.ToString() + objaf.FileType);
            // file.PostedFile.SaveAs(baseDir);
            //filename = Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + obj.ToString() + Fileformat;
            return filename;
        }

        public string uploadVideoOnServerByIos(string LocalFilePath, int userid, int Feedid, string Fileformat)
        {

            string filename = null;

            Guid obj = Guid.NewGuid();
            string path = Directory.GetCurrentDirectory();
            baseDir = path + "\\Upload\\";
            if (!Directory.Exists(baseDir))
            {
                Directory.CreateDirectory(baseDir);
            }
            byte[] Bytes = null;
            // File.ReadAllBytes()
            //using (StreamReader sr = new StreamReader(LocalFilePath))
            //{
            //    String AsString = sr.ReadToEnd();
            //    Bytes = new byte[AsString.Length];
            //}
            Bytes = File.ReadAllBytes(LocalFilePath);
            string date = DateTime.Now.ToString().Replace(@"/", @"_").Replace(@":", @"_").Replace(@" ", @"_");
            FileInfo fil = new FileInfo(baseDir + Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + obj.ToString() + Fileformat);
            // ret= Compress(ret);
            using (Stream sw = fil.OpenWrite())
            {
                sw.Write(Bytes, 0, Bytes.Length);
                sw.Close();
            }

            filename = Convert.ToString(userid) + "_" + Convert.ToString(Feedid) + "_" + obj.ToString() + Fileformat;
            return filename;
        }

        public int AddVideoForIos(FeedRequest objaf, string currentUserId)
        {
            int feedid = 0;
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "AddVideoForIos";
            objLog.Request = JsonConvert.SerializeObject(objaf).ToString();
            try
            {
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {

                        var ud = new Models.FeedMaster()
                        {
                            FromUserId = objaf.FromUserID,
                            ToUserId = objaf.ToUserID,
                            FeedType = objaf.FeedType,
                            FileType = objaf.FileType,
                            Description = objaf.Description,
                            CreatedDate = Currentdatetime,
                            CreatedBy = objaf.FromUserID
                        };
                        db.FeedMaster.Add(ud);
                        db.SaveChanges();
                        feedid = ud.FeedId;
                        if (objaf.FileType == "text")
                        {
                            ret = ud.FeedId;
                        }
                        foreach (var uwdr in objaf.feedImagePathslst)
                        {
                            string filename = null;
                            if (uwdr.FileType == "video")
                            {
                                filename = uploadVideoOnServerByIos(uwdr.filePath, objaf.FromUserID, feedid, uwdr.Fileformat);
                            }
                            else
                            {
                                filename = Base64ToImage1(uwdr.filePath, objaf.FromUserID, feedid, uwdr.Fileformat);
                            }

                            var u = new Models.FeedImagePath()
                            {
                                FeedId = feedid,
                                ImagePath = @"\Upload\" + filename,
                                Filter = uwdr.Filter,
                                Description = uwdr.Description,
                                CreatedDate = Currentdatetime,
                                CreatedBy = objaf.FromUserID
                            };
                            db.FeedImagePath.Add(u);
                            ret = db.SaveChanges();
                        }

                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                            foreach (var uwdr in objaf.taggingslst)
                            {
                                var ut = new Models.Tagging()
                                {
                                    FeedId = feedid,
                                    UserId = uwdr.Touserid,
                                    //Description = uwdr.Description,
                                    CreatedDate = Currentdatetime,
                                    CreatedBy = uwdr.userid.ToString()
                                };
                                db.Tagging.Add(ut);
                                ret = db.SaveChanges();
                                addNotification(uwdr.userid, uwdr.Touserid, "Tagging", feedid, feedid);
                            }
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.Response = ex.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(ex.ToString());
                throw ex;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                //ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var res = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }


        public int DeActivateAccount(UserDetailsRequest Obj, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "DeActivateAccount";
            objLog.Request = JsonConvert.SerializeObject(Obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        var userMaster = (from usr in db.UserMaster where (usr.UserId == Obj.UserID) && (usr.Status == "A") select usr).FirstOrDefault();
                        if (userMaster != null)
                        {
                            userMaster.Status = "D";
                            userMaster.ModifiedBy = Obj.UserID;
                            userMaster.ModifiedDate = Currentdatetime;
                            db.UserMaster.Update(userMaster);
                            db.SaveChanges();
                            ret = userMaster.UserId;
                        }
                        else
                        {
                            ret = -1; //user not found 
                        }
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }

                    }

                }

            }
            catch (Exception e)
            {
                ret = -1;
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                // throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;
        }

        public int UserPrivacyDetails(UserPrivacyDetails Obj, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "UserPrivacyDetails";
            objLog.Request = JsonConvert.SerializeObject(Obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        var Upd = (from usr in db.UserPrivacyDetails where (usr.UserId == Obj.UserId) select usr).FirstOrDefault();
                        if (Upd != null)
                        {
                            if (Obj.Categorycode == 1)
                            {
                                Upd.FamilyStaus = Obj.Staus;
                            }
                            else if (Obj.Categorycode == 2)
                            {
                                Upd.FriendsStaus = Obj.Staus;
                            }
                            else if (Obj.Categorycode == 3)
                            {
                                Upd.CoWorkersStaus = Obj.Staus;
                            }
                            else if (Obj.Categorycode == 4)
                            {
                                Upd.AcquaintancesStaus = Obj.Staus;
                            }
                            else if (Obj.Categorycode == 5)
                            {
                                Upd.AllCategory = Obj.Staus;
                            }


                            Upd.ModifiedBy = Obj.UserId;
                            Upd.ModifiedDate = Currentdatetime;
                            db.UserPrivacyDetails.Update(Upd);
                            db.SaveChanges();
                            ret = Upd.Id;
                        }
                        else
                        {

                            var u = new Models.UserPrivacyDetails()
                            {
                                UserId = Obj.UserId,
                                FamilyStaus = Obj.FamilyStaus,
                                FriendsStaus = Obj.FriendsStaus,
                                CoWorkersStaus = Obj.CoWorkersStaus,
                                AcquaintancesStaus = Obj.AcquaintancesStaus,
                                AllCategory = Obj.AllCategory,
                                CreatedDate = Currentdatetime,
                                CreatedBy = Obj.UserId,


                            };
                            db.UserPrivacyDetails.Add(u);
                            db.SaveChanges();
                            ret = u.Id;
                        }
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ret = -1;
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                // throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;

        }

        public int RemoveTagging(GetFeedList Obj, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "RemoveTagging";
            objLog.Request = JsonConvert.SerializeObject(Obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        var Tag = (from usr in db.Tagging where (usr.UserId == Obj.UserId) && (usr.FeedId == Obj.FeedId) select usr).FirstOrDefault();
                        if (Tag != null)
                        {
                            db.Tagging.Remove(Tag);
                            db.SaveChanges();
                            ret = Tag.Id;
                        }
                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ret = -1;
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
               // throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;

        }

        public string getUserPrivacyDetails(UserPrivacyDetails Objreq, string currentUserId)
        {
            string data = string.Empty;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "getUserPrivacyDetails";
            objLog.Request = JsonConvert.SerializeObject(Objreq).ToString();
            objLog.currentUserId = currentUserId;
            try
            {
                using (var db = new TPContext())
                {
                    DataTable dt = new DataTable();
                    string connStr = CResources.GetConnectionString();
                    DataSet ds = new DataSet();
                    SearchList searchList = new SearchList();

                    using (SqlConnection connect = new SqlConnection(connStr))
                    {
                        connect.Open();
                        SqlCommand cmd = new SqlCommand("getUserPrivacyDetails_SP", connect)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        cmd.Parameters.AddWithValue("@UserId", Objreq.UserId);
                        SqlDataAdapter adap = new SqlDataAdapter(cmd);
                        adap.Fill(ds);

                    }
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ds.Tables[0].TableName = "UserPrivacyDetails";
                        Manager mn = new Manager();
                        // mn.Response(data.ToString());
                        data = JsonConvert.SerializeObject(ds);

                    }
                    else
                    {
                        data = null;
                    }

                }
            }
            catch (Exception e)
            {
                data = "-1";
                Manager mn = new Manager();
                mn.LogError(e.ToString());
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
               // throw e;
            }
            finally
            {
                Manager mn = new Manager();
                mn.Request(JsonConvert.SerializeObject(Objreq).ToString());
                objLog.Response = data.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public void pushMessage(string deviceID)
        {
            int port = 2195;
            string hostname = "gateway.sandbox.push.apple.com";
            //  String hostname = "gateway.push.apple.com";
            String certificatePath = @"C:\Ravi\development\dev p12-pem file\Certificates.p12";
            X509Certificate2 clientCertificate = new X509Certificate2(System.IO.File.ReadAllBytes(certificatePath), "charactify");
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);

            TcpClient client = new TcpClient(hostname, port);
            SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);

            try
            {
                // client.Connect(hostname,port);
                //sslStream.AuthenticateAsServer(hostname, certificatesCollection, SslProtocols.Default, true);
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                // sslStream.AuthenticateAsServer(clientCertificate, false, SslProtocols.Ssl3, true);
                MemoryStream memoryStream = new MemoryStream();
                BinaryWriter writer = new BinaryWriter(memoryStream);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((byte)32);

                //writer.Write(ConvertToByteArray(deviceID));
                String payload = "{\"aps\":{\"alert\":\"" + "Hi,, This Is a Sample Push Notification For IPhone.." + "\",\"badge\":1,\"sound\":\"default\"}}";
                writer.Write((byte)0);
                writer.Write((byte)payload.Length);
                byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
                writer.Write(b1);
                writer.Flush();
                byte[] array = memoryStream.ToArray();
                sslStream.Write(array);
                sslStream.Flush();
                client.Close();
            }
            catch (System.Security.Authentication.AuthenticationException ex)
            {
                client.Close();
            }
            catch (Exception e)
            {
                client.Close();
            }
        }


        public static byte[] ConvertToByteArray(string value)
        {
            byte[] bytes = null;
            if (String.IsNullOrEmpty(value))
                bytes = null;
            else
            {
                int string_length = value.Length;
                int character_index = (value.StartsWith("0x", StringComparison.Ordinal)) ? 2 : 0; // Does the string define leading HEX indicator '0x'. Adjust starting index accordingly.
                int number_of_characters = string_length - character_index;

                bool add_leading_zero = false;
                if (0 != (number_of_characters % 2))
                {
                    add_leading_zero = true;

                    number_of_characters += 1;  // Leading '0' has been striped from the string presentation.
                }

                bytes = new byte[number_of_characters / 2]; // Initialize our byte array to hold the converted string.

                int write_index = 0;
                if (add_leading_zero)
                {
                    bytes[write_index++] = FromCharacterToByte(value[character_index], character_index);
                    character_index += 1;
                }

                for (int read_index = character_index; read_index < value.Length; read_index += 2)
                {
                    byte upper = FromCharacterToByte(value[read_index], read_index, 4);
                    byte lower = FromCharacterToByte(value[read_index + 1], read_index + 1);

                    bytes[write_index++] = (byte)(upper | lower);
                }
            }

            return bytes;
        }

        private static byte FromCharacterToByte(char character, int index, int shift = 0)
        {
            byte value = (byte)character;
            if (((0x40 < value) && (0x47 > value)) || ((0x60 < value) && (0x67 > value)))
            {
                if (0x40 == (0x40 & value))
                {
                    if (0x20 == (0x20 & value))
                        value = (byte)(((value + 0xA) - 0x61) << shift);
                    else
                        value = (byte)(((value + 0xA) - 0x41) << shift);
                }
            }
            else if ((0x29 < value) && (0x40 > value))
                value = (byte)((value - 0x30) << shift);
            else
                throw new InvalidOperationException(String.Format("Character '{0}' at index '{1}' is not valid alphanumeric character.", character, index));

            return value;
        }


        public static bool ValidateServerCertificate(
           object sender,
           X509Certificate certificate,
           X509Chain chain,
           SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);

            // Do not allow this client to communicate with unauthenticated servers.
            return false;
        }

        private byte[] HexString2Bytes(String hexString)
        {
            //check for null
            if (hexString == null) return null;
            //get length
            int len = hexString.Length;
            if (len % 2 == 1) return null;
            int len_half = len / 2;
            //create a byte array
            byte[] bs = new byte[len_half];
            try
            {
                //convert the hexstring to bytes
                for (int i = 0; i != len_half; i++)
                {
                    bs[i] = (byte)Int32.Parse(hexString.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Exception : " + ex.Message);
            }
            //return the byte array
            return bs;
        }

        public void pushios(String deviceId)
        {
            int port = 2195;
            String hostname = "gateway.sandbox.push.apple.com";

            //load certificate
            string certificatePath = @"C:\Ravi\development\dev p12-pem file\Certificates.p12";
            string certificatePassword = "charactify";
            X509Certificate2 clientCertificate = new X509Certificate2(certificatePath, certificatePassword);
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);

            TcpClient client = new TcpClient(hostname, port);
            SslStream sslStream = new SslStream(
                    client.GetStream(),
                    false,
                    new RemoteCertificateValidationCallback(ValidateServerCertificate),
                    null
            );

            try
            {
                sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Tls, true);
            }
            catch (AuthenticationException ex)
            {
                client.Close();
                return;
            }

            // Encode a test message into a byte array.
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);

            writer.Write((byte)0);  //The command
            writer.Write((byte)0);  //The first byte of the deviceId length (big-endian first byte)
            writer.Write((byte)32); //The deviceId length (big-endian second byte)
                                    //byte[] Bytearray =  ConvertToByteArray(deviceId.ToUpper());
                                    // String deviceId = "DEVICEIDGOESHERE";
            writer.Write(HexString2Bytes(deviceId.ToUpper()));

            String payload = "{\"aps\":{\"alert\":\"I like spoons also\",\"badge\":14}}";

            writer.Write((byte)0); //First byte of payload length; (big-endian first byte)
            writer.Write((byte)payload.Length); //payload length (big-endian second byte)

            byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
            writer.Write(b1);
            writer.Flush();

            byte[] array = memoryStream.ToArray();
            sslStream.Write(array);
            sslStream.Flush();

            // Close the client connection.
            client.Close();
            PendingNotification(deviceId);
        }

        public void PendingNotification(String DeviceToken)
        {
            int port = 2195;
            String payload = null;
            String hostname = "gateway.sandbox.push.apple.com";
            string certificatePath = @"C:\Ravi\development\dev p12-pem file\Certificates.p12";
            string certificatePassword = "charactify";
            //String certificatePassword = "Password";
            //string certificatePath = "cert.p12";
            TcpClient client = new TcpClient(hostname, port);
            X509Certificate2 clientCertificate = new X509Certificate2(System.IO.File.ReadAllBytes(certificatePath), certificatePassword);
            X509Certificate2Collection certificatesCollection = new X509Certificate2Collection(clientCertificate);
            SslStream sslStream = new SslStream(client.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            sslStream.AuthenticateAsClient(hostname, certificatesCollection, SslProtocols.Tls, false);
            // String DeviceToken = "5f6b09edd88ae928e877d1d74d60ab92beca337e946d4932588768d1679f60d5";
            String LoginName = "Name";
            int NotificationId = 1;
            int Counter = 0;//Badge Count;  
            String Message = "your choice Message";
            String UID = "your choice UID";
            payload = "{\"aps\":{\"alert\":\"" + Message + "\",\"badge\":" + Counter + ",\"sound\":\"default\"},\"UID\":\"" + UID + "\",\"LoginName\":\"" + LoginName + "\"}";
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)32);
            writer.Write(HexString2Bytes(DeviceToken.ToUpper()));
            writer.Write((byte)0);
            writer.Write((byte)payload.Length);
            byte[] b1 = System.Text.Encoding.UTF8.GetBytes(payload);
            writer.Write(b1);
            writer.Flush();
            byte[] array = memoryStream.ToArray();
            sslStream.Write(array);
            iospush(DeviceToken);
        }

        public void iospush(string DeviceId)
        {
            int port = 2195;
            string hostname = "gateway.sandbox.push.apple.com";
            string Message = "ios noti", Content = null;

            // I have removed certificate. Keep your certificate in wwwroot / certificate location.This location is not mandatory
            //  string certificatePath = $@"{_env.WebRootPath}\Certificate\YOUR_CERTIFICATE.pfx";
            string certificateFilePath = @"C:\Ravi\development\dev p12-pem file\Certificates.p12";

            //var certificatePassword = ""; // We keep password empty
            //var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Sandbox, certificateFilePath, certificatePassword);
            //var broker = new ApnsServiceBroker(config);
            //broker.OnNotificationFailed += (notification, exception) =>
            //{
            //    failed++;
            //};
            //broker.OnNotificationSucceeded += (notification) =>
            //{
            //    succeeded++;
            //};
            //broker.Start();

            //foreach (var dt in Settings.Instance.ApnsDeviceTokens)
            //{
            //    attempted++;
            //    broker.QueueNotification(new ApnsNotification
            //    {
            //        DeviceToken = dt,
            //        Payload = JObject.Parse("{ \"aps\" : { \"alert\" : \"Hello PushSharp!\" } }")
            //    });
            //}

            //broker.Stop();

            var Applecertificate = System.IO.File.ReadAllBytes("C:\\Ravi\\development\\dev p12-pem file\\Certificates.p12");
            var certificate = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Sandbox, Applecertificate, "charactify");
            var ApnsBroker = new ApnsServiceBroker(certificate);
            ApnsBroker.OnNotificationFailed += (notification, exception) =>
            {
                //exception.Handle(ex =>
                //{


                //}

                // )
            };

            ApnsBroker.OnNotificationSucceeded += (notification) =>
            {

            };
            ApnsBroker.Start();
            ApnsBroker.QueueNotification(new ApnsNotification
            {
                DeviceToken = DeviceId,
                Payload = JObject.Parse("{ \"aps\" : { \"alert\" : \"Hello PushSharp!\" } }")
            }
                );
            ApnsBroker.Stop();
        }

        public int Updatetagging(FeedRequest feedRequest, string currentUserId)
        {
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "Updatetagging";
            objLog.Request = JsonConvert.SerializeObject(feedRequest).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        var Tag = (from usr in db.Tagging where (usr.FeedId == feedRequest.FeedID) select usr).ToList();
                        ret = 0;
                        if (Tag.Count > 0)
                        {
                            foreach (var i in Tag)
                            {
                                db.Tagging.Remove(i);
                                db.SaveChanges();
                                ret = i.Id;
                            }
                        }

                        if (ret >= 0)
                        {
                            int id = 0;
                            // dbContextTransaction.Commit();
                            foreach (var res in feedRequest.taggingslst)
                            {
                                var ut = new Models.Tagging()
                                {
                                    FeedId = feedRequest.FeedID,
                                    UserId = res.userid,
                                    CreatedDate = Currentdatetime,
                                    CreatedBy = feedRequest.FromUserID.ToString()
                                };
                                db.Tagging.Add(ut);
                                db.SaveChanges();
                                id = ut.Id;
                                ret = id;
                            }
                            if ((id > 0) || (ret > 0))
                            {
                                dbContextTransaction.Commit();
                            }
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ret = -1;
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
                //throw e;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return ret;

        }

        public async Task WriteContentToStream(Stream outputStream, HttpContent content, TransportContext transportContext)
        {
            //path of file which we have to read//  

            //var filePath = HttpContext.Current.Server.MapPath("~/MicrosoftBizSparkWorksWithStartups.mp4");
            var filePath = "https://www.charactify.net/upload/2513_1989_c4b2db5c-efa0-4f93-a38f-0d7f489fddbc.mp4";
            //here set the size of buffer, you can set any size  
            int bufferSize = 1000;
            byte[] buffer = new byte[bufferSize];
            //here we re using FileStream to read file from server//  
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int totalSize = (int)fileStream.Length;
                /*here we are saying read bytes from file as long as total size of file 
 
                is greater then 0*/
                while (totalSize > 0)
                {
                    int count = totalSize > bufferSize ? bufferSize : totalSize;
                    //here we are reading the buffer from orginal file  
                    int sizeOfReadedBuffer = fileStream.Read(buffer, 0, count);
                    //here we are writing the readed buffer to output//  
                    await outputStream.WriteAsync(buffer, 0, sizeOfReadedBuffer);
                    //and finally after writing to output stream decrementing it to total size of file.  
                    totalSize -= sizeOfReadedBuffer;
                }
            }
        }

        //public string GetVideoContent()
        //{
        //    string res = null;
        //    Task.Run(() =>
        //    {
        //        new PushStreamContent((Action<Stream, HttpContent, TransportContext>)WriteContentToStream); ;
        //    });

        //    return res;
        //}

        public string GetFeedListForNode(GetFeedList obj, string currentUserId)
        {
            List<GetFeedRections> getFeedReactions = new List<GetFeedRections>();
            List<UserRectionsType> getUserRectionsType = new List<UserRectionsType>();

            string data = string.Empty;
            //  Scores usr = new Scores();
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetFeedListForIos";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserId", obj.UserId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageNo", obj.pageNo.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageSize", obj.pageSize.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Story", obj.Story.ToString(), "VARCHAR", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "getFeedWithReactions_SP", arrList);
                //if (ds.Tables.Count > 0)
                //{
                //    ds.Tables[0].TableName = "Feed";
                //    ds.Tables[1].TableName = "FeedReactions";
                //}
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    GetFeedRections getFeedReaction = new GetFeedRections();
                    getFeedReaction.UserID = Convert.ToInt32(dr["createdby"].ToString());
                    getFeedReaction.feedID = Convert.ToInt32(dr["feedid"].ToString());
                    getFeedReaction.feedType = dr["feedtype"].ToString();
                    getFeedReaction.Name = dr["Name"].ToString();
                    getFeedReaction.UserProfilePic = dr["ProfilePic"].ToString();
                    getFeedReaction.description = dr["Description"].ToString();
                    getFeedReaction.filePath = dr["FilePath"].ToString();
                    getFeedReaction.createdDate = dr["CreatedDate"].ToString();
                    getFeedReaction.noRections = Convert.ToInt32(dr["React_count"].ToString());
                    getFeedReaction.noComments = Convert.ToInt32(dr["Comments_count"].ToString());
                    getFeedReaction.noShares = Convert.ToInt32(dr["Share_count"].ToString());
                    getFeedReaction.FirstName = dr["firstname"].ToString();
                    getFeedReaction.LastName = dr["LastName"].ToString();
                    DataRow[] dataRowsCurReactions = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + "  and CreatedBy='" + obj.UserId.ToString() + "'  and Name <>'CM'  ");
                    foreach (DataRow drReaction in dataRowsCurReactions)
                    {
                        getFeedReaction.currentUserRection = drReaction["Name"].ToString();
                    }
                    getFeedReaction.userRectionsTypeLst = new List<UserRectionsType>();
                    getFeedReaction.filePathlst = new List<filePath>();

                    if (getFeedReaction.feedType == "Charactify")
                    {
                        getFeedReaction.charactifyScores = new List<CharactifyScore>();

                        DataRow[] dataRorschar = ds.Tables[4].Select("Crid=" + dr["Crid"].ToString());
                        foreach (DataRow dataRorsch in dataRorschar)
                        {
                            CharactifyScore charactifyScore = new CharactifyScore();
                            charactifyScore.Score = dataRorsch["Score"].ToString();
                            charactifyScore.TraitsID = dataRorsch["TraitsID"].ToString();
                            getFeedReaction.charactifyScores.Add(charactifyScore);
                        }
                    }
                    else
                    {
                        DataRow[] dataRorspath = ds.Tables[2].Select("feedid=" + dr["feedid"].ToString() + " and feedtype<>'Charactify'");
                        //DataRow[] dataRorspath = ds.Tables[2].Select("feedid=" + dr["feedid"].ToString());
                        foreach (DataRow dataRorspa in dataRorspath)
                        {
                            filePath filepath = new filePath();
                            filepath.filter = dataRorspa["Filter"].ToString();
                            filepath.Description = dataRorspa["Description"].ToString();
                            // filepath.Path = URL + c["ImagePath"].ToString();
                            if (getFeedReaction.feedType == "video")
                            {
                                string myString = dataRorspa["ImagePath"].ToString();
                                myString = myString.Remove(0, 1);
                                filepath.Path = UrlNew + myString;
                            }
                            else
                            {
                                filepath.Path = URL + dataRorspa["ImagePath"].ToString();
                            }
                            getFeedReaction.filePathlst.Add(filepath);

                        }
                    }
                    //DataRow[] data1 = ds.Tables[3].Select(y => y.feedid = dr["feedid"].ToString());
                    DataRow[] dataRorsRec = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + " and Name<>'CM' ");
                    var x = (from r in dataRorsRec.AsEnumerable()
                             select new
                             {
                                 feedid = r.Field<int>("feedid"),
                                 Name = r.Field<string>("Name")
                             }).Distinct().ToList();
                    foreach (var i in x)
                    {
                        UserRectionsType userRectionsType = new UserRectionsType();

                        userRectionsType.ReactionType = i.Name.ToString();

                        userRectionsType.userRectionsLst = new List<UserRections>();

                        DataRow[] dataRowsReactions = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + "  and Name='" + i.Name.ToString() + "'  and Name <>'CM'  ");
                        foreach (DataRow drReaction in dataRowsReactions)
                        {
                            UserRections userRections = new UserRections();
                            userRections.ReactionType = drReaction["Name"].ToString();
                            userRections.UserID = Convert.ToInt32(drReaction["CreatedBy"].ToString());
                            userRections.ReactionID = Convert.ToInt32(drReaction["ReactionID"].ToString());
                            userRections.feedID = Convert.ToInt32(drReaction["feedid"].ToString());
                            userRections.Name = drReaction["UName"].ToString();
                            userRections.UserProfilePic = drReaction["ProfilePic"].ToString();
                            userRections.FirstName = drReaction["FirstName"].ToString();
                            userRections.LastName = drReaction["LastName"].ToString();

                            userRectionsType.userRectionsLst.Add(userRections);
                        }
                        //getFeedReaction.userRectionsLst.add
                        getFeedReaction.userRectionsTypeLst.Add(userRectionsType);
                    }
                    getFeedReaction.usercommentLst = new List<Usercomments>();
                    DataRow[] dataRowsComment = ds.Tables[1].Select("feedid=" + dr["feedid"].ToString() + " and Name='CM' ");
                    foreach (DataRow drReaction in dataRowsComment)
                    {
                        Usercomments userComments = new Usercomments();
                        userComments.UserID = Convert.ToInt32(drReaction["createdby"].ToString());
                        userComments.ReactionID = Convert.ToInt32(drReaction["ReactionID"].ToString());
                        userComments.feedID = Convert.ToInt32(drReaction["feedid"].ToString());
                        userComments.Name = drReaction["UName"].ToString();
                        userComments.ReactionType = drReaction["Name"].ToString();
                        userComments.Description = drReaction["description"].ToString();
                        userComments.UserProfilePic = drReaction["ProfilePic"].ToString();
                        userComments.createdDate = drReaction["CreatedDate"].ToString();
                        getFeedReaction.usercommentLst.Add(userComments);
                    }
                    getFeedReaction.taggingslst = new List<tagging>();
                    DataRow[] dataRowstagging = ds.Tables[5].Select("feedid=" + dr["feedid"].ToString());
                    foreach (DataRow drTagging in dataRowstagging)
                    {
                        tagging objtagging = new tagging();
                        objtagging.userid = Convert.ToInt32(dr["createdby"].ToString());
                        objtagging.Touserid = Convert.ToInt32(drTagging["toUserId"].ToString());
                        objtagging.FirstName = drTagging["FirstName"].ToString();
                        objtagging.LastName = drTagging["LastName"].ToString();
                        getFeedReaction.taggingslst.Add(objtagging);
                    }

                    getFeedReaction.takenScorelst = new List<takenScore>();
                    DataRow[] dataRowstakenScore = ds.Tables[6].Select("feedid=" + dr["feedid"].ToString());
                    foreach (DataRow drTagging in dataRowstakenScore)
                    {
                        takenScore objtakenby = new takenScore();
                        objtakenby.userid = Convert.ToInt32(drTagging["UserID"].ToString());
                        objtakenby.FirstName = drTagging["FirstName"].ToString();
                        objtakenby.LastName = drTagging["LastName"].ToString();
                        objtakenby.UserProfilePic = drTagging["UserProfilePic"].ToString();
                        getFeedReaction.takenScorelst.Add(objtakenby);
                    }


                    getFeedReactions.Add(getFeedReaction);
                }



                data = JsonConvert.SerializeObject(getFeedReactions);
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
               // throw e;
            }
            finally
            {
                objLog.Response = "Success";
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public int AddFeedForNode(FeedRequest objaf, string currentUserId)
        {
            int feedid = 0;
            int ret = -1;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "AddFeedForIos";
            objLog.Request = JsonConvert.SerializeObject(objaf).ToString();
            objLog.currentUserId = currentUserId;
            //dynamic res =null;
            //RequestID = Convert.ToInt32(res);
            //RequestID = Task.Run(ResponseLog1(objLog));
            try
            {

                // 
                using (var db = new TPContext())
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {

                        var ud = new Models.FeedMaster()
                        {
                            FromUserId = objaf.FromUserID,
                            ToUserId = objaf.ToUserID,
                            FeedType = objaf.FeedType,
                            FileType = objaf.FileType,
                            Description = objaf.Description,
                            CreatedDate = Currentdatetime,
                            CreatedBy = objaf.FromUserID
                        };
                        db.FeedMaster.Add(ud);
                        db.SaveChanges();
                        feedid = ud.FeedId;
                        if (objaf.FileType == "text")
                        {
                            ret = ud.FeedId;
                        }
                        foreach (var uwdr in objaf.feedImagePathslst)
                        {
                            string filename = null;
                            if (uwdr.FileType == "video")
                            {
                                //filename = ConvertToVideo(uwdr.filePath, objaf.FromUserID, feedid, uwdr.Fileformat);
                                filename = ConvertToVideoFornode(uwdr.filePath, objaf.FromUserID, feedid, uwdr.Fileformat);
                                // mn.LogError(uwdr.filePath);
                            }
                            else
                            {
                                filename = Base64ToImage1(uwdr.filePath, objaf.FromUserID, feedid, uwdr.Fileformat);
                            }

                            var u = new Models.FeedImagePath()
                            {
                                FeedId = feedid,
                                ImagePath = @"\Upload\" + filename,
                                Filter = uwdr.Filter,
                                Description = uwdr.Description,
                                CreatedDate = Currentdatetime,
                                CreatedBy = objaf.FromUserID
                            };
                            db.FeedImagePath.Add(u);
                            ret = db.SaveChanges();
                        }

                        if (ret > 0)
                        {
                            dbContextTransaction.Commit();
                            foreach (var uwdr in objaf.taggingslst)
                            {
                                var ut = new Models.Tagging()
                                {
                                    FeedId = feedid,
                                    UserId = uwdr.Touserid,
                                    //Description = uwdr.Description,
                                    CreatedDate = Currentdatetime,
                                    CreatedBy = uwdr.userid.ToString()
                                };
                                db.Tagging.Add(ut);
                                ret = db.SaveChanges();
                                addNotification(uwdr.userid, uwdr.Touserid, "Tagging", feedid, objaf.FeedID);
                            }
                        }
                        else
                        {
                            dbContextTransaction.Rollback();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                objLog.Response = ex.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(ex.ToString());
                throw ex;
            }
            finally
            {
                objLog.Response = ret.ToString();
                objLog.LogId = RequestID;
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var res = RequestLog1(objLog);
                    });
                }
            }
            return ret;


        }

        public string GetStoryListForNode(GetFeedList obj, string currentUserId)
        {
            List<Story> storylst = new List<Story>();
            string data = string.Empty;
            int RequestID = 0;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "GetStoryListForNode";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {
                string connStr = CResources.GetConnectionString();
                ArrayList arrList = new ArrayList();
                SP.spArgumentsCollection(arrList, "@UserId", obj.UserId.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageNo", obj.pageNo.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@pageSize", obj.pageSize.ToString(), "INT", "I");
                SP.spArgumentsCollection(arrList, "@Story", obj.Story.ToString(), "VARCHAR", "I");
                DataSet ds = new DataSet();
                ds = SP.RunStoredProcedure(connStr, ds, "GetStory_Sp", arrList);
                foreach (DataRow drStory in ds.Tables[1].Rows)
                {
                    Story story = new Story();
                    List<Story> storieslst = new List<Story>();
                    Usercomments userComments = new Usercomments();
                    story.Userid = Convert.ToInt32(drStory["UserID"].ToString());
                    story.UserName = drStory["UserName"].ToString();
                    story.UserProfilePic = drStory["UserProfilePic"].ToString();
                    story.FristName = drStory["FirstName"].ToString();
                    story.LastName = drStory["LastName"].ToString();
                    story.items = new List<Items>();
                    DataRow[] dataRorsItem = ds.Tables[0].Select("FromUserID=" + drStory["UserID"].ToString());
                    foreach (DataRow drItem in dataRorsItem)
                    {
                        Items items = new Items();
                        items.id = Convert.ToInt32(drItem["Id"].ToString());
                        // long timestamp = ConvertToTimestamp(Convert.ToDateTime(drItem["Createddate"].ToString()));
                        items.Date = drItem["Createddate"].ToString();
                        // items.Date = drItem["Createddate"].ToString();
                        items.FileType = drItem["FileType"].ToString();
                        if (items.FileType == "video")
                        {
                            string myString = drItem["ImagePath"].ToString();
                            myString = myString.Remove(0, 1);
                            items.Path = UrlNew + myString;
                        }
                        else
                        {
                            items.Path = URL + drItem["ImagePath"].ToString();
                        }

                        items.Description = drItem["Description"].ToString();
                        story.items.Add(items);
                    }
                    storylst.Add(story);
                }


                data = JsonConvert.SerializeObject(storylst);
            }
            catch (Exception e)
            {
                objLog.Response = e.ToString();
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                mn.LogError(e.ToString());
                throw e;
            }
            finally
            {
                objLog.Response = "Success";
                objLog.LogId = RequestID;
                ResponseLog(objLog);
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return data;
        }

        public string base64(FeedRequest objaf, string currentUserId)
        {
            string filename = objaf.FileType;
            try
            {

                string date = DateTime.Now.ToString().Replace(@"/", @"_").Replace(@":", @"_").Replace(@" ", @"_");
                string baseDir = @"C:\Users\gNxt007\Desktop\Image\", Fileformat = ".mp4";
                string path = Directory.GetCurrentDirectory();
                baseDir = path + "\\Video\\";
                using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {

                    // Create a byte array of file stream length
                    byte[] bytes = System.IO.File.ReadAllBytes(filename);
                    //Read block of bytes from stream into the byte array
                    fs.Read(bytes, 0, System.Convert.ToInt32(fs.Length));
                    //Close the File Stream
                    fs.Close();
                    string base64String = Convert.ToBase64String(bytes);
                    byte[] Bytes = Convert.FromBase64String(base64String);
                    FileInfo fil = new FileInfo(baseDir + Convert.ToString(21) + "_" + Convert.ToString(71) + "_" + date.ToString() + Fileformat);
                    mn.LogError(fil.ToString());
                    using (Stream sw = fil.OpenWrite())
                    {
                        sw.Write(Bytes, 0, Bytes.Length);
                        sw.Close();
                    }
                    {
                        Task.Run(() =>
                        {
                            var resul = ConvertMp4Video(filename);
                        }
                    );
                    }

                }
                mn.LogError(objaf.FileType.ToString());

            }
            catch (Exception e)
            {
                mn.LogError(e.ToString());
            }
            return filename;
        }

        public string sendinvitation(string mobileNumber, string TxtMsg)
        {
            mobileNumber = mobileNumber.Replace(" ", String.Empty);
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "sendinvitation";

            string Res = null;
            //Your authentication key

            string authKey = "321791AXvGbcQgSoqf5e61f03cP1";

            //Sender ID,While using route4 sender id should be 6 characters long.

            string senderId = "Charac";
            //Your message to send, Add URL encoding here.

            string message = HttpUtility.UrlEncode("Join Charactify, see what people truly think of you! Now the fun begins! https://play.google.com/store/apps/details?id=com.app.charactify&hl=en");

            string Otp = HttpUtility.UrlEncode(TxtMsg);
            //Prepare you post parameters

            StringBuilder sbPostData = new StringBuilder();

            sbPostData.AppendFormat("authkey={0}", authKey);

            sbPostData.AppendFormat("&mobiles={0}", mobileNumber.Replace("+", " "));

            sbPostData.AppendFormat("&message={0}", message);

            sbPostData.AppendFormat("&sender={0}", senderId);
            // "default" or 1 for promotion  4 is trans...
            sbPostData.AppendFormat("&route={0}", 4);
            objLog.Request = JsonConvert.SerializeObject(sbPostData).ToString();

            try

            {

                //Call Send SMS API

                string sendSMSUri = "http://api.msg91.com/api/sendhttp.php";
                //string sendSMSUri = "https://control.msg91.com/api/sendotp.php";
                //Create HTTPWebrequest   

                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                //Prepare and Add URL Encoded data

                UTF8Encoding encoding = new UTF8Encoding();

                byte[] data = encoding.GetBytes(sbPostData.ToString());
                //Specify post method

                httpWReq.Method = "POST";

                httpWReq.ContentType = "application/x-www-form-urlencoded";

                httpWReq.ContentLength = data.Length;
                using (Stream stream = httpWReq.GetRequestStream())

                {

                    stream.Write(data, 0, data.Length);

                }
                //Get the response

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream());

                string responseString = reader.ReadToEnd();

                //Close the response

                reader.Close();

                response.Close();

            }

            catch (SystemException ex)

            {

                // MessageBox.Show(ex.Message.ToString());

            }
            finally
            {
                objLog.Response = "Success";
                if (Apilog == true)
                {
                    Task.Run(() =>
                    {
                        var resul = RequestLog1(objLog);
                    });
                }
            }
            return Res;

        }

        public string sendOtp(string mobileNumber, string TxtMsg)
        {
            string Res = null;
            //Your authentication key

            string authKey = "321791AXvGbcQgSoqf5e61f03cP1";
            //Multiple mobiles numbers separated by comma

            //mobileNumber = "9140095326";
            //Sender ID,While using route4 sender id should be 6 characters long.

            string senderId = "Charac";
            //Your message to send, Add URL encoding here.

            string message = HttpUtility.UrlEncode(TxtMsg + " is the verification code (OTP) for your Charactify App. Please do not share with anyone. Thanks!");

            string Otp = HttpUtility.UrlEncode(TxtMsg);
            //Prepare you post parameters

            StringBuilder sbPostData = new StringBuilder();

            sbPostData.AppendFormat("authkey={0}", authKey);

            sbPostData.AppendFormat("&mobiles={0}", mobileNumber);

            sbPostData.AppendFormat("&message={0}", message);

            sbPostData.AppendFormat("&sender={0}", senderId);

            sbPostData.AppendFormat("&otp={0}", Otp);
            sbPostData.AppendFormat("&otp_length={0}", 4);


            // sbPostData.AppendFormat("&route={0}", "default");


            try

            {

                //Call Send SMS API

                //string sendSMSUri = "http://api.msg91.com/api/sendhttp.php";
                string sendSMSUri = "https://control.msg91.com/api/sendotp.php";
                //Create HTTPWebrequest   

                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(sendSMSUri);
                //Prepare and Add URL Encoded data

                UTF8Encoding encoding = new UTF8Encoding();

                byte[] data = encoding.GetBytes(sbPostData.ToString());
                //Specify post method

                httpWReq.Method = "POST";

                httpWReq.ContentType = "application/x-www-form-urlencoded";

                httpWReq.ContentLength = data.Length;
                using (Stream stream = httpWReq.GetRequestStream())

                {

                    stream.Write(data, 0, data.Length);

                }
                //Get the response

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();

                StreamReader reader = new StreamReader(response.GetResponseStream());

                string responseString = reader.ReadToEnd();


                //Close the response

                reader.Close();

                response.Close();

            }

            catch (SystemException ex)

            {

                // MessageBox.Show(ex.Message.ToString());

            }
            return Res;

        }

        public void SendMsg()
        {

            //var client = new RestClient("https://control.msg91.com/api/sendotp.php?authkey=%24authkey&mobile=%24mobile_no&message=%24message&sender=%24senderid&otp_expiry=&otp_length=&country=&otp=%24otp&email=&template=");
            //var request = new RestRequest(Method.POST);
            //IRestResponse response = client.Execute(request);
        }

        public string VerifyEmailOrPhone(VerifyEmailOrPhone obj, string currentUserId)
        {
            int RequestID = 0;
            string Res = null;
            LogRequest objLog = new LogRequest();
            objLog.MethodName = "VerifyEmailOrPhone";
            objLog.Request = JsonConvert.SerializeObject(obj).ToString();
            objLog.currentUserId = currentUserId;
            RequestID = RequestLog(objLog);
            try
            {



            }

            catch (SystemException ex)
            {
            }
            return Res;

        }
    }
}


