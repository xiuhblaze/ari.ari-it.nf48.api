using Arysoft.ARI.NF48.Api.Exceptions;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Response;
using Arysoft.ARI.NF48.Api.Services;
using Arysoft.ARI.NF48.Api.Tools;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class AuthController : ApiController
    {
        private UserService _userService;

        public AuthController()
        {
            _userService = new UserService();
        }

        [HttpGet]
        [Route("api/Auth/echo-ping")]
        public IHttpActionResult EchoPing()
        {
            return Ok(true);
        } // EchoPing

        [HttpGet]
        [Route("api/Auth/echo-user")]
        public IHttpActionResult EchoUser()
        {
            var identity = Thread.CurrentPrincipal.Identity;
            return Ok($" IPrincipal-user: {identity.Name} - IsAuthenticated: {identity.IsAuthenticated}");
        } // EchoUser

        [HttpPost]
        [Route("api/Auth/{id}/validate")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> ValidateUser(Guid id, [FromBody] AuthValidateDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            if (id != itemDto.ID)
                throw new BusinessException("ID mismatch");

            var isValid = await _userService.ValidatePasswordAsync(itemDto.ID, itemDto.Password);
            var response = new ApiResponse<bool>(isValid);

            return Ok(response);
        } // ValidateUser

        [ResponseType(typeof(ApiResponse<string>))]
        public async Task<IHttpActionResult> Login([FromBody] AuthLoginDto userDto)
        {   
            var user = await _userService.LoginAsync(userDto.Username, userDto.Password);

            var tokenJwt = Tools.TokenGenerator.GenerateTokenJwt(user);
            //var token = GetToken(user.ID.ToString(), user.Username, user.Email);
            var response = new ApiResponse<string>(tokenJwt);

            return Ok(response);
        } // Login

        [HttpPut]
        [Route("api/Auth/change-password")]
        [ResponseType(typeof(ApiResponse<bool>))]
        public async Task<IHttpActionResult> ChangePassword([FromBody] AuthChangePasswordDto itemDto)
        {
            if (!ModelState.IsValid)
                throw new BusinessException(Strings.GetModelStateErrors(ModelState));

            var isValid = await _userService.ValidatePasswordAsync(itemDto.ID, itemDto.OldPassword);

            if (!isValid)
                throw new BusinessException("Invalid old password");

            await _userService.UpdatePasswordAsync(itemDto.ID, itemDto.NewPassword);

            var response = new ApiResponse<bool>(true);

            return Ok(response);
        } // ChangePassword



        ///// <summary>
        ///// Basado en: https://www.linkedin.com/pulse/tutorial-jwt-token-aspnet-48-webapi-mohamed-ebrahim
        ///// </summary>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //private string GetToken(string userId, string username, string email)
        //{
        //    var key = ConfigurationManager.AppSettings["JwtKey"];
        //    var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
        //    var audience = ConfigurationManager.AppSettings["JwtAudience"];

        //    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        //    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        //    //Create a List of Claims, Keep claims name short    
        //    var permClaims = new List<Claim>
        //    {
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        new Claim("userid", userId),
        //        new Claim("username", username),
        //        new Claim("useremail", email)
        //    };

        //    //Create Security Token object by giving required parameters    
        //    var token = new JwtSecurityToken(issuer, //Issure    
        //                    audience,  //Audience    
        //                    permClaims,
        //                    expires: DateTime.Now.AddDays(1),
        //                    signingCredentials: credentials);
        //    var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);

        //    return jwt_token;
        //}
    }
}
