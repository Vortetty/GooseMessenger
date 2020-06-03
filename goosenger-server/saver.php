<html><head></head><body><?php
		
		require_once 'limonade/limonade.php';
		//require_once('json.php');
		//$JSON = new Services_JSON;

		dispatch('/goosedb/write/:base64', 'writestring');
			function writestring()
			{
				$action = params('action');
				$base64 = params('base64');
				$myfile = fopen("messages.json", "r+") or die("Unable to open file!");
				$messages = json_decode(fread($myfile,filesize("messages.json")));
				array_push($messages, $base64);
				fclose($myfile);
				file_put_contents("messages.json", json_encode($messages));
				return "saved"; //json_encode($messages);
				//return json_last_error_msg0();
				//return $messages[array_rand($messages)];
			}
		run();

		?></body></html>