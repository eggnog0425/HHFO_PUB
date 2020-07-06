using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Navigation;

namespace HHFO.Models
{
    class Auth : IAuth
    {
        private bool hasAuthenticated = false;

        bool IAuth.HasAuthenticated
        {
            get => hasAuthenticated;
            set => this.hasAuthenticated = value;

        }

        // Token

    }
}
