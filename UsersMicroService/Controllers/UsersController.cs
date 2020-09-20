using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Reipush.Api.Entities.User;
using UsersMicroService.Entities;
using UsersMicroService.Services;
using UsersMicroService.ViewModels;

namespace UsersMicroService.Controllers
{
    [Route("user")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ReipushContext _context;
        private readonly IConfiguration _config;
        private static readonly ILog log = LogManager.GetLogger(typeof(UsersController));

        public UsersController(ReipushContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }



        //// GET: api/Users/GetAll
        //[HttpGet("GetAll")]
        //public async Task<ActionResult<IEnumerable<User>>> GetUser()
        //{

        //    Services.UserService _UsersService = new UserService(_context);

        //    // This is utilizing Store Procedures
        //    return await _UsersService.GetAllUsers();
           
        //    // This is using the Entity Framework
        //    //return await _context.User.ToListAsync();
        //}

        //// GET: api/Users/GetById
        //[HttpPost("GetById")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public  ActionResult<User> GetUser(viUserId iuser)
        //{
        //    Services.UserService _UsersService = new UserService(_context);

        //    var user =  _UsersService.GetUserById(iuser);
            
        //    //var user = await _context.User.FindAsync(id);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return user;
        //}


        //// GET: api/Users/GetById
        //[HttpPost("GetFullNameById")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public ActionResult<voUser> GetUserFN(viUserId iuser)
        //{
        //    Services.UserService _UsersService = new UserService(_context);

        //    var user = _UsersService.GetUserCombineNameById(iuser);

        //    //var user = await _context.User.FindAsync(id);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return user;
        //}



        //// PUT: api/Users/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutUser(int id, User user)
        //{
        //    if (id != user.UserId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(user).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!UserExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        //// POST: api/Users
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPost("AddUser")]
        //public  ActionResult<User> PostUser(User iuser)
        //{
        //    //_context.User.Add(user);
        //    //await _context.SaveChangesAsync();

        //    Services.UserService _UsersService = new UserService(_context);

        //    var user = _UsersService.CreateUser(iuser);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return user;

        //    //  return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        //}

        //// DELETE: api/Users/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<User>> DeleteUser(int id)
        //{
        //    var user = await _context.User.FindAsync(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.User.Remove(user);
        //    await _context.SaveChangesAsync();

        //    return user;
        //}

        //private bool UserExists(int id)
        //{
        //    return _context.User.Any(e => e.UserId == id);
        //}


        // GET: api/Users/emailaddressexist


