<html>
    <meta charset="utf-8"/>
    <title>게시판</title>
</html>
<body>
    <form action="write.php" method="POST" enctype="multipart/form-data" />
	<table  align= "center" >
	<col width=100></col><col width=100></col>
		<tr>
			<td>가수:</td>
			<td><input type="text" name="musician" /></td>
		</tr>
		<tr>
			<td>음악이름:</td>
			<td><input type="text" name="name" /></td>
		</tr>
		<tr>
			<td>부가설명:</td>
			<td><textarea name="memo" rows="20"></textarea></td>
		</tr>
		<tr>
			<td>URL:</td>
			<td><input type="text" name="url" /></td>
		</tr>
		<tr>
			<td>
				<input type="submit" value="전송" />
			</td>
		</tr>
		</table>
    </form>
</body>
</html>