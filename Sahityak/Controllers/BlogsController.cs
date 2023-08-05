using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sahityak.Data;
using Sahityak.Models;
using System.Security.Claims;

namespace Sahityak.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class BlogsController:Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public BlogsController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        private async Task<ActionResult> AddOrEdit(Blog req, EHttpAction action)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var claimsUserId = claimsIdentity.FindFirst("UserId")?.Value;
            int TokenUserId = 0;
            int.TryParse(claimsUserId, out TokenUserId);
            int newId = 0;

            try
            {
                if(!ModelState.IsValid) { return BadRequest(new Response(false, "Invalid Parameters")); }

                if(action== EHttpAction.Post)
                {
                    req.UserId = TokenUserId;
                    var NewBlog = await _dbContext.AddAsync(req);
                }
                else if(action== EHttpAction.Put)
                {

                }
                await _dbContext.SaveChangesAsync();
                newId = req.Id;
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            if (action == EHttpAction.Post)
            {
                return Ok(new PostResponse(newId, true, "New Blog Added"));
            }
            else
            {
                return Ok(new Response(true, "Blog Updated Successfully"));
            }
        }

        [HttpPost("Add")]
        [Authorize]
        public async Task<ActionResult> AddBlog(Blog req)
        {
            Console.WriteLine("Hello2");
            return await AddOrEdit(req, EHttpAction.Post);
        }
    }
}
