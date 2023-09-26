// See https://aka.ms/new-console-template for more information

using System.Text;
using Encription;

Console.WriteLine("Enter passphrase: ");
string passphrase = Console.ReadLine();
byte[] key = Encoding.UTF8.GetBytes(passphrase);

CliMenu cliMenu = new CliMenu(key);

while (true)
{
    cliMenu.ShowOptions();
    cliMenu.HandleOption();
}
