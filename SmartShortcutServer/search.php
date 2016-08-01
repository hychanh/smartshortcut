<?php
if (sizeof($_GET) == 1){
	$place = $_GET['place'];
	$end = 0;
	$data = '';
	for($i = 0;$i < strlen($place);$i++){
		$t = ord(substr($place,$i,1));
		if (($t == 32) or ($t == 21) or ($t == 44) or ($t == 46)or ($t == 30) or ($t >= 32 and $t <=41) or ($t >= 65 and $t <=90) or ($t >= 81 and $t <=122)){
			if ($t == 32){
				$data .= '%20';
			}else{
				$data .=substr($place,$i,1);
			}
		}
		else{
			$end = 1;
		}
	}
if ($end == 1){
	echo '1';// fails  (k nen co ki tu dac biet)
}else{
//$b = json_decode(file_get_contents('http://dev.virtualearth.net/REST/v1/Locations/'.$json.'?o=xml&key=Ar52ZO_55DYdvAGxHND6F5dTaRTLxTSOZa0-pR16w3HpVkuz2FYtSIxsz-PGJhEA'),true);
	
//$c = $b['resourceSets'][0]['resources'][0]['point']['coordinates'];
	//jsonObject["resourceSets"].GetArray()[0].GetObject()["resources"].GetArray()[0].GetObject()["point"].GetObject()["coordinates"].GetObject()["latitude"].GetString();
//foreach ($c as $key => $value) {
//	echo $value.' ';
//}

echo file_get_contents('http://dev.virtualearth.net/REST/v1/Locations/'.$data.'?o=json&key=Ar52ZO_55DYdvAGxHND6F5dTaRTLxTSOZa0-pR16w3HpVkuz2FYtSIxsz-PGJhEA');
}	
}
?>