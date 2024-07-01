using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; // Import UnityWebRequest

public class datainserter : MonoBehaviour
{
    public string inputid; // krijg de input van de gebruiker
    public string inputchat; // krijg de input van de gebruiker

    string CreateUserURL = "http://localhost/apg/chatdata.php"; //maak verbinding met de database

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) CreateData(inputchat, inputid); //verstuur de chat en het id
    }

    public void CreateData(string id, string chat)
    {
        WWWForm form = new WWWForm();
        form.AddField("idPost", id);
        form.AddField("chatPost", chat);

        UnityWebRequest www = UnityWebRequest.Post(CreateUserURL, form); // Use UnityWebRequest.Post method
        www.SendWebRequest(); // Send the request asynchronously

        // Optionally handle response here
    }
}
