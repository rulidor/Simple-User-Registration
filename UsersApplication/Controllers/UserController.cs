using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using UsersApplication.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace UsersApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment  _env;

        public UserController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                            select username, firstName, lastName, timeZone, phoneNum,
                            aboutMe, photoFileName from
                            dbo.users
                            ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("UsersAppCon");
            SqlDataReader myReader;
            using(SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using(SqlCommand myCommand=new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpGet("{username}")]
        public JsonResult Get(string username)
        {
            string query = @"
                            select username, firstName, lastName, timeZone, phoneNum,
                            aboutMe, photoFileName from
                            dbo.users where username=@username
                            ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("UsersAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@username", username);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(User user)
        {
            string query = @"
                            insert into dbo.users
                            (username,firstName,lastName,timeZone,phoneNum,aboutMe,photoFileName)
                            values (@username,@firstName,@lastName,@timeZone,@phoneNum,@aboutMe,@photoFileName)
                            ";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("UsersAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@username", user.username);
                    myCommand.Parameters.AddWithValue("@firstName", user.firstName);
                    myCommand.Parameters.AddWithValue("@lastName", user.lastName);
                    myCommand.Parameters.AddWithValue("@timeZone", user.timeZone);
                    myCommand.Parameters.AddWithValue("@phoneNum", user.phoneNum);
                    myCommand.Parameters.AddWithValue("@aboutMe", user.aboutMe);
                    myCommand.Parameters.AddWithValue("@photoFileName", user.photoFileName);

                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Added successfully");
        }

        [HttpDelete("{username}")]
        public JsonResult Delete(string username)
        {
            string query = @"
                            delete from dbo.users where username=@username";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("UsersAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@username", username);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Deleted successfully");
        }

        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;
                using(var stream=new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }
                return new JsonResult(filename);
            } catch (Exception)
            {
                return new JsonResult("anonymous.png");
            }
        }
    }
}
