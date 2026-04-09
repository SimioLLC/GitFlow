using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Meziantou.Framework.Win32;

namespace GitFlow
{
    internal class CredentialHandler
    {
        public static void SaveCredential(string RepoIdentifier, string userName, string secret, string email, CredentialPersistence persistence)
        {
            CredentialManager.WriteCredential(
                applicationName: RepoIdentifier,
                userName: userName,
                secret: secret,
                comment: email,
                persistence: persistence);
        }

        public static Credential ReadCredential(string RepoIdentifier)
        {
            var credential = CredentialManager.ReadCredential(RepoIdentifier);
            if (credential == null)
            {
                return null;
            }

            return credential;
        }

        public static void DeleteCredential(string RepoIdentifier)
        {
            CredentialManager.DeleteCredential(RepoIdentifier);
        }

        public static void UpdateCredential(string RepoIdentifier, string newUserName, string newSecret, string newEmail)
        {
            SaveCredential(RepoIdentifier, newUserName, newSecret, newEmail, CredentialPersistence.LocalMachine);
        }


    }
}
