<?php
include '../includes/db.php'; // 데이터베이스 연결

// POST 요청에서 기존 및 새로운 사용자 정보를 가져오기
$old_name = $_POST['old_name'];
$old_email = $_POST['old_email'];

$new_name = $_POST['new_name'];
$new_email = $_POST['new_email'];
$new_password = $_POST['new_password'];

// 결과 응답
$response = array('success' => false, 'message' => '');

try {
    // 기존 이메일과 이름을 기준으로 사용자를 찾고, 현재 저장된 해시된 비밀번호를 가져옵니다.
    $stmt = $conn->prepare("SELECT password FROM users WHERE name=? AND email=?");
    if (!$stmt) {
        throw new Exception("Prepared statement failed: " . $conn->error);
    }

    // 파라미터 바인딩 및 쿼리 실행
    $stmt->bind_param("ss", $old_name, $old_email);
    $stmt->execute();
    $stmt->store_result();

    if ($stmt->num_rows > 0) {
        $stmt->bind_result($storedPasswordHash);
        $stmt->fetch();

        // 비밀번호 해싱
        $hashedNewPassword = password_hash($new_password, PASSWORD_BCRYPT);

        // 사용자의 정보를 업데이트하는 SQL 쿼리 준비
        $update_stmt = $conn->prepare("UPDATE users SET name=?, email=?, password=? WHERE name=? AND email=?");
        if (!$update_stmt) {
            throw new Exception("Prepared statement failed: " . $conn->error);
        }

        // 파라미터 바인딩 및 쿼리 실행
        $update_stmt->bind_param("sssss", $new_name, $new_email, $hashedNewPassword, $old_name, $old_email);

        if ($update_stmt->execute()) {
            $response['success'] = true;
            $response['message'] = "User information updated successfully.";
        } else {
            $response['success'] = false;
            $response['message'] = "Error updating user information: " . $update_stmt->error;
        }

        $update_stmt->close();
    } else {
        // 사용자가 존재하지 않을 경우
        $response['success'] = false;
        $response['message'] = "User not found.";
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
