﻿using Microsoft.AspNetCore.Identity;

namespace Anigure.Data
{
    public class ApplicationUserLogin : IdentityUserLogin<string>
    {
        public virtual ApplicationUser User { get; set; }
    }
}
