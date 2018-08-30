<?php

$id = $_SESSION[ 'id' ];

$query  = "SELECT * FROM users WHERE user_id = '$id'";

$result = mysqli_query($query);