﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenSSL.Core;
using OpenSSL.Crypto;

namespace CommonTools
{
    public class OpensslHelper
    {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
        public byte[] Password;
        string EncodingName { get; set; }
        CipherContext cipherContext;

        public OpensslHelper(string password, string encodingname)
        {
            this.EncodingName = encodingname;
            this.Password = Encoding.GetEncoding(EncodingName).GetBytes(password);
            cipherContext = new CipherContext(Cipher.DES_EDE3_CBC);
            GenerateKeyIV();
        }

        public void GenerateKeyIV()
        {
            byte[] Iv;
            Key = cipherContext.BytesToKey(MessageDigest.MD5, null, this.Password, 8, out Iv);
            this.IV = Iv;
        }

        public byte[] Encrypt(byte[] msg)
        {
            return cipherContext.Encrypt(msg, this.Key, this.IV);
        }

        public byte[] Decrypt(byte[] msg)
        {
            return cipherContext.Decrypt(msg, this.Key, this.IV);
        }
    }
}
