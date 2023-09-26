using System.Security.Cryptography;
using System.Text;

namespace Encription;

public class MessageProcessor {
  
  // Instance variable for storing the encryption key
  private byte[] _key;

  // Constructor to initialize the encryption key
  public MessageProcessor(byte[] key) {
    this._key = key;
  }

  // Method to encrypt and save the message
  public void EncryptAndSaveMessage(string message) {
    // Encrypt the message and save it as a file
    byte[] encryptedMessage = EncryptMessage(Encoding.UTF8.GetBytes(message), _key);
    File.WriteAllBytes("encryptedMessage.bin", encryptedMessage);
  }

  // Method to read and decrypt the message from the saved file
  public string ReadAndDecryptMessage() {
    // Read the encrypted message from the file
    byte[] encryptedMessage = File.ReadAllBytes("encryptedMessage.bin");

    // Decrypt and return as string
    return Encoding.UTF8.GetString(DecryptMessage(encryptedMessage, _key));
  }

  // Static method to encrypt a message
  static byte[] EncryptMessage(byte[] message, byte[] key) {
    // Initialize nonce and tag
    byte[] nonce = new byte[12];
    byte[] tag = new byte[16];

    // Generate a random nonce
    RandomNumberGenerator.Fill(nonce);

    // Space for ciphertext
    byte[] ciphertext = new byte[message.Length];

    // Perform the encryption
    using(AesGcm aesGcm = new AesGcm(key)) {
      aesGcm.Encrypt(nonce, message, ciphertext, tag);
    }

    // Combine nonce, tag, and ciphertext into a single byte array
    byte[] encryptedMessage = new byte[nonce.Length + tag.Length + ciphertext.Length];
    Buffer.BlockCopy(nonce, 0, encryptedMessage, 0, nonce.Length);
    Buffer.BlockCopy(tag, 0, encryptedMessage, nonce.Length, tag.Length);
    Buffer.BlockCopy(ciphertext, 0, encryptedMessage, nonce.Length + tag.Length, ciphertext.Length);

    return encryptedMessage;
  }

  // Method to decrypt a message
  private byte[] DecryptMessage(byte[] encryptedMessage, byte[] key) {
    // Extract nonce, tag, and ciphertext from the encrypted message
    byte[] nonce = new byte[12];
    byte[] tag = new byte[16];
    byte[] ciphertext = new byte[encryptedMessage.Length - nonce.Length - tag.Length];

    Buffer.BlockCopy(encryptedMessage, 0, nonce, 0, nonce.Length);
    Buffer.BlockCopy(encryptedMessage, nonce.Length, tag, 0, tag.Length);
    Buffer.BlockCopy(encryptedMessage, nonce.Length + tag.Length, ciphertext, 0, ciphertext.Length);

    // Space for the decrypted plaintext
    byte[] plaintext = new byte[ciphertext.Length];

    // Perform the decryption
    using(AesGcm aesGcm = new AesGcm(key)) {
      aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
    }

    return plaintext;
  }
}