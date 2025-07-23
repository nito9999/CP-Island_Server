using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ice_light_server.api.server
{
    internal class Rsa_Encrytion
    {
        public byte[] Encrypt(byte[] plaintext, RSAParameters publicKey)
        {
            byte[] array;
            using (RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider())
            {
                rsacryptoServiceProvider.ImportParameters(publicKey);
                array = rsacryptoServiceProvider.Encrypt(plaintext, false);
            }
            return array;
        }
    }
}
