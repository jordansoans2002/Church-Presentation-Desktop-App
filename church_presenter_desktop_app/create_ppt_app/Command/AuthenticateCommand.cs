using create_ppt_app.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace create_ppt_app.Command
{
    class AuthenticateCommand : CommandBase
    {
        public override void Execute(object? parameter)
        {
            ReAutheticate();
        }

        public async void ReAutheticate()
        {
            await Authentication.Instance.AuthenticateAsync();
        }
    }
}
