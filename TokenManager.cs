using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Web;

namespace AceThatJob
{
    public class TokenManager
    {
        public static string Secret = "c34beb58a76c02a04cba42897201d8fd8138c34d59a5a4aab3741d32535dfc8fab25bf4212345554e363186c7ba597daf53088a2506ef3097a363809e156226b94f1";
        public static string GenerateToken(string email, string isDeletable)
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            // above code is used to create a security key using the secret key
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[] {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, email),
                        new System.Security.Claims.Claim("isDeletable", isDeletable)
                    }), //this is used to create a claim which is used to store the email and isDeletable
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
                //this is used to create a signing credential which is used to sign the token
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler(); //this is used to create a jwt token
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor); 
            return handler.WriteToken(token); 


        }
    }
}