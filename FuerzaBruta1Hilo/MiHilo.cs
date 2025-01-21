 
using System.Security.Cryptography;
using System.Text;

namespace FuerzaBruta1Hilo
{
    public class MiHilo
    {
        Thread hilo;
        private string passwordHash;
        private string password;
        private string[] passwords;
        private string filePath = "C:\\2ºDAM\\PSP\\FuerzaBruta1Hilo\\FuerzaBruta1Hilo\\password.txt";
        Random random = new Random();
    
        public MiHilo()
        {
            passwords = File.ReadAllLines(filePath);
            password = passwords[random.Next(0, 100)];
            passwordHash = HashPassword(password);
            hilo = new Thread(_process);
        }

        public void Start()
        {
            hilo.Start();
        }

        void _process()
        {
            Console.WriteLine($"El hash de la contraseña es {passwordHash}");
            Console.WriteLine($"La contraseña es {password}");

            for (int i = 0; i < passwords.Length; i++)
            {
                string hashedPassword = HashPassword(passwords[i]);
                if (hashedPassword == passwordHash)  
                {
                    Console.WriteLine($"Ha terminado: {passwords[i]}");
                    break;
                }
            }
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Convierte la contraseña a un array de bytes
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convierte los bytes a una cadena hexadecimal
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
        
    }
}
