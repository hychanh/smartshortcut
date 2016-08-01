<?php
include 'dat_hash.php';
if(sizeof($_GET) == 5){
	$need = $_GET['needhelp'];
	$user = $_GET['user'];
	$pass = $_GET['pass'];
	$text = 'user_db/'.md5(dat_hash($user)).'.json';
	if(file_exists($text)){
		$data = file_get_contents($text);
		if (isset($data)){
			$data_json = json_decode($data,true);
			if ($data_json['pass'] == md5($pass)){
				$lat=floatval($_GET['lat']);
				$loc=floatval($_GET['loc']);
				$locat = $lat.",".$loc;
				$json = file_get_contents('http://dev.virtualearth.net/REST/v1/Locations/'.$locat.'?o=json&key=Ar52ZO_55DYdvAGxHND6F5dTaRTLxTSOZa0-pR16w3HpVkuz2FYtSIxsz-PGJhEA');
				$data = json_decode($json,true);
				$jsonfile = md5($data['resourceSets'][0]['resources'][0]['address']['adminDistrict']);
				$path = 'api/traficjam/'.md5($jsonfile).'.json';
				//creat file 
				$n = 0;
				if (file_exists($path)){
					$trafic = file_get_contents($path);
					$trafic_json = json_decode($trafic,true);
					if($need == "no"){
					if ($trafic_json!=null){
						foreach ($trafic_json as $key => $value) {
								if((string)$value['coor']['lat'] == (string)$lat and (string)$value['coor']['loc']==(string)$loc) {
								$trafic_json[$key]['t']+=1;
								$trafic_json[$key]['time'] = $_SERVER['REQUEST_TIME'];
								$n = 1;	
								break;
							}
						}
						if ($n == 0){
							array_push($trafic_json,array('coor'=>array('lat'=>$lat,'loc'=>$loc),'t'=>1,'time'=>$_SERVER['REQUEST_TIME']));
						}
					}else{
						$trafic_json = array();
						array_push($trafic_json,array('coor'=>array('lat'=>$lat,'loc'=>$loc),'t'=>1,'time'=>$_SERVER['REQUEST_TIME']));
					}
					$myfile = fopen($path, "w") or die("400");//400 la k the mo file
					fwrite($myfile, json_encode($trafic_json));
					fclose($myfile);
					echo '100';}elseif($need == "yes"){
						$check = 0;
							if ($trafic_json!=null){
								foreach ($trafic_json as $key => $value) {
									if((string)$value['coor']['lat'] == (string)$lat and (string)$value['coor']['loc']==(string)$loc and (string)$value['t']=="needhelp") {
									$check +=1;
								}
							}
							if ($check ==0){
			
									array_push($trafic_json,array('coor'=>array('lat'=>$lat,'loc'=>$loc),'t'=>'needhelp','time' => $_SERVER['REQUEST_TIME']));
									$myfile = fopen($path, "w") or die("400");//400 la k the mo file
									fwrite($myfile, json_encode($trafic_json));
									fclose($myfile);
									echo '100';
							}else{
								echo '50';// co ng da bao can giup do!!.
							}
						}else{
							$trafic_json = array();
								array_push($trafic_json,array('coor'=>array('lat'=>$lat,'loc'=>$loc),'t'=>'needhelp','time' => $_SERVER['REQUEST_TIME']));
									$myfile = fopen($path, "w") or die("400");//400 la k the mo file
									fwrite($myfile, json_encode($trafic_json));
									fclose($myfile);
									echo '100';
						}

					}
				}else{
					$ke = 1;
					if($need == "yes"){
						$ke="needhelp";
					}
					$trafic_json=array();
					array_push($trafic_json,array('coor'=>array('lat'=>$lat,'loc'=>$loc),'t'=>$ke,'time' => $_SERVER['REQUEST_TIME']));
					$myfile = fopen($path, "w") or die("400");//400 la k the mo file
					fwrite($myfile, json_encode($trafic_json));
					fclose($myfile);
					echo '100';
				}

}else{
			echo '1';// sai passwor
		}

}}
else{
		echo '0';//  0user chua tao
	}
}

?>