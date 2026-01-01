namespace APICOOKIESTESTING
{
    public interface IUserManagement
    {
        public void AddUser(string username);
        public void RemoveUser(string username);
        public void UpdateUser(string username);
        public void DeleteUser(string username);
        public string GetUser(string username);
        public List<string> GetAllUsers();
    }
    public class UserManagement : IUserManagement
    {
        private readonly List<string> _users = new List<string>();
        public void AddUser(string username)
        {
            _users.Add(username);
        }

        public void DeleteUser(string username)
        {
            throw new NotImplementedException();
        }

        public List<string> GetAllUsers()
        {
            return _users;
        }

        public string GetUser(string username)
        {
            throw new NotImplementedException();
        }

        public void RemoveUser(string username)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(string username)
        {
            throw new NotImplementedException();
        }
    }
}
