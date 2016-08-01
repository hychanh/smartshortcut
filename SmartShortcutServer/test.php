<?php
$a = array(
	1,2,3,4
	);
unset($a[2]);
foreach ($a as $key => $value) {
	echo (string)$value;
	echo (string)$key;
}
var_dump($a);
?>
