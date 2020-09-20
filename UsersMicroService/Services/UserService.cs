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

namespace UsersMicroService.Services
{

    public class UserService
    {
        private readonly ReipushContext _reipushcontext;
        private static readonly ILog log = LogManager.GetLogger(typeof(UserService));

        public UserService(ReipushContext context)
        {
            _reipushcontext = context;
        }

        //public async Task<List<User>> GetAllUsers()
        //{
        //    var iUsers = await  _reipushcontext.User
        //                        .FromSqlRaw("REIPUSH_GETUSERS")
        //                        .ToArrayAsync();

        //    return iUsers.ToList();
        //}
        //public User GetUserById(viUserId iuser)
        //{
        //    var rUser =  _reipushcontext.User
        //                .FromSqlRaw("REIPUSH_GETUSERBYID @UserId",
        //                 new  SqlParameter("UserId", iuser.UserId))
        //                .AsEnumerable()
        //                .FirstOrDefault();

        //    return rUser;
        //}

        public User GetUserByEmail(string iemail)
        {
            User rUser = new User();

            try {
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
                   .Select(u => new User(){
                       PasswordHash = u.PasswordHash,
                       PasswordSalt = u.PasswordSalt
                   }).ToArray();
                  

                //  Convert the Current Password to Hash via the Salt key
                if (iCred.Password != ""){
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


        public User CreateUser(User iuser)
        {


            var vUser = _reipushcontext.User.FromSqlRaw("REIPUSH_CREATEUSER  @Email, @FirstName, @LastName, @MobileNumber, @UserType", 
                         new SqlParameter("Email", iuser.Email),
                         new SqlParameter("FirstName", iuser.FirstName),
                         new SqlParameter("LastName", iuser.LastName),
                         new SqlParameter("MobileNumber", iuser.MobileNumber),
                         new SqlParameter("UserType", iuser.UserType)
                         )
                        .AsEnumerable()
                        .FirstOrDefault();

            
            return vUser;
        }

        public string GenerateUserToken(User user, string tokenSecret) {
          
            var result =  Helper.GenerateToken(user.UserId, user.Email, false, tokenSecret);
            return result;
        }

        public  string GenerateRefreshToken(int userID)
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
            return  GetRefreshToken(Convert.ToInt32(sid));
        }

        public string GetRefreshToken(int userID)
        {
            var query = _reipushcontext.UserRefreshTokens.FirstOrDefault(a => a.UserId == userID);
            if (query == null) return "";
            return query.RefreshToken;
        }

        public User CreateAccount(viEmailPwd iCred)
        {


            User iuser = new User();
            iuser.CreatedOn = DateTime.Now;
            iuser.UpdatedOn = DateTime.Now;
            iuser.Email = iCred.Email;
            iuser.UserType = 0;
            if (iCred.Password != ""){
                iuser.PasswordSalt = CreateSalt(5);             
                iuser.PasswordHash = CreatePasswordHash(iCred.Password, iuser.PasswordSalt);
            }

            User ruserId = _reipushcontext.User.FromSqlRaw("CreateAccount  @Email, @UserType, @PasswordHash, @PasswordSalt",
                         new SqlParameter("Email", iuser.Email),
                         new SqlParameter("UserType", iuser.UserType),
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
                if (vnote != null){
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
          return  Helper.GetHash(password + salt, "SHA1");
        }

    }

 


}
