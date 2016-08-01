<?php
$location = $_GET['local'];
$locat = '';
for($i = 0;$i < strlen($location);$i++){
	if (substr($location, $i,1) === " "){
		$locat.='%20';
	}else{
		$locat.=substr($location, $i,1);
	}
}
$json = file_get_contents('http://dev.virtualearth.net/REST/v1/Locations/'.$locat.'?o=json&key=Ar52ZO_55DYdvAGxHND6F5dTaRTLxTSOZa0-pR16w3HpVkuz2FYtSIxsz-PGJhEA');
$data = json_decode($json,true);
$local = md5($data['resourceSets'][0]['resources'][0]['address']['adminDistrict']);
$path = 'api/traficjam/'.md5($local).'.json';
if (file_exists($path)){
$data1 = json_decode(file_get_contents($path),true);
$thismoment = $_SERVER['REQUEST_TIME'];
$changed = 0;
if ($data1!= ""){
foreach ($data1 as $key => $value) {
	if($thismoment - $value['time'] > 1200){
		unset($data1[$key]);
		$changed = 1;
	}
}
	$backup=array();
foreach ($data1 as $key => $value) {
			array_push($backup, array('coor'=>$value['coor'],'t'=>$value['t']));
			}
foreach ($backup as $key => $value) {
	echo $value['coor']['lat'].'_'.$value['coor']['loc'].'='.$value['t'].'|';
}
if ($changed == 1){
	$myfile = fopen($path, "w") or die("400");//400 la k the mo file
	fwrite($myfile, json_encode($data1));
	fclose($myfile);
}
}
}else{
	echo '1';
}
?>