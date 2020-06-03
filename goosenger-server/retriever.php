<html><head></head><body><?php
		
		require_once 'limonade/limonade.php';
		//require_once('json.php');
		//$JSON = new Services_JSON;

		dispatch('/goosedb/get', 'getstring');
			function getstring()
			{
				$action = params('action');
				//$base64 = params('base64');
				$myfile = fopen("messages.json", "r") or die("Unable to open file!");
				$messages = json_decode(fread($myfile,filesize("messages.json")));
				//array_push($messages, $base64);
				//file_put_contents($myfile, $messages);
				fclose($myfile);
				//return json_last_error_msg0();
				return $messages[array_rand($messages)];
			}
		run();

		?></body></html>