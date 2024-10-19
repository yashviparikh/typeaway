using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eup
{
    internal class Class1
    {
    }
    public static class CurrentUser
    {
        public static int UserID { get; set; } = -1; // Default to -1 to indicate no user logged in
        public static string FirstName { get; set; }
        public static string LastName { get; set; }
        public static string Email { get; set; }
    }


}
