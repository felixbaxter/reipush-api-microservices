using Reipush.Api.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Internal;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text;
using Reipush.Api.Entities.User;
using System.Security.Claims;
using System.Runtime.InteropServices.WindowsRuntime;
using System.IO;
using System.Runtime.InteropServices;
using log4net;
using UsersMicroService.Entities;
using UsersMicroService.ViewModels;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;


namespace UsersMicroService.Services
{

    public class UserService
    {
        private readonly ReipushContext _reipushcontext;
        private static readonly ILog log = LogManager.GetLogger(typeof(UserService));
        private static readonly HttpClient HttpClient = new HttpClient();

        private string microservicBaseUrl = string.Empty;

        public UserService(ReipushContext context)
        {
            _reipushcontext = context;
            microservicBaseUrl = GetValueByName("PaymentMicorServiceURL");
        }



        public User GetUserByEmail(string iemail)
        {
            User rUser = new User();

            try
            {
                rUser = _reipushcontext.User
                           .FromSqlRaw("DoesEmailExit @Email",
                            new SqlParameter("Email", iemail))
                           .AsEnumerable()
                           .FirstOrDefault();

            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return rUser;
        }

        public voUserAccountVerify GetUserAccountByEmail(string xemail)
        {

            voUserAccountVerify xreturn = new voUserAccountVerify();
            try
            {
                    xreturn = (from u in _reipushcontext.User
                                    join t in _reipushcontext.UserAccounts
                                    on u.UserId equals t.UserId
                                    where u.Email == xemail
                                    select new voUserAccountVerify
                                    {
                                          AuthNetProfileId = t.AuthNetProfileId,
                                          IsActive = u.IsActive,
                                          IsVerified = u.IsVerified,
                                          UserId = u.UserId
                                    }).First();

            }
            catch (Exception e)
            {
                log.Error(e);
                return xreturn;
            }
            return xreturn;

        }


        public int AuthenticateUser(viEmailPwd iCred)
        {
            int result = 0;
            try
            {
                // Let's get the current user SALT Key
                User iuser = new User();
                iuser.Email = iCred.Email;

                var tUser = _reipushcontext.User
                   .Where(u => u.Email == iCred.Email)
                   .Select(u => new User()
                   {
                       PasswordHash = u.PasswordHash,
                       PasswordSalt = u.PasswordSalt
                   }).ToArray();


                //  Convert the Current Password to Hash via the Salt key
                if (iCred.Password != "")
                {
                    iuser.PasswordSalt = CreateSalt(5);
                    iuser.PasswordHash = CreatePasswordHash(iCred.Password, tUser[0].PasswordSalt);
                }

                // Compare the two Hashed Passwords
                if (iuser.PasswordHash.Equals(tUser[0].PasswordHash))
                {
                    result = tUser[0].UserId;
                }

                return result;


            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return result;
        }



        public voUser GetUserCombineNameById(viUserId iuser)
        {
            var rUser = _reipushcontext.voUser
                        .FromSqlRaw("REIPUSH_GETCOMBINENAMEUSERBYID @UserId",
                         new SqlParameter("UserId", iuser.UserId))
                        .AsEnumerable()
                        .FirstOrDefault();

            return rUser;
        }


        public string GenerateUserToken(User user, string tokenSecret)
        {

            var result = Helper.GenerateToken(user.UserId, user.Email, false, tokenSecret);
            return result;
        }

        public string GenerateRefreshToken(int userID)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var sReturn = Convert.ToBase64String(randomNumber);
                //store refreshtoken
                try
                {
                    var query = _reipushcontext.UserRefreshTokens.FirstOrDefault(a => a.UserId == userID);
                    if (query != null && query.UserId == userID)
                    {
                        query.RefreshToken = sReturn;
                        query.ExpiresOn = DateTime.Now.AddDays(30);
                        _reipushcontext.UserRefreshTokens.Update(query);
                    }
                    else
                    {
                        query = new UserRefreshToken()
                        {
                            UserId = userID,
                            RefreshToken = sReturn,
                            CreatedOn = DateTime.Now,
                            ExpiresOn = DateTime.Now.AddDays(30)
                        };
                        _reipushcontext.UserRefreshTokens.Add(query);
                    }
                    _reipushcontext.SaveChanges();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    sReturn = "";
                }
                return sReturn;
            }
        }


