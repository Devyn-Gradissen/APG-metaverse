<?php
    $servername = "localhost";
    $username = "Gaijin";
    $password = "";
    $dbName = "chatdata";


    $id = $_POST["idPost"];//"3";
    $chat = $_POST["chatPost"];//"Hallo";
    // maak connectie
    $conn = new mysqli($servername, $username, $password, $dbName);

    // bekijk connectie
    if ($conn->connect_error) {
        die("Connection failed: " . $conn->connect_error);
    }

    $sql = "INSERT INTO chatlog(id, chat)
            VALUES ('".$id."','".$chat."')";
    $result = $conn->query($sql);

    if(!$result) echo "er is een fout";
    else echo "alles oke";

?>


