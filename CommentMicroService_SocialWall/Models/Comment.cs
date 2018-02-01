using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommentMicroService_SocialWall.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public byte[] Attachment { get; set; }

        public int PostId { get; set; }
        public int UserId { get; set; }

        public bool Active { get; set; }

        
    }
}