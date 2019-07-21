using Microsoft.AspNetCore.Mvc;
using APITestGateway.Presentation.WebAPI.Models;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace APITestGateway.Presentation.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Gateway")]
    public class GatewayController : BaseController
    {
        private readonly JWTOptions options;
        public GatewayController(IOptions<Routes> r, IOptions<JWTOptions> o) : base(r)
        {
            options = o.Value;
        }

        /// <summary>
        /// Generates the authorization token
        /// </summary>
        [AllowAnonymous]
        [HttpGet("Token")]
        [SwaggerResponse(200)]
        [SwaggerResponse(400)]
        [SwaggerResponse(500)]
        public async Task<IActionResult> Token()
        {
            var claims = new List<Claim>
                                 {
                                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                    new Claim(JwtRegisteredClaimNames.Iss, options.Issuer),
                                    new Claim(JwtRegisteredClaimNames.Aud, options.Audience),
                                    new Claim(JwtRegisteredClaimNames.Iat, options.Iat)
                                 };

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(options.Secret));

            var jwt = new JwtSecurityToken(
                        issuer: options.Issuer,
                        claims: claims,
                        expires: options.Expires,
                        signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
                    );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return Ok( new{ token });
        }

        /// <summary>
        /// Redirects the request to the route
        /// </summary>
        [Authorize]
        [HttpPost("{route}")]
        [SwaggerResponse(200)]
        [SwaggerResponse(400)]
        [SwaggerResponse(500)]
        public async Task<IActionResult> PostAuth([FromRoute] string route, [FromBody] dynamic vm)
        {
            return await GetAPIRoute(vm);
        }
    }
}