using Arysoft.ARI.NF48.Api.Models;
using Arysoft.ARI.NF48.Api.Models.DTOs;
using Arysoft.ARI.NF48.Api.Response;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Arysoft.ARI.NF48.Api.Controllers
{
    public class AuthController : ApiController
    {
        [ResponseType(typeof(ApiResponse<Object>))]
        public async Task<IHttpActionResult> Login([FromBody] AuthLoginDto user)
        { 
            var token = GetToken(Guid.NewGuid().ToString(), user.Username);

            var response = new ApiResponse<Object>(token);
            return Ok(response);
        }

        /// <summary>
        /// Basado en: https://www.linkedin.com/pulse/tutorial-jwt-token-aspnet-48-webapi-mohamed-ebrahim
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private Object GetToken(string userId, string username)
        {
            var key = ConfigurationManager.AppSettings["JwtKey"];

            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Create a List of Claims, Keep claims name short    
            var permClaims = new List<Claim>();
            permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            permClaims.Add(new Claim("userid", userId));
            permClaims.Add(new Claim("username", username));

            //Create Security Token object by giving required parameters    
            var token = new JwtSecurityToken(issuer, //Issure    
                            issuer,  //Audience    
                            permClaims,
                            expires: DateTime.Now.AddDays(1),
                            signingCredentials: credentials);
            var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
            
            return new ReturnValue().Data = jwt_token;
        }
    }

    public class ReturnValue {
        public string Data { get; set; }
    }
}
