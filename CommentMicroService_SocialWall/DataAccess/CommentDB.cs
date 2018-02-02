using CommentMicroService_SocialWall.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using URISUtil.DataAccess;
using URISUtil.Logging;
using URISUtil.Response;

namespace CommentMicroService_SocialWall.DataAccess
{
    public class CommentDB
    {
        private static Comment ReadRow(SqlDataReader reader)
        {
            Comment retVal = new Comment
            {
                Id = (int)reader["Id"],
                Text = reader["Text"] as string,
                Date = (DateTime)reader["Date"],
                Attachment = (byte[])reader["Attachment"],
                PostId = (int)reader["PostId"],
                UserId = (int)reader["UserId"],
                Active = (bool)reader["Active"]
            };

            return retVal;
        }

        private static void ReadId(SqlDataReader reader, Comment comment)
        {
            comment.Id = (int)reader["id"];
        }

        private static string AllColumnSelect
        {
            get
            {
                return @"
                    [Comment].[Id],
                    [Comment].[Text],
                    [Comment].[Date],
                    [Comment].[Attachment],
                    [Comment].[PostId],
                    [Comment].[UserId],
                    [Comment].[Active]
                ";
            }
        }

        private static void FillData(SqlCommand command, Comment comment)
        {
            //command.AddParameter("@Id", SqlDbType.Int, comment.Id);
            command.AddParameter("@Text", SqlDbType.NVarChar, comment.Text);
            command.AddParameter("@Date", SqlDbType.DateTime, comment.Date);
            command.AddParameter("@Attachment", SqlDbType.VarBinary, comment.Attachment);
            command.AddParameter("@PostId", SqlDbType.Int, comment.PostId);
            command.AddParameter("@UserId", SqlDbType.Int, comment.UserId);
            command.AddParameter("@Active", SqlDbType.Bit, comment.Active);
        }

        public static List<Comment> GetCommentsByPostId(ActiveStatusEnum active, int id)
        {
            try
            {
                List<Comment> retVal = new List<Comment>();
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                    SELECT {0}  
                    FROM [dbo].[Comment]
                    WHERE ([dbo].[Comment].PostId = @Id) 
                    AND ([dbo].[Comment].Active = @Active)
                    
                ", AllColumnSelect);
                    command.AddParameter("@Id", SqlDbType.NVarChar, id);
                    command.Parameters.Add("@Active", SqlDbType.Bit);

                    switch (active)
                    {
                        case ActiveStatusEnum.Active:
                            command.Parameters["@Active"].Value = true;
                            break;
                        case ActiveStatusEnum.Inactive:
                            command.Parameters["@Active"].Value = false;
                            break;
                        case ActiveStatusEnum.All:
                            command.Parameters["@Active"].Value = DBNull.Value;
                            break;
                    }

                    System.Diagnostics.Debug.WriteLine(command.CommandText);
                    connection.Open();


                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            retVal.Add(ReadRow(reader));
                        }
                    }
                }
                return retVal;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                return null;
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }

        }

        public static Comment GetComment(int id)
        {
            try
            {
                Comment retVal = new Comment();
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        SELECT {0} FROM [dbo].[Comment] 
                        WHERE [dbo].[Comment].Id = @Id;
                    ", AllColumnSelect);
                    command.AddParameter("@Id", SqlDbType.Int, id);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retVal = ReadRow(reader);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                return retVal;
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static Comment InsertComment(Comment comment)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        INSERT INTO [dbo].[Comment] 
                         (
                            [Text],
                            [Date],
                            [Attachment],
                            [PostId],
                            [UserId],
                            [Active]
                                          
                        )
                        VALUES
                        (
                            @Text,
                            @Date,
                            @Attachment,
                            @PostId,
                            @UserId,
                            @Active
                        )
                    ");
                    FillData(command, comment);
                    if (comment.Text == null || comment.Text == "")
                    {
                        return null;
                    }
                    connection.Open();
                    command.ExecuteNonQuery();

                    return comment;
                    //da vrati objekat u POSTMENU (id 0, nemam get za ID)
                    //return GetComment(comment.Id);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                return null;
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static Comment UpdateComment(Comment comment, int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        UPDATE [dbo].[Comment] 
                        SET 
                            [Text] = @Text 
                           
                        WHERE
                            [Id] = @Id
                    ");

                    command.AddParameter("@Id", SqlDbType.Int, id);
                    command.AddParameter("@Text", SqlDbType.NVarChar, comment.Text);
                    if (comment.Text == null || comment.Text == "")
                    {
                        return null;
                    }
                    connection.Open();
                    command.ExecuteNonQuery();
                    return GetComment(id);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }

        public static void DeleteComment(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(DBFunctions.ConnectionString))
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandText = String.Format(@"
                        UPDATE
                            [dbo].[Comment]
                        SET
                            [Active] = 0
                        WHERE
                            [Id] = @Id     
                    ");

                    command.AddParameter("@Id", SqlDbType.Int, id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(ex);
                throw ErrorResponse.ErrorMessage(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}