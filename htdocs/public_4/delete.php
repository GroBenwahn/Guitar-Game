<?php 
 
if(!isset($_GET['num']))
{
    echo "<script>alert('이상하게 접근하셨습니다;;');";
    echo "history.back();</script>";
    exit; // 추가적인 처리를 방지하기 위해 exit를 추가
}
    
include '../includes/db.php';

$num = intval($_GET['num']); // 입력 값을 정수로 변환하여 SQL 인젝션 방지

$sql = "DELETE FROM url_upload WHERE num = $num";
$res = $conn->query($sql);

if(!$res)
{
    echo "<script>alert('삭제 중 오류가 발생했습니다.');</script>";
    exit;
}

// 성공적으로 삭제되었을 때, 목록 페이지로 리다이렉트
echo "<script>
    alert('파일이 삭제되었습니다.');
    window.location.href = 'http://localhost/public_4/index.php'; // 목록 페이지의 URL로 변경
</script>";

mysqli_close($conn);
?>
