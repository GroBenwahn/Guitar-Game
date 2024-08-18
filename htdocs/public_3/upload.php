<?php
include '../includes/db.php'; // 데이터베이스 연결 파일 포함

$response = array('success' => false, 'message' => '');

if ($_SERVER['REQUEST_METHOD'] === 'POST' && isset($_FILES['files'])) {
    $uploadedFiles = $_FILES['files'];

    // 파일이 여러 개 업로드된 경우 처리
    for ($i = 0; $i < count($uploadedFiles['name']); $i++) {
        $fileName = basename($uploadedFiles['name'][$i]);
        $fileSize = $uploadedFiles['size'][$i];
        $fileType = pathinfo($fileName, PATHINFO_EXTENSION);

        // 파일을 임시 디렉토리에서 저장할 디렉토리로 이동
        $targetDirectory = '/audio/mp3';
        if (!is_dir($targetDirectory)) {
            mkdir($targetDirectory, 0755, true);
        }

        $targetFile = $targetDirectory . $fileName;
        if (move_uploaded_file($uploadedFiles['tmp_name'][$i], $targetFile)) {
            // DB에 파일 정보 저장
            $stmt = $conn->prepare("INSERT INTO uploaded_files (file_name, file_size, file_type) VALUES (?, ?, ?)");
            $stmt->bind_param("sis", $fileName, $fileSize, $fileType);
            if ($stmt->execute()) {
                $response['success'] = true;
                $response['message'] = "Files uploaded successfully!";
            } else {
                $response['message'] = "Database error: " . $conn->error;
            }
            $stmt->close();
        } else {
            $response['message'] = "Failed to upload file: " . $fileName;
        }
    }
} else {
    $response['message'] = "No files were uploaded.";
}

echo json_encode($response);

$conn->close();
?>
