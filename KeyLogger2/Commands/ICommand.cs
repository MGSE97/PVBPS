using System.Collections.Generic;

namespace KeyLogger.Commands
{
    public interface ICommand
    {
        public void RunAsUser(string runFile);

        public void RunAsAdmin(string runFile);

        public bool ChecksFailed();
    }
}