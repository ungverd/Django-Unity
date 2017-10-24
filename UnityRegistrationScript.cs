using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System.Collections.Generic;

public class RegScript : MonoBehaviour
{
    string state = "main";
    string logged_username = null;
    string username = "";
    string password1 = "";
    string password2 = "";
    string url = "paste your URL here!" + "api/";
    string errorstring = "";

    void OnGUI()
    {
        if (state == "register")
        {
            GUILayout.Label("Registration. Enter username.");
            username = GUILayout.TextField(username, 150, GUILayout.MinWidth(20));
            GUILayout.Label("Enter password twice for verification.");
            password1 = GUILayout.PasswordField(password1, '*', 25);
            password2 = GUILayout.PasswordField(password2, '*', 25);
            if (errorstring != "")
            {
                string[] errors = errorstring.Split(';');
                foreach (string error in errors)
                {
                    GUILayout.Label(error);
                }
            }
            if (GUILayout.Button("submit"))
            {
                if (password1 != password2) errorstring = "The two password fields didn't match.";
                else StartCoroutine(DoRequest(username, password1, state));
            }
            if (GUILayout.Button("Already registered? Login"))
            {
                state = "login";
            }
        }
        if (state == "main")
        {
            if (logged_username == null)
            {
                GUILayout.Label("You have not entered");
                if (GUILayout.Button("registration"))
                {
                    state = "register";
                }
                if (GUILayout.Button("login"))
                {
                    state = "login";
                }
            }
            else
            {
                GUILayout.Label("You are " + logged_username);
                if (GUILayout.Button("log out"))
                {
                    logged_username = null;
                }
            }
        }
        if (state == "login")
        {
            GUILayout.Label("Login. Enter your username and password");
            username = GUILayout.TextField(username, 25, GUILayout.MinWidth(20));
            password1 = GUILayout.PasswordField(password1, '*', 25);
            if (errorstring != "")
            {
                string[] errors = errorstring.Split(';');
                foreach (string error in errors)
                {
                    GUILayout.Label(error);
                }
            }
            if (GUILayout.Button("login"))
            {
                StartCoroutine(DoRequest(username, password1, state));
            }
            if (GUILayout.Button("Haven't username and password yet? Register here!"))
            {
                state = "register";
            }
        }
    }

    IEnumerator DoRequest(string username, string password, string postfix)
    {
        Dictionary<string, string> dict = new Dictionary<string, string>();
        dict.Add("username", username);
        dict.Add("password", password);
        string urlparams = ConstructUrlParams(dict);

        using (UnityWebRequest www = UnityWebRequest.Get(url + postfix + "/?" + urlparams))
        {
            www.SetRequestHeader("User-Agent", "Unity");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                errorstring = www.error;
            }
            else
            {
                Response resp = JsonUtility.FromJson<Response>(www.downloadHandler.text);
                errorstring = resp.errors;
                if (resp.status == "success")
                {
                    if (postfix == "login")
                    {
                        logged_username = username;
                        state = "main";
                    }
                    if (postfix == "register")
                    {
                        state = "login";
                    }
                }
            }
        }
    }

    string ConstructUrlParams(Dictionary<string, string> dict)
    {
        StringBuilder sb = new StringBuilder();
        foreach (KeyValuePair<string, string> pair in dict)
        {
            if (sb.Length != 0) sb.Append("&");
            sb.Append(WWW.EscapeURL(pair.Key));
            sb.Append("=");
            sb.Append(WWW.EscapeURL(pair.Value));
        }
        string urlParams = sb.ToString();
        return urlParams;
    }
}

[System.Serializable]
public class Response
{
    public string status;
    public string errors;
}