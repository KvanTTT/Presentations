<?php

$id = $_SESSION[ 'id' ];

$query  = "SELECT * FROM users WHERE user_id = '$id'";

$query = mysqli_real_escape_string($query);

$result = mysqli_query($query);