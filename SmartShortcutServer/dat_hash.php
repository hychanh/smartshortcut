<?php
function dat_hash($str){
	if (!function_exists('get'))
{
function get($x,$i){
	return substr($x, $i,2);
}}
if (!function_exists('hasha'))
{
function hasha($x,$y){
	return (string)(ord($x) + ord($y));
}}
	$cha = $str;
	$result = '';
	for($i = 0;$i<=strlen($str)-1;$i+=2){
		$t = get($str,$i);
		if (strlen($t) == 2)
			$num = hasha(substr($t, 0, 1),substr($t, 1, 2));
		else
			$num = hasha(substr($t, 0, 1),'d');
		$d = (string)((int)(strrev($num))+ (int)($num));
		$result .= $d;
	}
return $result;
}

?>