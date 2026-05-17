using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace D424.Classes
{
    public class Users
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }

        [Unique]
        public string Username { get; set; }

        public string Password { get; set; }
    }
}
