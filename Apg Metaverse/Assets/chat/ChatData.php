<?php
    $servername = "localhost";
    $username = "Gaijin";
    $password = "";
    $dbName = "chatdata";

    // maak connectie
    $conn = new mysqli($servername, $username, $password, $dbName);

    // bekijk connectie
    if ($conn->connect_error) {
        die("Connection failed: " . $conn->connect_error);
    }

    $sql = "SELECT id, chat FROM chatlog";
    $result = $conn->query($sql);

    if ($result->num_rows > 0) {
        // data voor elke rij vanuit de database halen
        while ($row = $result->fetch_assoc()) {
            echo "id: " . $row['id'] . " chat: " . $row['chat'] . "<br>";
        }
    } else {
        echo "0 results";
    }

    $conn->close();
?>
