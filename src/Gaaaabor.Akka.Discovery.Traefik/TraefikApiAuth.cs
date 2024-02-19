using System.Text;

namespace Gaaaabor.Akka.Discovery.Traefik
{
    public record TraefikApiAuth
    {
        /// <summary>
        /// Header to send credentials
        /// </summary>
        public string HeaderName { get; init; } = "Authorization";

        /// <summary>
        /// User used for authorization
        /// </summary>
        public string? User { get; init; }

        /// <summary>
        /// Password used for authorization
        /// </summary>
        public string? Password { get; init; }

        public override string ToString()
        {
            return $"{HeaderName}:{GetUserAndPasswordBase64()}";
        }

        /// <summary>
        /// User and Password base64 in form of Base64(User:Password)
        /// </summary>
        /// <returns></returns>
        public string GetUserAndPasswordBase64()
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{User}:{Password}"));
        }

        public static TraefikApiAuth? Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var values = value.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (values.Length != 2)
            {
                return null;
            }

            var rawUserAndPass = Encoding.UTF8.GetString(Convert.FromBase64String(values[1]));
            var userAndPass = rawUserAndPass.Split(':', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (userAndPass.Length != 2)
            {
                return null;
            }

            return new TraefikApiAuth
            {
                HeaderName = values[0],
                User = userAndPass[0],
                Password = userAndPass[1]
            };
        }

        public static explicit operator TraefikApiAuth?(string value)
        {
            return Parse(value);
        }

        public static explicit operator string(TraefikApiAuth value)
        {
            return value.ToString();
        }
    }
}
