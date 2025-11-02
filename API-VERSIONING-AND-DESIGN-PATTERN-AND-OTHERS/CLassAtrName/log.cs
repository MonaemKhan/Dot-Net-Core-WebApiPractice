using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CLassAtrName
{
    public class LoginModelDTO
    {
        public string _userId { get; set; }
        public string _password { get; set; }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public LoginModelEntitySearch getSerarchEntity()
        {
            return new LoginModelEntitySearch
            {
                UserId = _userId,
                Password = _password
            };
        }
    }

    [MapStoreProcedure("user_login")]
    public class LoginModelEntitySearch
    {
        [MapColumn("userid")]
        public string UserId { get; set; }
        [MapColumn("userpass")]
        public string Password { get; set; }
        [MapColumn("errormsg", true)]
        public string UserOfficeId { get; set; }
    }
}
