using AceThatJob.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AceThatJob.Controllers
{
    [RoutePrefix("article")]
    public class ArticleController : ApiController
    {
        AceThatJobEntities db = new AceThatJobEntities();

        [HttpPost, Route("addnewarticle")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage AddNewArticle([FromBody] Article article)
        {
            try
            {
                article.publication_date = DateTime.Now;
                db.Articles.Add(article);
                db.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, new { message = "Article added successfully" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "An error occurred", error = e.Message });
            }
        }

        //get all articles
        [HttpGet, Route("getallarticles")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage GetAllArticles()
        {
            try
            {
                //we use joins to get the category name
                var articles = db.Articles
                    .Join(db.Categories,
                    article => article.categoryID,
                    category => category.id,
                    (article, category) => new
                    {
                        article.id,
                        article.title,
                        article.content,
                        article.publication_date,
                        article.status,
                        CategoryId = category.id,
                        CategoryName = category.name
                    }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, articles);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "An error occurred", error = e.Message });
            }
        }
        //get all articles that are published so we dont add filter cause all users should see them
        [HttpGet, Route("getallpublishedarticles")]
        public HttpResponseMessage GetAllPublishedArticles()
        {
            try
            {
                //we use joins to get the category name
                var articles = db.Articles
                    .Join(db.Categories,
                    article => article.categoryID,
                    category => category.id,
                    (article, category) => new
                    {
                        article.id,
                        article.title,
                        article.content,
                        article.publication_date,
                        article.status,
                        CategoryId = category.id,
                        CategoryName = category.name
                    }).Where(a => a.status == "published")
                    .ToList();
                return Request.CreateResponse(HttpStatusCode.OK, articles);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "An error occurred", error = e.Message });
            }
        }

        //update article
        [HttpPost, Route("updatearticle")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage UpdateArticle([FromBody] Article article)
        {
            try
            {
                Article obj = db.Articles.Where(x => x.id == article.id).FirstOrDefault();
                if (obj != null)
                {
                    obj.title = article.title;
                    obj.content = article.content;
                    obj.categoryID = article.categoryID;
                    obj.status = article.status;
                    obj.publication_date = DateTime.Now;
                    db.Entry(obj).State = System.Data.Entity.EntityState.Modified; //this is used to update the entity
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Article updated successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { message = "Article not found" });
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "An error occurred", error = e.Message });
            }
        }

        [HttpGet, Route("deletearticle/{id}")]
        [CustomAuthenticationFilter]
        public HttpResponseMessage DeleteArticle(int id)
        {
            try
            {
                Article obj = db.Articles.Where(x => x.id == id).FirstOrDefault();
                if (obj != null)
                {
                    db.Articles.Remove(obj);
                    db.SaveChanges();
                    return Request.CreateResponse(HttpStatusCode.OK, new { message = "Article deleted successfully" });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new { message = "Article not found" });
                }
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { message = "An error occurred", error = e.Message });
            }
        }
    }
}
