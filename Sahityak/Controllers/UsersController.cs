using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sahityak.Data;
using Sahityak.Helper;
using Sahityak.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Sahityak.Helper.DataHelper;

namespace Sahityak.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UsersController:Controller
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _dbContext;

        public UsersController(IConfiguration config, ApplicationDbContext dbContext)
        {
            _config = config;
            _dbContext = dbContext;
        }

        [HttpGet("GetAll")]
        [Authorize]

        public async Task<ActionResult> GetAll()
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                
                var UserQuery = from u in _dbContext.Users
                                join b in _dbContext.Blogs on u.UserId equals b.UserId                               
                                select new
                                {
                                    u.UserId,
                                    u.SahityakId,
                                    u.FirstName,
                                    u.LastName,
                                    u.DOB,
                                    u.Institution,
                                    u.CollegeId,
                                    u.MobNo,
                                    u.WhatsappNo,
                                    u.EmailId,
                                    BlogUserId = b.UserId
                                };
                var User = await UserQuery.ToListAsync();
                
                List<UserResponse> res = new List<UserResponse>();
                for (int i = 0; i < User.Count(); i++)
                {
                    UserResponse userRes = new UserResponse();
                    userRes.UserId = User[i].UserId;
                    userRes.SahityakId = User[i].SahityakId;
                    userRes.FirstName = User[i].FirstName;
                    userRes.LastName = User[i].LastName;
                    userRes.DOB = User[i].DOB;
                    userRes.Institution = User[i].Institution;
                    userRes.CollegeId = User[i].CollegeId;
                    userRes.MobNo = User[i].MobNo;
                    userRes.WhatsappNo = User[i].WhatsappNo;
                    userRes.EmailId = User[i].EmailId;
                    while (i<User.Count() && User[i].UserId == userRes.UserId)
                    {
                        userRes.BlogsPosted++;
                        i++;
                    }
                    res.Add(userRes);
                }
                return Ok(res);
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("GetById")]
        [Authorize]

        public async Task<ActionResult> GetById(int id)
        {
            try
            {
               var user= await _dbContext.Users.FirstOrDefaultAsync(u=> u.UserId== id);
               var Blogs = await _dbContext.Blogs.Where(b=> b.UserId== id).ToListAsync();

                UserResponse userRes = new UserResponse();
                userRes.UserId = user.UserId;
                userRes.SahityakId = user.SahityakId;
                userRes.FirstName = user.FirstName;
                userRes.LastName = user.LastName;
                userRes.DOB = user.DOB;
                userRes.Institution = user.Institution;
                userRes.CollegeId = user.CollegeId;
                userRes.MobNo = user.MobNo;
                userRes.WhatsappNo = user.WhatsappNo;
                userRes.EmailId = user.EmailId;
                userRes.Blogs= Blogs.ToList();

                return Ok(userRes);
            }
            catch(Exception ex)
            {
                return Problem(ex.Message);
            }
        }
        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
             {
                    new Claim(JwtRegisteredClaimNames.Sub, _config["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", user.UserId.ToString())
             };

            var token = new JwtSecurityToken
                (
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials

                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpPost("login")]

        public async Task<ActionResult> login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new Response(false, "Invalid Parameters"));
            }
            if(string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new Response(false, "Username or password not entered"));
            }

            var user= await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailId == model.Username && u.Password == model.Password);
            if(user == null)
            {
                return BadRequest(new Response(false, "Username or password Invalid"));
            }

            var Token=GenerateToken(user);
            return Ok(new LoginResponse(Token, user));
        }

        [AllowAnonymous]
        [HttpPost("SignUp")]

        public async Task<ActionResult> Signup(User user)
        {
            if (!ModelState.IsValid) { return BadRequest(new Response(false, "Invalid Parameters")); }

            if (user.SahityakId == 0)
            {

                var ExistingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailId == user.EmailId);
                if (ExistingUser != null)
                {
                    return BadRequest(new Response(false, "User Already Exists"));
                }
            }
            else
            {
                if(user.UserType != Models.User.EUserType.SahityakMember || user.UserType != Models.User.EUserType.Admin)
                {
                    return BadRequest(new Response(false, "Invalid Request"));
                }
                var ExistingUser= await _dbContext.Users.FirstOrDefaultAsync(u=> u.SahityakId== user.SahityakId || u.EmailId== user.EmailId);
                if(ExistingUser != null)
                {
                    return BadRequest(new Response(false, "User Already Exists"));
                }
            }
            var newUser= await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();
            return Ok(new PostResponse(user.UserId, true, "User Added Successfully"));
        }

        [HttpPut]
        [Authorize]

        public async Task<ActionResult> EditUser(User req)
        {
            try
            {
                if (!ModelState.IsValid) { return BadRequest(new Response(false, "Invalid Parameters")); }

                var UserToEdit= await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u=> u.UserId==req.UserId);
                if(UserToEdit == null)
                {
                    return BadRequest(new Response(false, "User Not found"));
                }
                if (req.SahityakId != 0)
                {
                    var ConflictingUser = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId != req.UserId && u.SahityakId==req.SahityakId && u.EmailId == req.EmailId);
                    if(ConflictingUser!=null)
                    {
                        return BadRequest(new Response(false, "User already Exists"));
                    }

                }
                else
                {
                    var ConflictingUser = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId != req.UserId && u.EmailId == req.EmailId);
                    if (ConflictingUser != null)
                    {
                        return BadRequest(new Response(false, "User already Exists"));
                    }
                }
                _dbContext.Entry(req).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                return Ok(new Response(true, "User Updated Successfully"));

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpDelete("Delete")]
        [Authorize]

        public async Task<ActionResult> DeleteUser(DeleteModel req)
        {
            try
            {
                if (!ModelState.IsValid) { return  BadRequest(new Response(false, "Invalid Parameters")); }

                DataHelper dh = new DataHelper();
                List<SqlPara> Paras = new List<SqlPara>();
                Paras.Add(new SqlPara("@p_Id", req.Id));
                ExecuteNonQueryResult result = dh.ExecuteNonQuery(@"Delete from Users where UserId = @p_Id;
Delete from Blogs where UserId= @p_Id;", Paras);

                if (result.NoOfRowsAffected == 0)
                {
                    return NotFound("User Not Found");
                }
                return Ok(new Response(true, "User Deleted Successfully"));
            }
            catch(Exception ex)
            {
                return Problem(ex.Message); 
            }
        }
    }
}
