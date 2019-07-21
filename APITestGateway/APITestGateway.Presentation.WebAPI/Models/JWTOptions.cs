using System;

namespace APITestGateway.Presentation.WebAPI.Models
{
    public class JWTOptions
    {
        private DateTime _Now;
        public double Hours { get; set; }
        public string Issuer { get; set; }
        public string Secret { get; set; }
        public string Audience { get; set; }

        public string Iat
        {
            get
            {
                return _Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }


        public DateTime Now
        {
            get
            {
                return _Now;
            }
        }

        public DateTime Expires
        {
            get
            {
                return _Now.Add(TimeSpan.FromHours(Hours));
            }
        }

        public int Expires_in
        {
            get
            {
                var a = DateTime.UtcNow;
                return (int)TimeSpan.FromHours(Hours).TotalSeconds;
            }
        }

        public JWTOptions()
        {
            _Now = DateTime.Now;
        }
    }
}
