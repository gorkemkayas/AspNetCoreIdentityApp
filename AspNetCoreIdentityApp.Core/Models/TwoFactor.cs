using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCoreIdentityApp.Core.Models
{
    public enum TwoFactor
    {
        [Display(Name ="None")]
        None=0,
        [Display(Name = "Authentication by phone")]
        Phone =1,
        [Display(Name = "Authentication by Email")]
        Email =2,
        [Display(Name = "Authentication by Microsoft/Google Authenticator")]
        MicrosoftGoogle =3
    }
}
