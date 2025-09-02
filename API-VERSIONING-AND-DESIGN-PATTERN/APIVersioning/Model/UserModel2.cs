namespace WebAPI.Model
{
    public class UserModel2
    {
        public string Name { get; }
        public int Age { get; }
        public string Email { get; }
        public UserModel2(string name, int age, string email)
        {
            Name = name;
            Age = age;
            Email = email;
        }
    }
}
