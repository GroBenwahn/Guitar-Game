<?php
include '../includes/db.php'; // 데이터베이스 연결

// POST 요청에서 이메일과 비밀번호 가져오기
$email = $_POST['email'];
$password = $_POST['password'];

// 결과 응답
$response = array('success' => false, 'message' => '');

try {
    // SQL 쿼리 준비 (이메일로 사용자 검색)
    $stmt = $conn->prepare("SELECT name, password FROM users WHERE email = ?");

    if (!$stmt) {
        throw new Exception("Prepared statement failed: " . $conn->error);
    }

    // 파라미터 바인딩
    $stmt->bind_param("s", $email);

    // 쿼리 실행
    $stmt->execute();

    // 결과 가져오기
    $result = $stmt->get_result();

    if ($result->num_rows > 0) {
        $row = $result->fetch_assoc();
        $hashedPassword = $row['password'];
        $name = $row['name'];

        // 비밀번호 검증
        if (password_verify($password, $hashedPassword)) {
            $response['success'] = true;
            $response['name'] = $name;  // 사용자 이름 반환
            $response['message'] = "Sign in successful. Welcome, $name!";
        } else {
            $response['success'] = false;
            $response['message'] = "Invalid email or password.";
        }
    } else {
        $response['success'] = false;
        $response['message'] = "Invalid email or password.";
    }

    $stmt->close();
} catch (Exception $e) {
    $response['success'] = false;
    $response['message'] = $e->getMessage();
}

$conn->close();

// JSON 응답
echo json_encode($response);
?>
