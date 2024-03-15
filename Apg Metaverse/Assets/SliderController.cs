using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider slider;

    private MySqlConnection connection;
    private string serverAddress = "http://localhost/phpmyadmin/index.php?route=/sql&server=1&db=chatdata&table=instellingen&pos=0";
    private string databaseName = "chatdata";
    private string username = "root";
    private string password = "";

    private void Start()
    {
        ConnectToDatabase();
        RetrieveSliderValueFromDatabase();
    }

    private void ConnectToDatabase()
    {
        string connectionString = $"Server={serverAddress};Database={databaseName};Uid={username};Pwd={password};";
        connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();
            Debug.Log("Connected to MySQL database!");
        }
        catch (MySqlException e)
        {
            Debug.LogError("Error connecting to MySQL database: " + e.Message);
        }
    }

    private void RetrieveSliderValueFromDatabase()
    {
        string query = "SELECT slider_value FROM slider_table WHERE id = 1";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        MySqlDataReader reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            float sliderValue = reader.GetFloat(0);
            slider.value = sliderValue;
        }
        reader.Close();
    }

    public void UpdateSliderValueInDatabase()
    {
        float sliderValue = slider.value;
        string query = $"UPDATE slider_table SET slider_value = {sliderValue} WHERE id = 1";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.ExecuteNonQuery();
    }

    private void OnDestroy()
    {
        if (connection != null && connection.State == System.Data.ConnectionState.Open)
        {
            connection.Close();
            Debug.Log("Disconnected from MySQL database!");
        }
    }
}