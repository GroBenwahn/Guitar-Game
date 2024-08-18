<html>
   <meta charset="utf-8">

<?php

include '../includes/db.php';

$musician = $_POST['musician'];
$name = $_POST['name'];
$memo = $_POST['memo'];
$url = mysqli_real_escape_string($conn, $_POST['url']);  // URL 입력값만 이스케이프 처리

$date = date("YmdHis", time());

// SQL 쿼리 작성 (각 값을 작은따옴표로 감쌈)
$sql = "INSERT INTO url_upload (name, url, musician, memo, uploaded_time) ";
$sql .= "VALUES ('$name', '$url', '$musician', '$memo', '$date')";

// SQL 쿼리 실행
$result = $conn->query($sql);

if (!$result) {
    echo "DB upload error: " . $conn->error;
    exit;
}

mysqli_close($conn);
echo("<script>location.href='index.php';</script>");
echo "<script>alert('업로드 성공');</script>";

?>
</html>