        [HttpPost("emailaddressexist")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<User> EmailExist(viEmail iEmail)
        {
            UserService _UsersService = new UserService(_context);
            User user;
            try
            {
                user = _UsersService.GetUserByEmail(iEmail.Email);


                if (user == null)
                {
                    return NotFound("The Email Address Was Not Found");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(user);
        }



        // GET: api/Users/createaccount
        [HttpPost("createaccount")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<User> CreateAccount(viEmailPwd iCred)
        {
            Services.UserService _UsersService = new UserService(_context);
            User user;
            
            try
            {
                // Verify the email address does not exist.
                user = _UsersService.GetUserByEmail(iCred.Email);                           
                if (user != null) {
                    return BadRequest("This email address already exist in our system.");
                }

                user = _UsersService.CreateAccount(iCred);

                if (user != null)
                {
                    viUserAccess uAccess = new viUserAccess();
                    uAccess.UserId = user.UserId;                
                    uAccess.refreshAccesToken.token = _UsersService.GenerateUserToken(user, _config.GetValue<string>("TokenSecretKey"));
                    uAccess.refreshAccesToken.refreshToken = _UsersService.GenerateRefreshToken(user.UserId);
                    return Ok(uAccess);
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(user);
        }


        [HttpPost("authenticate")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<User> AuthenticateUser(viEmailPwd creds)
        {
            Services.UserService _UsersService = new UserService(_context);
            User user = new User();
            try
            {
              
                if ((creds.Email==null) || (creds.Password == null)){
                    return BadRequest("Username and Password must be supplied");
                }

                // Authentic the user with the email address and password.
                user.UserId = _UsersService.AuthenticateUser(creds);
                if (user.UserId< 1){
                    return NotFound("Invalid Username or Password");
                }


                if (user != null)
                {
                    viUserAccess uAccess = new viUserAccess();
                    uAccess.UserId = user.UserId;
                    uAccess.refreshAccesToken.token = _UsersService.GenerateUserToken(user, _config.GetValue<string>("TokenSecretKey"));  
                    uAccess.refreshAccesToken.refreshToken = _UsersService.GenerateRefreshToken(user.UserId);
                    return Ok(uAccess);
                }


            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(user);
        }



        [HttpPost("refreshtoken")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult<RefreshAccessToken> RefreshToken(RefreshAccessToken creds)
        {
            Services.UserService _UsersService = new UserService(_context);
            try
            {

                if ((creds.token == null) || (creds.refreshToken == null))
                {
                    return BadRequest("A Token and Refresh Token must be supplied");
                }

                // Authentic the user with the email address and password.
                string savedRefreshToken = _UsersService.GetSavedRefreshToken(creds.token, _config.GetValue<string>("TokenSecretKey"));
                if (savedRefreshToken == null) {
                    return NotFound("Invalid Token or Refresh Token");
                }

                if (savedRefreshToken != creds.refreshToken)   {
                    return NotFound("Invalid Refresh Token");

                }
                    RefreshAccessToken uToken = new  RefreshAccessToken();
                    uToken = _UsersService.GenerateRefreshTokenFromPrincipal(creds.token, _config.GetValue<string>("TokenSecretKey"));
                    return Ok(uToken);

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
        }


        [HttpPost("forgotpassword")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult ForgotPassword(viEmail iemail)
        {
            Services.UserService _UsersService = new UserService(_context);
            try
            {

                if ((iemail == null)){
                    return BadRequest("Invalid Email Address");
                }

                var user = _UsersService.GetUserByEmail(iemail.Email);
                if (user == null)
                {
                    return NotFound("Email not found");
                }

                // Generate the password reset record

                var IsEmailSent = _UsersService.ResetPasswordSendEmail(user);
                // ******* FELIX ********* NEED TO COMPLETE *******
                //  Call Routine that will send a email to the user to reset the password

                return Ok();

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }

        }

        [HttpPost("addtag")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<Tag> AddTag(viNewTag itag)
        {
            Services.UserService _UsersService = new UserService(_context);
            Tag TagCreated = new Tag();

            try
            {
                if ((itag.UserId < 1) || (itag.Name.Trim() == string.Empty))
                {
                    return BadRequest("You must provide all required data to add a tag.");
                }

                TagCreated = _UsersService.CreateTag(itag);

                if (TagCreated == null)
                {
                    return BadRequest("Not Created");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(TagCreated);
        }


        [HttpPost("getalltags")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<List<Tag>> GetTag(int UserId)
        {
            Services.UserService _UsersService = new UserService(_context);
            List<Tag> MyTags = new List<Tag>();

            try
            {
                if ((UserId < 1))
                {
                    return BadRequest("You must provide a UserId to get tags.");
                }

                MyTags = _UsersService.GetMyTags(UserId);

                if (MyTags == null)
                {
                    return BadRequest("No Records Found");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(MyTags);
        }

        [HttpPost("getallvoicenotes")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public ActionResult<List<VoiceNote>> GetVoiceNotes(int UserId)
        {
            Services.UserService _UsersService = new UserService(_context);
            List<VoiceNote> MyVocieNotes = new List<VoiceNote>();

            try
            {
                if ((UserId < 1))
                {
                    return BadRequest("You must provide a UserId to get voicenotes.");
                }

                MyVocieNotes = _UsersService.GetMyVoiceNotes(UserId);

                if (MyVocieNotes == null)
                {
                    return BadRequest("No Records Found");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok(MyVocieNotes);
        }



        [HttpPost("addvoicenote")]
        [ApiVersion("1")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> AddVoiceNote(int userid, IFormFile iform)
        {
            Services.UserService _UserService = new UserService(_context);
            
            bool validUser = false;

            try
            {
                if (userid > 0){
                    voUser rvalue =  _UserService.GetUserCombineNameById(new viUserId { UserId = userid.ToString() });
                    if (rvalue != null) { validUser = true; }
                }
                if (!validUser) {
                    return BadRequest("You must provide all required data to add a voicenote.");
                }

                if (!Helper.IsValidFileExtention(iform.FileName,0)){
                    return BadRequest("The file type you are trying to upload is not valid.");

                }

                bool isCreated = await _UserService.StoreVoiceNote(userid, iform);


                if (!isCreated)
                {
                    return BadRequest("Not Created");
                }

            }
            catch (Exception e)
            {
                log.Error(e);
                return BadRequest(e.Message.ToString());
            }
            return Ok();
        }

    }
}
