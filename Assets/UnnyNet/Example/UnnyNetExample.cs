using UnityEngine;
using UnnyNet;

public class UnnyNetExample : MonoBehaviour
{
    private const string TEST_COLLECTION = "CTest";
    private const string TEST_KEY_STRING = "Key_string";
    private const string TEST_KEY_OBJECT = "Key_string";

    private class StorageObject
    {
        public int Number;
        public string String;
    }
    
    private void Start()
    {
        Main.Init(new AppConfig
        {
            ApiGameId = "3ba357ae-03e9-11eb-be4f-0684909fddca",
            PublicKey = "NjlkZjQzYzUtNmExOS00",
            OnReadyCallback = responseData =>
            {
                Debug.Log("UnnyNet Initialized: " + responseData.Success);
                AuthAsGuestUsingUniqueDeviceId();
            },
            Environment = Constants.Environment.Development
        });
    }

    private void AuthAsGuestUsingUniqueDeviceId()
    {
        UnnyNet.Auth.AsGuest(response =>
        {
            Debug.Log("Authorized " + response.Success);
            if (response.Success)
                Debug.Log("User id: " + response.UserId);

            // SaveAndLoadString();
            SaveAndLoadObject();
        });
    }
    
    private void SaveAndLoadString()
    {
        UnnyNet.Storage.Save(TEST_COLLECTION, TEST_KEY_STRING, "Hello", saveResponse =>
        {
            Debug.Log("Save string " + saveResponse.Success);
            UnnyNet.Storage.Load<string>(TEST_COLLECTION, TEST_KEY_STRING, loadResponse =>
            {
                Debug.Log("Load string " + loadResponse.Success);
                if (loadResponse.Success)
                {
                    if (loadResponse.Data != null)
                        Debug.LogWarning("Load Value " + loadResponse.Data.Value);
                    else
                        Debug.LogError("Data is null");
                }
            });
        });
    }
    
    private void SaveAndLoadObject()
    {
        var testObject = new StorageObject
        {
            Number = 26,
            String = "Hello World"
        };
        
        UnnyNet.Storage.Save(TEST_COLLECTION, TEST_KEY_OBJECT, testObject, saveResponse =>
        {
            Debug.Log("Save object " + saveResponse.Success);
            UnnyNet.Storage.Load<StorageObject>(TEST_COLLECTION, TEST_KEY_OBJECT, loadResponse =>
            {
                Debug.Log("Load object " + loadResponse.Success);
                if (loadResponse.Success)
                {
                    if (loadResponse.Data != null)
                    {
                        var loadedObject = loadResponse.Data.Value;
                        Debug.LogWarning("Loaded Value: number = " + loadedObject.Number + " ; string = " + loadedObject.String);
                    }
                    else
                        Debug.LogError("Data is null");
                }
            });
        });
    }
}
