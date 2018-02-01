using CommentMicroService_SocialWall.DataAccess;
using CommentMicroService_SocialWall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using URISUtil.DataAccess;

namespace CommentMicroService_SocialWall.Controllers
{
    [RoutePrefix("api/Comment")]
    public class CommentController : ApiController
    {
        [Route("PostComments/{id}"), HttpGet]
        public IEnumerable<Comment> GetCommentsByPostId(int id, [FromUri]ActiveStatusEnum active = ActiveStatusEnum.Active)
        {
            return CommentDB.GetCommentsByPostId(active, id);
        }

        [Route("{id}"), HttpGet]
        public Comment GetComment(int id)
        {
            return CommentDB.GetComment(id);
        }

        [Route(""), HttpPost]
        public Comment InsertComment([FromBody]Comment comment)
        {
            return CommentDB.InsertComment(comment);
        }

        [Route("{id}"), HttpPut]
        public Comment UpdateComment([FromBody]Comment comment, int id)
        {
            return CommentDB.UpdateComment(comment, id);
        }

        [Route("{id}"), HttpDelete]
        public void DeleteComment(int id)
        {
            CommentDB.DeleteComment(id);
        }
    }
}