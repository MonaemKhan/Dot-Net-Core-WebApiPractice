namespace WebAPI.Model
{
    public class UserModelBuilder
    {
        private string? _name;
        private int _age;
        private string? _email;
        private string? _portfolio;

        public UserModelBuilder withName(string name)
        {
            _name = name;
            return this;
        }

        public UserModelBuilder withAge(int age)
        {
            _age = age;
            return this;
        }

        public UserModelBuilder withEmail(string email)
        {
            _email = email;
            return this;
        }

        public UserModelBuilder withPortfolio(string portfolio)
        {
            _portfolio = portfolio;
            return this;
        }

        public UserModel Build()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("Name is required");
            }
            if (_age <= 0)
            {
                throw new InvalidOperationException("Age must be greater than zero");
            }
            if (string.IsNullOrEmpty(_email) || !_email.Contains("@"))
            {
                throw new InvalidOperationException("A valid email is required");
            }
            var data = new UserModel(_name, _age, _email,_portfolio);
            return data;
        }

        public UserModel2 Builder()
        {
            if (string.IsNullOrEmpty(_name))
            {
                throw new InvalidOperationException("Name is required");
            }
            if (_age <= 0)
            {
                throw new InvalidOperationException("Age must be greater than zero");
            }
            if (string.IsNullOrEmpty(_email) || !_email.Contains("@"))
            {
                throw new InvalidOperationException("A valid email is required");
            }
            var data = new UserModel2(_name, _age, _email);
            return data;
        }
    }
}
