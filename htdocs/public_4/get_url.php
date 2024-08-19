<?php
include '../includes/db.php';

$response = array("success" => false, "message" => "", "data" => array());

try {
    // 정렬 기준을 파라미터로 받아옵니다. 기본값은 'ASC'로 설정
    $sortOrder = isset($_GET['sortOrder']) && $_GET['sortOrder'] === 'DESC' ? 'DESC' : 'ASC';
    $groupByMusician = isset($_GET['groupByMusician']) && $_GET['groupByMusician'] === 'true';

    // 그룹화와 정렬 기준에 따라 데이터를 조회하는 SQL 쿼리
    $orderBy = $groupByMusician ? "musician ASC, name $sortOrder" : "name $sortOrder";
    $stmt = $conn->prepare("SELECT name, musician, url, memo FROM url_upload ORDER BY $orderBy");

    if (!$stmt) {
        throw new Exception("Prepared statement failed: " . $conn->error);
    }

    // 쿼리 실행
    $stmt->execute();

    // 결과 가져오기
    $result = $stmt->get_result();

    if ($result->num_rows > 0) {
        // 결과를 배열에 저장
        while ($row = $result->fetch_assoc()) {
            $response['data'][] = $row;
        }
        $response['success'] = true;
        $response['message'] = "데이터를 성공적으로 가져왔습니다.";
    } else {
        $response['success'] = false;
        $response['message'] = "데이터가 없습니다.";
    }

    $stmt->close();
} catch (Exception $e) {
    $response['success'] = false;
    $response['message'] = $e->getMessage();
}

$conn->close();
echo json_encode($response);
?>