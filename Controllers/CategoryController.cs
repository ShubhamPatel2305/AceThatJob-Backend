using AceThatJob.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AceThatJob.Controllers
{
    [RoutePrefix("category")]
    public class CategoryController : ApiController
    {
        AceThatJobEntities db = new AceThatJobEntities();

        [HttpPost, Route("addnewcategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewCategory([FromBody] Category category)
        {
            try
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Category added successfully" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "An error occurred", error = e.Message });
            }
        }

        [HttpGet, Route("getallcategories")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllCategories()
        {
            try
            {
               var categories = db.Categories.OrderBy(x => x.name).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, categories);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "An error occurred", error = e.Message });
            }
        }

        [HttpPost, Route("updatecategory")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpdateCategory([FromBody] Category category)
        {
            try
            {
                Category obj = db.Categories.Where(x => x.id == category.id).FirstOrDefault();
                if (obj != null)
                {
                    obj.name = category.name;
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Category updated successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { message = "Category not found" });
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "An error occurred", error = e.Message });
            }
        }

        
    }
}
