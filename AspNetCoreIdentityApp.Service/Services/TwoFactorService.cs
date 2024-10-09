using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Service.Services
{
    public class TwoFactorService
    {
        private readonly UrlEncoder _urlEncoder;

        public TwoFactorService(UrlEncoder urlEncoder)
        {
            _urlEncoder = urlEncoder;
        }

        public string GenerateQrCodeUri(string email,string unformattedKey)
        {
            const string format = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6}";

            return string.Format(format, _urlEncoder.Encode("localhost:7074"),_urlEncoder.Encode(email),unformattedKey);
        }

    }
}