        public RefreshAccessToken GenerateRefreshTokenFromPrincipal(string token, string secret)
        {
            var principal = Helper.GetPrincipalFromExpiredToken(token, secret);
            var sid = principal.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
            var newJwtToken = Helper.GenerateToken(principal.Claims, secret);

            var newRfreshToken = GenerateRefreshToken(Convert.ToInt32(sid));

            return new RefreshAccessToken
            {
                token = newJwtToken,
                refreshToken = newRfreshToken
            };

        }

        public string GetSavedRefreshToken(string token, string seceret)
        {
            var principal = Helper.GetPrincipalFromExpiredToken(token, seceret);
            var sid = principal.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
            return GetRefreshToken(Convert.ToInt32(sid));
        }

        public string GetRefreshToken(int userID)
        {
            var query = _reipushcontext.UserRefreshTokens.FirstOrDefault(a => a.UserId == userID);
            if (query == null) return "";
            return query.RefreshToken;
        }

        public User CreateUser(viEmailPwd iCred)
        {


            User iuser = new User();
            iuser.CreatedOn = DateTime.Now;
            iuser.UpdatedOn = DateTime.Now;
            iuser.Email = iCred.Email;
            iuser.UserTypeId = 2;                   // This is set  by default to  2 - Standard User
            iuser.IsActive = true;                     // Set the User to Active when creating
            iuser.IsVerified = false;
            if (iCred.Password != "")
            {
                iuser.PasswordSalt = CreateSalt(5);
                iuser.PasswordHash = CreatePasswordHash(iCred.Password, iuser.PasswordSalt);
            }


            User ruserId = _reipushcontext.User.FromSqlRaw("CreateUser @Email, @UserTypeId, @IsActive, @IsVerified," +
                                                            " @PasswordHash, @PasswordSalt",
                         new SqlParameter("Email", iuser.Email),
                         new SqlParameter("UserTypeId", iuser.UserTypeId),
                         new SqlParameter("IsActive", iuser.IsActive),
                         new SqlParameter("IsVerified", iuser.IsVerified),
                         new SqlParameter("PasswordHash", iuser.PasswordHash),
                         new SqlParameter("PasswordSalt", iuser.PasswordSalt)
                         ).ToArray().FirstOrDefault();



            return ruserId;
        }


        public bool CreateUserAccount(UserAccount iaccount)
        {

            try
            {
                iaccount.UpdatedUserId = iaccount.UserId;
                iaccount.CreatedUserId = iaccount.UserId;
                iaccount.AccountBalance = 0;

                _reipushcontext.UserAccounts.Add(iaccount);
                _reipushcontext.SaveChanges();



                return true;

            }
            catch (Exception e)
            {
                log.Error(e);
                return false;
            }
        }


