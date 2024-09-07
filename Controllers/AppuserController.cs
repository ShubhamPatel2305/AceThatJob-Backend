using AceThatJob.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AceThatJob.Controllers
{
    [RoutePrefix("appuser")] //this makes it so that all routes in this api controller will start with /appuser
    public class AppuserController : ApiController
    {
        AceThatJobEntities db = new AceThatJobEntities();

        [HttpPost, Route("login")] //this will be /appuser/login for post requests
        public HttpResponseMessage Login([FromBody] AppUser appuser)
        {
            try
            {
                AppUser obj = db.AppUsers.Where(x => x.email == appuser.email && x.password == appuser.password).FirstOrDefault();
                if (obj != null)
                {
                    if (obj.status == "True")
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { token = TokenManager.GenerateToken(obj.email, obj.isDeletable) });
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "Account is not active contact admin" });
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.Unauthorized, new { message = "Incorrect email or password" });
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "An error occurred", error = e.Message });
            }
        }

        [HttpPost, Route("addnewuser")]
        [CustomAuthenticationFilter] //this will make it so that the user must be authenticated to access this route
        public HttpResponseMessage AddNewAppuser([FromBody] AppUser appuser)
        {
            try
            {
                AppUser obj = db.AppUsers.Where(x => x.email == appuser.email).FirstOrDefault();
                if (obj == null)
                {
                    appuser.status= "False";
                    appuser.isDeletable = "True";
                    db.AppUsers.Add(appuser);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "User added successfully" });
                }
                else
                {   
                    return Request.CreateResponse(HttpStatusCode.Conflict, new { message = "User already exists" });
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "An error occurred", error = e.Message });
            }
        }

        [HttpGet, Route("getallusers")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllAppusers()
        {
            try
            {
                var token = Request.Headers.GetValues("authorization").First();
                TokenClaim claim = TokenManager.ValidateToken(token);
                if (claim.isDeletable == "False")
                {
                    //return all users except the one who is not deletable
                    var result = db.AppUsers.Select(x => new { x.id, x.name, x.email, x.status, x.isDeletable }).Where(x => (x.email != claim.email) && (x.isDeletable == "True")).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, result);
                }
                else
                {
                    //return all users except the one who is not deletable and the one who is logged in
                    var result = db.AppUsers.Select(x => new { x.id, x.name, x.email, x.status, x.isDeletable }).Where(x => (x.email != claim.email)&&(x.isDeletable=="True")).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, result);

                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "An error occurred", error = e.Message });
            }
        }
    }
}
