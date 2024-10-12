// wwwroot/cryptoHelper.js

// Encrypt using AES-GCM
async function encryptData(plainText, key) {
    const encoder = new TextEncoder();
    const data = encoder.encode(plainText);
    const iv = crypto.getRandomValues(new Uint8Array(12)); // AES-GCM requires a 12-byte IV

    const cryptoKey = await crypto.subtle.importKey(
        "raw",
        key,
        { name: "AES-GCM" },
        false,
        ["encrypt"]
    );

    const encrypted = await crypto.subtle.encrypt(
        {
            name: "AES-GCM",
            iv: iv,
        },
        cryptoKey,
        data
    );

    const encryptedData = new Uint8Array(encrypted);
    const result = new Uint8Array(iv.length + encryptedData.length);
    result.set(iv);
    result.set(encryptedData, iv.length);

    return btoa(String.fromCharCode.apply(null, result));
}

// Decrypt using AES-GCM
async function decryptData(encryptedBase64, key) {
    const encryptedData = atob(encryptedBase64);
    const encryptedArray = Uint8Array.from(encryptedData, c => c.charCodeAt(0));

    const iv = encryptedArray.slice(0, 12); // The first 12 bytes are the IV
    const encrypted = encryptedArray.slice(12); // The rest is the encrypted data

    const cryptoKey = await crypto.subtle.importKey(
        "raw",
        key,
        { name: "AES-GCM" },
        false,
        ["decrypt"]
    );

    const decrypted = await crypto.subtle.decrypt(
        {
            name: "AES-GCM",
            iv: iv,
        },
        cryptoKey,
        encrypted
    );

    const decoder = new TextDecoder();
    return decoder.decode(decrypted);
}

// Export the functions for Blazor WebAssembly to call
window.cryptoHelper = {
    encryptData,
    decryptData
};
