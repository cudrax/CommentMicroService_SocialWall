using System;
using URISUtil.DataAccess;
using NUnit.Framework;
using CommentMicroService_SocialWall.DataAccess;
using CommentMicroService_SocialWall.Models;
using System.Text;

namespace CommentMicroService_SocialWall.Tests
{
    public class CommentUnitTest
    {
        ActiveStatusEnum active = ActiveStatusEnum.Active;

        [Test]
        public void GetCommentsByPostIdSuccess()
        {
            Assert.AreEqual(CommentDB.GetCommentsByPostId(active, 3).Count, 4);
        }

        [Test]
        public void GetCommentsByPostIdFailed()
        {
            Assert.IsEmpty(CommentDB.GetCommentsByPostId(active, 100));
        }

        [Test]
        public void GetCommentSuccess()
        {
            int id = CommentDB.GetCommentsByPostId(active, 3)[0].Id;
            Assert.IsNotNull(CommentDB.GetComment(id));
        }

        [Test]
        public void GetCommentFailed()
        {
            int id = 100;
            Assert.IsNull(CommentDB.GetComment(id));
        }

        [Test]
        public void InsertCommentSuccess()
        {
            Comment comment = new Comment
            {
                Text = "Neki novi komentar",
                Date = new DateTime(2018, 2, 1, 5, 0, 0),
                Attachment = Encoding.ASCII.GetBytes("dodatak"),
                PostId = 3,
                UserId = 2,
                Active = true
            };

            Assert.IsNotNull(CommentDB.InsertComment(comment));
        }
        [Test]
        public void InsertCommentFailed()
        {
            Comment comment = new Comment
            {
                Text = "",
                Date = new DateTime(2018, 2, 1, 5, 0, 0),
                Attachment = Encoding.ASCII.GetBytes("dodatak"),
                PostId = 3,
                UserId = 2,
                Active = true
            };
            Assert.IsNull(CommentDB.InsertComment(comment));
        }

        [Test]
        public void UpdateCommentSuccess()
        {
            int id = CommentDB.GetCommentsByPostId(active, 3)[0].Id;
            Comment comment = new Comment
            {
                Text = "UPDATE",
                Date = new DateTime(2018, 2, 1, 5, 0, 0),
                Attachment = Encoding.ASCII.GetBytes("dodatak"),
                PostId = 3,
                UserId = 2,
                Active = true
            };
            Comment updatedComment = CommentDB.UpdateComment(comment, id);
            Assert.AreEqual(comment.Text, updatedComment.Text);
        }

        [Test]
        public void UpdateCommentFailed()
        {
            int id = CommentDB.GetCommentsByPostId(active, 3)[0].Id;
            Comment comment = new Comment
            {
                Text = "",
                Date = new DateTime(2018, 2, 1, 5, 0, 0),
                Attachment = Encoding.ASCII.GetBytes("dodatak"),
                PostId = 3,
                UserId = 2,
                Active = true
            };
            Comment updatedComment = CommentDB.UpdateComment(comment, id);
            Assert.IsNull(updatedComment);
        }

        [Test]
        public void DeleteCommentSuccess()
        {
       
            int id = CommentDB.GetCommentsByPostId(active, 1)[0].Id;
            int oldNumberOfCommentsByPostId = CommentDB.GetCommentsByPostId(active, 1).Count;
            CommentDB.DeleteComment(id);
            int newNumberOfCommentsByPostId = CommentDB.GetCommentsByPostId(active, 1).Count;
            Assert.AreEqual(oldNumberOfCommentsByPostId - 1, newNumberOfCommentsByPostId);
        }

        [Test]
        public void DeleteCommentFailed()
        {
            int id = 100;

            int oldNumberOfCommentsByPostId = CommentDB.GetCommentsByPostId(active, 1).Count;
            CommentDB.DeleteComment(id);
            int newNumberOfCommentsByPostId = CommentDB.GetCommentsByPostId(active, 1).Count;
            Assert.AreEqual(oldNumberOfCommentsByPostId, newNumberOfCommentsByPostId);
        }


    }
}
