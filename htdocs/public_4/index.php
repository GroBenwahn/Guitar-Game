<html>
<head>
    <meta charset="utf-8"/>
    <title>File list</title>
</head>
<body>
    <?php
    // 데이터베이스 연결 파일 포함
    include '../includes/db.php';

    // num 값을 재정렬하는 함수
    function reorderNum($conn) {
        // 모든 데이터를 가져와서 순차적으로 num 값을 업데이트
        $sql = "SELECT num FROM url_upload ORDER BY num ASC";
        $res = $conn->query($sql);
        $i = 1;

        while ($row = $res->fetch_assoc()) {
            $update_sql = "UPDATE url_upload SET num = $i WHERE num = ".$row['num'];
            $conn->query($update_sql);
            $i++;
        }

        // 최대 num 값을 가져와서 AUTO_INCREMENT를 재설정
        $max_num = $i - 1; // 마지막 $i 값은 최대 num 값 + 1이므로 1을 뺌
        $reset_auto_increment_sql = "ALTER TABLE url_upload AUTO_INCREMENT = " . ($max_num + 1);
        $conn->query($reset_auto_increment_sql);
    }

    // num 값 재정렬 함수 호출
    reorderNum($conn);

    // SQL 쿼리 실행 (재정렬 후)
    $sql = "SELECT * FROM url_upload ORDER BY num ASC";
    $res = $conn->query($sql);

    // 결과의 행 수 계산
    $num_result = $res->num_rows;
    ?>
    
    <table border='1' align="center">
        <thead>
            <tr>
                <th width="50">NUM</th>
                <th width="250">NAME</th>
                <th width="200">URL</th>
                <th width="100">MUSICIAN</th>
                <th width="70">MEMO</th>
                <th width="250">TIME</th>
                <th width="50">DEL</th>
            </tr>
        </thead>
        <tbody>
            <?php
                for($i = 0; $i < $num_result; $i++) {
                    $row = $res->fetch_assoc();
                    echo "<tr>";
                    echo "<td align='center'>".$row['num']."</td>";
                    echo "<td align='center'>".$row['name']."</td>";
                    echo "<td align='center'><a href='".$row['url']."' target='_blank'>".$row['url']."</a></td>";
                    echo "<td align='center'>".$row['musician']."</td>";
                    echo "<td align='center'>".$row['memo']."</td>"; 
                    echo "<td align='center'>".$row['uploaded_time']."</td>";
                    echo "<td align='center'>
                          <a href='./delete.php?num=".$row['num']."'>DEL</a></td>";                    
                    echo "</tr>";
                }
                // MySQL 연결 종료
                $conn->close();
            ?>
            <input type="button" name="table" value="글쓰기" onclick="location.href='table.php';">
        </tbody>
    </table>

</body>
</html>
