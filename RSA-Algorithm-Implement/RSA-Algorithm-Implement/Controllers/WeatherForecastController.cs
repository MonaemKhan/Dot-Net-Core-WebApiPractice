using Microsoft.AspNetCore.Mvc;

namespace RSA_Algorithm_Implement.Controllers
{
    public class UserDetails
    {
        public string UserName { get; set; }
        public string Email { get; set; }
    }
    [ApiController]
    [Route("api/Algo")]
    public class WeatherForecastController : ControllerBase
    {       

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly string Publickey = "<RSAKeyValue><Modulus>uy9NHmj7HIj1VEa2ylBFgkW+6RTwSDIgqgZzTfSFn2M94dof4VyPPUOKs6toPig7ZManRrqnCbRahhriv1OtW9L6Cdczzm3S9Ulr0soSLlmbMmb/Kav2T5VEn1ZZi3hIBAiunwtleewND0xNnW6MQc0T6YsPsHCVY+Bs2/kAkJJVE8dF23gibEBBo9hJuDBM8qzLHpIyTJOP/auzofWT8v6mKzL0Ez4YQUCMV43nNgTa6EbxsTb/PYnftFc7Z6G77XbsScazS4uj77Mu1H4W84BxcaExiSEh6WK8db9VThsUhScdffyEMwCpeRCgPQHqYjI2J1GMaNdEDpFGZs07dLWVhzIOM37+uoSAz/ydRBIlWwVa/B5DiYZxn/aDXACy8zGEnkyBnvgwicQgHPFSQN7oAI3+0HIqDyo1Oex9aBzdwZHcXU+/zM2yIHz1gcTkPQnDkRkkiKvpQVczEt/5WvTRBO8SzZLCAG9aEXE5/hIEqCzwZaogf1GrzttIBB+/oQi0zIRFEJXxJ0y1wr+fP4mzx8EXlufV2st/ZdrBzc7RSMaSxFoKmosEOGgaRxlb0K5flvdccWIGedaUWXyB8vuP6oHspzWzCBIDfnEvqVHJ+mWju2wHFZxQ9Pk34zKGSiCrVQ/IdhR0NVLxTRYoWidFzbbGxLDYe8T7JdIcG9E=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        private readonly string Privatekey = "<RSAKeyValue><Modulus>r1N8SIJGP4QQfP6KHa8lyCzE69rBbWoX8SPIXyPw9/zfFAiFVFz6HioUtnLNTc/9ZU5rWuTNUG32iCrsK1I9o7KtBGI97rDlm6DpNpC0EHMkMyHTVe7XbORStD7Rh8OciyE0VnoZOzHk/3g2z394b/aRyhctDEPe3yGEZZlUn2PW6yQWuUx1czuy4sZVW6yNbhDMvYBrdfoPZFZgLhAVo+uKOVCqkIK2C7klHCJmtllTys/bM+s4MW40OD1yzJ0zjFeQPfq5eDWqyGYj9bkKAZNlhByf4ghSdFGalweFMNsO4uMWC+Np6jkKNMDXX0AAv60MdM316g0W8Su9jqeaOtV/W7SIrduCTn8Osn5FBu5P1cjDbzsg2v2yM30JTHjCcy6EGbQ8BnYCU+HSe9aLfvPwe20CoHr7tjldwMrbtBSicgLhnWvn+X8OMIozKLTNw9Ps6afJW0nOS1BxJ4JHkgPx3xA7JM4Ok0bK4Sj6bh9YOdYlEsvTzLjdORGiWCXzB4uOoDEDypoqWEURQSb7jPn5rek7X0dfUxS0HHc1oM6zQIBf5qqKRLR/cA352zKAKVF5vTryZMYCPWPtlqJPHaGkNr+NrtHdeshO8lWs3+EYVsm7g9iUiwgV6qCvaDkkO2i624TzchFjL4ZOqryhmHpIlmqoRI4QWVJeKX8izPk=</Modulus><Exponent>AQAB</Exponent><P>yo8EBb0aghKXMQqdnzh9PsVzY+MNMCXWV69WRCdaKRBvCaYZPfs9TH9qrzMsdOc1dWABifGHGyZUOK0/TeOJC6E6fSxtd/+GJhJQN1L/uDz82Y58ohKIcy+0vWx27+KCkCQXIKyHu1yIg3W9j7al5xpwjc3y5cu/NWm9tzqTSl3euDoZkJQ2i3KGynZk0Pcp0Fc1f2666/sjk8EmBIEKaXNQZrh/ucTbdiLwexh3uTdyFDXghLVBWR/esrSkrFCXLjVW50endBzPFggrthraIl53p1eNjsKEMk3xqjZUzhb+LQlXN0w6T7/bkeHk19ttj55MzPlgXKUnq/iZyrA7iw==</P><Q>3ZUpKhi9+HqSigRnlLOKz6YTiFMA2kbu0iYaWiACmACVtRoLOU7Wl0lM0EmqDdkx4OvLmLg906d+mKEI2RHkW7Z805/IyaR5MzCTWHFmhJMvMtw+tloSh79W6FY+nqOoGK/f1ixty7yiTjh1a6FqQQPdwV4S4xj+XWBARgn+Yg2W5Y1L7Rlq8uIRjNduCH+CLMgHk+R8dEvvTVGOPv79gjXBYUtVUr4DMytnIpjVCen1xGvMj8xHPSm7sL9jxoWzitYKhU18ZkQ7Gq7ErUfpWEndwghET+m6t8p1/Jht4aGeCj0MStzI38LbqPNJTTxQ+SgLCh7hNR8LUgMyzZB6Cw==</Q><DP>EWg48EVQAd2XIJ/I6+Xu3XYadHsOpxSZVu+6eiDnnp/K6wlbIfh1TB2nDWdYarNz5KJbf39CHRMuKbC5v/fNzzIL1LX8slNDNAJe4mI/l5WmJQwZqdPt5fgzkBPCJNtSXRC87BHnmDJbiPCVWcRYfYPIKGeKeDUKOYzcwOJHxhxLB4n2qHYUOeedrch+4XZcIYtdDZZ2edEbPZdbrQdp3aYLQsiYMV65fW2DPRDdOQ2KwDHhGYZ6Fy6l9LWRcT17wHzU+wstWM0g/sO0/fFQzaNQpRDhGWOd+LS6D+abYxqWvUgQNrBWUoQqWl8PS32P7Met0e+cWFnZDu5OSRzcAQ==</DP><DQ>GVLsy3TccUOA7+kk9We3m5e9b70cwdWNgdL8/APgiAK5GWOKmNgbylqEPuHMX5zNCImqUR/tYP0piMQOwNA8qwamKYF/bxwvBdytWlRLhSdVI5jsSebtazpA8ni422SU141yJYYaXkiGvC3TraNbA2cvtuPLgMzgdUeE0PxfaFFpR37oiIZ5JJ0VqGUOnEYGWJvb0dkxKBFtngaOHUEDNBh0q2vyqmww7W4ucVPKNUHZ5sLzNBQCqEjWvJKDErNBG8LunOGIMB/oKVqD+9HWJ1L51y0esYti3ZJz/ZRmKn1QFJd5iizFgF1CwYJchEk6b99Vs6hn0KxlnM/kxH2YlQ==</DQ><InverseQ>huyEORz3+WG945Loq15iRkWJS+tcNNvots13dlI6U80he2oq6bqzTifHvONVaEiha1xJAQQLyrykU843LBUMkqTXpemSIedU8xNy7M9JJFtZHDQYw8M7pSJI7BkDHqgvMnIkT16v1uTE5NTN8gLT3XWN/nsKZk8Pxro7FuD1ac/adAXv6dNfUCXMdSZB/7+QxEncA8vY4+ucZa7X1CoXG+PuO7PBoz3NP0NrTZrici7h++4L7UjyRhSXD0OPdPq60nIjNyndZFu0cP+Ui0/k0qsv6OW1N7RR2lkd+vIYcDoQNJp3AIvnR4sChntS+J3n6asB3xR8v6IRCIUak/hQgQ==</InverseQ><D>O0A3jDmai/SRrexHuOhsGE4o7pwrKlLkbHXvTVfUI7qtmROYG36geHk4LsB1aYqU1BvAZCNWz4uZ+UKoNavBcQT9O7IHxBa8QfALYIlekQCYhz8zMKNa4k33NTrVjNAqo0pBacgxprOh9EQoIc+uYAgwFN/oNXDqbPXizIMY+hS/mQ/FLjIMrzjJp6HnTuftk/jReGlh56jucrZOIsMEzTTYra35NoU2CJg1VmC53X4qtyuvpMglYW8g7rC1jzjBJvigopGNBHb+lGMQXAaAvy+U6yTRU44Eb5L0b333IR+m1D5yQOpF7QYB0PoqzuHOXviYcRD5vnNWAYMKuJ8jm5ccbYgi98GglyA109aAZQI9IfmfPe07rvS+cozddanaqLvUuZgzHAsWtSb2Ur0SsZRxBwXRKcEh8OxF7jtVoyUFUDrGyOqbtnn3TriDnez3Isgl0oi3i7Yb9GQ2OARspAVDAiEx8VBFt1t8qdB6Qs8+H76phCAtsizn6ttjJaQEkOiOn+PjhlkFz6sGlMxVz8Mj/2YK4xFSPMA2GAUBpf10f32Gf92sGmxF10B7BMfWegPg4PRcx8zI1S1f5xFb/8rje5RisnQY9eQFVwMO0LI9v/lUg701KCQqfIz9wnPCjzYjajtnNjnXpk+7asO7ro/TuFoeHSQuh/WUmFjep5E=</D></RSAKeyValue>";

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("GenerateKeys")]
        public IActionResult Get()
        {
            var (publicKey, privateKey) = RSA_Algorithm.RSA_Algorithm.GenerateKeys();
            return Ok(new { PublicKey = publicKey, PrivateKey = privateKey });
        }

        [HttpGet("EncryptText")]
        public IActionResult GetEncrypt(string plainText)
        {
            var en = RSA_Algorithm.RSA_Algorithm.EncryptText(plainText, Publickey);
            return Ok(new { EncryptedText = en });
        }

        [HttpPost("EncryptObject")]
        public IActionResult GetEncryptObjetc(UserDetails plainText)
        {
            var en = RSA_Algorithm.RSA_Algorithm.EncryptObject(plainText, Publickey);
            return Ok(new { EncryptedText = en });
        }

        [HttpGet("DecryptText")]
        public IActionResult GetDecrypt(string encryptedText)
        {
            var de = RSA_Algorithm.RSA_Algorithm.DecryptText(encryptedText, Privatekey);
            return Ok(new { DecryptedText = de });
        }

        [HttpPost("DecryptObject")]
        public IActionResult GetDecryptObj(string encryptedText)
        {
            var de = RSA_Algorithm.RSA_Algorithm.DecryptObject<UserDetails>(encryptedText, Privatekey);
            return Ok(de);
        }
    }
}
