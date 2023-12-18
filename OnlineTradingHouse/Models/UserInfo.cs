using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineTradingHouse.Models
{
    public class UserInfo
    {
        private static OnlineTradingHouseEntities db = new OnlineTradingHouseEntities();
        public static int GetUserID(string email)
        {
            var user = db.Users.Where(u => u.Email == email).FirstOrDefault();
            if (user == null)
            {
                return 0;
            }
            else
            {
                return user.Id;
            };
        }
    }
}