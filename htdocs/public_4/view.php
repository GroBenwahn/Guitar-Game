


<html>
 <meta charset="utf-8"/>
    <title>게시판</title>

<?php

include '../includes/db.php';
	
    
   $sql = "select * from mp3_upload where num=".$_GET['num'];
       $res = $conn->query($sql);
    $res = $res->fetch_assoc();
	



	echo "id:" .$res['id'];
	echo '<p/>';



	echo "subject:" .$res['subject'];
	echo '<p/>';


	echo "memo:" .$res['memo'];
	echo '<p/>';

	echo "<td align='left'>
          <mp3_upload href='./download.php?num=".$res['num']."'>".$res['name']."</a></td>";
	
	 mysqli_close($conn);
 
 ?>
   



</html>


