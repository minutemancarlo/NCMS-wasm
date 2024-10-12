using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

public class CryptoService
{
    private readonly IJSRuntime _jsRuntime;

    public CryptoService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string> EncryptAsync(string plainText, byte[] key)
    {
        // Convert byte[] key to Base64 for JavaScript interop
        string base64Key = Convert.ToBase64String(key);

        // Call the JavaScript function to encrypt
        return await _jsRuntime.InvokeAsync<string>("cryptoHelper.encryptData", plainText, Convert.FromBase64String(base64Key));
    }

    public async Task<string> DecryptAsync(string encryptedText, byte[] key)
    {
        // Convert byte[] key to Base64 for JavaScript interop
        string base64Key = Convert.ToBase64String(key);

        // Call the JavaScript function to decrypt
        return await _jsRuntime.InvokeAsync<string>("cryptoHelper.decryptData", encryptedText, Convert.FromBase64String(base64Key));
    }
}