        public async Task<string> CreateAuthoritNetProfileAsync(string iemail)
        {
            string iAuthNetProfileid = string.Empty;
            microservicBaseUrl = microservicBaseUrl + "createAuthorizeNetProfile";
            try
            {

                var postData = new
                {
                    email = iemail
                };

                var serializedRequest = JsonConvert.SerializeObject(postData);

                var requestBody = new StringContent(serializedRequest);
                requestBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var response = await HttpClient.PostAsync(microservicBaseUrl, requestBody))
                {
                    if (!response.IsSuccessStatusCode)
                        return response.StatusCode.ToString();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var deserializedResponse = JsonConvert.DeserializeObject<string>(responseContent);
                    iAuthNetProfileid = deserializedResponse;
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return iAuthNetProfileid;
        }


        public async Task<string> VerifyPaymentSource(viBillingInformation xbill, voUserAccountVerify xacctinfo)
        {
            string iAuthNetPaymentProfileid = string.Empty;
            microservicBaseUrl = microservicBaseUrl + "validatePaymentSource";

            try
            {

                var postData = new
                {
                     AuthNetProfileid = xacctinfo.AuthNetProfileId,
                     cardNumber = xbill.CardNumber,
                     expMonth = xbill.ExpirationDate,
                     expYear = xbill.ExpirationDate,
                     ccv = xbill.CVC,
                     amount = "0.01",
                     invoiceHeader = "",
                     description = "",
                     orderid = "",
                     firstname = xbill.CardHolderName,
                     lastname = xbill.CardHolderName,
                     addressline = xbill.BillingAddress1,
                     city = xbill.BillingAddress2,
                     state = xbill.BillingAddress2,
                     zip = xbill.BillingAddress2
                };

                var serializedRequest = JsonConvert.SerializeObject(postData);

                var requestBody = new StringContent(serializedRequest);
                requestBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using (var response = await HttpClient.PostAsync(microservicBaseUrl, requestBody))
                {
                    if (!response.IsSuccessStatusCode)
                        return response.StatusCode.ToString();

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var deserializedResponse = JsonConvert.DeserializeObject<string>(responseContent);
                    iAuthNetPaymentProfileid = deserializedResponse;
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return iAuthNetPaymentProfileid;
        }

        public User StorePaymentInfo(viEmailPwd iCred)
        {


            User iuser = new User();
            iuser.CreatedOn = DateTime.Now;
            iuser.UpdatedOn = DateTime.Now;
            iuser.Email = iCred.Email;
            iuser.UserTypeId = 2;                   // This is set  by default to  2 - Standard User
            if (iCred.Password != "")
            {
                iuser.PasswordSalt = CreateSalt(5);
                iuser.PasswordHash = CreatePasswordHash(iCred.Password, iuser.PasswordSalt);
            }


            User ruserId = _reipushcontext.User.FromSqlRaw("CreateUser  @Email, @UserTypeId, @PasswordHash, @PasswordSalt",
                         new SqlParameter("Email", iuser.Email),
                         new SqlParameter("UserTypeId", iuser.UserTypeId),
                         new SqlParameter("PasswordHash", iuser.PasswordHash),
                         new SqlParameter("PasswordSalt", iuser.PasswordSalt)
                         ).ToArray().FirstOrDefault();



            return ruserId;
        }

        public Tag CreateTag(viNewTag itag)
        {
            var vtag = _reipushcontext.Tag
                        .FirstOrDefault(t => t.Name.Trim().ToLower() == itag.Name.Trim().ToLower()
                                        && t.UserId == itag.UserId);

            if (vtag == null)
            {

                vtag = new Tag()
                {
                    Name = itag.Name,
                    UserId = itag.UserId
                };

                _reipushcontext.Tag.Add(vtag);
                _reipushcontext.SaveChanges();
                return vtag;
            }
            return null;

        }

        public List<Tag> GetMyTags(int userid)
        {
            var vtag = _reipushcontext.Tag
                        .Where(t => t.UserId == userid).ToList();

            return vtag;
        }


        public User ChangeUser(User iuser)
        {
            var iUser = _reipushcontext.User
                        .FromSqlRaw("REIPUSH_GETUSERBYID @UserId",
                         new SqlParameter("UserId", iuser.UserId))
                        .AsEnumerable()
                        .FirstOrDefault();

            return iUser;
        }
        public User DeleteUser(User iuser)
        {
            var iUser = _reipushcontext.User
                        .FromSqlRaw("REIPUSH_GETUSERBYID @UserId",
                         new SqlParameter("UserId", iuser.UserId))
                        .AsEnumerable()
                        .FirstOrDefault();

            return iUser;
        }


        public async Task<bool> StoreVoiceNote(int userid, IFormFile iform)
        {
            bool isSaveSuccess = false;

            try
            {
                VoiceNote vnote = await Helper.CreateVoiceNote(userid, iform);
                if (vnote != null)
                {
                    _reipushcontext.VoiceNote.Add(vnote);
                    _reipushcontext.SaveChanges();

                    isSaveSuccess = true;
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            return isSaveSuccess;
        }

        public List<VoiceNote> GetMyVoiceNotes(int userid)
        {
            List<VoiceNote> rVoiceNotes = new List<VoiceNote>();

            try
            {
                rVoiceNotes = (List<VoiceNote>)_reipushcontext.VoiceNote
                    .Where(v => v.UserId == userid);


            }
            catch (Exception e)
            {
                log.Error(e);
                return rVoiceNotes;
            }
            return rVoiceNotes;
        }

        public bool ResetPasswordSendEmail(User iuser)
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////
        //                                   PRIVATE ROUTINES                         //
        /// ////////////////////////////////////////////////////////////////////////////
        private string CreateSalt(int size)
        {
            var provider = new RNGCryptoServiceProvider();
            byte[] data = new byte[size];
            provider.GetBytes(data);
            return Convert.ToBase64String(data);
        }

        private string CreatePasswordHash(string password, string salt)
        {
            //return FormsAuthentication.HashPasswordForStoringInConfigFile(password + salt, "SHA1");
            return Helper.GetHash(password + salt, "SHA1");
        }

        public static IConfiguration AppSetting { get; }

        public string GetValueByName(string ivalue)
        {
            try
            {
                GlobalSettingItem rvalue = new GlobalSettingItem();

                if (ivalue == null)
                {
                    return null;
                }

                ivalue = _reipushcontext.GlobalSettings
                         .FirstOrDefault(u => u.Name == ivalue).value;
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            return ivalue;
        }
    }




}