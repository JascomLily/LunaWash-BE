using System;
using System.IO;
using BCrypt.Net;

namespace HashGen {
    public class Program {
        public static void Main() {
            Console.WriteLine(BCrypt.Net.BCrypt.HashPassword("123456"));
        }
    }
}
