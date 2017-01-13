using System.Threading.Tasks;
using AspNetCore.Identity.MongoDB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Showaspnetcore.Model;

namespace Showaspnetcore
{
    public class MySignInManager :  SignInManager<MongoIdentityUser>
    {
        public MySignInManager(UserManager<MongoIdentityUser> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<MongoIdentityUser> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<MongoIdentityUser>> logger) 
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger)
        {
        }

        public override Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            // here goes the external username and password look up

            if (userName.ToLower() == "username@conn.com" && password.ToLower() == "password")
            {
                return base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
            }
            else
            {
                return Task.FromResult(SignInResult.Failed);
            }
        }
    }
}