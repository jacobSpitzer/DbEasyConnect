using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dapper.TQuery.Development;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDapper.Tables
{
    abstract class Base
    {
        [Key]
        [AutoIncrement]
        [Required]
        public int Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsDelete { get; set; }
    }
    
    [Table("ForumUsers")]
    class ForumUser: Base
    {
        public string Username { get; set; }
        public int Role { get; set; }
        public int Status { get; set; }
    }
    abstract class ForumBase : Base
    {
        public int UserId { get; set; }
        public string Body { get; set; }
        public int Upvotes { get; set; }
        public int Downvotes { get; set; }
        public int Views { get; set; }
    }
    
    [Table("Questions")]
    class Question : ForumBase
    {
        public string Subject { get; set; }
        public string CensorBody { get; set; }
        public string Status { get; set; }
    }
    
    [Table("Tags")]
    class Tag
    {
        public int Id { get; set; }
        public string TagName { get; set; }
    }
    
    [Table("TagJoins")]
    class TagJoin
    {
        public int TagId { get; set; }
        public int QuestionId { get; set; }
    }
    
    [Table("Answers")]
    class Answer : ForumBase
    {
        public int QuestionId { get; set; }
        public bool AcceptedAnswer { get; set; }
    }
    
    [Table("Comments")]
    class Comment : ForumBase
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
    }
}
