namespace WebAPI.Model
{
    public class UserModel
    {
        public string Name { get; }
        public int Age { get; }
        public string Email { get; }
        public string Portfolio { get; }
        public UserModel(string name, int age, string email, string portfolio)
        {
            Name = name;
            Age = age;
            Email = email;
            Portfolio = portfolio;
        }
    }
}
