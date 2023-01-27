using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browser
{
    public static class UserSession
    {
        private static volatile User currentUser;//The volatile keyword indicates that a field might be modified by multiple threads that are executing at the same time
        private static  object syncRoot = new Object(); 
        public static User GetUser()
        {
            if (currentUser == null) return null; 
            return currentUser;
        }

        public static User Login(User user)
        {
            if (currentUser != null)
            { 
                return currentUser;
            }
            lock (syncRoot)
            {
                currentUser = user;
            }
            return null;
        }

        public static void Logout()
        {
            lock (syncRoot)
            {
                currentUser = null;
            }
        }
    }
}
